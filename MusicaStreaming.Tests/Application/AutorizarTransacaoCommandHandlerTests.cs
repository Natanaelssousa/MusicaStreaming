using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Application.UseCases;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;
using Xunit;

namespace MusicaStreaming.Tests.Application
{
    public class AutorizarTransacaoCommandHandlerTests
    {
        private readonly Mock<IRepositorioTransacao> _transacaoRepo = new();
        private readonly Mock<IRepositorioCartao> _cartaoRepo = new();
        private readonly Mock<IRepositorioUsuario> _usuarioRepo = new();
        private readonly Mock<IRepositorioAssinatura> _assinaturaRepo = new();
        private readonly Mock<IRepositorioNotificacao> _notificacaoRepo = new();

        private readonly Guid _usuarioId = Guid.NewGuid();
        private readonly Guid _cartaoId = Guid.NewGuid();

        private AutorizarTransacaoCommandHandler CriarHandler() =>
      new AutorizarTransacaoCommandHandler(
          _transacaoRepo.Object,
          _cartaoRepo.Object,
          _usuarioRepo.Object,
          _assinaturaRepo.Object,
          _notificacaoRepo.Object);

        private AutorizarTransacaoCommand CriarComando() =>
        new AutorizarTransacaoCommand
        {
            UsuarioId = _usuarioId,
            CartaoId = _cartaoId,
            Valor = 100m,
            Comerciante = "Loja Teste"
        };

        private Cartao CriarCartaoValido() =>
            Cartao.Create(_usuarioId, "4532015112830366", 12, DateTime.Now.Year + 2, "123", "Joao Silva");

        private Usuario CriarUsuarioAtivo() =>
            Usuario.Create("joao@example.com", "Joao Silva", "11144477735", "senha123456");

        private Assinatura CriarAssinaturaAtiva() =>
            Assinatura.Create(_usuarioId, Plan.Premium);



        [Fact]
        public async Task Autorizar_ComTudoValido_DeveAutorizar()
        {
            _cartaoRepo.Setup(r => r.ObterPorIdAsync(_cartaoId)).ReturnsAsync(CriarCartaoValido());
            _usuarioRepo.Setup(r => r.ObterPorIdAsync(_usuarioId)).ReturnsAsync(CriarUsuarioAtivo());
            _assinaturaRepo.Setup(r => r.ObterPorUsuarioAsync(_usuarioId)).ReturnsAsync(CriarAssinaturaAtiva());
            _transacaoRepo.Setup(r => r.ObterUltimaTransacaoPorCartaoAsync(_cartaoId))
                .ReturnsAsync((Transacao)null);

            _notificacaoRepo.Setup(r => r.AdicionarAsync(It.IsAny<Notificacao>()))
                .Returns(Task.CompletedTask);
            _notificacaoRepo.Setup(r => r.SalvarAsync())
                .Returns(Task.CompletedTask);

            var resultado = await CriarHandler().Handle(CriarComando(), CancellationToken.None);

            resultado.Sucesso.Should().BeTrue();
            resultado.Status.Should().Be(TransacaoStatus.Authorized.ToString());
            _transacaoRepo.Verify(r => r.SalvarAsync(), Times.AtLeastOnce);
            _notificacaoRepo.Verify(r => r.AdicionarAsync(It.IsAny<Notificacao>()), Times.Exactly(2));
            resultado.Sucesso.Should().BeTrue(because: resultado.Mensagem);
        }


        [Fact]
        public async Task Autorizar_CartaoInexistente_DeveNegar()
        {
            _cartaoRepo.Setup(r => r.ObterPorIdAsync(_cartaoId)).ReturnsAsync((Cartao)null);

            var resultado = await CriarHandler().Handle(CriarComando(), CancellationToken.None);

            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Cartao invalido");
            resultado.Status.Should().Be(TransacaoStatus.Denied.ToString());
        }

       

