using System;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class UsuarioTests
    {
        [Fact]
        public void CreateValidUsuario_ShouldSucceed()
        {
            var usuario = Usuario.Create("usuario@example.com", "João Silva", "11144477735", "senha123456");

            usuario.Id.Should().NotBe(Guid.Empty);
            usuario.Email.Value.Should().Be("usuario@example.com");
            usuario.Nome.Should().Be("João Silva");
            usuario.Status.Should().Be(UserStatus.Active);
            usuario.EmailConfirmado.Should().BeFalse();
        }

        [Fact]
        public void CreateUsuarioWithoutEmail_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() =>
                Usuario.Create("", "João Silva", "11144477735", "senha123456"));
        }

        [Fact]
        public void CreateUsuarioWithShortPassword_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() =>
                Usuario.Create("usuario@example.com", "João Silva", "11144477735", "123"));
        }

        [Fact]
        public void ConfirmarEmail_ShouldSucceed()
        {
            var usuario = Usuario.Create("usuario@example.com", "João Silva", "11144477735", "senha123456");
            usuario.ConfirmarEmail();

            usuario.EmailConfirmado.Should().BeTrue();
        }

        [Fact]
        public void ConfirmarEmailDuplicated_ShouldThrowException()
        {
            var usuario = Usuario.Create("usuario@example.com", "João Silva", "11144477735", "senha123456");
            usuario.ConfirmarEmail();

            Assert.Throws<DomainException>(() => usuario.ConfirmarEmail());
        }

        [Fact]
        public void FazerLoginWithoutEmailConfirmation_ShouldThrowException()
        {
            var usuario = Usuario.Create("usuario@example.com", "João Silva", "11144477735", "senha123456");

            Assert.Throws<DomainException>(() => usuario.FazerLogin("senha123456"));
        }

        [Fact]
        public void FazerLoginWithWrongPassword_ShouldThrowException()
        {
            var usuario = Usuario.Create("usuario@example.com", "João Silva", "11144477735", "senha123456");
            usuario.ConfirmarEmail();

            Assert.Throws<DomainException>(() => usuario.FazerLogin("senhaerrada"));
        }

        [Fact]
        public void FazerLoginSuccessful_ShouldUpdateLastLogin()
        {
            var usuario = Usuario.Create("usuario@example.com", "João Silva", "11144477735", "senha123456");
            usuario.ConfirmarEmail();

            usuario.FazerLogin("senha123456");

            usuario.DataUltimoLogin.Should().NotBeNull();
        }

        [Fact]
        public void Suspender_ShouldChangeStatus()
        {
            var usuario = Usuario.Create("usuario@example.com", "João Silva", "11144477735", "senha123456");
            usuario.Suspender("Teste");

            usuario.Status.Should().Be(UserStatus.Suspended);
        }

        [Fact]
        public void Reativar_ShouldChangeStatus()
        {
            var usuario = Usuario.Create("usuario@example.com", "João Silva", "11144477735", "senha123456");
            usuario.Suspender("Teste");
            usuario.Reativar();

            usuario.Status.Should().Be(UserStatus.Active);
        }
    }
}
