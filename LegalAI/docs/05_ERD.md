# 5. Thiết kế cơ sở dữ liệu

## 5.1 Các bảng
- **Departments**: danh mục phòng ban. PK: `Id`, UNIQUE: `Name`.
- **Users**: tài khoản. PK: `Id`, UNIQUE: `Username`, FK: `DepartmentId -> Departments.Id`.
- **LegalRecords**: hồ sơ pháp lý. PK: `Id`, FK: `DepartmentId -> Departments.Id`.
- **AuditLogs**: nhật ký thao tác. PK: `Id`, FK: `UserId -> Users.Id`.

## 5.2 Quan hệ
```mermaid
erDiagram
    Departments ||--o{ Users : "co"
    Departments ||--o{ LegalRecords : "quan_ly"
    Users ||--o{ AuditLogs : "tao"

    Departments { int Id PK; string Name UK }
    Users { int Id PK; string Username UK; string PasswordHash; string Role; int DepartmentId FK }
    LegalRecords { int Id PK; string Title; string RecordType; string ResponsiblePerson; int DepartmentId FK; string Content; date ExpiryDate; string Status }
    AuditLogs { int Id PK; int UserId FK; string Username; string Action; string Entity; int EntityId; datetime CreatedAt }
```

## 5.3 Ràng buộc
- SQLite bật `PRAGMA foreign_keys=ON`.
- Role chỉ nhận `Admin` hoặc `Employee`.
- Username và tên phòng ban là duy nhất.
- Không xóa phòng ban nếu còn Users/LegalRecords tham chiếu.
- Xóa User thì `AuditLogs.UserId` được đặt NULL để giữ lịch sử thao tác.
- Truy vấn dùng parameterized SQL để giảm nguy cơ SQL injection.
