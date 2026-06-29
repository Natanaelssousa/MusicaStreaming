using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Infrastructure.Data;

namespace MusicaStreaming.Infrastructure.Repositories
{
    public class RepositorioMusica : RepositorioGenerico<Musica>, IRepositorioMusica
    {
        public RepositorioMusica(MusicaStreamingDbContext context) : base(context) { }

        public async Task<IEnumerable<Musica>> ObterMusicasPorArtistaAsync(Guid artistaId)
        {
            return await _dbSet.Where(m => m.ArtistaId == artistaId && m.Status == MusicaStatus.Available).ToListAsync();
        }

        public async Task<IEnumerable<Musica>> ObterMusicasPorGeneroAsync(Genre genero)
        {
            return await _dbSet.Where(m => m.Genero == genero && m.Status == MusicaStatus.Available).ToListAsync();
        }

        public async Task<IEnumerable<Musica>> BuscarPorTituloAsync(string titulo)
        {
            return await _dbSet.Where(m =>
                (m.Titulo.Contains(titulo) || m.Genero.Name.Contains(titulo) ) &&
                m.Status == MusicaStatus.Available
            ).ToListAsync();
        }

        public async Task<IEnumerable<Musica>> BuscarPorTituloOuArtistaAsync(string termo)
        {
            return await _dbSet.Where(m =>
                (m.Titulo.Contains(termo) || m.Status == MusicaStatus.Available)
            ).ToListAsync();
        }

        public async Task<IEnumerable<Musica>> ObterMusicasDisponiveisAsync()
        {
            return await _dbSet.Where(m => m.Status == MusicaStatus.Available).ToListAsync();
        }

        public async Task<IEnumerable<Musica>> ObterMusicasMaisReproduzidasAsync(int limite = 10)
        {
            return await _dbSet.Where(m => m.Status == MusicaStatus.Available)
                .OrderByDescending(m => m.NumeroReproducoes)
                .Take(limite)
                .ToListAsync();
        }
    }
}