using System;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class GenreTests
    {
        [Fact]
        public void CreateValidGenre_ShouldSucceed()
        {
            var genre = new Genre("Rock");
            genre.Name.Should().Be("Rock");
        }

        [Fact]
        public void EmptyGenre_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => new Genre(""));
        }

        [Fact]
        public void GenreWithSpaces_ShouldBeTrimmed()
        {
            var genre = new Genre("  Jazz  ");
            genre.Name.Should().Be("Jazz");
        }

        [Fact]
        public void TwoGenresWithSameName_ShouldBeEqual()
        {
            var genre1 = new Genre("Rock");
            var genre2 = new Genre("Rock");
            genre1.Should().Be(genre2);
        }

        [Fact]
        public void PredefinedGenres_ShouldExist()
        {
            Genre.Rock.Name.Should().Be("Rock");
            Genre.Pop.Name.Should().Be("Pop");
            Genre.Jazz.Name.Should().Be("Jazz");
        }

        [Fact]
        public void ToStringGenre_ShouldReturnName()
        {
            var genre = new Genre("Rock");
            genre.ToString().Should().Be("Rock");
        }
    }
}