using Microsoft.AspNetCore.Http;
using Serilog;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ApiLib.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            Log.Information("HTTP Request Information:\n" +
                            $"Schema: {request.Scheme}\n" +
                            $"Host: {request.Host}\n" +
                            $"Path: {request.Path}\n" +
                            $"QueryString: {request.QueryString}");

            request.EnableBuffering();
            var requestBody = string.Empty;
            if (request.Body.CanRead)
            {
                using var reader = new StreamReader(
                    request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true);
                requestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
                request.Body.Position = 0;
            }
            if (!string.IsNullOrEmpty(requestBody))
            {
                Log.Information("Request Body: {RequestBody}", requestBody);
            }

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context).ConfigureAwait(false);
            }
            finally
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseContent = await new StreamReader(responseBody).ReadToEndAsync().ConfigureAwait(false);
                Log.Information("HTTP Response Information:\n" +
                                $"Status Code: {context.Response.StatusCode}\n" +
                                $"Response Body: {responseContent}");

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream).ConfigureAwait(false);
                context.Response.Body = originalBodyStream;
            }
        }
    }
}