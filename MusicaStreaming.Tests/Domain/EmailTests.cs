using System;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{

    public class EmailTests
    {

        [Fact]
        public void CriarEmailValido_DeveComSucesso()
        {

            var emailValue = "usuario@example.com";

            var email = new Email(emailValue);

            email.Value.Should().Be("usuario@example.com");
        }

        [Fact]
        public void Email_DeveSerArmazenadoEmMinusculas()
        {
            var emailValue = "Usuario@Example.COM";

            var email = new Email(emailValue);

            email.Value.Should().Be("usuario@example.com");
        }

        [Fact]
        public void CriarEmailVazio_DeveLanguarException()
        {
            var emailValue = "";

            Assert.Throws<DomainException>(() => new Email(emailValue));
        }

        [Fact]
        public void CriarEmailComApenasEspacos_DeveLanguarException()
        {
            var emailValue = "   ";

            Assert.Throws<DomainException>(() => new Email(emailValue));
        }

        [Theory]
        [InlineData("invalido")]
        [InlineData("invalido@")]
        [InlineData("@example.com")]
        [InlineData("usuario@.com")]
        [InlineData("usuario @example.com")]
        public void CriarEmailInvalido_DeveLanguarException(string emailInvalido)
        {
            Assert.Throws<DomainException>(() => new Email(emailInvalido));
        }


        [Fact]
        public void DoisEmailsComMesmoValor_DevemSerIguais()
        {
            var email1 = new Email("usuario@example.com");
            var email2 = new Email("usuario@example.com");

            email1.Should().Be(email2);
        }


        [Fact]
        public void DoisEmailsComValoresDiferentes_NaoDevemSerIguais()
        {

            var email1 = new Email("usuario1@example.com");
            var email2 = new Email("usuario2@example.com");


            email1.Should().NotBe(email2);
        }

        [Fact]
        public void EmailsComMesmoValor_DevemTerMesmoHashCode()
        {
            var email1 = new Email("usuario@example.com");
            var email2 = new Email("usuario@example.com");


            var hash1 = email1.GetHashCode();
            var hash2 = email2.GetHashCode();

            hash1.Should().Be(hash2);
        }


        [Fact]
        public void ToString_DeveRetornarValorDoEmail()
        {
            var email = new Email("usuario@example.com");

            var result = email.ToString();

            result.Should().Be("usuario@example.com");
        }
    }
}