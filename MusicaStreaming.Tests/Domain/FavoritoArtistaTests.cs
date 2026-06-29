using System;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class FavoritoArtistaTests
    {
        [Fact]
        public void CreateValid_ShouldSucceed()
        {
            var u = Guid.NewGuid();
            var a = Guid.NewGuid();

            var fav = FavoritoArtista.Create(u, a);

            fav.Id.Should().NotBe(Guid.Empty);
            fav.UsuarioId.Should().Be(u);
            fav.ArtistaId.Should().Be(a);
        }

        [Fact]
        public void CreateWithoutUsuario_ShouldThrow()
        {
            Assert.Throws<DomainException>(() => FavoritoArtista.Create(Guid.Empty, Guid.NewGuid()));
        }

        [Fact]
        public void CreateWithoutArtista_ShouldThrow()
        {
            Assert.Throws<DomainException>(() => FavoritoArtista.Create(Guid.NewGuid(), Guid.Empty));
        }

        [Fact]
        public void TwoSame_ShouldBeEqual()
        {
            var u = Guid.NewGuid();
            var a = Guid.NewGuid();

            FavoritoArtista.Create(u, a).Should().Be(FavoritoArtista.Create(u, a));
        }
    }
}