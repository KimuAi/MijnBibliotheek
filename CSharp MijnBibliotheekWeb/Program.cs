using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Identity;
using MijnBibliotheekWeb.Middleware;
using MijnBibliotheekWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// DbContext (gebruik BibliotheekContext uit MijnBibliotheekModels)
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? "Data Source=bibliotheek.web.db";

builder.Services.AddDbContext<BibliotheekContext>(options =>
    options.UseSqlite(cs));

// Identity met custom AppUser
builder.Services
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<BibliotheekContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Cookie settings (API-friendly)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".MijnBibliotheek.Auth";
    options.Cookie.HttpOnly = true;
    options.SlidingExpiration = true;

    options.Events.OnRedirectToLogin = ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        ctx.Response.Redirect(ctx.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        ctx.Response.Redirect(ctx.RedirectUri);
        return Task.CompletedTask;
    };
});

// CORS (voor MAUI)
builder.Services.AddCors(options =>
{
    options.AddPolicy("maui", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true) // dev only
            .AllowCredentials();
    });
});

// Localisatie: nl, en, fr (zorg dat je resources toevoegt)
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
var supportedCultures = new[] { "nl", "en", "fr" };
builder.Services.Configure<RequestLocalizationOptions>(opts =>
{
    var cultures = supportedCultures.Select(c => new CultureInfo(c)).ToArray();
    opts.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("nl");
    opts.SupportedCultures = cultures;
    opts.SupportedUICultures = cultures;
});

// E-mail sender (Smtp) registreren
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, SmtpEmailSender>();

// Middleware registratie (optioneel)
builder.Services.AddTransient<CookieLoggingMiddleware>();

var app = builder.Build();

// Seed DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BibliotheekContext>();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    await MijnBibliotheekModels.Data.BibliotheekSeeder.SeedAsync(db, roleMgr, userMgr);
}

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("maui");

app.UseAuthentication();
app.UseAuthorization();

// Gebruik cookie-logging middleware early in pipeline
app.UseMiddleware<CookieLoggingMiddleware>();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Boeken}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapControllers();

app.Run();