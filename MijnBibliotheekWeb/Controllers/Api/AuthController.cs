using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using MijnBibliotheekModels.Identity;
using MijnBibliotheekWeb.ApiDtos;
using System.Security.Claims;

namespace MijnBibliotheekWeb.Controllers.Api;

/// API voor authenticatie (login/register/me/logout)
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender;

    public AuthController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IEmailSender emailSender)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest();

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return Unauthorized();

        var res = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, lockoutOnFailure: false);
        if (!res.Succeeded) return Unauthorized();

        return Ok();
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest();

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            VolledigeNaam = dto.FullName,
            EmailConfirmed = false // stuur bevestiging
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        // gebruiker standaard rol 'Lid' toekennen
        await _userManager.AddToRoleAsync(user, "Lid");

        // genereer bevestigingstoken en stuur e-mail (wachtwoord/credentials nooit in repo)
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme) ?? "n/a";

        // Nederlandse commentaar: e-mail met bevestigingslink wordt verstuurd. Zorg dat Smtp is geconfigureerd.
        await _emailSender.SendEmailAsync(user.Email, "Bevestig je e-mail", $"Klik op de link om te bevestigen: {confirmUrl}");

        return Ok();
    }

    // POST: api/auth/logout
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    // GET: api/auth/me
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);

        var me = new MeDto
        {
            UserName = user.UserName ?? string.Empty,
            FullName = user.VolledigeNaam,
            Roles = roles.ToList()
        };

        return Ok(me);
    }
}