using System;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class MusicaTests
    {
        [Fact]
        public void CreateValidMusica_ShouldSucceed()
        {
            var artistaId = Guid.NewGuid();
            var musica = Musica.Create(
                artistaId,
                "Imagine",
                Genre.Rock,
                new Duration(3, 3),
                DateTime.Now
            );

            musica.Id.Should().NotBe(Guid.Empty);
            musica.ArtistaId.Should().Be(artistaId);
            musica.Titulo.Should().Be("Imagine");
            musica.NumeroReproducoes.Should().Be(0);
        }

        [Fact]
        public void CreateMusicaWithoutTitulo_ShouldThrowException()
        {
            var artistaId = Guid.NewGuid();

            Assert.Throws<DomainException>(() =>
                Musica.Create(artistaId, "", Genre.Rock, new Duration(3, 3), DateTime.Now));
        }

        [Fact]
        public void RegistrarReproducao_ShouldIncrementCounter()
        {
            var artistaId = Guid.NewGuid();
            var musica = Musica.Create(
                artistaId,
                "Imagine",
                Genre.Rock,
                new Duration(3, 3),
                DateTime.Now
            );

            musica.RegistrarReproducao();
            musica.RegistrarReproducao();

            musica.NumeroReproducoes.Should().Be(2);
        }

        [Fact]
        public void Remover_ShouldChangeStatus()
        {
            var artistaId = Guid.NewGuid();
            var musica = Musica.Create(
                artistaId,
                "Imagine",
                Genre.Rock,
                new Duration(3, 3),
                DateTime.Now
            );

            musica.Remover();

            musica.Status.Should().Be(MusicaStatus.Removed);
        }

        [Fact]
        public void Restaurar_ShouldChangeStatus()
        {
            var artistaId = Guid.NewGuid();
            var musica = Musica.Create(
                artistaId,
                "Imagine",
                Genre.Rock,
                new Duration(3, 3),
                DateTime.Now
            );
            musica.Remover();
            musica.Restaurar();

            musica.Status.Should().Be(MusicaStatus.Available);
        }
    }
}