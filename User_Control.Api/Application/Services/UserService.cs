using System.ComponentModel.DataAnnotations;
using User_Control.Api.Application.Entities;
using User_Control.Api.Application.Models.Email;
using User_Control.Api.Application.Models.User;
using User_Control.Api.Application.Repositories.Interfaces;
using User_Control.Api.Application.Services.Interfaces;
using User_Control.Api.Infrastucture.Services.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace User_Control.Api.Application.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _repository;
        private readonly IEmailService _emailService;
        public UserService(IUserRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public List<UserResponse> GetAll()
        {
            return _repository.GetAll().Select(user => new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            }).ToList();
        }

        public UserResponse GetById(Guid id)
        {
            var user = _repository.GetById(id);

            if (user == null)
            {
                return null;
            }

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };
        }

        public async void Register(InsertUserRequest user)
        {
            var mailValidator = new EmailAddressAttribute();

            if (!mailValidator.IsValid(user.Email))
            {
                throw new ArgumentException("Please insert a valid email address");
            }

            _repository.Register(new User
            {
                Id = Guid.NewGuid(),
                Name = user.Name,
                Email = user.Email,
                PasswordHash = BC.HashPassword(user.Password)
            });

            await _emailService.NewUserEmail(new NewUserResponse
            {
                Name = user.Name,
                Email = user.Email
            });
        }

        public void ResertPasswordMail(string email)
        {
            Guid id = Guid.NewGuid();
            var user = _repository.GetByEmail(email);

            if(user == null)
            {
                throw new NullReferenceException("This user does not exists");
            }

            _repository.RegisterRecoveryToken(id, email);
            _emailService.ForgotPasswordEmail(new ForgotPasswordRequest
            {
                Email = email,
                Name = user.Name,
                RecoveryToken = id
            });
        }

        public void ResetPassword(Guid recoveryToken, string newPassword)
        {
            _repository.ResetPassword(recoveryToken, BC.HashPassword(newPassword));
            _repository.RemoveRecoveryToken(recoveryToken);
        }

        public void Update(UpdateUserRequest user)
        {
            if (_repository.GetById(user.Id) == null)
            {
                throw new NullReferenceException("This user not exists!");
            }

            _repository.Update(new User
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
            });
        }
        public void Delete(Guid id)
        {
            _repository.Delete(id);
        }
    }
}
