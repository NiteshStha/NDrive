using Contract;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NDriveAPI.Helpers;
using NDriveAPI.Models.UserModels;
using NDriveAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Helpers;
using Utilities.Responses;

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

        [AllowAnonymous]
        [HttpPost("authenticate", Name = "AuthenticateUser")]
        public async Task<IActionResult> Authenticate([FromBody] SignInModel model)
        {
            try
            {
                var response = await _authenticationService.Authenticate(model, ipAddress());

                if (response == null)
                    return BadRequest(new { message = "User Name or password is incorrect" });

                setTokenCookie(response.RefreshToken);

                return Ok(response);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                var response = await _authenticationService.RefreshToken(refreshToken, ipAddress());

                if (response == null)
                    return Unauthorized(new { message = "Invalid token" });

                setTokenCookie(response.RefreshToken);

                return Ok(response);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model)
        {
            try
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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }

        }

        [HttpGet("{id}/refresh-tokens")]
        public async Task<IActionResult> GetRefreshTokens(int id)
        {
            try
            {
                var user = await _repo.User.GetRefreshTokens(id);
                if (user == null) return NotFound();

                return Ok(new JsonResponse<IEnumerable<RefreshToken>>
                    (
                        StatusCodes.Status200OK,
                        user.RefreshTokens.Count(),
                        user.RefreshTokens
                    ));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _repo.User.FindAll();
                return Ok(new JsonResponse<IEnumerable<User>>
                    (
                        StatusCodes.Status200OK,
                        null,
                        users
                    ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while retrieving data");
            }
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var user = await _repo.User.FindById(id);
                if (user == null) return NotFound();

                return Ok(new JsonResponse<User>
                    (
                        StatusCodes.Status200OK,
                        null,
                        user
                    ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while retrieving data");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post(SignUpModel user)
        {
            try
            {
                var checkUser = await _repo.User.FindById(u => u.Username == user.Username || u.Email == user.Email);
                if (checkUser != null)
                    return StatusCode(StatusCodes.Status409Conflict, "User Name or Email already exists");

                User newUser = new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Password = PasswordHasher.Hash(user.Password),
                    Email = user.Email,
                    DateOfBirth = user.DateOfBirth,
                    CreatedDate = DateTime.Now
                };
                await _repo.User.Create(newUser);
                await _repo.Commit();
                return Created("GetUser", newUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while posting data");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserUpdateModel model)
        {
            try
            {
                var user = await _repo.User.FindById(id);

                if (user == null) return BadRequest();
                if((await _repo.User.FindById(u => u.Username == model.Username)) != null)
                    return StatusCode(StatusCodes.Status409Conflict, "User Name already exists");
                if ((await _repo.User.FindById(u => u.Email == model.Email)) != null)
                    return StatusCode(StatusCodes.Status409Conflict, "Email already exists");

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Username = model.Username;
                user.Email = model.Email;
                user.DateOfBirth = model.DateOfBirth;

                _repo.User.Update(user);
                await _repo.Commit();

                return StatusCode(StatusCodes.Status204NoContent, "User Updated Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while updating data");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _repo.User.FindById(id);

                if (user == null) return NotFound();
                _repo.User.Delete(user);
                await _repo.Commit();
                return StatusCode(StatusCodes.Status204NoContent, "User Deleted Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while deleting data");
            }
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
