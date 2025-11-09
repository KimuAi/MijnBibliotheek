using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MijnBibliotheekModels.Identity;
using MijnBibliotheekModels.Models;

namespace MijnBibliotheekModels.Data
{
    public class BibliotheekContext : IdentityDbContext<AppUser>
    {
        public DbSet<Categorie> Categorieen => Set<Categorie>();
        public DbSet<Boek> Boeken => Set<Boek>();
        public DbSet<Uitlening> Uitleningen => Set<Uitlening>();

        public BibliotheekContext(DbContextOptions<BibliotheekContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=mijnbibliotheek.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Boek>().HasIndex(x => x.ISBN).IsUnique();

            b.Entity<Boek>()
                .HasOne(x => x.Categorie)
                .WithMany(c => c.Boeken)
                .HasForeignKey(x => x.CategorieId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Uitlening>()
                .HasOne(u => u.Boek)
                .WithMany(bk => bk.Uitleningen)
                .HasForeignKey(u => u.BoekId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Uitlening>()
                .HasOne(u => u.AppUser)
                .WithMany()
                .HasForeignKey(u => u.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Categorie>().HasQueryFilter(e => !e.IsDeleted);
            b.Entity<Boek>().HasQueryFilter(e => !e.IsDeleted);
            b.Entity<Uitlening>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
