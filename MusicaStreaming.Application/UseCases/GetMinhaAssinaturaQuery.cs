using MediatR;
using MusicaStreaming.Application.Abstractions;

namespace MusicaStreaming.Application.UseCases
{
    public class GetMinhaAssinaturaQuery : IRequest<GetMinhaAssinaturaResponse>
    {
        public Guid UsuarioId { get; set; }
    }

    public class GetMinhaAssinaturaResponse
    {
        public bool TemAssinatura { get; set; }
        public string PlanType { get; set; }
        public decimal Preco { get; set; }
        public string Status { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool EstaExpirada { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class GetMinhaAssinaturaQueryHandler : IRequestHandler<GetMinhaAssinaturaQuery, GetMinhaAssinaturaResponse>
    {
        private readonly IRepositorioAssinatura _repositorio;

        public GetMinhaAssinaturaQueryHandler(IRepositorioAssinatura repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<GetMinhaAssinaturaResponse> Handle(GetMinhaAssinaturaQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var assinatura = await _repositorio.ObterPorUsuarioAsync(request.UsuarioId);

                if (assinatura == null)
                {
                    return new GetMinhaAssinaturaResponse
                    {
                        TemAssinatura = false,
                        Sucesso = true,
                        Mensagem = "Usuario nao possui assinatura ativa"
                    };
                }

                return new GetMinhaAssinaturaResponse
                {
                    TemAssinatura = true,
                    PlanType = assinatura.Plan.Type.ToString(),
                    Preco = assinatura.Plan.Price.Amount,
                    Status = assinatura.Status.ToString(),
                    DataInicio = assinatura.DataInicio,
                    DataFim = assinatura.DataFim,
                    EstaExpirada = assinatura.EstaExpirada(),
                    Sucesso = true,
                    Mensagem = "Assinatura encontrada"
                };
            }
            catch (Exception ex)
            {
                return new GetMinhaAssinaturaResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao consultar assinatura: {ex.Message}"
                };
            }
        }
    }
}