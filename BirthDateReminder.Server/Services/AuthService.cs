using BirthDateReminder.Server.Data;
using BirthDateReminder.Server.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BirthDateReminder.Server.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> LoginAsync(LoginDto formData)
        {
            var user = await _userManager.FindByEmailAsync(formData.Email) ?? throw new Exception("Пользователь не найден");

            var isValid = await _userManager.CheckPasswordAsync(user, formData.Password);
            if (!isValid) throw new Exception("Пароль не подходит");

            var token = GenerateJwtToken(user);
            return token;
        }

        public async Task<string> RegisterAsync(RegisterDto formData)
        {
            var userExists = await _userManager.FindByEmailAsync(formData.Email);
            if (userExists != null) throw new Exception("Пользователь с таким email уже существует");

            var user = new ApplicationUser
            {
                Email = formData.Email,
                UserName = formData.Email
            };

            var result = await _userManager.CreateAsync(user, formData.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new Exception(string.Join(", ", errors));
            }

            var token = GenerateJwtToken(user);
            return token;
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("Ключ для JWT токенов не задан в .env");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_config.GetValue<double>("Jwt:ExpireHours")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
