using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Application.Abstractions
{
    public interface IRepositorioAssinatura : IRepositorio<Assinatura>
    {
        Task<Assinatura> ObterPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<Assinatura>> ObterAssinaturasProximasDeExpirarAsync();
        Task<IEnumerable<Assinatura>> ObterAssinaturasExpirasAsync();
        Task<bool> UsuarioTemAssinaturaAtiva(Guid usuarioId);
    }
}
