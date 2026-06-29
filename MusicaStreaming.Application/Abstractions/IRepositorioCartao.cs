using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Application.Abstractions
{
    public interface IRepositorioCartao : IRepositorio<Cartao>
    {
        Task<IEnumerable<Cartao>> ObterCartoesPorUsuarioAsync(Guid usuarioId);
        Task<Cartao> ObterCartaoValidoPorUsuarioAsync(Guid usuarioId);
        Task<bool> UsuarioTemCartaoAtivo(Guid usuarioId);
        Task<IEnumerable<Cartao>> ObterCartoesExpiradosAsync();
    }
}