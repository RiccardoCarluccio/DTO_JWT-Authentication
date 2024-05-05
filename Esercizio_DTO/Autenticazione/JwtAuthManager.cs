using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Esercizio_DTO.Autenticazione
{
    public class JwtAuthManager
    {
        private readonly String _key;

        public JwtAuthManager(string key)
        {
            _key = key;
        }

        public String Auth(String username, String password)
        {
            var user = Utenti.GetUser(username);

            if(user == null || user.Password != password)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_key);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, user.Ruolo) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
