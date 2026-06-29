using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Infrastructure.Data;

namespace MusicaStreaming.Infrastructure.Repositories
{
    public class RepositorioNotificacao : RepositorioGenerico<Notificacao>, IRepositorioNotificacao
    {
        public RepositorioNotificacao(MusicaStreamingDbContext context) : base(context) { }

        public async Task<IEnumerable<Notificacao>> ObterPendentesAsync()
        {
            return await _dbSet
                .Where(n => n.Status == NotificacaoStatus.Pendente)
                .ToListAsync();
        }
    }
}
