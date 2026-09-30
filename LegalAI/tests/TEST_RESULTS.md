# Kết quả kiểm thử mẫu – LegalAI

> Chạy các test này sau khi khởi động ứng dụng. Cột Actual/Result dưới đây là biểu mẫu ghi nhận; không thay thế cho ảnh/video demo thực tế.

| ID | Kiểm thử | Expected | Actual | Result |
|---|---|---|---|---|
| TC01 | Admin đăng nhập đúng | 200 + cookie | Ghi khi demo | PENDING |
| TC02 | Sai mật khẩu | 401 | Ghi khi demo | PENDING |
| TC03 | Employee chỉ xem phòng ban của mình | Không thấy dữ liệu phòng khác | Ghi khi demo | PENDING |
| TC04 | GET hồ sơ tồn tại | 200 | Ghi khi demo | PENDING |
| TC05 | GET hồ sơ không tồn tại | 404 | Ghi khi demo | PENDING |
| TC06 | POST thiếu Title/Content | 400 | Ghi khi demo | PENDING |
| TC07 | PUT thiếu dữ liệu bắt buộc | 400 | Ghi khi demo | PENDING |
| TC08 | Employee sửa hồ sơ khác phòng | 403 | Ghi khi demo | PENDING |
| TC09 | Employee DELETE | 403 | Ghi khi demo | PENDING |
| TC10 | Admin DELETE | 200 | Ghi khi demo | PENDING |
| TC11 | Search keyword | Trả đúng hồ sơ | Ghi khi demo | PENDING |
| TC12 | Filter type/status | Trả đúng điều kiện | Ghi khi demo | PENDING |
| TC13 | Dashboard | Có total/active/expired/expiring30 | Ghi khi demo | PENDING |
| TC14 | AI không có key | Demo response, không crash | Ghi khi demo | PENDING |
| TC15 | AI rate limit | Thông báo 429 | Ghi khi demo/mock | PENDING |
| TC16 | Audit log | Có LOGIN/CRUD/AI/BACKUP | Ghi khi demo | PENDING |

Sau khi chạy thật, thay `PENDING` bằng `PASS/FAIL` và điền Actual.
