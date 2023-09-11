using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers {
    [Route("api/webui-support")]
    [ApiController]
    public class WebUiSupportController : ControllerBase {

        private readonly WebUiSupportService _webUiSupportService;

        public WebUiSupportController(WebUiSupportService webUiSupportService) {
            _webUiSupportService = webUiSupportService;
        }

        [HttpGet("background-image")]
        public async Task<MemoryStream> GetWebUiBackgroundImage() {
            return await _webUiSupportService.GetWebUiBackgroundImage();
        }

    }
}
