using System;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class CVVTests
    {
        [Fact]
        public void CreateValidCVV3Digits_ShouldSucceed()
        {
            var cvv = new CVV("123");
            cvv.Value.Should().Be("123");
        }

        [Fact]
        public void CreateValidCVV4Digits_ShouldSucceed()
        {
            var cvv = new CVV("1234");
            cvv.Value.Should().Be("1234");
        }

        [Fact]
        public void EmptyCVV_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CVV(""));
        }

        [Fact]
        public void CVVTooShort_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CVV("12"));
        }

        [Fact]
        public void CVVTooLong_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CVV("12345"));
        }

        [Fact]
        public void CVVWithLetters_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CVV("12A"));
        }

        [Fact]
        public void ToStringCVV_ShouldReturnMasked()
        {
            var cvv = new CVV("123");
            cvv.ToString().Should().Be("***");
        }

        [Fact]
        public void TwoCVVsWithSameValue_ShouldBeEqual()
        {
            var cvv1 = new CVV("123");
            var cvv2 = new CVV("123");
            cvv1.Should().Be(cvv2);
        }
    }
}