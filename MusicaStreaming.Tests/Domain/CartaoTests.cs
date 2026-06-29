using System;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class CartaoTests
    {
        [Fact]
        public void CreateValidCartao_ShouldSucceed()
        {
            var usuarioId = Guid.NewGuid();
            var cartao = Cartao.Create(usuarioId, "4532015112830366", 12, DateTime.Now.Year + 2, "123", "João Silva");

            cartao.Id.Should().NotBe(Guid.Empty);
            cartao.UsuarioId.Should().Be(usuarioId);
            cartao.Status.Should().Be(CartaoStatus.Active);
        }

        [Fact]
        public void CreateCartaoWithoutUsuario_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() =>
                Cartao.Create(Guid.Empty, "4532015112830366", 12, DateTime.Now.Year + 2, "123", "João Silva"));
        }

        [Fact]
        public void RegistrarUso_ShouldUpdateDataUltimoUso()
        {
            var usuarioId = Guid.NewGuid();
            var cartao = Cartao.Create(usuarioId, "4532015112830366", 12, DateTime.Now.Year + 2, "123", "João Silva");

            cartao.RegistrarUso();

            cartao.DataUltimoUso.Should().NotBeNull();
        }

        [Fact]
        public void Desativar_ShouldChangeStatus()
        {
            var usuarioId = Guid.NewGuid();
            var cartao = Cartao.Create(usuarioId, "4532015112830366", 12, DateTime.Now.Year + 2, "123", "João Silva");

            cartao.Desativar();

            cartao.Status.Should().Be(CartaoStatus.Inactive);
        }

        [Fact]
        public void Reativar_ShouldChangeStatus()
        {
            var usuarioId = Guid.NewGuid();
            var cartao = Cartao.Create(usuarioId, "4532015112830366", 12, DateTime.Now.Year + 2, "123", "João Silva");
            cartao.Desativar();

            cartao.Reativar();

            cartao.Status.Should().Be(CartaoStatus.Active);
        }

        [Fact]
        public void EstaValido_ShouldReturnTrue()
        {
            var usuarioId = Guid.NewGuid();
            var cartao = Cartao.Create(usuarioId, "4532015112830366", 12, DateTime.Now.Year + 2, "123", "João Silva");

            cartao.EstaValido().Should().BeTrue();
        }
    }
}