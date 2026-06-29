using MediatR;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Application.UseCases
{
    public class GetMusicasQuery : IRequest<GetMusicasResponse>
    {
        public string Termo { get; set; }
    }

    public class GetMusicasResponse
    {
        public List<MusicaDto> Musicas { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class MusicaDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Artista { get; set; }
        public string Genero { get; set; }
        public int DuracaoSegundos { get; set; }
        public int NumeroReproducoes { get; set; }
    }

    public class GetMusicasQueryHandler : IRequestHandler<GetMusicasQuery, GetMusicasResponse>
    {
        private readonly IRepositorioMusica _repositorioMusica;
        private readonly IRepositorioArtista _repositorioArtista;

        public GetMusicasQueryHandler(IRepositorioMusica repositorioMusica, IRepositorioArtista repositorioArtista)
        {
            _repositorioMusica = repositorioMusica;
            _repositorioArtista = repositorioArtista;
        }

        public async Task<GetMusicasResponse> Handle(GetMusicasQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Musica> musicas;

                if (string.IsNullOrWhiteSpace(request.Termo))
                {
                    musicas = await _repositorioMusica.ObterMusicasDisponiveisAsync();
                }
                else
                {
                    musicas = await _repositorioMusica.BuscarPorTituloAsync(request.Termo);
                }

                var musicasDtos = new List<MusicaDto>();

                foreach (var musica in musicas)
                {
                    var artista = await _repositorioArtista.ObterPorIdAsync(musica.ArtistaId);
                    musicasDtos.Add(new MusicaDto
                    {
                        Id = musica.Id,
                        Titulo = musica.Titulo,
                        Artista = artista?.Nome ?? "Desconhecido",
                        Genero = musica.Genero.Name,
                        DuracaoSegundos = musica.Duracao.TotalSeconds,
                        NumeroReproducoes = musica.NumeroReproducoes
                    });
                }

                return new GetMusicasResponse
                {
                    Musicas = musicasDtos,
                    Sucesso = true,
                    Mensagem = $"{musicasDtos.Count} músicas encontradas"
                };
            }
            catch (Exception ex)
            {
                return new GetMusicasResponse
                {
                    Musicas = new List<MusicaDto>(),
                    Sucesso = false,
                    Mensagem = $"Erro ao buscar músicas: {ex.Message}"
                };
            }
        }
    }
}