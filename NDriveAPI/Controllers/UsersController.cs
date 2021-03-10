using Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NDriveAPI.Helpers;
using NDriveAPI.Models.User;
using NDriveAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NDriveAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositoryWrapper _repo;
        private readonly IAuthenticationService _authenticationService;
        private readonly AppSettings _appSettings;

        public UsersController(IRepositoryWrapper repo, IAuthenticationService authenticationService, IOptions<AppSettings> appSettings)
        {
            this._repo = repo;
            this._authenticationService = authenticationService;
            this._appSettings = appSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _repo.User.FindAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while retrieving data");
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate", Name = "AuthenticateUser")]
        public async Task<IActionResult> Authenticate([FromBody] SignInModel model)
        {
            try
            {
                var response = await _authenticationService.Authenticate(model, ipAddress());

                if (response == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

                setTokenCookie(response.RefreshToken);

                return Ok(response);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _authenticationService.RefreshToken(refreshToken, ipAddress());

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            setTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var response = await _authenticationService.RevokeToken(token, ipAddress());

            if (!response)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }

        // Helper Methods 

        /// <summary>
        /// Set Cookie on the client browser
        /// </summary>
        /// <param name="token">The Token to be set in cookie</param>
        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(1)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        /// <summary>
        /// Get the IP Address
        /// </summary>
        /// <returns>The IP Address</returns>
        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
