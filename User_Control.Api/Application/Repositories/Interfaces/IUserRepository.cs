using User_Control.Api.Application.Entities;

namespace User_Control.Api.Application.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User GetById(Guid id);
        User GetByEmail(string email);
        void Register(User user);
        void Update(User user);
        void RegisterRecoveryToken(Guid recoveryToken, string userEmail);
        void ResetPassword(Guid recoveryToken, string newPassword);
        void Delete(Guid id);
    }
}
