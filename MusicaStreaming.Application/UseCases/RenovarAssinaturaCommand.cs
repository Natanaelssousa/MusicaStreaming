using MediatR;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Application.UseCases
{
    public class RenovarAssinaturaCommand : IRequest<RenovarAssinaturaResponse>
    {
        public Guid UsuarioId { get; set; }
    }

    public class RenovarAssinaturaResponse
    {
        public DateTime NovaDataFim { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class RenovarAssinaturaCommandHandler : IRequestHandler<RenovarAssinaturaCommand, RenovarAssinaturaResponse>
    {
        private readonly IRepositorioAssinatura _repositorio;

        public RenovarAssinaturaCommandHandler(IRepositorioAssinatura repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<RenovarAssinaturaResponse> Handle(RenovarAssinaturaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var assinatura = await _repositorio.ObterPorUsuarioAsync(request.UsuarioId);

                if (assinatura == null)
                {
                    return new RenovarAssinaturaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Assinatura não encontrada"
                    };
                }

                assinatura.Renovar();
                await _repositorio.AtualizarAsync(assinatura);
                await _repositorio.SalvarAsync();

                return new RenovarAssinaturaResponse
                {
                    NovaDataFim = assinatura.DataFim,
                    Sucesso = true,
                    Mensagem = "Assinatura renovada com sucesso"
                };
            }
            catch (DomainException ex)
            {
                return new RenovarAssinaturaResponse
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RenovarAssinaturaResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao renovar assinatura: {ex.Message}"
                };
            }
        }
    }
}