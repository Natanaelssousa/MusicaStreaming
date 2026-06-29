using System;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class CPFTests
    {
        [Fact]
        public void CreateValidCPF_ShouldSucceed()
        {
            var cpf = new CPF("11144477735");
            cpf.Value.Should().Be("11144477735");
        }

        [Fact]
        public void CPFWithDots_ShouldBeClean()
        {
            var cpf = new CPF("111.444.777-35");
            cpf.Value.Should().Be("11144477735");
        }

        [Fact]
        public void EmptyCPF_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CPF(""));
        }

        [Fact]
        public void AllSameDigitsCPF_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CPF("11111111111"));
        }

        [Theory]
        [InlineData("111")]
        [InlineData("111444777")]
        [InlineData("111444777353")]
        public void InvalidLengthCPF_ShouldThrowException(string cpf)
        {
            Assert.Throws<DomainException>(() => new CPF(cpf));
        }

        [Fact]
        public void TwoCPFsWithSameValue_ShouldBeEqual()
        {
            var cpf1 = new CPF("11144477735");
            var cpf2 = new CPF("11144477735");
            cpf1.Should().Be(cpf2);
        }

        [Fact]
        public void TwoCPFsWithDifferentValues_ShouldNotBeEqual()
        {
            var cpf1 = new CPF("88761448036");
            var cpf2 = new CPF("56069535014");
            cpf1.Should().NotBe(cpf2);
        }

        [Fact]
        public void ToStringCPF_ShouldReturnFormatted()
        {
            var cpf = new CPF("11144477735");
            cpf.ToString().Should().Be("111.444.777-35");
        }
    }
}