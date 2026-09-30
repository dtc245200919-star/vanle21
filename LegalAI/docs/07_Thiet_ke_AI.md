# 7-8. Thiết kế AI
## Vị trí AI
1. Tóm tắt hồ sơ pháp lý.
2. Hỏi đáp nội bộ trên dữ liệu được phép.
3. AI không thay thế tư vấn pháp lý.
## System prompt
Bạn là trợ lý AI nội bộ... Chỉ dùng context, không bịa dữ kiện, không tư vấn pháp lý cuối cùng, nói rõ khi thiếu dữ liệu.
## User prompt mẫu
Tóm tắt hồ sơ theo 5 mục: nội dung, phụ trách, thời hạn, điểm chú ý, dữ liệu thiếu.
## Input
LegalRecord hoặc danh sách LegalRecord trong phạm vi quyền.
## Output
Văn bản tiếng Việt có tiêu đề/gạch đầu dòng.
## Giới hạn
Timeout 30s; xử lý HTTP 429; HTTP lỗi; JSON lỗi; dữ liệu được giới hạn theo quyền trước khi gửi AI.
