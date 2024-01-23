namespace CMS.Infrastructure
{
    using System.Linq.Expressions;
    using Domain;
    using Domain.Context;
    using Microsoft.EntityFrameworkCore;

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext appDbContext;

        public Repository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public virtual IQueryable<T> Table => this.appDbContext.Set<T>();

        public virtual async Task<T> FindAsync(int id)
        {
            return await this.appDbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> filter)
        {
            return await this.appDbContext.Set<T>().FirstOrDefaultAsync(filter);
        }

        public virtual async Task<int> CountAsync()
        {
            return await this.appDbContext.Set<T>().CountAsync();
        }

        public virtual async Task<IEnumerable<T>> FetchAllAsync()
        {
            return await this.appDbContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetManyAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? top  = null,
            int? skip = null,
            params string[] includeProperties)
        {
            var query = this.Table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties.Length > 0)
            {
                query = includeProperties.Aggregate(query, (theQuery, theInclude) => theQuery.Include(theInclude));
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (top.HasValue)
            {
                query = query.Take(top.Value);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await this.appDbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(List<T> entities)
        {
            await this.appDbContext.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            this.appDbContext.Entry(entity).CurrentValues.SetValues(entity);
            await Task.CompletedTask;
        }

        public virtual async Task UpdateRangeAsync(List<T> entities)
        {
            try
            {
                this.appDbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                foreach (var e in entities)
                {
                    await this.UpdateAsync(e);
                }
            }
            finally
            {
                this.appDbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            this.appDbContext.Set<T>().Remove(entity);
            await Task.CompletedTask;
        }

        public Task DeleteManyAsync(Expression<Func<T, bool>> filter)
        {
            var appDbSet = this.appDbContext.Set<T>();
            var entities = appDbSet.Where(filter);
            appDbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteRangeAsync(List<T> entities)
        {
            try
            {
                this.appDbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                foreach (var e in entities)
                {
                    await this.DeleteAsync(e);
                }
            }
            finally
            {
                this.appDbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }
    }
}
