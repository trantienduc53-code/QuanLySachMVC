using Microsoft.EntityFrameworkCore;
using QuanLySachMVC.Data;
using QuanLySachMVC.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký DbContext với chuỗi kết nối từ appsettings.json
builder.Services.AddDbContext<QuanLySachDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 4. Middleware tự viết: ghi log request/status code và chặn id không hợp lệ.
// Đặt trước UseRouting và MapControllerRoute để request đi qua middleware trước khi vào Controller.
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();

app.UseAuthorization();

// 3. Đặt route mặc định chạy thẳng vào trang Quản lý Sách
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Saches}/{action=Index}/{id?}");

app.Run();