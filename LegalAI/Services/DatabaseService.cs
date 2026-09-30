using Microsoft.Data.Sqlite;
using LegalAI.Models;

namespace LegalAI.Services;

public class DatabaseService
{
    private readonly string _cs;
    private readonly string _dbPath;
    public DatabaseService(IWebHostEnvironment env)
    {
        _dbPath = Path.Combine(env.ContentRootPath, "legalai.db");
        _cs = $"Data Source={_dbPath};Foreign Keys=True";
    }

    public void Initialize()
    {
        using var c = Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "PRAGMA foreign_keys=ON;";
        cmd.ExecuteNonQuery();
        cmd.CommandText = """
CREATE TABLE IF NOT EXISTS Departments(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE
);
CREATE TABLE IF NOT EXISTS Users(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Role TEXT NOT NULL CHECK(Role IN ('Admin','Employee')),
    DepartmentId INTEGER NOT NULL,
    FOREIGN KEY(DepartmentId) REFERENCES Departments(Id) ON UPDATE CASCADE ON DELETE RESTRICT
);
CREATE TABLE IF NOT EXISTS LegalRecords(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    RecordType TEXT NOT NULL,
    ResponsiblePerson TEXT,
    DepartmentId INTEGER NOT NULL,
    Content TEXT NOT NULL,
    ExpiryDate TEXT,
    Status TEXT NOT NULL,
    FOREIGN KEY(DepartmentId) REFERENCES Departments(Id) ON UPDATE CASCADE ON DELETE RESTRICT
);
CREATE TABLE IF NOT EXISTS AuditLogs(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER,
    Username TEXT NOT NULL,
    Action TEXT NOT NULL,
    Entity TEXT,
    EntityId INTEGER,
    CreatedAt TEXT NOT NULL,
    FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE SET NULL
);
CREATE INDEX IF NOT EXISTS IX_LegalRecords_DepartmentId ON LegalRecords(DepartmentId);
CREATE INDEX IF NOT EXISTS IX_LegalRecords_ExpiryDate ON LegalRecords(ExpiryDate);
CREATE INDEX IF NOT EXISTS IX_AuditLogs_CreatedAt ON AuditLogs(CreatedAt);
""";
        cmd.ExecuteNonQuery();

        foreach (var d in new[] { "Phòng Pháp chế", "Phòng CNTT", "Phòng Kinh doanh" })
            EnsureDepartment(d);

        cmd.CommandText = "SELECT COUNT(*) FROM Users";
        if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
        {
            AddUser("admin", "admin123", "Admin", "Phòng Pháp chế");
            AddUser("nhanvien", "nv123", "Employee", "Phòng CNTT");
        }
        cmd.CommandText = "SELECT COUNT(*) FROM LegalRecords";
        if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
        {
            AddRecord(new LegalRecord { Title = "Hợp đồng dịch vụ CNTT", RecordType = "Hợp đồng", ResponsiblePerson = "Nguyễn Văn A", Department = "Phòng CNTT", Content = "Hợp đồng cung cấp dịch vụ phần mềm 12 tháng. Cần theo dõi thanh toán và thời hạn gia hạn.", ExpiryDate = DateTime.Today.AddDays(20), Status = "Đang hiệu lực" });
            AddRecord(new LegalRecord { Title = "Giấy phép kinh doanh", RecordType = "Giấy phép", ResponsiblePerson = "Trần Thị B", Department = "Phòng Pháp chế", Content = "Giấy phép hoạt động của doanh nghiệp. Cần theo dõi thời hạn và hồ sơ gia hạn.", ExpiryDate = DateTime.Today.AddDays(90), Status = "Đang hiệu lực" });
        }
    }

    private SqliteConnection Open() { var c = new SqliteConnection(_cs); c.Open(); return c; }

