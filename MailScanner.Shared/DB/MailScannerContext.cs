using System.Security.Cryptography.X509Certificates;

namespace MailScanner.Shared.DB;

public class MailScannerContext : IdentityDbContext<User>
{

    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Keyword> Keywords { get; set; }
    public MailScannerContext(DbContextOptions<MailScannerContext> options) : base(options)
    {

    }

    public override int SaveChanges()
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
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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
        return base.SaveChangesAsync(cancellationToken);
    }
}