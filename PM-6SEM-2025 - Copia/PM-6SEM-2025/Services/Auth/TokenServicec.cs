using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PM_6SEM_2025.Models;

namespace PM_6SEM_2025.Services.Auth
{
    public static class TokenService
    {
        // Chave secreta (pode depois ir para appsettings.json)
        private static readonly string SecretKey =
            "e831a029470ec4cacc9862d0558665ef"; // chave estática

        public static string GerarToken(UsuariosModel usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email),
                new Claim("nome", usuario.Nome)
            };

            var token = new JwtSecurityToken(
                issuer: "PM_6SEM_2025",
                audience: "PM_6SEM_2025",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        }
    }
}
