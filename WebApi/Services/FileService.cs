using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using System.Net;
using WebApi.Configuration;
using WebApi.Dto;
using WebApi.Utilities;

namespace WebApi.Services {
    public class FileService {

        private readonly string _storageRoot;
        private HttpContext _httpContext;

        public FileService(AppConfig config, IHttpContextAccessor httpContextAccessor) {
            _storageRoot = Path.GetFullPath(config.Storage.Root);
            Debug.Assert(httpContextAccessor.HttpContext is not null);
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<AppResponse> UploadFileAsync(HttpRequest request) {

            try {
                if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType)) {
                    throw new ArgumentException("Invalid Content Type");
                }

                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(request.ContentType), new FormOptions().MultipartBoundaryLengthLimit);
                var reader = new MultipartReader(boundary, request.Body);
                var section = await reader.ReadNextSectionAsync();

                string? storagePath = null;
                while (section != null) {
                    if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition)) {
                        Debug.Assert(contentDisposition is not null);
                        if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition)) {
                            storagePath = GetBaseDirectory(contentDisposition, section);
                            if (!PathFilter(storagePath)) {
                                return ResponseBuilder.InvalidParameter.Build();
                            }
                        } else if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition)) {
                            var fileName = WebUtility.HtmlEncode(contentDisposition.FileName.Value) ?? throw new NullReferenceException("Can not get the file name");
                            using var targetStream = File.Create(Path.Combine(storagePath ?? throw new NullReferenceException("Missing base directory"), fileName));
                            await section.Body.CopyToAsync(targetStream, 1048576);
                        }
                    }
                    section = await reader.ReadNextSectionAsync();
                }
            } catch {
                return ResponseBuilder.FileOrDirectoryNotFound.Build();
            }

            return ResponseBuilder.Success.Build();
        }

        private string GetBaseDirectory(ContentDispositionHeaderValue contentDisposition, MultipartSection section) {
            var filedName = WebUtility.HtmlEncode(contentDisposition.Name.Value) ?? throw new NullReferenceException("Can not get the field name");
            if (filedName == "baseDir") {
                using var streamReader = new StreamReader(section.Body);
                var baseDir = streamReader.ReadToEnd();
                var storagePath = Path.GetFullPath(Path.Combine(_storageRoot, baseDir));
                if (!Directory.Exists(storagePath)) {
                    throw new DirectoryNotFoundException();
                }
                return storagePath;
            }
            return "";
        }

        public async Task<AppResponse> GetDirectoryTree() {
            var dirTree = await Task.Run(() => GetNodeTree(_storageRoot));
            return ResponseBuilder.Success.Build(dirTree);
        }

        private static Dictionary<string, object> GetNodeTree(string nodePath) {
            var resultItems = new Dictionary<string, object>();
            var dirItemsSize = 0L;
            foreach (var file in Directory.GetFiles(nodePath)) {
                var fileInfo = new FileInfo(file);
                dirItemsSize += fileInfo.Length;
                resultItems.Add(fileInfo.Name, new Dictionary<string, object> {
                    { "type", "file" },
                    { "size", fileInfo.Length }
                });
            }

            foreach (var dir in Directory.GetDirectories(nodePath)) {
                var dirInfo = new DirectoryInfo(dir);
                var childNode = GetNodeTree(dir);
                dirItemsSize += (long)childNode["size"];
                resultItems.Add(dirInfo.Name, childNode);
            }

            return new Dictionary<string, object> {
                { "type", "directory" },
                { "size", dirItemsSize },
                { "items", resultItems }
            };
        }

        public dynamic GetStorageFileContent(string filePath, bool isDownload) {

            var storagePath = Path.GetFullPath(Path.Combine(_storageRoot, filePath));
            if (!PathFilter(storagePath)) {
                return ResponseBuilder.InvalidParameter.Build();
            }

            if (!File.Exists(storagePath)) {
                _httpContext.Response.StatusCode = 404;
                return Task.FromResult(ResponseBuilder.FileOrDirectoryNotFound.Build());
            }

            var fileInfo = new FileInfo(storagePath);
            var stream = File.OpenRead(storagePath);
            if (isDownload) {
                var streamResult = new FileStreamResult(stream, "application/octet-stream") {
                    FileDownloadName = fileInfo.Name
                };
                return Task.FromResult(streamResult);
            } else {
                return Task.FromResult(stream);
            }
        }

        public AppResponse DeletePath(PathDto dto) {

            string storagePath = Path.GetFullPath(Path.Combine(_storageRoot, dto.Path));
            if (!PathFilter(storagePath)) {
                return ResponseBuilder.InvalidParameter.Build();
            }

            if (Directory.Exists(storagePath)) {
                Directory.Delete(storagePath, true);
            } else if (File.Exists(storagePath)) {
                File.Delete(storagePath);
            } else {
                return ResponseBuilder.FileOrDirectoryNotFound.Build();
            }

            return ResponseBuilder.Success.Build();
        }

        private bool PathFilter(string path) {
            return !(path == _storageRoot || !path.StartsWith(_storageRoot));
        }

    }

}
