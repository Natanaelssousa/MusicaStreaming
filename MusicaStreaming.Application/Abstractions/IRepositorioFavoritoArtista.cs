using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Application.Abstractions
{
    public interface IRepositorioFavoritoArtista : IRepositorio<FavoritoArtista>
    {
        Task<IEnumerable<FavoritoArtista>> ObterFavoritosPorUsuarioAsync(Guid usuarioId);
        Task<FavoritoArtista> ObterFavoritoAsync(Guid usuarioId, Guid artistaId);
        Task<bool> ArtistaEhFavoritoAsync(Guid usuarioId, Guid artistaId);
        Task RemoverFavoritoAsync(Guid usuarioId, Guid artistaId);
    }
}