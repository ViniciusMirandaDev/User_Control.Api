using User_Control.Api.Application.Models.Email;

namespace User_Control.Api.Infrastucture.Services.Interfaces
{
    public interface IEmailService
    {
        Task ForgotPasswordEmail(ForgotPasswordRequest model);
        Task NewUserEmail(NewUserResponse model);
    }
}
