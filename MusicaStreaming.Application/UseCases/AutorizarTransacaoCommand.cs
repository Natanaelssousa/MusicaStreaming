using MediatR;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Application.UseCases
{
    public class AutorizarTransacaoCommand : IRequest<AutorizarTransacaoResponse>
    {
        public Guid UsuarioId { get; set; }
        public Guid CartaoId { get; set; }
        public decimal Valor { get; set; }
        public string Comerciante { get; set; }
    }

    public class AutorizarTransacaoResponse
    {
        public Guid TransacaoId { get; set; }
        public string Status { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class AutorizarTransacaoCommandHandler : IRequestHandler<AutorizarTransacaoCommand, AutorizarTransacaoResponse>
    {
        // Intervalo minimo entre transacoes autorizadas do mesmo cartao
        private static readonly TimeSpan IntervaloMinimo = TimeSpan.FromMinutes(5);

        private readonly IRepositorioTransacao _repositorioTransacao;
        private readonly IRepositorioCartao _repositorioCartao;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IRepositorioAssinatura _repositorioAssinatura;
        private readonly IRepositorioNotificacao _repositorioNotificacao;

        public AutorizarTransacaoCommandHandler(
            IRepositorioTransacao repositorioTransacao,
            IRepositorioCartao repositorioCartao,
            IRepositorioUsuario repositorioUsuario,
            IRepositorioAssinatura repositorioAssinatura,
            IRepositorioNotificacao repositorioNotificacao)
        {
            _repositorioTransacao = repositorioTransacao;
            _repositorioCartao = repositorioCartao;
            _repositorioUsuario = repositorioUsuario;
            _repositorioAssinatura = repositorioAssinatura;
            _repositorioNotificacao = repositorioNotificacao;
        }

        public async Task<AutorizarTransacaoResponse> Handle(AutorizarTransacaoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var valor = new Money(request.Valor, "BRL");
                var transacao = Transacao.Create(request.UsuarioId, request.CartaoId, valor, request.Comerciante);

                // Regra 1: cartao valido?
                var cartao = await _repositorioCartao.ObterPorIdAsync(request.CartaoId);
                if (cartao == null || !cartao.EstaValido())
                {
                    return await NegarEGravar(transacao, "Cartao invalido ou expirado");
                }

                // Regra 2: estado da conta - usuario ativo?
                var usuario = await _repositorioUsuario.ObterPorIdAsync(request.UsuarioId);
                if (usuario == null || usuario.Status != UserStatus.Active)
                {
                    return await NegarEGravar(transacao, "Conta do usuario nao esta ativa");
                }

                // Regra 3: estado da conta - assinatura ativa e nao expirada?
                var assinatura = await _repositorioAssinatura.ObterPorUsuarioAsync(request.UsuarioId);
                if (assinatura == null || assinatura.EstaExpirada())
                {
                    return await NegarEGravar(transacao, "Assinatura inativa ou expirada");
                }

                // Regra 4: intervalo minimo desde a ultima transacao AUTORIZADA do cartao
                var ultima = await _repositorioTransacao.ObterUltimaTransacaoPorCartaoAsync(request.CartaoId);
                if (ultima != null
                    && ultima.Status == TransacaoStatus.Authorized
                    && (DateTime.UtcNow - ultima.DataTransacao) < IntervaloMinimo)
                {
                    return await NegarEGravar(transacao, "Intervalo minimo entre transacoes nao respeitado");
                }

                // Passou em todas as regras -> autoriza
                transacao.Autorizar();
                await _repositorioTransacao.AdicionarAsync(transacao);
                await _repositorioTransacao.SalvarAsync();

                cartao.RegistrarUso();
                await _repositorioCartao.AtualizarAsync(cartao);
                await _repositorioCartao.SalvarAsync();

                // Notificacao do dono do cartao
                var notifDono = Notificacao.Create(
                    transacao.Id,
                    TipoDestinatario.DonoCartao,
                    usuario.Email.Value,
                    $"Sua transacao de {valor} para {transacao.Comerciante} foi autorizada.");

                // Notificacao do comerciante (registro nominal, sem email)
                var notifComerciante = Notificacao.Create(
                    transacao.Id,
                    TipoDestinatario.Comerciante,
                    transacao.Comerciante,
                    $"Transacao de {valor} autorizada.");

                await _repositorioNotificacao.AdicionarAsync(notifDono);
                await _repositorioNotificacao.AdicionarAsync(notifComerciante);
                await _repositorioNotificacao.SalvarAsync();

                return new AutorizarTransacaoResponse
                {
                    TransacaoId = transacao.Id,
                    Status = transacao.Status.ToString(),
                    Sucesso = true,
                    Mensagem = "Transacao autorizada com sucesso"
                };
            }
            catch (DomainException ex)
            {
                return new AutorizarTransacaoResponse
                {
                    Sucesso = false,
                    Status = "Denied",
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new AutorizarTransacaoResponse
                {
                    Sucesso = false,
                    Status = "Denied",
                    Mensagem = $"Erro ao autorizar transacao: {ex.Message}"
                };
            }
        }

        private async Task<AutorizarTransacaoResponse> NegarEGravar(Transacao transacao, string motivo)
        {
            transacao.Negar(motivo);
            await _repositorioTransacao.AdicionarAsync(transacao);
            await _repositorioTransacao.SalvarAsync();

            return new AutorizarTransacaoResponse
            {
                TransacaoId = transacao.Id,
                Status = transacao.Status.ToString(),
                Sucesso = false,
                Mensagem = motivo
            };
        }
    }
}