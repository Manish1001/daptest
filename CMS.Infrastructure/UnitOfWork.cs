namespace CMS.Infrastructure
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Context;
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.Configuration;
    using Npgsql;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection connection;
        private readonly Lazy<IRepository<AuditLog>> auditLogRepository;
        private readonly Lazy<IRepository<Cafeteria>> cafeteriaRepository;
        private readonly Lazy<IRepository<CafeteriaLocation>> cafeteriaLocationRepository;
        private readonly Lazy<IRepository<Customer>> customerRepository;
        private readonly Lazy<IRepository<CustomerInvoice>> customerInvoiceRepository;
        private readonly Lazy<IRepository<CustomerInvoiceTransaction>> customerInvoiceTransactionRepository;
        private readonly Lazy<IRepository<CustomerLocation>> customerLocationRepository;
        private readonly Lazy<IRepository<CustomerOrder>> customerOrderRepository;
        private readonly Lazy<IRepository<Lookup>> lookupRepository;
        private readonly Lazy<IRepository<LookupType>> lookupTypeRepository;
        private readonly Lazy<IRepository<Meal>> mealRepository;
        private readonly Lazy<IRepository<MealItem>> mealItemRepository;
        private readonly Lazy<IRepository<Product>> productRepository;
        private readonly Lazy<IRepository<Role>> roleRepository;
        private readonly Lazy<IRepository<User>> userRepository;
        private readonly Lazy<IRepository<UserAccount>> userAccountRepository;
        private readonly Lazy<IRepository<UserCredential>> userCredentialRepository;
        private readonly Lazy<IRepository<UserLocation>> userLocationRepository;
        private readonly Lazy<IRepository<UserPermission>> userPermissionRepository;
        private readonly Lazy<IRepository<UserToken>> userTokenRepository;
        private IDbContextTransaction transaction;
        private bool disposed;

        public UnitOfWork(AppDbContext appDbContext, IConfiguration configuration)
        {
            this.auditLogRepository = new Lazy<IRepository<AuditLog>>(() => new Repository<AuditLog>(appDbContext));
            this.cafeteriaRepository = new Lazy<IRepository<Cafeteria>>(() => new Repository<Cafeteria>(appDbContext));
            this.cafeteriaLocationRepository = new Lazy<IRepository<CafeteriaLocation>>(() => new Repository<CafeteriaLocation>(appDbContext));
            this.customerRepository = new Lazy<IRepository<Customer>>(() => new Repository<Customer>(appDbContext));
            this.customerInvoiceRepository = new Lazy<IRepository<CustomerInvoice>>(() => new Repository<CustomerInvoice>(appDbContext));
            this.customerInvoiceTransactionRepository = new Lazy<IRepository<CustomerInvoiceTransaction>>(() => new Repository<CustomerInvoiceTransaction>(appDbContext));
            this.customerLocationRepository = new Lazy<IRepository<CustomerLocation>>(() => new Repository<CustomerLocation>(appDbContext));
            this.customerOrderRepository = new Lazy<IRepository<CustomerOrder>>(() => new Repository<CustomerOrder>(appDbContext));
            this.lookupRepository = new Lazy<IRepository<Lookup>>(() => new Repository<Lookup>(appDbContext));
            this.lookupTypeRepository = new Lazy<IRepository<LookupType>>(() => new Repository<LookupType>(appDbContext));
            this.mealRepository = new Lazy<IRepository<Meal>>(() => new Repository<Meal>(appDbContext));
            this.mealItemRepository = new Lazy<IRepository<MealItem>>(() => new Repository<MealItem>(appDbContext));
            this.productRepository = new Lazy<IRepository<Product>>(() => new Repository<Product>(appDbContext));
            this.roleRepository = new Lazy<IRepository<Role>>(() => new Repository<Role>(appDbContext));
            this.userRepository = new Lazy<IRepository<User>>(() => new Repository<User>(appDbContext));
            this.userAccountRepository = new Lazy<IRepository<UserAccount>>(() => new Repository<UserAccount>(appDbContext));
            this.userCredentialRepository = new Lazy<IRepository<UserCredential>>(() => new Repository<UserCredential>(appDbContext));
            this.userLocationRepository = new Lazy<IRepository<UserLocation>>(() => new Repository<UserLocation>(appDbContext));
            this.userPermissionRepository = new Lazy<IRepository<UserPermission>>(() => new Repository<UserPermission>(appDbContext));
            this.userTokenRepository = new Lazy<IRepository<UserToken>>(() => new Repository<UserToken>(appDbContext));

            this.DbContext = appDbContext;
            this.connection = new NpgsqlConnection(configuration.GetConnectionString("DBConnection"));
        }

        ~UnitOfWork()
        {
            this.Dispose(false);
        }

        public virtual IDbConnection Connection
        {
            get
            {
                this.OpenConnection();
                return this.connection;
            }
        }

        public AppDbContext DbContext { get; }

        public IRepository<AuditLog> AuditLogRepository => this.auditLogRepository.Value;

        public IRepository<Cafeteria> CafeteriaRepository => this.cafeteriaRepository.Value;

        public IRepository<CafeteriaLocation> CafeteriaLocationRepository => this.cafeteriaLocationRepository.Value;

        public IRepository<Customer> CustomerRepository => this.customerRepository.Value;

        public IRepository<CustomerInvoice> CustomerInvoiceRepository => this.customerInvoiceRepository.Value;

        public IRepository<CustomerInvoiceTransaction> CustomerInvoiceTransactionRepository => this.customerInvoiceTransactionRepository.Value;

        public IRepository<CustomerLocation> CustomerLocationRepository => this.customerLocationRepository.Value;

        public IRepository<CustomerOrder> CustomerOrderRepository => this.customerOrderRepository.Value;

        public IRepository<Lookup> LookupRepository => this.lookupRepository.Value;

        public IRepository<LookupType> LookupTypeRepository => this.lookupTypeRepository.Value;

        public IRepository<Meal> MealRepository => this.mealRepository.Value;

        public IRepository<MealItem> MealItemRepository => this.mealItemRepository.Value;

        public IRepository<Product> ProductRepository => this.productRepository.Value;

        public IRepository<Role> RoleRepository => this.roleRepository.Value;

        public IRepository<User> UserRepository => this.userRepository.Value;

        public IRepository<UserAccount> UserAccountRepository => this.userAccountRepository.Value;

        public IRepository<UserCredential> UserCredentialRepository => this.userCredentialRepository.Value;

        public IRepository<UserLocation> UserLocationRepository => this.userLocationRepository.Value;

        public IRepository<UserPermission> UserPermissionRepository => this.userPermissionRepository.Value;

        public IRepository<UserToken> UserTokenRepository => this.userTokenRepository.Value;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IDbTransaction CreateTransaction() => this.Connection.BeginTransaction();

        public IDisposable BeginTransaction()
        {
            if (this.transaction != null)
            {
                throw new Exception("A transaction is already in progress.");
            }

            this.transaction = this.DbContext.Database.BeginTransaction();
            return this.transaction;
        }

        public void CommitTransaction()
        {
            this.transaction.Commit();
            this.transaction = null;
        }

        public void RollbackTransaction()
        {
            this.transaction.Rollback();
            this.transaction = null;
        }

        public async Task<int> CommitAsync()
        {
            return await this.DbContext.SaveChangesAsync();
        }

        private void OpenConnection()
        {
            if (this.connection.State != ConnectionState.Open && this.connection.State != ConnectionState.Connecting)
            {
                this.connection.Open();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                this.DbContext.Dispose();
                if (this.connection != null && this.connection.State != ConnectionState.Closed)
                {
                    this.connection.Close();
                    this.connection.Dispose();
                }
            }

            this.disposed = true;
        }
    }
}
