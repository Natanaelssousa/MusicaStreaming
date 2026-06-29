using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MusicaStreaming.Application.Abstractions
{
    public interface IRepositorio<T> where T : class
    {
        Task<T> ObterPorIdAsync(Guid id);
        Task<IEnumerable<T>> ObterTodosAsync();
        Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicate);
        Task AdicionarAsync(T entidade);
        Task AtualizarAsync(T entidade);
        Task SalvarAsync();
        Task RemoverAsync(T entidade);
        Task RemoverPorIdAsync(Guid id);
        Task<bool> ExisteAsync(Guid id);
        Task<int> ContarAsync();
    }
}