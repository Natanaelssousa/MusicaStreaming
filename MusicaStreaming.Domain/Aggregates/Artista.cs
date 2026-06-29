using System;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Domain.Aggregates
{
    public class Artista
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Biografia { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public ArtistaStatus Status { get; private set; }

        protected Artista() { }

        public static Artista Create(string nome, string biografia = "")
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("Nome do artista é obrigatório");

            var novoArtista = new Artista
            {
                Id = Guid.NewGuid(),
                Nome = nome.Trim(),
                Biografia = biografia ?? "",
                DataCriacao = DateTime.UtcNow,
                Status = ArtistaStatus.Active
            };

            return novoArtista;
        }

        public void AlterarBiografia(string novaBiografia)
        {
            if (novaBiografia == null)
                throw new DomainException("Biografia não pode ser nula");

            Biografia = novaBiografia;
        }

        public void Desativar()
        {
            if (Status == ArtistaStatus.Inactive)
                throw new DomainException("Artista já está desativado");

            Status = ArtistaStatus.Inactive;
        }

        public void Reativar()
        {
            if (Status == ArtistaStatus.Active)
                throw new DomainException("Artista já está ativo");

            Status = ArtistaStatus.Active;
        }
    }

    public enum ArtistaStatus
    {
        Active = 1,
        Inactive = 2
    }
}