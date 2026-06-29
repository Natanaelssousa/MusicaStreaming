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
    public class RepositorioArtista : RepositorioGenerico<Artista>, IRepositorioArtista
    {
        public RepositorioArtista(MusicaStreamingDbContext context) : base(context) { }

        public async Task<Artista> ObterPorNomeAsync(string nome)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Nome == nome);
        }

        public async Task<IEnumerable<Artista>> BuscarPorNomeAsync(string termo)
        {
            return await _dbSet.Where(a => a.Nome.Contains(termo)).ToListAsync();
        }

        public async Task<IEnumerable<Artista>> ObterArtistasAtivosAsync()
        {
            return await _dbSet.Where(a => a.Status == ArtistaStatus.Active).ToListAsync();
        }

        public async Task<bool> NomeJaExisteAsync(string nome)
        {
            return await _dbSet.AnyAsync(a => a.Nome == nome);
        }
    }
}
