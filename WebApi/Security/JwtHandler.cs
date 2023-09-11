using JWT.Algorithms;
using JWT.Builder;
using System.Security.Cryptography;

namespace WebApi.Security {

    public class JwtHandler {

        private readonly TimeSpan _expireTime = new(7, 0, 0, 0);

        private readonly ECDsa _key;

        public JwtHandler() {
            _key = ECDsa.Create();
        }

        public async Task<string> Create(Dictionary<string, object> payload) {
            return await Task.Run(() => {
                var now = DateTime.Now;
                var builder = JwtBuilder.Create()
                           .WithAlgorithm(new ES512Algorithm(_key, _key))
                           .IssuedAt(now)
                           .NotBefore(now)
                           .ExpirationTime(now + _expireTime);
                foreach (var item in payload) {
                    builder.AddClaim(item.Key, item.Value);
                }
                return builder.Encode();
            });
        }

        public Dictionary<string, object>? Verify(string token) {
            try {
                return JwtBuilder.Create()
                     .WithAlgorithm(new ES512Algorithm(_key, _key))
                     .MustVerifySignature()
                     .Decode<Dictionary<string, object>>(token);
            } catch (Exception) {
                return null;
            }
        }

    }

}
