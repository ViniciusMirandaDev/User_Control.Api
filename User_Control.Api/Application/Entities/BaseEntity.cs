using System.ComponentModel.DataAnnotations;

namespace User_Control.Api.Application.Entities
{
    public class BaseEntity
    {
        //
        [Required]
        public Guid Id { get; set; }
    }
}
