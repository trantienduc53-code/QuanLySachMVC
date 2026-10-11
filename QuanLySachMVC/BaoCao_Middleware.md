# Báo cáo: Middleware trong ASP.NET Core

**1. Middleware trong ASP.NET Core dùng để làm gì?**

Middleware là các thành phần xử lý nằm trong pipeline của ứng dụng, mỗi request đi qua lần lượt từng middleware trước khi tới Controller và response đi ngược lại qua chúng. Middleware dùng để xử lý các việc chung cho mọi request như: ghi log, xác thực/phân quyền, HTTPS redirect, phục vụ file tĩnh, xử lý lỗi, giới hạn truy cập.

**2. Middleware khác Controller ở điểm nào?**

- Middleware chạy ở tầng pipeline, áp dụng cho **mọi request** (kể cả file tĩnh, URL không tồn tại), còn Controller chỉ xử lý các request đã được route tới một Action cụ thể.
- Middleware không biết về View, Model hay Action; nó chỉ thao tác với `HttpContext` (request/response). Controller chứa logic nghiệp vụ theo từng chức năng.
- Middleware có thể quyết định dừng request (không gọi `next`), Controller thì luôn là điểm kết thúc của việc xử lý nghiệp vụ.

**3. Dòng lệnh `await _next(context);` có ý nghĩa gì?**

`_next` là delegate đại diện cho middleware tiếp theo trong pipeline (cuối cùng là routing/Controller). Gọi `await _next(context)` nghĩa là chuyển request sang middleware kế tiếp và **chờ** nó xử lý xong. Code đặt sau dòng này chạy khi response đã được tạo, nên có thể đọc `context.Response.StatusCode` để ghi log. Code đặt trước dòng này là xử lý trước khi request vào Controller.

**4. Vì sao khi middleware trả về `return;` thì request không đi tiếp vào Controller?**

Vì middleware không gọi `_next(context)`. Pipeline được xây dựng theo chuỗi gọi lồng nhau: mỗi middleware quyết định có gọi middleware sau hay không. Khi `return` mà không gọi `_next`, chuỗi bị cắt ngay tại đó nên Controller không bao giờ được gọi. Trong bài này, middleware tự ghi `400` và nội dung "Book id không hợp lệ" rồi kết thúc request.

**5. Nếu đặt middleware sau `app.MapControllerRoute(...)` thì có thể xảy ra vấn đề gì?**

`MapControllerRoute` chỉ đăng ký endpoint (route → Controller); việc thực thi endpoint do middleware routing/endpoint đảm nhận, và request thường được xử lý ngay tại endpoint đó mà không chuyển tiếp về các middleware đăng ký sau. Do đó middleware đặt sau route thường **không được chạy** cho các request đã khớp route, nghĩa là không ghi log và không chặn được id không hợp lệ. Ngoài ra, thứ tự đúng cũng rất quan trọng với các middleware như Authentication/Authorization: phải đặt trước endpoint thì mới bảo vệ được Controller.

**6. Nếu cần sử dụng thêm middleware khác thì viết tiếp thế nào?**

Thêm các lệnh `app.UseXxx(...)` hoặc `app.UseMiddleware<TenMiddleware>()` vào `Program.cs`, **trước** các lệnh `app.MapControllerRoute(...)` và `app.Run()`. Thứ tự đăng ký chính là thứ tự chạy. Ví dụ:

```csharp
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<TenMiddlewareKhac>();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Saches}/{action=Index}/{id?}");
```
