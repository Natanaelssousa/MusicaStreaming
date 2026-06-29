using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Application.Abstractions
{
    public interface IRepositorioArtista : IRepositorio<Artista>
    {
        Task<Artista> ObterPorNomeAsync(string nome);
        Task<IEnumerable<Artista>> BuscarPorNomeAsync(string termo);
        Task<IEnumerable<Artista>> ObterArtistasAtivosAsync();
        Task<bool> NomeJaExisteAsync(string nome);
    }
}