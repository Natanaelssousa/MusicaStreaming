using MediatR;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Application.UseCases
{
    public class FavoritarArtistaCommand : IRequest<FavoritarArtistaResponse>
    {
        public Guid UsuarioId { get; set; }
        public Guid ArtistaId { get; set; }
    }

    public class FavoritarArtistaResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class FavoritarArtistaCommandHandler : IRequestHandler<FavoritarArtistaCommand, FavoritarArtistaResponse>
    {
        private readonly IRepositorioFavoritoArtista _repositorio;

        public FavoritarArtistaCommandHandler(IRepositorioFavoritoArtista repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<FavoritarArtistaResponse> Handle(FavoritarArtistaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var jaEhFavorito = await _repositorio.ArtistaEhFavoritoAsync(request.UsuarioId, request.ArtistaId);

                if (jaEhFavorito)
                {
                    await _repositorio.RemoverFavoritoAsync(request.UsuarioId, request.ArtistaId);
                    await _repositorio.SalvarAsync();
                    return new FavoritarArtistaResponse
                    {
                        Sucesso = true,
                        Mensagem = "Artista removido dos favoritos"
                    };
                }

                var favorito = FavoritoArtista.Create(request.UsuarioId, request.ArtistaId);
                await _repositorio.AdicionarAsync(favorito);
                await _repositorio.SalvarAsync();

                return new FavoritarArtistaResponse
                {
                    Sucesso = true,
                    Mensagem = "Artista adicionado aos favoritos"
                };
            }
            catch (Exception ex)
            {
                return new FavoritarArtistaResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao favoritar artista: {ex.Message}"
                };
            }
        }
    }
}