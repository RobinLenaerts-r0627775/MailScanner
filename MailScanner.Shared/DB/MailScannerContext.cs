

namespace MailScanner.Shared.DB
{
    public class MailScannerContext : DbContext
    {

    public DbSet<Invoice> Invoices { get; set; }
        public MailScannerContext(DbContextOptions<MailScannerContext> options) : base(options)
        {
            
        }

        public override int SaveChanges(bool hardDelete = false)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);
            foreach (var entityEntry in entries)
            {
                if (entityEntry.Entity is BaseModel model)
                {
                    switch (entityEntry.State)
                    {
                        case EntityState.Added:
                            model.CreatedAt = DateTime.Now;
                            break;
                        case EntityState.Modified:
                            model.ModifiedAt = DateTime.Now;
                            break;
                        case EntityState.Deleted:
                            model.DeletedAt = DateTime.Now;
                            entityEntry.State = EntityState.Modified;
                            break;
                    }
                }
            }
            return base.SaveChanges();
        }
    }
}