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
    public class RepositorioTransacao : RepositorioGenerico<Transacao>, IRepositorioTransacao
    {
        public RepositorioTransacao(MusicaStreamingDbContext context) : base(context) { }

        public async Task<IEnumerable<Transacao>> ObterTransacoesPorUsuarioAsync(Guid usuarioId)
        {
            return await _dbSet.Where(t => t.UsuarioId == usuarioId).ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> ObterTransacoesPorCartaoAsync(Guid cartaoId)
        {
            return await _dbSet.Where(t => t.CartaoId == cartaoId).ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> ObterTransacoesPendentesAsync()
        {
            return await _dbSet.Where(t => t.Status == TransacaoStatus.Pending).ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> ObterTransacoesNegadasAsync()
        {
            return await _dbSet.Where(t => t.Status == TransacaoStatus.Denied).ToListAsync();
        }

        public async Task<Transacao> ObterUltimaTransacaoPorCartaoAsync(Guid cartaoId)
        {
            return await _dbSet.Where(t => t.CartaoId == cartaoId)
                .OrderByDescending(t => t.DataTransacao)
                .FirstOrDefaultAsync();
        }
    }
}