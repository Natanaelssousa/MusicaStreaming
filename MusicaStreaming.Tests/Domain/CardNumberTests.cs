using System;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class CardNumberTests
    {
        [Fact]
        public void CreateValidCardNumber_ShouldSucceed()
        {
            var cardNumber = new CardNumber("4532015112830366");
            cardNumber.Value.Should().Be("4532015112830366");
        }

        [Fact]
        public void CardNumberWithSpaces_ShouldBeClean()
        {
            var cardNumber = new CardNumber("4532 0151 1283 0366");
            cardNumber.Value.Should().Be("4532015112830366");
        }

        [Fact]
        public void CardNumberWithDashes_ShouldBeClean()
        {
            var cardNumber = new CardNumber("4532-0151-1283-0366");
            cardNumber.Value.Should().Be("4532015112830366");
        }

        [Fact]
        public void EmptyCardNumber_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CardNumber(""));
        }

        [Fact]
        public void InvalidCardNumber_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CardNumber("1234567890123456"));
        }

        [Fact]
        public void CardNumberTooShort_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CardNumber("123456"));
        }

        [Fact]
        public void GetMaskedCardNumber_ShouldReturnMasked()
        {
            var cardNumber = new CardNumber("4532015112830366");
            cardNumber.GetMasked().Should().Be("**** **** **** 0366");
        }

        [Fact]
        public void GetLastFourDigits_ShouldReturn()
        {
            var cardNumber = new CardNumber("4532015112830366");
            cardNumber.GetLastFourDigits().Should().Be("0366");
        }

        [Fact]
        public void ToStringCardNumber_ShouldReturnMasked()
        {
            var cardNumber = new CardNumber("4532015112830366");
            cardNumber.ToString().Should().Be("**** **** **** 0366");
        }
    }
}
