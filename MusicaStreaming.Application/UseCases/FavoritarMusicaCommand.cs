using MediatR;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Application.UseCases
{
    public class FavoritarMusicaCommand : IRequest<FavoritarMusicaResponse>
    {
        public Guid UsuarioId { get; set; }
        public Guid MusicaId { get; set; }
    }

    public class FavoritarMusicaResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class FavoritarMusicaCommandHandler : IRequestHandler<FavoritarMusicaCommand, FavoritarMusicaResponse>
    {
        private readonly IRepositorioFavorito _repositorio;

        public FavoritarMusicaCommandHandler(IRepositorioFavorito repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<FavoritarMusicaResponse> Handle(FavoritarMusicaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var jEhFavorita = await _repositorio.MusicaEhFavoritaAsync(request.UsuarioId, request.MusicaId);

                if (jEhFavorita)
                {
                    await _repositorio.RemoverFavoritoAsync(request.UsuarioId, request.MusicaId);
                    return new FavoritarMusicaResponse
                    {
                        Sucesso = true,
                        Mensagem = "Música removida dos favoritos"
                    };
                }

                var favorito = Favorito.Create(request.UsuarioId, request.MusicaId);
                await _repositorio.AdicionarAsync(favorito);
                await _repositorio.SalvarAsync();

                return new FavoritarMusicaResponse
                {
                    Sucesso = true,
                    Mensagem = "Música adicionada aos favoritos"
                };
            }
            catch (Exception ex)
            {
                return new FavoritarMusicaResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao favoritar música: {ex.Message}"
                };
            }
        }
    }
}
