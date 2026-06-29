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
    public class RepositorioFavorito : RepositorioGenerico<Favorito>, IRepositorioFavorito
    {
        public RepositorioFavorito(MusicaStreamingDbContext context) : base(context) { }

        public async Task<IEnumerable<Favorito>> ObterFavoritosPorUsuarioAsync(Guid usuarioId)
        {
            return await _dbSet.Where(f => f.UsuarioId == usuarioId).ToListAsync();
        }

        public async Task<Favorito> ObterFavoritoAsync(Guid usuarioId, Guid musicaId)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.UsuarioId == usuarioId && f.MusicaId == musicaId);
        }

        public async Task<bool> MusicaEhFavoritaAsync(Guid usuarioId, Guid musicaId)
        {
            return await _dbSet.AnyAsync(f => f.UsuarioId == usuarioId && f.MusicaId == musicaId);
        }

        public async Task<int> ContarFavoritosPorUsuarioAsync(Guid usuarioId)
        {
            return await _dbSet.CountAsync(f => f.UsuarioId == usuarioId);
        }

        public async Task RemoverFavoritoAsync(Guid usuarioId, Guid musicaId)
        {
            var favorito = await ObterFavoritoAsync(usuarioId, musicaId);
            if (favorito != null)
            {
                _dbSet.Remove(favorito);
            }
        }
    }
}
