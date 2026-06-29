using System;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class AssinaturaTests
    {
        [Fact]
        public void CreateValidAssinatura_ShouldSucceed()
        {
            var usuarioId = Guid.NewGuid();
            var assinatura = Assinatura.Create(usuarioId, Plan.Premium);

            assinatura.Id.Should().NotBe(Guid.Empty);
            assinatura.UsuarioId.Should().Be(usuarioId);
            assinatura.Plan.Should().Be(Plan.Premium);
            assinatura.Status.Should().Be(SubscriptionStatus.Active);
        }

        [Fact]
        public void CreateAssinaturaWithoutUsuario_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() =>
                Assinatura.Create(Guid.Empty, Plan.Premium));
        }

        [Fact]
        public void CreateAssinaturaWithoutPlan_ShouldThrowException()
        {
            var usuarioId = Guid.NewGuid();
            Assert.Throws<DomainException>(() =>
                Assinatura.Create(usuarioId, null));
        }

        [Fact]
        public void Renovar_ShouldUpdateDataFim()
        {
            var usuarioId = Guid.NewGuid();
            var assinatura = Assinatura.Create(usuarioId, Plan.Premium);
            var dataFimAnterior = assinatura.DataFim;

            System.Threading.Thread.Sleep(100);
            assinatura.Renovar();

            assinatura.DataFim.Should().BeAfter(dataFimAnterior);
            assinatura.DataRenovacao.Should().NotBeNull();
        }

        [Fact]
        public void RenovarCancelledAssinatura_ShouldThrowException()
        {
            var usuarioId = Guid.NewGuid();
            var assinatura = Assinatura.Create(usuarioId, Plan.Premium);
            assinatura.Cancelar("Teste");

            Assert.Throws<DomainException>(() => assinatura.Renovar());
        }

        [Fact]
        public void Cancelar_ShouldChangeStatus()
        {
            var usuarioId = Guid.NewGuid();
            var assinatura = Assinatura.Create(usuarioId, Plan.Premium);
            assinatura.Cancelar("Teste");

            assinatura.Status.Should().Be(SubscriptionStatus.Cancelled);
            assinatura.RenovacaoAutomatica.Should().BeFalse();
        }

        [Fact]
        public void AlterarPlan_ShouldSucceed()
        {
            var usuarioId = Guid.NewGuid();
            var assinatura = Assinatura.Create(usuarioId, Plan.Premium);
            assinatura.AlterarPlan(Plan.Family);

            assinatura.Plan.Should().Be(Plan.Family);
        }

        [Fact]
        public void EstaProximaDeExpirar_ShouldReturnFalse_WhenMoreThan7Days()
        {
            var usuarioId = Guid.NewGuid();
            var assinatura = Assinatura.Create(usuarioId, Plan.Premium);

            var resultado = assinatura.EstaProximaDeExpirar();

            resultado.Should().BeFalse();
        }
    }
}
