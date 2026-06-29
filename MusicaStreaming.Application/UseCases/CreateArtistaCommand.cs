using MediatR;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Application.UseCases
{
    public class CreateArtistaCommand : IRequest<CreateArtistaResponse>
    {
        public string Nome { get; set; }
        public string Biografia { get; set; }
    }

    public class CreateArtistaResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class CreateArtistaCommandHandler : IRequestHandler<CreateArtistaCommand, CreateArtistaResponse>
    {
        private readonly IRepositorioArtista _repositorio;

        public CreateArtistaCommandHandler(IRepositorioArtista repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<CreateArtistaResponse> Handle(CreateArtistaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var nomeJaExiste = await _repositorio.NomeJaExisteAsync(request.Nome);
                if (nomeJaExiste)
                {
                    return new CreateArtistaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Artista com este nome já existe"
                    };
                }

                var artista = Artista.Create(request.Nome, request.Biografia);
                await _repositorio.AdicionarAsync(artista);
                await _repositorio.SalvarAsync();

                return new CreateArtistaResponse
                {
                    Id = artista.Id,
                    Nome = artista.Nome,
                    Sucesso = true,
                    Mensagem = "Artista criado com sucesso"
                };
            }
            catch (DomainException ex)
            {
                return new CreateArtistaResponse
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new CreateArtistaResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao criar artista: {ex.Message}"
                };
            }
        }
    }
}