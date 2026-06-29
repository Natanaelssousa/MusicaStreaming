using System;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class NotificacaoTests
    {
        [Fact]
        public void CreateValidNotificacao_ShouldSucceed()
        {
            var transacaoId = Guid.NewGuid();

            var n = Notificacao.Create(transacaoId, TipoDestinatario.DonoCartao,
                "joao@example.com", "Transacao autorizada");

            n.Id.Should().NotBe(Guid.Empty);
            n.Status.Should().Be(NotificacaoStatus.Pendente);
            n.Destinatario.Should().Be("joao@example.com");
        }

        [Fact]
        public void CreateNotificacaoWithoutDestinatario_ShouldThrowException()
        {
            var transacaoId = Guid.NewGuid();

            Assert.Throws<DomainException>(() =>
                Notificacao.Create(transacaoId, TipoDestinatario.Comerciante, "", "conteudo"));
        }

        [Fact]
        public void MarcarComoEnviada_ShouldChangeStatus()
        {
            var n = Notificacao.Create(Guid.NewGuid(), TipoDestinatario.DonoCartao,
                "joao@example.com", "Transacao autorizada");

            n.MarcarComoEnviada();

            n.Status.Should().Be(NotificacaoStatus.Enviada);
            n.DataEnvio.Should().NotBeNull();
        }

        [Fact]
        public void MarcarComoEnviadaDuasVezes_ShouldThrowException()
        {
            var n = Notificacao.Create(Guid.NewGuid(), TipoDestinatario.DonoCartao,
                "joao@example.com", "Transacao autorizada");
            n.MarcarComoEnviada();

            Assert.Throws<DomainException>(() => n.MarcarComoEnviada());
        }
    }
}