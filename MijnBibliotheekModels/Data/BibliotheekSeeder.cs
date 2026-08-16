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
            Console.WriteLine("--> Start Seeding...");
            // Database + migraties
            await db.Database.MigrateAsync();
            Console.WriteLine("--> Migrations applied.");

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
                var wetenschap = await db.Categorieen.FirstAsync(c => c.Naam == "Wetenschap");

                db.Boeken.AddRange(
                    // Fantasy
                    new Boek { Titel = "Harry Potter en de Steen der Wijzen", Auteur = "J.K. Rowling", ISBN = "HP001", CategorieId = fantasy.Id, IsBeschikbaar = true },
                    new Boek { Titel = "Harry Potter en de Geheime Kamer", Auteur = "J.K. Rowling", ISBN = "HP002", CategorieId = fantasy.Id, IsBeschikbaar = true },
                    new Boek { Titel = "The Hobbit", Auteur = "J.R.R. Tolkien", ISBN = "HB001", CategorieId = fantasy.Id, IsBeschikbaar = true },
                    new Boek { Titel = "The Lord of the Rings: The Fellowship of the Ring", Auteur = "J.R.R. Tolkien", ISBN = "LOTR01", CategorieId = fantasy.Id, IsBeschikbaar = true },
                    new Boek { Titel = "A Game of Thrones", Auteur = "George R.R. Martin", ISBN = "GOT001", CategorieId = fantasy.Id, IsBeschikbaar = true },
                    
                    // Thriller
                    new Boek { Titel = "Da Vinci Code", Auteur = "Dan Brown", ISBN = "DV001", CategorieId = thriller.Id, IsBeschikbaar = true },
                    new Boek { Titel = "Het Bernini Mysterie", Auteur = "Dan Brown", ISBN = "DV002", CategorieId = thriller.Id, IsBeschikbaar = true },
                    new Boek { Titel = "De Eetclub", Auteur = "Saskia Noort", ISBN = "SN001", CategorieId = thriller.Id, IsBeschikbaar = true },
                    new Boek { Titel = "Millennium: Mannen die vrouwen haten", Auteur = "Stieg Larsson", ISBN = "MIL001", CategorieId = thriller.Id, IsBeschikbaar = true },

                    // Wetenschap
                    new Boek { Titel = "A Brief History of Time", Auteur = "Stephen Hawking", ISBN = "BH001", CategorieId = wetenschap.Id, IsBeschikbaar = true },
                    new Boek { Titel = "Sapiens", Auteur = "Yuval Noah Harari", ISBN = "SAP001", CategorieId = wetenschap.Id, IsBeschikbaar = true },
                    new Boek { Titel = "Cosmos", Auteur = "Carl Sagan", ISBN = "COS001", CategorieId = wetenschap.Id, IsBeschikbaar = true },
                    new Boek { Titel = "The Selfish Gene", Auteur = "Richard Dawkins", ISBN = "SG001", CategorieId = wetenschap.Id, IsBeschikbaar = true }
                );

                await db.SaveChangesAsync();
            }
        }
    }
}
