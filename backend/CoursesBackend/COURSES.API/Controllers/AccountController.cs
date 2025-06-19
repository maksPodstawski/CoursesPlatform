using BL.Exceptions;
using BL.Services;
using IBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using System.Security.Claims;

namespace COURSES.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IRecaptchaService _recaptchaService;

        public AccountController(IAccountService accountService, IRecaptchaService recaptchaService)
        {
            this._accountService = accountService;
            this._recaptchaService = recaptchaService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _recaptchaService.VerifyAsync(registerRequest.RecaptchaToken))
            {
                return BadRequest("CAPTCHA verification failed");
            }

            try
            {
                await _accountService.RegisterAsync(registerRequest);
                return Ok();
            }
            catch (UserAlreadyExistsException ex)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "Email", new[] { ex.Message } }
                };

                return BadRequest(new { errors });
            }
            catch (RegistrationFailedException ex)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "Password", ex.Message.Split(Environment.NewLine) }
                };

                return BadRequest(new { errors });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _accountService.LoginAsync(loginRequest);
                return Ok();
            }
            catch (LoginFailedException ex)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "Login", new[] { ex.Message } }
                };

                return BadRequest(new { errors });
            }

        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["REFRESH_TOKEN"];

            await _accountService.RefreshTokenAsync(refreshToken);

            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var userDTO = await _accountService.GetMeAsync();
                return Ok(userDTO);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return Ok();
        }
    }
}
