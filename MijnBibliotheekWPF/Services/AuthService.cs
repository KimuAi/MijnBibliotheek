using Microsoft.AspNetCore.Identity;
using MijnBibliotheekModels.Identity;

namespace MijnBibliotheekWPF.Services
{
    public class AuthService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<(bool ok, string? msg, AppUser? user, IList<string> roles)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return (false, "Onbekende gebruiker.", null, Array.Empty<string>());
            if (user.IsGeblokkeerd) return (false, "Gebruiker is geblokkeerd.", null, Array.Empty<string>());

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded) return (false, "Ongeldig wachtwoord.", null, Array.Empty<string>());

            var roles = await _userManager.GetRolesAsync(user);
            return (true, null, user, roles);
        }

        public async Task<(bool ok, string? msg)> RegisterAsync(string email, string naam, string pw)
        {
            var user = new AppUser { UserName = email, Email = email, VolledigeNaam = naam };

            var res = await _userManager.CreateAsync(user, pw);
            if (!res.Succeeded) return (false, string.Join("\n", res.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Lid");
            return (true, null);
        }

        public Task LogoutAsync() => Task.CompletedTask;
    }
}
