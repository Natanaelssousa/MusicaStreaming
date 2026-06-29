using System;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class MoneyTests
    {
        [Fact]
        public void CreateValidMoney_ShouldSucceed()
        {
            var money = new Money(100.50m, "BRL");
            money.Amount.Should().Be(100.50m);
            money.Currency.Should().Be("BRL");
        }

        [Fact]
        public void MoneyDefaultCurrency_ShouldBeBRL()
        {
            var money = new Money(50m);
            money.Currency.Should().Be("BRL");
        }

        [Fact]
        public void NegativeMoney_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new Money(-10m));
        }

        [Fact]
        public void AddMoneySameCurrency_ShouldSucceed()
        {
            var money1 = new Money(100m, "BRL");
            var money2 = new Money(50m, "BRL");
            var result = money1.Add(money2);
            result.Amount.Should().Be(150m);
        }

        [Fact]
        public void AddMoneyDifferentCurrency_ShouldThrowException()
        {
            var money1 = new Money(100m, "BRL");
            var money2 = new Money(50m, "USD");
            Assert.Throws<DomainException>(() => money1.Add(money2));
        }

        [Fact]
        public void SubtractMoneySameCurrency_ShouldSucceed()
        {
            var money1 = new Money(100m, "BRL");
            var money2 = new Money(30m, "BRL");
            var result = money1.Subtract(money2);
            result.Amount.Should().Be(70m);
        }

        [Fact]
        public void SubtractMoneyInsufficientBalance_ShouldThrowException()
        {
            var money1 = new Money(50m, "BRL");
            var money2 = new Money(100m, "BRL");
            Assert.Throws<DomainException>(() => money1.Subtract(money2));
        }

        [Fact]
        public void TwoMoneyWithSameValues_ShouldBeEqual()
        {
            var money1 = new Money(100m, "BRL");
            var money2 = new Money(100m, "BRL");
            money1.Should().Be(money2);
        }

        [Fact]
        public void CompareMoneyDifferentCurrency_ShouldThrowException()
        {
            var money1 = new Money(100m, "BRL");
            var money2 = new Money(100m, "USD");
            Assert.Throws<DomainException>(() => money1.IsGreaterOrEqualTo(money2));
        }
    }
}