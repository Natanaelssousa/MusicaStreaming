using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Application.Abstractions
{
    public interface IRepositorioNotificacao : IRepositorio<Notificacao>
    {
        Task<IEnumerable<Notificacao>> ObterPendentesAsync();
    }

}
