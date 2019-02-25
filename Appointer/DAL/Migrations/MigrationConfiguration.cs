namespace Appointer.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class MigrationConfiguration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public MigrationConfiguration()
        {
            AutomaticMigrationDataLossAllowed = true;
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);
        }
    }
}
