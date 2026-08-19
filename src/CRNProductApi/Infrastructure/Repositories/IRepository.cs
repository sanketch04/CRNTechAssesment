using System.Linq.Expressions;
using CRNProductApi.Application.DTOs.Common;
using CRNProductApi.Domain.Entities;

namespace CRNProductApi.Infrastructure.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Table { get; }

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);

    Task<PageResponse<T>> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 10,
        params Expression<Func<T, object>>[] includes);

    Task<T?> GetByIdAsync(long id);

    Task SaveChangedAsync();
}
