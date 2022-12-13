namespace User_Control.Api.Application.Models.Email
{
    public class ForgotPasswordRequest
    {
        public Guid RecoveryToken { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
