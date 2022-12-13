namespace User_Control.Api.Infrastucture.Services.Models
{
    public class BuildEmailModel
    {
        public string NameTo { get; set; }
        public string EmailTo { get; set; }
        public string Title { get; set; }
        public string HtmlBody { get; set; }
        public string TextBody { get; set; }
    }
}
