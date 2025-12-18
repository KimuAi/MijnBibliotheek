using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Identity;
using MijnBibliotheekModels.Models;

namespace MijnBibliotheekModels.Data
{
    public static class BibliotheekSeeder
    {
        public static async Task SeedAsync(
            BibliotheekContext db,
            RoleManager<IdentityRole> roleMgr,
            UserManager<AppUser> userMgr)
        {
            // Database + migraties
            await db.Database.MigrateAsync();

            // Rollen
            string[] roles = { "Admin", "Medewerker", "Lid" };
            foreach (var r in roles)
            {
                if (!await roleMgr.RoleExistsAsync(r))
                {
                    await roleMgr.CreateAsync(new IdentityRole(r));
                }
            }

            // ============================
            // ADMIN GEBRUIKER (TOEGEVOEGD)
            // ============================
            var admin = await userMgr.FindByEmailAsync("admin@site.nl");
            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = "admin@site.nl",
                    Email = "admin@site.nl",
                    VolledigeNaam = "Beheerder",
                    EmailConfirmed = true
                };

                var result = await userMgr.CreateAsync(admin, "Admin123!");

                if (result.Succeeded)
                {
                    await userMgr.AddToRolesAsync(admin, new[] { "Admin", "Medewerker" });
                }
            }

            // ============================
            // CATEGORIEËN
            // ============================
            if (!await db.Categorieen.AnyAsync())
            {
                db.Categorieen.AddRange(
                    new Categorie { Naam = "Fantasy" },
                    new Categorie { Naam = "Thriller" },
                    new Categorie { Naam = "Wetenschap" }
                );
                await db.SaveChangesAsync();
            }

            // ============================
            // BOEKEN
            // ============================
            if (!await db.Boeken.AnyAsync())
            {
                var fantasy = await db.Categorieen.FirstAsync(c => c.Naam == "Fantasy");
                var thriller = await db.Categorieen.FirstAsync(c => c.Naam == "Thriller");

                db.Boeken.AddRange(
                    new Boek
                    {
                        Titel = "Harry Potter",
                        Auteur = "J.K. Rowling",
                        ISBN = "HP001",
                        CategorieId = fantasy.Id,
                        IsBeschikbaar = true
                    },
                    new Boek
                    {
                        Titel = "The Hobbit",
                        Auteur = "J.R.R. Tolkien",
                        ISBN = "HB001",
                        CategorieId = fantasy.Id,
                        IsBeschikbaar = true
                    },
                    new Boek
                    {
                        Titel = "Da Vinci Code",
                        Auteur = "Dan Brown",
                        ISBN = "DV001",
                        CategorieId = thriller.Id,
                        IsBeschikbaar = true
                    }
                );

                await db.SaveChangesAsync();
            }
        }
    }
}
