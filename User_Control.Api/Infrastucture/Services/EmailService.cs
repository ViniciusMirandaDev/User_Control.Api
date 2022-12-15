using MailKit.Net.Smtp;
using MimeKit;
using User_Control.Api.Application.Models.Email;
using User_Control.Api.Infrastucture.Services.Interfaces;
using User_Control.Api.Infrastucture.Services.Models;

namespace User_Control.Api.Infrastucture.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ForgotPasswordEmail(ForgotPasswordRequest model)
        {
            var builderModel = new BuildEmailModel
            {
                NameTo = model.Name,
                EmailTo = model.Email,
                Title = "Requisição de nova senha",
                HtmlBody = @$"<h2>Olá, {model.Name}</h2>
		                        <p>Você solicitou recentemente o envio de uma nova senha no portal User_Control.</p>
		                        <p>Utilize esse link para redefinir sua senha:</p>
		                        <a href='https://user-control-web.vercel.app/forgot-password/{model.RecoveryToken}'>Clique Aqui!</a>
		                        <p>A senha anterior continuará válida até sua mudança.</p>
		                        <p>Atenciosamente,</p>
		                        <p>Portal User_Control</p>",
                TextBody = $@"Olá, {model.Name}
		                        Você solicitou recentemente o envio de uma nova senha no portal User_Control.
		                        Utilize esse link para redefinir sua senha:
		                        {model.RecoveryToken}]
                                A senha anterior continuará válida até sua mudança.
		                        Atenciosamente,
		                        Portal User_Control"
            };

            await BuildAndSendEmail(builderModel);
        }

        public async Task NewUserEmail(NewUserResponse model)
        {
            var builderModel = new BuildEmailModel
            {
                NameTo = model.Name,
                EmailTo = model.Email,
                Title = "Bem vindo ao portal User_Control!",
                HtmlBody = @$"<h2>Olá, {model.Name}</h2>
		                        <p>Foi criado um novo usuário para seu acesso ao portal para o e-mail {model.Email}.</p>
                                <p>Em caso de dúvidas, consulte o suporte.</p>
		                        <p>Atenciosamente,</p>
		                        <p>Portal User_Control</p>",
                TextBody = @$"Olá, {model.Name}
		                        Foi criado um novo usuário para seu acesso ao portal para o e-mail {model.Email}
                                Em caso de dúvidas, consulte o suporte.
		                        Atenciosamente,
		                        Portal User_Control"
            };

            await BuildAndSendEmail(builderModel);
        }

        private async Task BuildAndSendEmail(BuildEmailModel model)
        {
            var displayName = _configuration.GetValue<string>("BuildAndSendEmail:displayName");
            var emailFrom = _configuration.GetValue<string>("BuildAndSendEmail:emailFrom");
            var passwordEmail = _configuration.GetValue<string>("BuildAndSendEmail:passwordEmail");
            var host = _configuration.GetValue<string>("BuildAndSendEmail:host");
            var port = _configuration.GetValue<int>("BuildAndSendEmail:port");

            MimeMessage message = new();

            MailboxAddress from = new(displayName, emailFrom);
            message.From.Add(from);

            MailboxAddress to = new(model.NameTo, model.EmailTo);
            message.To.Add(to);

            message.Subject = model.Title;

            BodyBuilder bodyBuilder = new()
            {
                HtmlBody = model.HtmlBody,
                TextBody = model.TextBody
            };

            message.Body = bodyBuilder.ToMessageBody();

            using SmtpClient client = new();
            client.Connect(host, port, false);
            client.Authenticate(emailFrom, passwordEmail);

            await client.SendAsync(message);
            client.Disconnect(true);

        }
    }
}
