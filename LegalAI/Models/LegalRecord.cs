namespace LegalAI.Models;
public class LegalRecord
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string RecordType { get; set; } = "Hợp đồng";
    public string ResponsiblePerson { get; set; } = "";
    public string Department { get; set; } = "";
    public int DepartmentId { get; set; }
    public string Content { get; set; } = "";
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "Đang hiệu lực";
}

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "Employee";
    public string Department { get; set; } = "";
    public int DepartmentId { get; set; }
}
