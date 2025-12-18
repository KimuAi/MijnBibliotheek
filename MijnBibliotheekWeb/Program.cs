using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Data;
using MijnBibliotheekModels.Identity;

var builder = WebApplication.CreateBuilder(args);

//MVC + Razor PagesIdentity UI
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//DbContext SQLite
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? "Data Source=bibliotheek.web.db";

builder.Services.AddDbContext<BibliotheekContext>(options =>
    options.UseSqlite(cs));

// Identity AppUser
builder.Services
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        // simpele password rules zodat seeding altijd lukt
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<BibliotheekContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Cookie settings
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

//zodat MAUI mag connecteren
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

var app = builder.Build();

// Seed DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BibliotheekContext>();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    await BibliotheekSeeder.SeedAsync(db, roleMgr, userMgr);
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("maui");

app.UseAuthentication();
app.UseAuthorization();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Boeken}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapControllers();

app.Run();
