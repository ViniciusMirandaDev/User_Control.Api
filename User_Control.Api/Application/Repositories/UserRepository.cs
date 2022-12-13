using Microsoft.EntityFrameworkCore;
using User_Control.Api.Application.Entities;
using User_Control.Api.Application.Repositories.Interfaces;
using User_Control.Api.Infrastucture.Data;

namespace User_Control.Api.Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CoreContext _context;

        public UserRepository(CoreContext context)
        {
            _context = context;
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User GetById(Guid id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }

        public User GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(x => x.Email == email);
        }

        public void Register(User user)
        {
            _context.Entry(user).State = EntityState.Added;
            _context.SaveChanges();
        }

        public void ResetPassword(Guid recoveryToken, string newPassword)
        {
            User updatedUser = _context.Users.FirstOrDefault(m => m.RecoveryToken == recoveryToken);

            if (updatedUser == null) throw new NullReferenceException("This token already be used or does not exist");

            updatedUser.PasswordHash = newPassword;

            _context.Entry(updatedUser).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            User updatedUser = _context.Users.FirstOrDefault(m => m.Id == user.Id);

            if (updatedUser == null) throw new NullReferenceException("This user does not exist");

            updatedUser.Name = user.Name;
            updatedUser.Email = user.Email;
            
            _context.Entry(updatedUser).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var user = _context.Users.FirstOrDefault(m => m.Id == id);

            if (user == null) throw new NullReferenceException("This user does not exist");

            _context.Entry(user).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public void RegisterRecoveryToken(Guid recoveryToken, string userEmail)
        {
            User newUser = _context.Users.FirstOrDefault(m => m.Email == userEmail);
            newUser.RecoveryToken = recoveryToken;

            _context.Entry(newUser).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void RemoveRecoveryToken(Guid recoveryToken)
        {
            User newUser = _context.Users.FirstOrDefault(m => m.RecoveryToken == recoveryToken);

            newUser.RecoveryToken = null;

            _context.Entry(newUser).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
