using System.ComponentModel.DataAnnotations;

namespace User_Control.Api.Application.Entities
{
    public class User : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public Guid? RecoveryToken { get; set; }
    }
}