    private int EnsureDepartment(string name)
    {
        using var c = Open(); using var cmd = c.CreateCommand();
        cmd.CommandText = "INSERT OR IGNORE INTO Departments(Name) VALUES($n); SELECT Id FROM Departments WHERE Name=$n;";
        cmd.Parameters.AddWithValue("$n", name);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
    private int GetDepartmentId(string name)
    {
        using var c = Open(); using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT Id FROM Departments WHERE Name=$n"; cmd.Parameters.AddWithValue("$n", name);
        var v = cmd.ExecuteScalar();
        return v is null ? 0 : Convert.ToInt32(v);
    }
    private string GetDepartmentName(SqliteConnection c, int id)
    {
        using var cmd = c.CreateCommand(); cmd.CommandText = "SELECT Name FROM Departments WHERE Id=$id"; cmd.Parameters.AddWithValue("$id", id);
        return Convert.ToString(cmd.ExecuteScalar()) ?? "";
    }

    public void AddUser(string u, string p, string role, string dept)
    {
        var departmentId = EnsureDepartment(dept);
        using var c = Open(); using var cmd = c.CreateCommand();
        cmd.CommandText = "INSERT INTO Users(Username,PasswordHash,Role,DepartmentId) VALUES($u,$p,$r,$d)";
        cmd.Parameters.AddWithValue("$u", u); cmd.Parameters.AddWithValue("$p", PasswordService.Hash(p)); cmd.Parameters.AddWithValue("$r", role); cmd.Parameters.AddWithValue("$d", departmentId); cmd.ExecuteNonQuery();
    }

    public AppUser? FindUser(string u)
    {
        using var c = Open(); using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT u.Id,u.Username,u.PasswordHash,u.Role,u.DepartmentId,d.Name FROM Users u JOIN Departments d ON d.Id=u.DepartmentId WHERE u.Username=$u";
        cmd.Parameters.AddWithValue("$u", u); using var r = cmd.ExecuteReader();
        return r.Read() ? new AppUser { Id=r.GetInt32(0), Username=r.GetString(1), PasswordHash=r.GetString(2), Role=r.GetString(3), DepartmentId=r.GetInt32(4), Department=r.GetString(5) } : null;
    }

    public List<LegalRecord> SearchRecords(string? q, string? type, string? status, string? sort, string? department)
    {
        using var c = Open(); using var cmd = c.CreateCommand(); var w = new List<string>();
        if (!string.IsNullOrWhiteSpace(q)) { w.Add("(l.Title LIKE $q OR l.Content LIKE $q OR l.ResponsiblePerson LIKE $q)"); cmd.Parameters.AddWithValue("$q", $"%{q}%"); }
        if (!string.IsNullOrWhiteSpace(type)) { w.Add("l.RecordType=$type"); cmd.Parameters.AddWithValue("$type", type); }
        if (!string.IsNullOrWhiteSpace(status)) { w.Add("l.Status=$status"); cmd.Parameters.AddWithValue("$status", status); }
        if (department is not null) { w.Add("d.Name=$dept"); cmd.Parameters.AddWithValue("$dept", department); }
        var order = sort == "expiry" ? "l.ExpiryDate ASC" : "l.Id DESC";
        cmd.CommandText = "SELECT l.Id,l.Title,l.RecordType,l.ResponsiblePerson,l.DepartmentId,d.Name,l.Content,l.ExpiryDate,l.Status FROM LegalRecords l JOIN Departments d ON d.Id=l.DepartmentId" + (w.Count > 0 ? " WHERE " + string.Join(" AND ", w) : "") + " ORDER BY " + order;
        using var r = cmd.ExecuteReader(); var list = new List<LegalRecord>(); while (r.Read()) list.Add(Read(r)); return list;
    }

    public LegalRecord? GetRecord(int id) => SearchRecords(null, null, null, null, null).FirstOrDefault(x => x.Id == id);

    public int AddRecord(LegalRecord x)
    {
        var departmentId = GetDepartmentId(x.Department); if (departmentId == 0) throw new InvalidOperationException("Phòng ban không tồn tại.");
        using var c = Open(); using var cmd = c.CreateCommand();
        cmd.CommandText = "INSERT INTO LegalRecords(Title,RecordType,ResponsiblePerson,DepartmentId,Content,ExpiryDate,Status) VALUES($t,$ty,$p,$d,$c,$e,$s); SELECT last_insert_rowid();";
        Params(cmd, x, departmentId); return Convert.ToInt32(cmd.ExecuteScalar());
    }
    public bool UpdateRecord(LegalRecord x)
    {
        var departmentId = GetDepartmentId(x.Department); if (departmentId == 0) return false;
        using var c = Open(); using var cmd = c.CreateCommand(); cmd.CommandText = "UPDATE LegalRecords SET Title=$t,RecordType=$ty,ResponsiblePerson=$p,DepartmentId=$d,Content=$c,ExpiryDate=$e,Status=$s WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", x.Id); Params(cmd, x, departmentId); return cmd.ExecuteNonQuery() > 0;
    }
    public bool DeleteRecord(int id) { using var c=Open(); using var cmd=c.CreateCommand(); cmd.CommandText="DELETE FROM LegalRecords WHERE Id=$id"; cmd.Parameters.AddWithValue("$id",id); return cmd.ExecuteNonQuery()>0; }

