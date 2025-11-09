using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MijnBibliotheekModels.Data
{
    public class BibliotheekContextFactory : IDesignTimeDbContextFactory<BibliotheekContext>
    {
        public BibliotheekContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BibliotheekContext>();

            optionsBuilder.UseSqlite("Data Source=bibliotheek.db");

            return new BibliotheekContext(optionsBuilder.Options);
        }
    }
}
