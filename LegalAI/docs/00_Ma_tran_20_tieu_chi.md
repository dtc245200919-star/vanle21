# Ma trận đối chiếu 20 tiêu chí

## Phân tích thiết kế
1. Phân tích bài toán → docs/01.
2. Yêu cầu chức năng → docs/02.
3. Yêu cầu phi chức năng → docs/03.
4. Actor/use case → docs/04 + Mermaid.
5. CSDL/PK/FK/ràng buộc → docs/05 + database/database.sql.
6. Kiến trúc → docs/06.
7. Vị trí AI → docs/07.
8. Prompt/luồng AI → prompts/ + docs/07.
9. Minh chứng AI → docs/08; bổ sung ảnh thật khi nộp.
10. Tài liệu/kế hoạch → docs/09 + README.

## Bài kiểm tra thường xuyên 2
1. Cấu trúc project → Controllers/API, Models, Services, prompts, wwwroot, docs, database, tests.
2. Đăng nhập/phân quyền → cookie auth + Admin/Employee + giới hạn department.
3. CRUD → POST/GET/PUT/DELETE `/api/ho-so`.
4. Tìm kiếm/lọc/sắp xếp → q/type/status/sort.
5. Dashboard → `/api/dashboard`.
6. UI → login, dashboard, form, table, cảnh báo, AI.
7. CSDL → SQLite + dữ liệu mẫu + parameterized SQL.
8. Xử lý lỗi → validation, 401/403/404, AI timeout/rate-limit/JSON lỗi.
9. Minh chứng AI lập trình → docs/08.
10. README/.env.example/Git → README.md, .env.example, .gitignore và lệnh commit.
