namespace QuanLySachMVC.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Xử lý TRƯỚC khi request đi vào Controller
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();

            Console.WriteLine($"[{time}] Method: {method} - Path: {path}");

            // Chặn id không hợp lệ (0, -1, ...) và không cho request đi tiếp
            if (IsInvalidBookId(path))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Book id không hợp lệ");

                Console.WriteLine($"Status Code: {context.Response.StatusCode}");
                return;
            }

            await _next(context);

            // Xử lý SAU khi Controller đã trả về response
            Console.WriteLine($"Status Code: {context.Response.StatusCode}");
        }

        // Kiểm tra path dạng /Saches/Details/{id} với id <= 0
        private static bool IsInvalidBookId(string path)
        {
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3) return false;
            if (!parts[0].Equals("Saches", StringComparison.OrdinalIgnoreCase)) return false;
            if (!parts[1].Equals("Details", StringComparison.OrdinalIgnoreCase)) return false;

            return int.TryParse(parts[2], out var id) && id <= 0;
        }
    }
}
