# 3. Yêu cầu phi chức năng
- Bảo mật: password PBKDF2, cookie auth, role + department authorization, API key không commit.
- Hiệu năng: truy vấn SQL trực tiếp, giới hạn AI timeout 30 giây.
- Khả dụng: lỗi API/AI trả thông báo thay vì crash; có dữ liệu mẫu.
- Sao lưu: Admin có API tạo bản sao SQLite trong thư mục backups.
- Phân quyền: Admin toàn hệ thống; Employee chỉ dữ liệu phòng ban của mình.
- UX: dashboard, tìm kiếm/lọc, thông báo lỗi, trạng thái AI rõ ràng.
