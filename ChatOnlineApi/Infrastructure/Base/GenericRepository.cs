using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Infrastructure.Persistence.Contexts;

namespace ChatOnlineApi.Infrastructure.Base;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    private readonly ChatOnlineDbContext _db;
    private readonly DbSet<T> _entities;

    public GenericRepository(ChatOnlineDbContext db)
    {
        _db = db;
        _entities = _db.Set<T>();
    }

    #region Create

    public async Task CreateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _entities.AddAsync(entity);
    }

    public async Task CreateRangeAsync(List<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        await _entities.AddRangeAsync(entities);
    }

    #endregion

    #region Read

    public IQueryable<T> Get()
    {
        return _entities.AsNoTracking();
    }

    public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
    {
        return _entities.Where(predicate).AsNoTracking();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _entities.FindAsync(id);
    }

    #endregion

    #region Update

    // حفظ signature قبلی برای جلوگیری از شکستن کدها
    public Task UpdateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _db.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(object id)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        var entity = await GetByIdAsync(id);
        if (entity == null)
            throw new InvalidOperationException("Entity not found.");

        _db.Entry(entity).State = EntityState.Modified;
    }

    #endregion

    #region Delete

    public Task RemoveAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _entities.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(object id)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        var entity = await GetByIdAsync(id);
        if (entity == null)
            throw new InvalidOperationException("Entity not found.");

        _entities.Remove(entity);
    }

    public Task RemoveRangeAsync(List<T> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        _entities.RemoveRange(entities);
        return Task.CompletedTask;
    }

    /// <summary>
    /// حذف تمامی آیتم‌ها از دیتابیس
    /// </summary>
    public async Task TruncateAsync()
    {
        var entityType = _db.Model.FindEntityType(typeof(T));
        if (entityType == null)
            throw new InvalidOperationException("Entity type not found in DbContext.");

        var tableName = entityType.GetTableName();
        var schema = entityType.GetSchema();

        if (string.IsNullOrWhiteSpace(tableName))
            throw new InvalidOperationException("Table name could not be resolved.");

        var fullTableName = string.IsNullOrEmpty(schema)
            ? $"[{tableName}]"
            : $"[{schema}].[{tableName}]";

        await _db.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {fullTableName}");
    }
    #endregion

    #region Raw SQL

    public async Task<List<T>> ExecuteSqlQueryAsync(string sql, params object[] parameters)
    {
        return await _entities
            .FromSqlRaw(sql, parameters)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<T?> ExecuteSqlQuerySingleAsync(string sql, params object[] parameters)
    {
        return await _entities
            .FromSqlRaw(sql, parameters)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
    {
        return await _db.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    #endregion
}