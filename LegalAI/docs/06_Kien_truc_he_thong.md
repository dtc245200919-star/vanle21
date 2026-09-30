# 6. Kiến trúc hệ thống
Frontend: HTML/CSS/JavaScript trong wwwroot.
Backend: ASP.NET Core .NET 8 Minimal API, authentication cookie, authorization.
Database: SQLite qua Microsoft.Data.Sqlite.
AI Service: AiService gọi API tương thích OpenAI; nếu không có key chạy DEMO để kiểm thử giao diện.
## Luồng
Browser → Cookie Auth → API → kiểm quyền → SQLite. Với AI: Browser → /api/ai → kiểm quyền → lấy dữ liệu DB theo department → prompt → AI API → kết quả → Browser.
## Bảo vệ API key
Đọc từ cấu hình hoặc biến môi trường OPENAI_API_KEY; không đưa key vào frontend.
