using System;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class TransacaoTests
    {
        [Fact]
        public void CreateValidTransacao_ShouldSucceed()
        {
            var usuarioId = Guid.NewGuid();
            var cartaoId = Guid.NewGuid();
            var valor = new Money(100m, "BRL");

            var transacao = Transacao.Create(usuarioId, cartaoId, valor, "Loja Teste");

            transacao.Id.Should().NotBe(Guid.Empty);
            transacao.Status.Should().Be(TransacaoStatus.Pending);
            transacao.NumeroTentativas.Should().Be(0);
            transacao.Comerciante.Should().Be("Loja Teste");
        }

        [Fact]
        public void CreateTransacaoWithoutUsuario_ShouldThrowException()
        {
            var cartaoId = Guid.NewGuid();
            var valor = new Money(100m, "BRL");

            Assert.Throws<DomainException>(() =>
                Transacao.Create(Guid.Empty, cartaoId, valor, "Loja Teste"));
        }

        [Fact]
        public void CreateTransacaoWithoutComerciante_ShouldThrowException()
        {
            var usuarioId = Guid.NewGuid();
            var cartaoId = Guid.NewGuid();
            var valor = new Money(100m, "BRL");

            Assert.Throws<DomainException>(() =>
                Transacao.Create(usuarioId, cartaoId, valor, ""));
        }

        [Fact]
        public void Autorizar_ShouldChangeStatus()
        {
            var usuarioId = Guid.NewGuid();
            var cartaoId = Guid.NewGuid();
            var valor = new Money(100m, "BRL");
            var transacao = Transacao.Create(usuarioId, cartaoId, valor, "Loja Teste");

            transacao.Autorizar();

            transacao.Status.Should().Be(TransacaoStatus.Authorized);
            transacao.DataAutorizacao.Should().NotBeNull();
        }

        [Fact]
        public void Negar_ShouldChangeStatus()
        {
            var usuarioId = Guid.NewGuid();
            var cartaoId = Guid.NewGuid();
            var valor = new Money(100m, "BRL");
            var transacao = Transacao.Create(usuarioId, cartaoId, valor, "Loja Teste");

            transacao.Negar("Saldo insuficiente");

            transacao.Status.Should().Be(TransacaoStatus.Denied);
            transacao.MotivoNegacao.Should().Be("Saldo insuficiente");
        }

        [Fact]
        public void Concluir_ShouldChangeStatus()
        {
            var usuarioId = Guid.NewGuid();
            var cartaoId = Guid.NewGuid();
            var valor = new Money(100m, "BRL");
            var transacao = Transacao.Create(usuarioId, cartaoId, valor, "Loja Teste");
            transacao.Autorizar();

            transacao.Concluir();

            transacao.Status.Should().Be(TransacaoStatus.Completed);
        }

        [Fact]
        public void Cancelar_ShouldChangeStatus()
        {
            var usuarioId = Guid.NewGuid();
            var cartaoId = Guid.NewGuid();
            var valor = new Money(100m, "BRL");
            var transacao = Transacao.Create(usuarioId, cartaoId, valor, "Loja Teste");

            transacao.Cancelar();

            transacao.Status.Should().Be(TransacaoStatus.Cancelled);
        }

        [Fact]
        public void IncrementarTentativa_ShouldSucceed()
        {
            var usuarioId = Guid.NewGuid();
            var cartaoId = Guid.NewGuid();
            var valor = new Money(100m, "BRL");
            var transacao = Transacao.Create(usuarioId, cartaoId, valor, "Loja Teste");

            transacao.IncrementarTentativa();

            transacao.NumeroTentativas.Should().Be(1);
        }
    }
}