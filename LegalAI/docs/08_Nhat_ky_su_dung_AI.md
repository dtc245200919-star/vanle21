# 8. Minh chứng sử dụng AI trong phân tích, thiết kế và lập trình

Tài liệu này ghi lại các prompt đã dùng và phần kiểm chứng/chỉnh sửa. Khi nộp bài, sinh viên đính kèm ảnh chụp hội thoại AI thật vào `docs/evidence/`.

## A. Phân tích nghiệp vụ
**Prompt:** Phân tích bài toán quản lý hồ sơ pháp lý doanh nghiệp; xác định bối cảnh, actor, dữ liệu, quy trình và vấn đề.

**Kết quả áp dụng:** Admin/Employee; hồ sơ; phòng ban; thời hạn; cảnh báo; quy trình đăng nhập → quản lý hồ sơ → AI.

**Kiểm chứng:** Đối chiếu yêu cầu đề bài, loại bỏ chức năng không cần thiết và giữ giới hạn dữ liệu theo phòng ban.

## B. Thiết kế CSDL và FK
**Prompt:** Review CSDL gồm Users, Departments, LegalRecords, AuditLogs; chỉ rõ PK, FK, UNIQUE và hành vi ON DELETE.

**Kết quả áp dụng:** Tách Departments; `Users.DepartmentId` và `LegalRecords.DepartmentId` tham chiếu Departments; `AuditLogs.UserId` tham chiếu Users.

**Kiểm chứng:** Đối chiếu với truy vấn thực tế trong `Services/DatabaseService.cs` và `database/database.sql`.

## C. Code review bảo mật và quyền truy cập
**Prompt:** Review API quản lý hồ sơ và AI; kiểm tra authentication, authorization, dữ liệu ngoài quyền truy cập, input validation, API key, timeout và lỗi HTTP.

**Kết quả áp dụng:** Cookie authentication; Admin/Employee; lọc department trước khi AI sử dụng dữ liệu; API key chỉ ở backend; timeout 30 giây; xử lý 429/HTTP/JSON/timeout.

**Kiểm chứng:** Kiểm tra lại từng endpoint và thêm validation cho POST/PUT.

## D. Tối ưu prompt – 3 vòng
| Vòng | Prompt | Quan sát | Cải tiến |
|---|---|---|---|
| 1 | `Tóm tắt hồ sơ này.` | Kết quả chưa có cấu trúc | Yêu cầu các mục cố định |
| 2 | `Tóm tắt theo 5 mục: nội dung, bên liên quan, thời hạn, rủi ro, lưu ý.` | Dễ đọc hơn | Giới hạn nguồn dữ liệu |
| 3 | `Chỉ dùng context được cung cấp; không bịa; không tư vấn pháp lý; trả về đúng 5 mục.` | Giảm nội dung ngoài phạm vi | Giữ phiên bản 3 trong `prompts/summary_prompt.txt` |

## E. Minh chứng review/refactor
Các thay đổi được áp dụng trong phiên bản này gồm:
- Bổ sung FK thật trong SQLite.
- Bổ sung AuditLogs và ghi log cho LOGIN/LOGOUT/CREATE/UPDATE/DELETE/BACKUP/AI.
- Bổ sung validation cho PUT.
- Bổ sung endpoint Admin xem audit log.
- Cập nhật ERD và ma trận tiêu chí.

> **Lưu ý:** Không dùng tài liệu này để giả lập ảnh chụp hội thoại. Hãy chụp màn hình các prompt/response AI thật khi làm bài và đặt vào `docs/evidence/`.
