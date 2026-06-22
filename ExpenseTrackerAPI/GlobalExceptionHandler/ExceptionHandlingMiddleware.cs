using ExpenseTracker.Application.CustomException;
using System.Net;
using System.Text.Json;

namespace ExpenseTrackerApi.GlobalExceptionHandler
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // ادامه دادن درخواست به مراحل بعدی
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // تعیین کد وضعیت بر اساس نوع خطا
            var statusCode = exception switch
            {
                BadRequestException => HttpStatusCode.BadRequest,            //400
                UnauthorizedAccessException => HttpStatusCode.Unauthorized, // 401
                KeyNotFoundException => HttpStatusCode.NotFound,           // 404
                _ => HttpStatusCode.InternalServerError                    // 500 (خطای پیش‌فرض)
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message                
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
