using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Application.Abstractions
{
    public interface IRepositorioTransacao : IRepositorio<Transacao>
    {
        Task<IEnumerable<Transacao>> ObterTransacoesPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<Transacao>> ObterTransacoesPorCartaoAsync(Guid cartaoId);
        Task<IEnumerable<Transacao>> ObterTransacoesPendentesAsync();
        Task<IEnumerable<Transacao>> ObterTransacoesNegadasAsync();
        Task<Transacao> ObterUltimaTransacaoPorCartaoAsync(Guid cartaoId);
    }
}
