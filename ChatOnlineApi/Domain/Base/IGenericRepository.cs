using System.Linq.Expressions;

namespace ChatOnlineApi.Domain.Base;

public interface IGenericRepository<T>
    where T : class
{
    #region Create

    Task CreateAsync(T entity);

    Task CreateRangeAsync(List<T> entities);

    #endregion

    #region Read

    IQueryable<T> Get();

    IQueryable<T> Get(Expression<Func<T, bool>> predicate);

    Task<T?> GetByIdAsync(object id);

    #endregion

    #region Update

    Task UpdateAsync(T entity);

    Task UpdateAsync(object id);

    #endregion

    #region Delete

    Task RemoveAsync(T entity);

    Task RemoveAsync(object id);

    Task RemoveRangeAsync(List<T> entities);

    Task TruncateAsync();

    #endregion

    #region Raw SQL

    Task<List<T>> ExecuteSqlQueryAsync(string sql, params object[] parameters);

    Task<T?> ExecuteSqlQuerySingleAsync(string sql, params object[] parameters);

    Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);

    #endregion
}