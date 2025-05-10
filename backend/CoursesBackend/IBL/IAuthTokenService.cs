using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IBL
{
    public interface IAuthTokenService
    {
        (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user);
        public string GenerateRefreshToken();
        public void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
    }
}
