using System.Text.RegularExpressions;

namespace WebAppStarter.Middleware {

    public partial class VueRewriteMiddleware {

        private readonly RequestDelegate _next;

        [GeneratedRegex("^/api.+")]
        private static partial Regex ApiPathRegex();

        public VueRewriteMiddleware(RequestDelegate next) {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext) {

            var reqPath = httpContext.Request.Path;

            if (ApiPathRegex().IsMatch(reqPath)) {
                return _next(httpContext);
            }

            var filePath = Path.Combine(
                Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory),
                "wwwroot",
                reqPath.ToString().TrimStart('/')
                );

            if (!File.Exists(filePath)) {
                httpContext.Request.Path = "/index.html";
            }
            return _next(httpContext);
        }


    }

    public static class IndexRedirectMiddlewareExtensions {
        public static IApplicationBuilder UseVueRewriteMiddleware(this IApplicationBuilder builder) {
            return builder.UseMiddleware<VueRewriteMiddleware>();
        }
    }

}



