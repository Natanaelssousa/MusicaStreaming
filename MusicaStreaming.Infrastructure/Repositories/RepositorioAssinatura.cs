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
    public class RepositorioAssinatura : RepositorioGenerico<Assinatura>, IRepositorioAssinatura
    {
        public RepositorioAssinatura(MusicaStreamingDbContext context) : base(context) { }

        public async Task<Assinatura> ObterPorUsuarioAsync(Guid usuarioId)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.UsuarioId == usuarioId && a.Status == SubscriptionStatus.Active);
        }

        public async Task<IEnumerable<Assinatura>> ObterAssinaturasProximasDeExpirarAsync()
        {
            var data = DateTime.UtcNow.AddDays(7);
            return await _dbSet.Where(a =>
                a.Status == SubscriptionStatus.Active &&
                a.DataFim <= data &&
                a.DataFim > DateTime.UtcNow
            ).ToListAsync();
        }

        public async Task<IEnumerable<Assinatura>> ObterAssinaturasExpirasAsync()
        {
            return await _dbSet.Where(a =>
                a.Status == SubscriptionStatus.Active &&
                a.DataFim < DateTime.UtcNow
            ).ToListAsync();
        }

        public async Task<bool> UsuarioTemAssinaturaAtiva(Guid usuarioId)
        {
            return await _dbSet.AnyAsync(a =>
                a.UsuarioId == usuarioId &&
                a.Status == SubscriptionStatus.Active
            );
        }
    }
}
