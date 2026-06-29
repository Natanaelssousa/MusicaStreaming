using MediatR;
using MusicaStreaming.Application.Abstractions;

namespace MusicaStreaming.Application.UseCases
{
    public class GetFavoritosQuery : IRequest<GetFavoritosResponse>
    {
        public Guid UsuarioId { get; set; }
    }

    public class GetFavoritosResponse
    {
        public List<FavoritoDto> Favoritos { get; set; }
        public int Total { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class FavoritoDto
    {
        public Guid MusicaId { get; set; }
        public string Titulo { get; set; }
        public string Artista { get; set; }
        public string Genero { get; set; }
        public DateTime DataFavoritacao { get; set; }
    }

    public class GetFavoritosQueryHandler : IRequestHandler<GetFavoritosQuery, GetFavoritosResponse>
    {
        private readonly IRepositorioFavorito _repositorioFavorito;
        private readonly IRepositorioMusica _repositorioMusica;
        private readonly IRepositorioArtista _repositorioArtista;

        public GetFavoritosQueryHandler(IRepositorioFavorito repositorioFavorito, IRepositorioMusica repositorioMusica, IRepositorioArtista repositorioArtista)
        {
            _repositorioFavorito = repositorioFavorito;
            _repositorioMusica = repositorioMusica;
            _repositorioArtista = repositorioArtista;
        }

        public async Task<GetFavoritosResponse> Handle(GetFavoritosQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var favoritos = await _repositorioFavorito.ObterFavoritosPorUsuarioAsync(request.UsuarioId);
                var favoritosDtos = new List<FavoritoDto>();

                foreach (var favorito in favoritos)
                {
                    var musica = await _repositorioMusica.ObterPorIdAsync(favorito.MusicaId);
                    if (musica != null)
                    {
                        var artista = await _repositorioArtista.ObterPorIdAsync(musica.ArtistaId);
                        favoritosDtos.Add(new FavoritoDto
                        {
                            MusicaId = musica.Id,
                            Titulo = musica.Titulo,
                            Artista = artista?.Nome ?? "Desconhecido",
                            Genero = musica.Genero.Name,
                            DataFavoritacao = favorito.DataFavoritacao
                        });
                    }
                }

                return new GetFavoritosResponse
                {
                    Favoritos = favoritosDtos,
                    Total = favoritosDtos.Count,
                    Sucesso = true,
                    Mensagem = $"{favoritosDtos.Count} músicas favoritadas"
                };
            }
            catch (Exception ex)
            {
                return new GetFavoritosResponse
                {
                    Favoritos = new List<FavoritoDto>(),
                    Sucesso = false,
                    Mensagem = $"Erro ao buscar favoritos: {ex.Message}"
                };
            }
        }
    }
}
