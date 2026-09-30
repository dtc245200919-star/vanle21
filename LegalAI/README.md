# LegalAI - Hệ thống quản lý hồ sơ pháp lý doanh nghiệp có tích hợp AI

## Yêu cầu
- Visual Studio 2022 với workload ASP.NET and web development
- .NET 8 SDK

## Chạy
1. Giải nén project.
2. Mở `LegalAI.csproj` bằng Visual Studio 2022.
3. Build → Rebuild Solution.
4. Ctrl + F5.
5. Trình duyệt mở trang đăng nhập.

## Tài khoản demo
- Admin: `admin` / `admin123`
- Nhân viên: `nhanvien` / `nv123`

## AI
Project chạy được ngay ở chế độ DEMO không cần API key. Muốn gọi AI thật, tạo biến môi trường `OPENAI_API_KEY` hoặc điền cấu hình cục bộ. Không commit API key.

PowerShell:
```powershell
$env:OPENAI_API_KEY="YOUR_KEY"
dotnet run
```

## Các tiêu chí đã triển khai
- Phân tích nghiệp vụ + yêu cầu: `docs/01..03`
- Actor/use case: `docs/04_Use_Case.md`
- ERD/CSDL: `docs/05_ERD.md`
- Kiến trúc: `docs/06_Kien_truc_he_thong.md`
- AI/prompt: `docs/07_Thiet_ke_AI.md`
- Minh chứng AI: `docs/08_Nhat_ky_su_dung_AI.md`
- Kế hoạch: `docs/09_Ke_hoach_trien_khai.md`
- Test: `tests/TEST_CASES.md`
- `.env.example`
- CRUD, tìm kiếm/lọc, dashboard, login, role, backup.

## Git
```bash
git init
git add .
git commit -m "Initial LegalAI project"
git add .
git commit -m "Add authentication CRUD search dashboard"
git commit -am "Integrate AI service and prompts"
```

## Phiên bản hoàn thiện theo rubric
- SQLite bật Foreign Keys với `Departments`, `Users`, `LegalRecords`, `AuditLogs`.
- Audit log cho đăng nhập, đăng xuất, CRUD, backup và AI.
- Admin có endpoint xem audit log: `GET /api/admin/audit-logs`.
- POST/PUT kiểm tra dữ liệu bắt buộc và trả lỗi 400 thay vì làm ứng dụng crash.
- Ma trận 30 tiêu chí: `docs/00_Ma_tran_30_tieu_chi.md`.
- Kết quả kiểm thử: `tests/TEST_RESULTS.md`.
- Minh chứng AI: `docs/08_Nhat_ky_su_dung_AI.md` và `docs/evidence/`.

### Chạy project
```bash
dotnet restore
dotnet build
dotnet run
```

Tài khoản demo:
- Admin: `admin` / `admin123`
- Employee: `nhanvien` / `nv123`

Nếu dùng AI thật, tạo biến môi trường `OPENAI_API_KEY`; không commit API key vào Git.
