using System.Diagnostics;
using WebApi.Configuration;

namespace WebApi.Services {
    public class WebUiSupportService {

        private readonly AppConfig _appConfig;
        private readonly HttpContext _httpContext;

        private static string? _backgroundImageContentType;
        private static byte[]? _backgroundImageBytes;

        public WebUiSupportService(AppConfig appConfig, IHttpContextAccessor httpContextAccessor) {
            _appConfig = appConfig;
            Debug.Assert(httpContextAccessor.HttpContext is not null);
            _httpContext = httpContextAccessor.HttpContext;

            LoadBackgroundImage(_appConfig.WebUi.BackgroundImage);

        }

        private void LoadBackgroundImage(string path) {

            if (_backgroundImageBytes is not null && _backgroundImageContentType is not null) {
                return;
            }

            if (!File.Exists(path)) {
                _backgroundImageContentType = null;
                _backgroundImageBytes = null;
                return;
            }

            var fileInfo = new FileInfo(path);
            var fileExt = fileInfo.Extension;
            _backgroundImageContentType = fileExt switch {
                ".jpg" or ".jpeg" or ".jfif" or ".pjpeg" or ".pjp" => "image/jpeg",
                ".png" => ".image/png",
                ".webp" => ".image/webp",
                ".apng" => ".image/apng",
                ".avif" => ".image/avif",
                ".gif" => ".image/gif",
                ".svg" => ".image/svg+xml",
                ".bmp" => ".image/bmp",
                ".ico" or ".cur" => "image/x-icon",
                ".tif" or ".tiff" => "image/tiff",
                _ => null
            };

            _backgroundImageBytes = File.ReadAllBytes(path);
        }

        public Task<MemoryStream> GetWebUiBackgroundImage() {

            if (_backgroundImageBytes is null || _backgroundImageContentType is null) {
                _httpContext.Response.StatusCode = 404;
                return Task.FromResult(new MemoryStream(new byte[0]));
            }

            if (_backgroundImageContentType is not null) {
                _httpContext.Response.ContentType = _backgroundImageContentType;
            }
            return Task.FromResult(new MemoryStream(_backgroundImageBytes));
        }

    }
}
