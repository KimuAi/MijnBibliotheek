namespace MijnBibliotheekWeb.Dtos
{
    public class RegisterDto
    {
        public string Email { get; set; } = "";
        public string VolledigeNaam { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class AuthResultDto
    {
        public string Token { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Email { get; set; } = "";
        public string VolledigeNaam { get; set; } = "";
        public List<string> Roles { get; set; } = new();
    }
}