    public void AddAuditLog(int? userId, string username, string action, string? entity=null, int? entityId=null)
    {
        using var c=Open(); using var cmd=c.CreateCommand(); cmd.CommandText="INSERT INTO AuditLogs(UserId,Username,Action,Entity,EntityId,CreatedAt) VALUES($u,$n,$a,$e,$i,$t)";
        cmd.Parameters.AddWithValue("$u", (object?)userId ?? DBNull.Value); cmd.Parameters.AddWithValue("$n", username); cmd.Parameters.AddWithValue("$a", action); cmd.Parameters.AddWithValue("$e", (object?)entity ?? DBNull.Value); cmd.Parameters.AddWithValue("$i", (object?)entityId ?? DBNull.Value); cmd.Parameters.AddWithValue("$t", DateTime.Now.ToString("O")); cmd.ExecuteNonQuery();
    }
    public List<object> GetAuditLogs(int limit=100)
    {
        using var c=Open(); using var cmd=c.CreateCommand(); cmd.CommandText="SELECT Id,Username,Action,Entity,EntityId,CreatedAt FROM AuditLogs ORDER BY Id DESC LIMIT $l"; cmd.Parameters.AddWithValue("$l", Math.Clamp(limit,1,500)); using var r=cmd.ExecuteReader(); var list=new List<object>();
        while(r.Read()) list.Add(new { id=r.GetInt32(0), username=r.GetString(1), action=r.GetString(2), entity=r.IsDBNull(3)?null:r.GetString(3), entityId=r.IsDBNull(4)?(int?)null:r.GetInt32(4), createdAt=r.GetString(5) }); return list;
    }
    public List<LegalRecord> GetExpiring(int days,string? dept)=>SearchRecords(null,null,null,"expiry",dept).Where(x=>x.ExpiryDate.HasValue&&x.ExpiryDate.Value.Date>=DateTime.Today&&x.ExpiryDate.Value.Date<=DateTime.Today.AddDays(days)).ToList();
    public object GetDashboard(string? dept){var a=SearchRecords(null,null,null,null,dept);return new{total=a.Count,active=a.Count(x=>x.Status=="Đang hiệu lực"),expired=a.Count(x=>x.ExpiryDate.HasValue&&x.ExpiryDate.Value.Date<DateTime.Today),expiring30=GetExpiring(30,dept).Count,contracts=a.Count(x=>x.RecordType=="Hợp đồng"),licenses=a.Count(x=>x.RecordType=="Giấy phép")};}
    public string Backup(){var dir=Path.Combine(Path.GetDirectoryName(_dbPath)!,"backups");Directory.CreateDirectory(dir);var target=Path.Combine(dir,$"legalai_{DateTime.Now:yyyyMMdd_HHmmss}.db");File.Copy(_dbPath,target,true);return target;}

    private static void Params(SqliteCommand cmd, LegalRecord x, int departmentId){cmd.Parameters.AddWithValue("$t",x.Title);cmd.Parameters.AddWithValue("$ty",x.RecordType);cmd.Parameters.AddWithValue("$p",(object?)x.ResponsiblePerson??DBNull.Value);cmd.Parameters.AddWithValue("$d",departmentId);cmd.Parameters.AddWithValue("$c",x.Content);cmd.Parameters.AddWithValue("$e",x.ExpiryDate?.ToString("yyyy-MM-dd")??(object)DBNull.Value);cmd.Parameters.AddWithValue("$s",x.Status);}
    private static LegalRecord Read(SqliteDataReader r){DateTime? d=null;if(!r.IsDBNull(7)&&DateTime.TryParse(r.GetString(7),out var x))d=x;return new LegalRecord{Id=r.GetInt32(0),Title=r.GetString(1),RecordType=r.GetString(2),ResponsiblePerson=r.IsDBNull(3)?"":r.GetString(3),DepartmentId=r.GetInt32(4),Department=r.GetString(5),Content=r.GetString(6),ExpiryDate=d,Status=r.GetString(8)};}
}
