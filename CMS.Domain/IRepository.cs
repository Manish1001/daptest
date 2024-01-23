namespace CMS.Domain
{
    using System.Linq.Expressions;

    public interface IRepository<T> where T : class
    {
        IQueryable<T> Table { get; }

        Task<T> FindAsync(int id);

        Task<T> FindAsync(Expression<Func<T, bool>> filter);

        Task<int> CountAsync();

        Task<IEnumerable<T>> FetchAllAsync();

        Task<IEnumerable<T>> GetManyAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? top = null,
            int? skip = null,
            params string[] includeProperties);

        Task<T> AddAsync(T entity);

        Task<IEnumerable<T>> AddRangeAsync(List<T> entities);

        Task UpdateAsync(T entity);

        Task UpdateRangeAsync(List<T> entities);

        Task DeleteAsync(T entity);

        Task DeleteManyAsync(Expression<Func<T, bool>> filter);

        Task DeleteRangeAsync(List<T> entities);
    }
}
