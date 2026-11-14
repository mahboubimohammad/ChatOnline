using Microsoft.EntityFrameworkCore;

namespace ChatOnlineApi.Domain.Base;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// دسترسی فقط‑خواندنی به DbContext جاری برای استفاده در EF یا Dapper
    /// </summary>
    DbContext Context { get; }

    /// <summary>
    /// آیا تراکنش فعال وجود دارد؟
    /// </summary>
    bool HasActiveTransaction { get; }

    /// <summary>
    /// اجرای تابع (Action) داخل تراکنش با الگوی ExecutionStrategy برای هندل Failover
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);

    /// <summary>
    /// آغاز تراکنش پایگاه داده
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit کردن تراکنش فعال
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback تراکنش فعال
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ذخیره تغییرات EF
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// تشخیص خطای Unique Constraint در SQL Server (کد خطاهای 2601 و 2627)
    /// </summary>
    bool IsUniqueViolation(DbUpdateException ex);
}
