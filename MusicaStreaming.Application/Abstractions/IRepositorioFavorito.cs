using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Application.Abstractions
{
    public interface IRepositorioFavorito : IRepositorio<Favorito>
    {
        Task<IEnumerable<Favorito>> ObterFavoritosPorUsuarioAsync(Guid usuarioId);
        Task<Favorito> ObterFavoritoAsync(Guid usuarioId, Guid musicaId);
        Task<bool> MusicaEhFavoritaAsync(Guid usuarioId, Guid musicaId);
        Task<int> ContarFavoritosPorUsuarioAsync(Guid usuarioId);
        Task RemoverFavoritoAsync(Guid usuarioId, Guid musicaId);
    }
}