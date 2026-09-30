# 2. Yêu cầu chức năng
| Chức năng | Đầu vào | Xử lý | Đầu ra |
|---|---|---|---|
| Đăng nhập | username/password | kiểm tra hash + tạo cookie | phiên đăng nhập |
| Phân quyền | role | Admin/Employee + department | quyền API |
| Thêm hồ sơ | form hồ sơ | validate + INSERT | hồ sơ mới |
| Xem hồ sơ | ID | SELECT + kiểm quyền | chi tiết |
| Sửa hồ sơ | ID + form | kiểm quyền + UPDATE | dữ liệu mới |
| Xóa | ID | Admin + DELETE | thông báo |
| Tìm kiếm/lọc | từ khóa, loại, trạng thái | SQL WHERE | danh sách |
| Dashboard | dữ liệu DB | COUNT/phân loại | thống kê |
| Cảnh báo | ngày hết hạn | lọc <=30 ngày | danh sách cảnh báo |
| AI tóm tắt | hồ sơ được phép | prompt + model | bản tóm tắt |
| AI hỏi đáp | câu hỏi + dữ liệu được phép | prompt + model | câu trả lời |
| Sao lưu | yêu cầu Admin | copy DB | file backup |
