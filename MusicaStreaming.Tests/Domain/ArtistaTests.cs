using System;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class ArtistaTests
    {
        [Fact]
        public void CreateValidArtista_ShouldSucceed()
        {
            var artista = Artista.Create("The Beatles", "Banda britânica de rock");

            artista.Id.Should().NotBe(Guid.Empty);
            artista.Nome.Should().Be("The Beatles");
            artista.Status.Should().Be(ArtistaStatus.Active);
        }

        [Fact]
        public void CreateArtistaWithoutNome_ShouldThrowException()
        {
            Assert.Throws<DomainException>(() => Artista.Create(""));
        }

        [Fact]
        public void AlterarBiografia_ShouldSucceed()
        {
            var artista = Artista.Create("The Beatles", "Antiga biografia");
            artista.AlterarBiografia("Nova biografia");

            artista.Biografia.Should().Be("Nova biografia");
        }

        [Fact]
        public void Desativar_ShouldChangeStatus()
        {
            var artista = Artista.Create("The Beatles");
            artista.Desativar();

            artista.Status.Should().Be(ArtistaStatus.Inactive);
        }

        [Fact]
        public void Reativar_ShouldChangeStatus()
        {
            var artista = Artista.Create("The Beatles");
            artista.Desativar();
            artista.Reativar();

            artista.Status.Should().Be(ArtistaStatus.Active);
        }
    }
}