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
    public class RepositorioFavoritoArtista : RepositorioGenerico<FavoritoArtista>, IRepositorioFavoritoArtista
    {
        public RepositorioFavoritoArtista(MusicaStreamingDbContext context) : base(context) { }

        public async Task<IEnumerable<FavoritoArtista>> ObterFavoritosPorUsuarioAsync(Guid usuarioId)
        {
            return await _dbSet.Where(f => f.UsuarioId == usuarioId).ToListAsync();
        }

        public async Task<FavoritoArtista> ObterFavoritoAsync(Guid usuarioId, Guid artistaId)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.UsuarioId == usuarioId && f.ArtistaId == artistaId);
        }

        public async Task<bool> ArtistaEhFavoritoAsync(Guid usuarioId, Guid artistaId)
        {
            return await _dbSet.AnyAsync(f => f.UsuarioId == usuarioId && f.ArtistaId == artistaId);
        }

        public async Task RemoverFavoritoAsync(Guid usuarioId, Guid artistaId)
        {
            var favorito = await ObterFavoritoAsync(usuarioId, artistaId);
            if (favorito != null)
            {
                _dbSet.Remove(favorito);
            }
        }
    }
}
