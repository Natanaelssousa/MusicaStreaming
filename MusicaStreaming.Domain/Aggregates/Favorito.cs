using System;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Domain.Aggregates
{
    public class Favorito
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid MusicaId { get; private set; }
        public DateTime DataFavoritacao { get; private set; }

        protected Favorito() { }

        public static Favorito Create(Guid usuarioId, Guid musicaId)
        {
            if (usuarioId == Guid.Empty)
                throw new DomainException("ID do usuário é obrigatório");

            if (musicaId == Guid.Empty)
                throw new DomainException("ID da música é obrigatório");

            var novoFavorito = new Favorito
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                MusicaId = musicaId,
                DataFavoritacao = DateTime.UtcNow
            };

            return novoFavorito;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Favorito other)
                return false;
            return UsuarioId == other.UsuarioId && MusicaId == other.MusicaId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UsuarioId, MusicaId);
        }
    }
}