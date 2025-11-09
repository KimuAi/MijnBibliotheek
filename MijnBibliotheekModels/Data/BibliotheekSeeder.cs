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
            await db.Database.MigrateAsync();

            string[] roles = { "Admin", "Medewerker", "Lid" };
            foreach (var r in roles)
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole(r));

            if (!await db.Categorieen.AnyAsync())
            {
                db.Categorieen.AddRange(
                    new Categorie { Naam = "Fantasy" },
                    new Categorie { Naam = "Thriller" },
                    new Categorie { Naam = "Wetenschap" }
                );
                await db.SaveChangesAsync();
            }

            if (!await db.Boeken.AnyAsync())
            {
                var fantasy = await db.Categorieen.FirstAsync(c => c.Naam == "Fantasy");
                var thriller = await db.Categorieen.FirstAsync(c => c.Naam == "Thriller");

                db.Boeken.AddRange(
                    new Boek { Titel = "Harry Potter", Auteur = "J.K. Rowling", ISBN = "HP001", CategorieId = fantasy.Id, IsBeschikbaar = true },
                    new Boek { Titel = "The Hobbit", Auteur = "J.R.R. Tolkien", ISBN = "HB001", CategorieId = fantasy.Id, IsBeschikbaar = true },
                    new Boek { Titel = "Da Vinci Code", Auteur = "Dan Brown", ISBN = "DV001", CategorieId = thriller.Id, IsBeschikbaar = true }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
