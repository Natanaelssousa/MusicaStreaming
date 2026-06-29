using MediatR;
using MusicaStreaming.Application.Abstractions;

namespace MusicaStreaming.Application.UseCases
{
    public class GetArtistasFavoritosQuery : IRequest<GetArtistasFavoritosResponse>
    {
        public Guid UsuarioId { get; set; }
    }

    public class GetArtistasFavoritosResponse
    {
        public List<ArtistaFavoritoDto> Artistas { get; set; }
        public int Total { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class ArtistaFavoritoDto
    {
        public Guid ArtistaId { get; set; }
        public string Nome { get; set; }
        public DateTime DataFavoritacao { get; set; }
    }

    public class GetArtistasFavoritosQueryHandler : IRequestHandler<GetArtistasFavoritosQuery, GetArtistasFavoritosResponse>
    {
        private readonly IRepositorioFavoritoArtista _repositorioFavorito;
        private readonly IRepositorioArtista _repositorioArtista;

        public GetArtistasFavoritosQueryHandler(
            IRepositorioFavoritoArtista repositorioFavorito,
            IRepositorioArtista repositorioArtista)
        {
            _repositorioFavorito = repositorioFavorito;
            _repositorioArtista = repositorioArtista;
        }

        public async Task<GetArtistasFavoritosResponse> Handle(GetArtistasFavoritosQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var favoritos = await _repositorioFavorito.ObterFavoritosPorUsuarioAsync(request.UsuarioId);
                var dtos = new List<ArtistaFavoritoDto>();

                foreach (var fav in favoritos)
                {
                    var artista = await _repositorioArtista.ObterPorIdAsync(fav.ArtistaId);
                    if (artista != null)
                    {
                        dtos.Add(new ArtistaFavoritoDto
                        {
                            ArtistaId = artista.Id,
                            Nome = artista.Nome,
                            DataFavoritacao = fav.DataFavoritacao
                        });
                    }
                }

                return new GetArtistasFavoritosResponse
                {
                    Artistas = dtos,
                    Total = dtos.Count,
                    Sucesso = true,
                    Mensagem = $"{dtos.Count} artistas favoritados"
                };
            }
            catch (Exception ex)
            {
                return new GetArtistasFavoritosResponse
                {
                    Artistas = new List<ArtistaFavoritoDto>(),
                    Sucesso = false,
                    Mensagem = $"Erro ao buscar artistas favoritos: {ex.Message}"
                };
            }
        }
    }
}