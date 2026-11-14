using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ChatOnlineApi.Domain.Base;

namespace ChatOnlineApi.Infrastructure.Base
{
    public class UnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {
        private readonly TContext _context;

        public DbContext Context => _context;

        private IDbContextTransaction? _transaction;
        private int _transactionDepth;
        private bool _rollbackRequested;
        private bool _disposed;

        public UnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool HasActiveTransaction => _transaction != null;

        // ---------------------------------------------------------------------
        //  EXECUTE IN TRANSACTION (Atomic Operation with Retry Strategy)
        // ---------------------------------------------------------------------
        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            ThrowIfDisposed();

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync();

                try
                {
                    var result = await action();
                    await tx.CommitAsync();
                    return result;
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            });
        }

        // ---------------------------------------------------------------------
        // BEGIN TRANSACTION
        // ---------------------------------------------------------------------
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                _transactionDepth = 1;
                _rollbackRequested = false;
            }
            else
            {
                _transactionDepth++;
            }
        }

        // ---------------------------------------------------------------------
        // COMMIT
        // ---------------------------------------------------------------------
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (_transaction == null)
                throw new InvalidOperationException("No active transaction.");

            if (_rollbackRequested)
                throw new InvalidOperationException("Transaction already marked for rollback.");

            _transactionDepth--;

            if (_transactionDepth > 0)
                return;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackInternalAsync(cancellationToken);
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        // ---------------------------------------------------------------------
        // ROLLBACK
        // ---------------------------------------------------------------------
        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (_transaction == null)
                return;

            await RollbackInternalAsync(cancellationToken);
            await DisposeTransactionAsync();
        }

        private async Task RollbackInternalAsync(CancellationToken cancellationToken)
        {
            _rollbackRequested = true;
            await _transaction!.RollbackAsync(cancellationToken);
        }

        // ---------------------------------------------------------------------
        // SAVE CHANGES
        // ---------------------------------------------------------------------
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            return _context.SaveChangesAsync(cancellationToken);
        }

        // ---------------------------------------------------------------------
        // DISPOSE TRANSACTION
        // ---------------------------------------------------------------------
        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
                _transactionDepth = 0;
                _rollbackRequested = false;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UnitOfWork<TContext>));
        }

        // ---------------------------------------------------------------------
        // DISPOSE
        // ---------------------------------------------------------------------
        public void Dispose()
        {
            if (_disposed)
                return;

            _transaction?.Dispose();
            _context.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            if (_transaction != null)
                await _transaction.DisposeAsync();

            await _context.DisposeAsync();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        // ---------------------------------------------------------------------
        // SQL UNIQUE VIOLATION DETECTION
        // ---------------------------------------------------------------------
        public bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sql)
                return sql.Number == 2601 || sql.Number == 2627;

            return false;
        }
    }
}