        [Fact]
        public async Task Autorizar_UsuarioSuspenso_DeveNegar()
        {
            var usuarioSuspenso = CriarUsuarioAtivo();
            usuarioSuspenso.Suspender("teste");

            _cartaoRepo.Setup(r => r.ObterPorIdAsync(_cartaoId)).ReturnsAsync(CriarCartaoValido());
            _usuarioRepo.Setup(r => r.ObterPorIdAsync(_usuarioId)).ReturnsAsync(usuarioSuspenso);

            var resultado = await CriarHandler().Handle(CriarComando(), CancellationToken.None);

            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Conta do usuario");
        }

     

        [Fact]
        public async Task Autorizar_SemAssinatura_DeveNegar()
        {
            _cartaoRepo.Setup(r => r.ObterPorIdAsync(_cartaoId)).ReturnsAsync(CriarCartaoValido());
            _usuarioRepo.Setup(r => r.ObterPorIdAsync(_usuarioId)).ReturnsAsync(CriarUsuarioAtivo());
            _assinaturaRepo.Setup(r => r.ObterPorUsuarioAsync(_usuarioId)).ReturnsAsync((Assinatura)null);

            var resultado = await CriarHandler().Handle(CriarComando(), CancellationToken.None);

            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Assinatura");
        }

     

        [Fact]
        public async Task Autorizar_UltimaTransacaoRecente_DeveNegar()
        {
            // Ultima transacao autorizada agora mesmo -> dentro dos 5 min
            var ultima = Transacao.Create(_usuarioId, _cartaoId, new Money(50m, "BRL"), "Loja Teste");
            ultima.Autorizar();

            _cartaoRepo.Setup(r => r.ObterPorIdAsync(_cartaoId)).ReturnsAsync(CriarCartaoValido());
            _usuarioRepo.Setup(r => r.ObterPorIdAsync(_usuarioId)).ReturnsAsync(CriarUsuarioAtivo());
            _assinaturaRepo.Setup(r => r.ObterPorUsuarioAsync(_usuarioId)).ReturnsAsync(CriarAssinaturaAtiva());
            _transacaoRepo.Setup(r => r.ObterUltimaTransacaoPorCartaoAsync(_cartaoId)).ReturnsAsync(ultima);

            var resultado = await CriarHandler().Handle(CriarComando(), CancellationToken.None);

            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Intervalo minimo");
        }

        [Fact]
        public async Task Autorizar_UltimaTransacaoNegada_NaoBloqueia()
        {
            // Ultima transacao foi NEGADA (nao autorizada) -> nao deve bloquear
            var ultimaNegada = Transacao.Create(_usuarioId, _cartaoId, new Money(50m, "BRL"), "Loja Teste");
            ultimaNegada.Negar("qualquer motivo");

            _cartaoRepo.Setup(r => r.ObterPorIdAsync(_cartaoId)).ReturnsAsync(CriarCartaoValido());
            _usuarioRepo.Setup(r => r.ObterPorIdAsync(_usuarioId)).ReturnsAsync(CriarUsuarioAtivo());
            _assinaturaRepo.Setup(r => r.ObterPorUsuarioAsync(_usuarioId)).ReturnsAsync(CriarAssinaturaAtiva());
            _transacaoRepo.Setup(r => r.ObterUltimaTransacaoPorCartaoAsync(_cartaoId)).ReturnsAsync(ultimaNegada);

            _notificacaoRepo.Setup(r => r.AdicionarAsync(It.IsAny<Notificacao>()))
                .Returns(Task.CompletedTask);
            _notificacaoRepo.Setup(r => r.SalvarAsync())
                .Returns(Task.CompletedTask);

            var resultado = await CriarHandler().Handle(CriarComando(), CancellationToken.None);

            resultado.Sucesso.Should().BeTrue();
            resultado.Status.Should().Be(TransacaoStatus.Authorized.ToString());

        }
    }
}