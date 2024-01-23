namespace CMS.Domain
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using Context;
    using Entities;

    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; }

        public AppDbContext DbContext { get; }

        IRepository<AuditLog> AuditLogRepository { get; }

        IRepository<Cafeteria> CafeteriaRepository { get; }

        IRepository<CafeteriaLocation> CafeteriaLocationRepository { get; }

        IRepository<Customer> CustomerRepository { get; }

        IRepository<CustomerInvoice> CustomerInvoiceRepository { get; }

        IRepository<CustomerInvoiceTransaction> CustomerInvoiceTransactionRepository { get; }

        IRepository<CustomerLocation> CustomerLocationRepository { get; }

        IRepository<CustomerOrder> CustomerOrderRepository { get; }

        IRepository<Lookup> LookupRepository { get; }

        IRepository<LookupType> LookupTypeRepository { get; }

        IRepository<Meal> MealRepository { get; }

        IRepository<MealItem> MealItemRepository { get; }

        IRepository<Product> ProductRepository { get; }

        IRepository<Role> RoleRepository { get; }

        IRepository<User> UserRepository { get; }

        IRepository<UserAccount> UserAccountRepository { get; }

        IRepository<UserCredential> UserCredentialRepository { get; }

        IRepository<UserLocation> UserLocationRepository { get; }

        IRepository<UserPermission> UserPermissionRepository { get; }

        IRepository<UserToken> UserTokenRepository { get; }

        Task<int> CommitAsync();

        IDbTransaction CreateTransaction();

        IDisposable BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}