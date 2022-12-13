using Microsoft.AspNetCore.Mvc;
using User_Control.Api.Application.Models.Login;
using User_Control.Api.Application.Services.Interfaces;

namespace User_Control.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Verify user data
        /// </summary>
        /// <param name="request">Email and password</param>
        /// <returns>Simple user infos and token</returns>
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token =  _authenticationService.Login(request);
            return Ok(token);
        }
    }
}
