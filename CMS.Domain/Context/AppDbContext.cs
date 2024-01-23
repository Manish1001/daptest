namespace CMS.Domain.Context
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<AuditLog> AuditLog { get; set; }

        public DbSet<Cafeteria> Cafeteria { get; set; }

        public DbSet<CafeteriaLocation> CafeteriaLocation { get; set; }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<CustomerInvoice> CustomerInvoice { get; set; }

        public DbSet<CustomerInvoiceTransaction> CustomerInvoiceTransaction { get; set; }

        public DbSet<CustomerLocation> CustomerLocation { get; set; }

        public DbSet<CustomerOrder> CustomerOrder { get; set; }

        public DbSet<Lookup> Lookup { get; set; }

        public DbSet<LookupType> LookupType { get; set; }

        public DbSet<Meal> Meal { get; set; }

        public DbSet<MealItem> MealItem { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<Role> Role { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<UserAccount> UserAccount { get; set; }

        public DbSet<UserCredential> UserCredential { get; set; }

        public DbSet<UserLocation> UserLocation { get; set; }

        public DbSet<UserPermission> UserPermission { get; set; }

        public DbSet<UserToken> UserToken { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
