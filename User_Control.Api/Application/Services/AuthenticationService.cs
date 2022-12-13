using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using User_Control.Api.Application.Entities;
using User_Control.Api.Application.Models.Login;
using User_Control.Api.Application.Repositories.Interfaces;
using User_Control.Api.Application.Services.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace User_Control.Api.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public LoginResponse Login(LoginRequest request)
        {
            var user = _userRepository.GetByEmail(request.Email);

            if (user == null)
            {
                throw new AuthenticationException("This user does not exists");
            }

            var passwordOk = BC.Verify(request.Password, user.PasswordHash);

            if (!passwordOk)
            {
                throw new AuthenticationException("Incorrect email or password");
            }

            var token = GenerateToken(user);

            return new LoginResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = token
            };
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Authentication:SecretKey"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
