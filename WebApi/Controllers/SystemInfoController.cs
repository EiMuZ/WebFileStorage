using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Services;

namespace WebApi.Controllers {
    [Route("api/sys")]
    [ApiController]
    public class SystemInfoController : ControllerBase {

        private readonly SystemInfoService _systemInfoService;

        public SystemInfoController(SystemInfoService systemInfoService) {
            _systemInfoService = systemInfoService;
        }

        [HttpGet("disk")]
        public AppResponse GetStorageDiskInfo() {
            return _systemInfoService.GetStorageDiskInfo();
        }

    }
}
