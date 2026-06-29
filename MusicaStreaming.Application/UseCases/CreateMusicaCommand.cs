using MediatR;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Application.UseCases
{
    public class CreateMusicaCommand : IRequest<CreateMusicaResponse>
    {
        public Guid ArtistaId { get; set; }
        public string Titulo { get; set; }
        public string Genero { get; set; }
        public int DuracaoSegundos { get; set; }
        public DateTime DataLancamento { get; set; }
    }

    public class CreateMusicaResponse
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class CreateMusicaCommandHandler : IRequestHandler<CreateMusicaCommand, CreateMusicaResponse>
    {
        private readonly IRepositorioMusica _repositorio;

        public CreateMusicaCommandHandler(IRepositorioMusica repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<CreateMusicaResponse> Handle(CreateMusicaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var duracao = new Duration(request.DuracaoSegundos / 60, request.DuracaoSegundos % 60);
                var genero = new Genre(request.Genero);

                var musica = Musica.Create(request.ArtistaId, request.Titulo, genero, duracao, request.DataLancamento);
                await _repositorio.AdicionarAsync(musica);
                await _repositorio.SalvarAsync();

                return new CreateMusicaResponse
                {
                    Id = musica.Id,
                    Titulo = musica.Titulo,
                    Sucesso = true,
                    Mensagem = "Música criada com sucesso"
                };
            }
            catch (DomainException ex)
            {
                return new CreateMusicaResponse
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new CreateMusicaResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao criar música: {ex.Message}"
                };
            }
        }
    }
}