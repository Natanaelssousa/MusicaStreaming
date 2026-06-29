using System;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class DurationTests
    {
        [Fact]
        public void CreateValidDuration_ShouldSucceed()
        {
            var duration = new Duration(TimeSpan.FromMinutes(3).Add(TimeSpan.FromSeconds(30)));
            duration.Value.TotalSeconds.Should().Be(210);
        }

        [Fact]
        public void CreateDurationWithMinutesSeconds_ShouldSucceed()
        {
            var duration = new Duration(3, 30);
            duration.TotalSeconds.Should().Be(210);
        }

        [Fact]
        public void ZeroDuration_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new Duration(TimeSpan.Zero));
        }

        [Fact]
        public void NegativeDuration_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new Duration(TimeSpan.FromSeconds(-10)));
        }

        [Fact]
        public void DurationGreaterThan10Hours_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new Duration(TimeSpan.FromHours(11)));
        }

        [Fact]
        public void TwoDurationsWithSameValue_ShouldBeEqual()
        {
            var duration1 = new Duration(3, 30);
            var duration2 = new Duration(3, 30);
            duration1.Should().Be(duration2);
        }

        [Fact]
        public void ToStringDuration_ShouldReturnFormatted()
        {
            var duration = new Duration(3, 45);
            duration.ToString().Should().Be("03:45");
        }
    }
}
