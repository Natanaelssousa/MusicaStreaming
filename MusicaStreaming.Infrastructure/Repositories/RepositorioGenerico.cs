using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Infrastructure.Data;

namespace MusicaStreaming.Infrastructure.Repositories
{
    public abstract class RepositorioGenerico<T> : IRepositorio<T> where T : class
    {
        protected readonly MusicaStreamingDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected RepositorioGenerico(MusicaStreamingDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> ObterPorIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> ObterTodosAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task AdicionarAsync(T entidade)
        {
            await _dbSet.AddAsync(entidade);
        }

        public virtual async Task AtualizarAsync(T entidade)
        {
            _dbSet.Update(entidade);
            await Task.CompletedTask;
        }
        public virtual async Task SalvarAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual async Task RemoverAsync(T entidade)
        {
            _dbSet.Remove(entidade);
            await Task.CompletedTask;
        }

        public virtual async Task RemoverPorIdAsync(Guid id)
        {
            var entidade = await ObterPorIdAsync(id);
            if (entidade != null)
            {
                _dbSet.Remove(entidade);
            }
        }

        public virtual async Task<bool> ExisteAsync(Guid id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        public virtual async Task<int> ContarAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}