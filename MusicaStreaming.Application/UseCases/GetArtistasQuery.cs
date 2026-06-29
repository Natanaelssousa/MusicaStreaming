using MediatR;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Application.UseCases
{
    public class GetArtistasQuery : IRequest<GetArtistasResponse>
    {
        public string Termo { get; set; }
    }

    public class GetArtistasResponse
    {
        public List<ArtistaDto> Artistas { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class ArtistaDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Biografia { get; set; }
    }

    public class GetArtistasQueryHandler : IRequestHandler<GetArtistasQuery, GetArtistasResponse>
    {
        private readonly IRepositorioArtista _repositorio;

        public GetArtistasQueryHandler(IRepositorioArtista repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<GetArtistasResponse> Handle(GetArtistasQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Artista> artistas;

                if (string.IsNullOrWhiteSpace(request.Termo))
                    artistas = await _repositorio.ObterArtistasAtivosAsync();
                else
                    artistas = await _repositorio.BuscarPorNomeAsync(request.Termo);

                var dtos = artistas.Select(a => new ArtistaDto
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    Biografia = a.Biografia
                }).ToList();

                return new GetArtistasResponse
                {
                    Artistas = dtos,
                    Sucesso = true,
                    Mensagem = $"{dtos.Count} artistas encontrados"
                };
            }
            catch (Exception ex)
            {
                return new GetArtistasResponse
                {
                    Artistas = new List<ArtistaDto>(),
                    Sucesso = false,
                    Mensagem = $"Erro ao buscar artistas: {ex.Message}"
                };
            }
        }
    }
}
