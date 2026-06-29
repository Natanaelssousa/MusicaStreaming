using System;
using MusicaStreaming.Domain.Exceptions;
using MusicaStreaming.Domain.ValueObjects;

namespace MusicaStreaming.Domain.Aggregates
{
    public class Musica
    {
        public Guid Id { get; private set; }
        public Guid ArtistaId { get; private set; }
        public string Titulo { get; private set; }
        public Genre Genero { get; private set; }
        public Duration Duracao { get; private set; }
        public DateTime DataLancamento { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public int NumeroReproducoes { get; private set; }
        public MusicaStatus Status { get; private set; }

        protected Musica() { }

        public static Musica Create(Guid artistaId, string titulo, Genre genero, Duration duracao, DateTime dataLancamento)
        {
            if (artistaId == Guid.Empty)
                throw new DomainException("ID do artista é obrigatório");

            if (string.IsNullOrWhiteSpace(titulo))
                throw new DomainException("Título da música é obrigatório");

            if (genero == null)
                throw new DomainException("Gênero é obrigatório");

            if (duracao == null)
                throw new DomainException("Duração é obrigatória");

            var novaMusica = new Musica
            {
                Id = Guid.NewGuid(),
                ArtistaId = artistaId,
                Titulo = titulo.Trim(),
                Genero = genero,
                Duracao = duracao,
                DataLancamento = dataLancamento,
                DataCriacao = DateTime.UtcNow,
                NumeroReproducoes = 0,
                Status = MusicaStatus.Available
            };

            return novaMusica;
        }

        public void RegistrarReproducao()
        {
            if (Status != MusicaStatus.Available)
                throw new DomainException("Música não está disponível para reprodução");

            NumeroReproducoes++;
        }

        public void Remover()
        {
            if (Status == MusicaStatus.Removed)
                throw new DomainException("Música já foi removida");

            Status = MusicaStatus.Removed;
        }

        public void Restaurar()
        {
            if (Status == MusicaStatus.Available)
                throw new DomainException("Música já está disponível");

            Status = MusicaStatus.Available;
        }
    }

    public enum MusicaStatus
    {
        Available = 1,
        Removed = 2
    }
}