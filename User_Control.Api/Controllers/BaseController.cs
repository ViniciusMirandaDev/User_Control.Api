using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace User_Control.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected Guid UserId => Guid.Parse(User.FindFirst(claim => claim.Type == "Id")?.Value);
        protected string? Name => User.FindFirst(claim => claim.Type == "Name")?.Value;
        protected string? Email => User.FindFirst(claim => claim.Type == "Email")?.Value;
    }
}
