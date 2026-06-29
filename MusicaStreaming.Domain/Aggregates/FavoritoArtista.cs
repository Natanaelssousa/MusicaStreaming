using System;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Domain.Aggregates
{
    public class FavoritoArtista
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid ArtistaId { get; private set; }
        public DateTime DataFavoritacao { get; private set; }

        protected FavoritoArtista() { }

        public static FavoritoArtista Create(Guid usuarioId, Guid artistaId)
        {
            if (usuarioId == Guid.Empty)
                throw new DomainException("ID do usuario e obrigatorio");

            if (artistaId == Guid.Empty)
                throw new DomainException("ID do artista e obrigatorio");

            return new FavoritoArtista
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                ArtistaId = artistaId,
                DataFavoritacao = DateTime.UtcNow
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is not FavoritoArtista other)
                return false;
            return UsuarioId == other.UsuarioId && ArtistaId == other.ArtistaId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UsuarioId, ArtistaId);
        }
    }
}
