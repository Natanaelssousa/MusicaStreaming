using MediatR;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Application.UseCases
{
    public class CreateAssinaturaCommand : IRequest<CreateAssinaturaResponse>
    {
        public Guid UsuarioId { get; set; }
        public int PlanType { get; set; } // 1=Free, 2=Premium, 3=Family
    }

    public class CreateAssinaturaResponse
    {
        public Guid Id { get; set; }
        public string PlanType { get; set; }
        public decimal Preco { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class CreateAssinaturaCommandHandler : IRequestHandler<CreateAssinaturaCommand, CreateAssinaturaResponse>
    {
        private readonly IRepositorioAssinatura _repositorio;

        public CreateAssinaturaCommandHandler(IRepositorioAssinatura repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<CreateAssinaturaResponse> Handle(CreateAssinaturaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var planType = (PlanType)request.PlanType;
                var plan = planType switch
                {
                    PlanType.Free => Plan.Free,
                    PlanType.Premium => Plan.Premium,
                    PlanType.Family => Plan.Family,
                    _ => Plan.Free
                };

                var assinatura = Assinatura.Create(request.UsuarioId, plan);
                await _repositorio.AdicionarAsync(assinatura);
                await _repositorio.SalvarAsync();

                return new CreateAssinaturaResponse
                {
                    Id = assinatura.Id,
                    PlanType = plan.Type.ToString(),
                    Preco = plan.Price.Amount,
                    Sucesso = true,
                    Mensagem = "Assinatura criada com sucesso"
                };
            }
            catch (DomainException ex)
            {
                return new CreateAssinaturaResponse
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new CreateAssinaturaResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao criar assinatura: {ex.Message}"
                };
            }
        }
    }
}
