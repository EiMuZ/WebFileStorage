using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Services;

namespace WebApi.Controllers {

    [ApiController]
    [Route("api/files")]
    [Authorize]

    public class FileController : ControllerBase {

        private readonly FileService _fileService;

        public FileController(FileService fileService) {
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<AppResponse> GetDirectoryTree() {
            return await _fileService.GetDirectoryTree();
        }

        [HttpGet("content/{*filePath}")]
        public async Task<dynamic> GetFileContent(string filePath) {
            return await _fileService.GetStorageFileContent(filePath, false);
        }

        [HttpGet("download/{*filePath}")]
        public async Task<dynamic> DownloadFile(string filePath) {
            return await _fileService.GetStorageFileContent(filePath, true);
        }

        [HttpPost]
        public async Task<AppResponse> UploadFile() {
            var request = HttpContext.Request;
            return await _fileService.UploadFileAsync(request);
        }

        [HttpDelete]
        public AppResponse DeleteFile(PathDto dto) {
            return _fileService.DeletePath(dto);
        }

    }
}
