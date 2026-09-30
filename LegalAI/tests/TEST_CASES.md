# Test Cases
| ID | Case | Kỳ vọng |
|---|---|---|
| TC01 | admin/admin123 | đăng nhập Admin |
| TC02 | sai password | 401, không crash |
| TC03 | Employee đăng nhập | chỉ thấy hồ sơ phòng ban của mình |
| TC04 | thêm hồ sơ thiếu tên | báo lỗi |
| TC05 | thêm hồ sơ hợp lệ | lưu DB |
| TC06 | sửa hồ sơ | dữ liệu cập nhật |
| TC07 | Employee xóa | 403 |
| TC08 | Admin xóa | xóa thành công |
| TC09 | tìm kiếm từ khóa | kết quả phù hợp |
| TC10 | lọc loại/trạng thái | đúng bộ lọc |
| TC11 | dashboard | số liệu đúng DB |
| TC12 | hồ sơ hạn <=30 ngày | xuất hiện cảnh báo |
| TC13 | AI không có key | DEMO, không crash |
| TC14 | AI 429 | thông báo rate limit |
| TC15 | AI timeout | thông báo timeout |
| TC16 | API key không có ở frontend | không lộ key |
