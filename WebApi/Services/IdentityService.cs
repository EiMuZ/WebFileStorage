using WebApi.Dto;
using WebApi.Security;

namespace WebApi.Services {

    public class IdentityService {

        private readonly List<User> _users;
        private readonly JwtHandler _jwtHandler;

        public IdentityService(List<User> users, JwtHandler jwtHandler) {
            _users = users;
            _jwtHandler = jwtHandler;
        }

        public async Task<AppResponse> SignIn(SignInDto dto) {

            var username = dto.Username.Trim();
            var password = dto.Password.Trim();

            var user = _users.SingleOrDefault(u => u.Username == username);
            if (user is null || user.Password != password) {
                return ResponseBuilder.AuthenticationFailed.Build();
            }

            var token = await _jwtHandler.Create(new Dictionary<string, object> { { "username", username } });

            return ResponseBuilder.Success.Build(new { token });
        }

    }

}
