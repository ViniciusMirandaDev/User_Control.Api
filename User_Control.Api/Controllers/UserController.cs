using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using User_Control.Api.Application.Models.User;
using User_Control.Api.Application.Services.Interfaces;

namespace User_Control.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get All users from DB
        /// </summary>
        /// <returns>User object list</returns>
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var user = _service.GetAll();
            return Ok(user);
        }

        /// <summary>
        /// Get an user by id
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User Object</returns>
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var user = _service.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] InsertUserRequest user)
        {
            _service.Register(user);
            return Ok();
        }

        /// <summary>
        /// Send a new link to user change password
        /// </summary>
        /// <param name="userEmail">User email</param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromQuery] string userEmail)
        {
            _service.ResertPasswordMail(userEmail);
            return Ok();
        }

        /// <summary>
        /// Update user's password
        /// </summary>
        /// <param name="recoveryToken">Token that was on the email link</param>
        /// <param name="newPassword">New password</param>
        /// <returns></returns>
        [HttpPost("{recoveryToken}")]
        public IActionResult UpdatePassword([FromRoute] Guid recoveryToken, [FromQuery] string newPassword)
        {
            _service.ResetPassword(recoveryToken, newPassword);
            return Ok();
        }

        /// <summary>
        /// Update and existing user
        /// </summary>
        /// <param name="user">Update user params</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public IActionResult Update([FromBody] UpdateUserRequest user)
        {
            _service.Update(user);
            return Ok();
        }

        /// <summary>
        /// Delete an user 
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            _service.Delete(id);
            return Ok();
        }
    }
}
