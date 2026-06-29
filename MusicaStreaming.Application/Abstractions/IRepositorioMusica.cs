using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;

namespace MusicaStreaming.Application.Abstractions
{
    public interface IRepositorioMusica : IRepositorio<Musica>
    {
        Task<IEnumerable<Musica>> ObterMusicasPorArtistaAsync(Guid artistaId);
        Task<IEnumerable<Musica>> ObterMusicasPorGeneroAsync(Genre genero);
        Task<IEnumerable<Musica>> BuscarPorTituloAsync(string titulo);
        Task<IEnumerable<Musica>> BuscarPorTituloOuArtistaAsync(string termo);
        Task<IEnumerable<Musica>> ObterMusicasDisponiveisAsync();
        Task<IEnumerable<Musica>> ObterMusicasMaisReproduzidasAsync(int limite = 10);
    }
}