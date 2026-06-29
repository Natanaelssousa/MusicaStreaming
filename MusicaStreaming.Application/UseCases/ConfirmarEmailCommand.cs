using MediatR;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Application.UseCases
{
    public class ConfirmarEmailCommand : IRequest<ConfirmarEmailResponse>
    {
        public Guid UsuarioId { get; set; }
    }

    public class ConfirmarEmailResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class ConfirmarEmailCommandHandler : IRequestHandler<ConfirmarEmailCommand, ConfirmarEmailResponse>
    {
        private readonly IRepositorioUsuario _repositorio;

        public ConfirmarEmailCommandHandler(IRepositorioUsuario repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<ConfirmarEmailResponse> Handle(ConfirmarEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await _repositorio.ObterPorIdAsync(request.UsuarioId);

                if (usuario == null)
                {
                    return new ConfirmarEmailResponse
                    {
                        Sucesso = false,
                        Mensagem = "Usuário não encontrado"
                    };
                }

                usuario.ConfirmarEmail();
                await _repositorio.SalvarAsync(); 

                return new ConfirmarEmailResponse
                {
                    Sucesso = true,
                    Mensagem = "Email confirmado com sucesso"
                };
            }
            catch (DomainException ex)
            {
                return new ConfirmarEmailResponse
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ConfirmarEmailResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao confirmar email: {ex.Message}"
                };
            }
        }
    }
}