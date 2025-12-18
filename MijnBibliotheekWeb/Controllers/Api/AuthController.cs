using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MijnBibliotheekModels.Identity;

namespace MijnBibliotheekWeb.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userMgr;
        private readonly SignInManager<AppUser> _signInMgr;

        public AuthController(UserManager<AppUser> userMgr, SignInManager<AppUser> signInMgr)
        {
            _userMgr = userMgr;
            _signInMgr = signInMgr;
        }

        public record LoginDto(string Email, string Password);
        public record RegisterDto(string FullName, string Email, string Password);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userMgr.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid login attempt.");

            if (user.IsGeblokkeerd) return Forbid();

            var res = await _signInMgr.PasswordSignInAsync(user, dto.Password, isPersistent: true, lockoutOnFailure: false);
            if (!res.Succeeded) return Unauthorized("Invalid login attempt.");

            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                VolledigeNaam = dto.FullName,
                IsGeblokkeerd = false
            };

            var res = await _userMgr.CreateAsync(user, dto.Password);
            if (!res.Succeeded) return BadRequest(res.Errors);

            // standaard rol
            await _userMgr.AddToRoleAsync(user, "Lid");
            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInMgr.SignOutAsync();
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await _userMgr.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var roles = await _userMgr.GetRolesAsync(user);

            return Ok(new
            {
                userName = user.UserName ?? "",
                fullName = user.VolledigeNaam,
                roles = roles
            });
        }
    }
}
