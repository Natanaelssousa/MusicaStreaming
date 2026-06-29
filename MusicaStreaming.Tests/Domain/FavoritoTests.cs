using System;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.Exceptions;
using Xunit;
using FluentAssertions;

namespace MusicaStreaming.Tests.Domain
{
    public class FavoritoTests
    {
        [Fact]
        public void CreateValidFavorito_ShouldSucceed()
        {
            var usuarioId = Guid.NewGuid();
            var musicaId = Guid.NewGuid();

            var favorito = Favorito.Create(usuarioId, musicaId);

            favorito.Id.Should().NotBe(Guid.Empty);
            favorito.UsuarioId.Should().Be(usuarioId);
            favorito.MusicaId.Should().Be(musicaId);
        }

        [Fact]
        public void CreateFavoritoWithoutUsuario_ShouldThrowException()
        {
            var musicaId = Guid.NewGuid();

            Assert.Throws<DomainException>(() => Favorito.Create(Guid.Empty, musicaId));
        }

        [Fact]
        public void CreateFavoritoWithoutMusica_ShouldThrowException()
        {
            var usuarioId = Guid.NewGuid();

            Assert.Throws<DomainException>(() => Favorito.Create(usuarioId, Guid.Empty));
        }

        [Fact]
        public void TwoFavoritosSameUsuarioMusica_ShouldBeEqual()
        {
            var usuarioId = Guid.NewGuid();
            var musicaId = Guid.NewGuid();

            var favorito1 = Favorito.Create(usuarioId, musicaId);
            var favorito2 = Favorito.Create(usuarioId, musicaId);

            favorito1.Should().Be(favorito2);
        }
    }
}