using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using WebApi.Dto;

namespace WebApi.Security {

    public class JwtAuthenticationHandler : IAuthenticationHandler {

        private AuthenticationScheme _scheme;
        private HttpContext _context;
        private JwtHandler _jwtHandler;

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) {
            _scheme = scheme;
            _context = context;
            _jwtHandler = context.RequestServices.GetService<JwtHandler>();
            return Task.CompletedTask;
        }

        public Task<AuthenticateResult> AuthenticateAsync() {
            var token = _context.Request.Headers["token"].ToString();
            if (string.IsNullOrEmpty(token)) {
                return Task.FromResult(AuthenticateResult.Fail("missing token"));
            }


            var payload = _jwtHandler.Verify(token);
            if (payload is null) {
                return Task.FromResult(AuthenticateResult.Fail("bad token"));
            }

            var ticket = GetAuthenticationTicket(payload);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private AuthenticationTicket GetAuthenticationTicket(Dictionary<string, object> payload) {

            var principal = new ClaimsPrincipal();
            var claimsIdentity = new ClaimsIdentity(new Claim[]{
                new Claim(ClaimTypes.Name, (string)payload["username"])
            }, _scheme.Name);
            principal.AddIdentity(claimsIdentity);

            return new AuthenticationTicket(principal, _scheme.Name);
        }

        public Task ChallengeAsync(AuthenticationProperties? properties) {
            _context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            _context.Response.ContentType = "application/json";
            JsonSerializer.SerializeAsync(_context.Response.BodyWriter.AsStream(), ResponseBuilder.AuthenticationFailed.Build());
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties? properties) {
            _context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            _context.Response.ContentType = "application/json";
            JsonSerializer.SerializeAsync(_context.Response.BodyWriter.AsStream(), ResponseBuilder.AuthorizationFailed.Build());
            return Task.CompletedTask;
        }
    }

    public class PermitAllAuthenticationHandler : IAuthenticationHandler {

        private AuthenticationScheme _scheme;
        private HttpContext _context;

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) {
            _scheme = scheme;
            _context = context;
            return Task.CompletedTask;
        }

        public Task<AuthenticateResult> AuthenticateAsync() {
            var principal = new ClaimsPrincipal();
            var claimsIdentity = new ClaimsIdentity(new Claim[]{
                new Claim(ClaimTypes.Name, "DefaultUser")
            }, _scheme.Name);
            principal.AddIdentity(claimsIdentity);
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, _scheme.Name)));
        }

        public Task ChallengeAsync(AuthenticationProperties? properties) {
            _context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            _context.Response.ContentType = "application/json";
            JsonSerializer.SerializeAsync(_context.Response.BodyWriter.AsStream(), ResponseBuilder.InternalError.Build());
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties? properties) {
            _context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            _context.Response.ContentType = "application/json";
            JsonSerializer.SerializeAsync(_context.Response.BodyWriter.AsStream(), ResponseBuilder.InternalError.Build());
            return Task.CompletedTask;
        }

    }

}
