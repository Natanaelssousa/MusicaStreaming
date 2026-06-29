using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Infrastructure.Data;

namespace MusicaStreaming.Infrastructure.Repositories
{
    public class RepositorioCartao : RepositorioGenerico<Cartao>, IRepositorioCartao
    {
        public RepositorioCartao(MusicaStreamingDbContext context) : base(context) { }

        public async Task<IEnumerable<Cartao>> ObterCartoesPorUsuarioAsync(Guid usuarioId)
        {
            return await _dbSet.Where(c => c.UsuarioId == usuarioId).ToListAsync();
        }

        public async Task<Cartao> ObterCartaoValidoPorUsuarioAsync(Guid usuarioId)
        {
            return await _dbSet.FirstOrDefaultAsync(c =>
                c.UsuarioId == usuarioId &&
                c.Status == CartaoStatus.Active
            );
        }

        public async Task<bool> UsuarioTemCartaoAtivo(Guid usuarioId)
        {
            return await _dbSet.AnyAsync(c =>
                c.UsuarioId == usuarioId &&
                c.Status == CartaoStatus.Active
            );
        }

        public async Task<IEnumerable<Cartao>> ObterCartoesExpiradosAsync()
        {
            return await _dbSet.Where(c => c.Status == CartaoStatus.Active).ToListAsync()
                .ContinueWith(task => task.Result.Where(c => c.DataValidade.IsExpired()).AsEnumerable());
        }
    }
}
