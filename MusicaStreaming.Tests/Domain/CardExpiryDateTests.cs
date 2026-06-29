using System;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class CardExpiryDateTests
    {
        [Fact]
        public void CreateValidCardExpiryDate_ShouldSucceed()
        {
            var expiryDate = new CardExpiryDate(12, DateTime.Now.Year + 1);
            expiryDate.Month.Should().Be(12);
            expiryDate.Year.Should().Be(DateTime.Now.Year + 1);
        }

        [Fact]
        public void InvalidMonth_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CardExpiryDate(13, DateTime.Now.Year + 1));
        }

        [Fact]
        public void MonthZero_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CardExpiryDate(0, DateTime.Now.Year + 1));
        }

        [Fact]
        public void PastYear_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new CardExpiryDate(12, DateTime.Now.Year - 1));
        }

        [Fact]
        public void PastMonthCurrentYear_ShouldThrowException()
        {
            var pastMonth = DateTime.Now.Month > 1 ? DateTime.Now.Month - 1 : 1;
            Assert.Throws<DomainException>(() => new CardExpiryDate(pastMonth, DateTime.Now.Year));
        }

        [Fact]
        public void ToStringCardExpiryDate_ShouldReturnFormatted()
        {
            var futureYear = DateTime.Now.Year + 2;
            var expiryDate = new CardExpiryDate(5, futureYear);
            expiryDate.ToString().Should().Be($"05/{futureYear}");
        }

        [Fact]
        public void TwoCardExpiryDatesWithSameValues_ShouldBeEqual()
        {
            var expiryDate1 = new CardExpiryDate(12, DateTime.Now.Year + 1);
            var expiryDate2 = new CardExpiryDate(12, DateTime.Now.Year + 1);
            expiryDate1.Should().Be(expiryDate2);
        }
    }
}