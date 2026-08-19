using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CRNProductApi.Application.DTOs.Common;
using CRNProductApi.Domain.Entities;
using CRNProductApi.Infrastructure.Data;

namespace CRNProductApi.Infrastructure.Repositories;

//Repository
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> Table => _context.Set<T>();

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await SaveChangedAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await SaveChangedAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await SaveChangedAsync();
    }

    public async Task<PageResponse<T>> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 10,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>().AsNoTracking();

        if (includes?.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PageResponse<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<T?> GetByIdAsync(long id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task SaveChangedAsync()
    {
        await _context.SaveChangesAsync();
    }
}
