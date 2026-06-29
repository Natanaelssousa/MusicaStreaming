using System;
using MusicaStreaming.Domain.Exceptions;
using MusicaStreaming.Domain.ValueObjects;

namespace MusicaStreaming.Domain.Aggregates
{
    public class Cartao
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public CardNumber Numero { get; private set; }
        public CardExpiryDate DataValidade { get; private set; }
        public CVV CVV { get; private set; }
        public string Titular { get; private set; }
        public CartaoStatus Status { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataUltimoUso { get; private set; }

        protected Cartao() { }

        public static Cartao Create(Guid usuarioId, string numeroCartao, int mes, int ano, string cvv, string titularCartao)
        {
            if (usuarioId == Guid.Empty)
                throw new DomainException("ID do usuário é obrigatório");

            if (string.IsNullOrWhiteSpace(titularCartao))
                throw new DomainException("Titular do cartão é obrigatório");

            var novoCartao = new Cartao
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Numero = new CardNumber(numeroCartao),
                DataValidade = new CardExpiryDate(mes, ano),
                CVV = new CVV(cvv),
                Titular = titularCartao.Trim(),
                Status = CartaoStatus.Active,
                DataCriacao = DateTime.UtcNow
            };

            return novoCartao;
        }

        public void RegistrarUso()
        {
            if (Status != CartaoStatus.Active)
                throw new DomainException("Cartão não está ativo");

            if (DataValidade.IsExpired())
                throw new DomainException("Cartão expirado");

            DataUltimoUso = DateTime.UtcNow;
        }

        public void Desativar()
        {
            if (Status == CartaoStatus.Inactive)
                throw new DomainException("Cartão já está desativado");

            Status = CartaoStatus.Inactive;
        }

        public void Reativar()
        {
            if (Status == CartaoStatus.Active)
                throw new DomainException("Cartão já está ativo");

            if (DataValidade.IsExpired())
                throw new DomainException("Cartão expirado não pode ser reativado");

            Status = CartaoStatus.Active;
        }

        public bool EstaValido()
        {
            return Status == CartaoStatus.Active && !DataValidade.IsExpired();
        }
    }

    public enum CartaoStatus
    {
        Active = 1,
        Inactive = 2
    }
}
