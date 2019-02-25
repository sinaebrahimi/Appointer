using System.Data.Entity.Infrastructure;

namespace Appointer.DAL.Migrations
{
    public class MigrationsContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
