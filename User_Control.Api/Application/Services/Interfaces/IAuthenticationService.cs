using User_Control.Api.Application.Models.Login;

namespace User_Control.Api.Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        LoginResponse Login(LoginRequest request);
    }
}
