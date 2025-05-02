using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage;
using OhBau.Repository.Implement;
using OhBau.Repository.Interface;

public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    public TContext Context { get; }
    private Dictionary<Type, object> _repositories;
    private IDbContextTransaction _currentTransaction;

    public UnitOfWork(TContext context)
    {
        Context = context;
    }

    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        _repositories ??= new Dictionary<Type, object>();
        if (_repositories.TryGetValue(typeof(TEntity), out object repository))
        {
            return (IGenericRepository<TEntity>)repository;
        }

        var repo = new GenericRepository<TEntity>(Context);
        _repositories.Add(typeof(TEntity), repo);
        return repo;
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        Context?.Dispose();
    }

    public int Commit()
    {
        TrackChanges();
        return Context.SaveChanges();
    }

    public async Task<int> CommitAsync()
    {
        TrackChanges();
        return await Context.SaveChangesAsync();
    }

    private void TrackChanges()
    {
        var validationErrors = Context.ChangeTracker.Entries<IValidatableObject>()
            .SelectMany(e => e.Entity.Validate(null))
            .Where(e => e != ValidationResult.Success)
            .ToArray();

        if (validationErrors.Any())
        {
            var exceptionMessage = string.Join(Environment.NewLine,
                validationErrors.Select(error => $"Properties {string.Join(", ", error.MemberNames)} Error: {error.ErrorMessage}"));
            throw new Exception(exceptionMessage);
        }
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction == null)
        {
            _currentTransaction = await Context.Database.BeginTransactionAsync();
        }
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await CommitAsync();
            await _currentTransaction?.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            await _currentTransaction?.RollbackAsync();
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
}
