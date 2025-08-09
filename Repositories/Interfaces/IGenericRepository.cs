using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace TestCaseManagement.Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task SaveChangesAsync();
}