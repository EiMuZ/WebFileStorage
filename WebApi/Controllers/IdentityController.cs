using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Services;

namespace WebApi.Controllers {
    [Route("api/identity")]
    [ApiController]
    public class IdentityController : ControllerBase {

        private readonly IdentityService _identityService;

        public IdentityController(IdentityService identityService) {
            _identityService = identityService;
        }

        [HttpPost("signin")]
        public async Task<AppResponse> SignIn(SignInDto dto) {
            return await _identityService.SignIn(dto);
        }

    }
}
