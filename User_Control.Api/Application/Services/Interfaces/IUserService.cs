using User_Control.Api.Application.Models.User;

namespace User_Control.Api.Application.Services.Interfaces
{
    public interface IUserService
    {
        List<UserResponse> GetAll();
        UserResponse GetById(Guid id);
        void Register(InsertUserRequest user);
        void Update(UpdateUserRequest user);
        void ResertPasswordMail(string email);
        void ResetPassword(Guid recoveryToken, string newPassword);
        void Delete(Guid id);
    }
}
