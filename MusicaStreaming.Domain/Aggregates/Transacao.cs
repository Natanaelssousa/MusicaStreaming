using System;
using MusicaStreaming.Domain.Exceptions;
using MusicaStreaming.Domain.ValueObjects;

namespace MusicaStreaming.Domain.Aggregates
{
    public class Transacao
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid CartaoId { get; private set; }
        public Money Valor { get; private set; }
        public string Comerciante { get; private set; }
        public TransacaoStatus Status { get; private set; }
        public DateTime DataTransacao { get; private set; }
        public DateTime? DataAutorizacao { get; private set; }
        public string MotivoNegacao { get; private set; }
        public int NumeroTentativas { get; private set; }

        protected Transacao() { }

        public static Transacao Create(Guid usuarioId, Guid cartaoId, Money valor, string comerciante)
        {
            if (usuarioId == Guid.Empty)
                throw new DomainException("ID do usuario e obrigatorio");

            if (cartaoId == Guid.Empty)
                throw new DomainException("ID do cartao e obrigatorio");

            if (valor == null || valor.Amount <= 0)
                throw new DomainException("Valor deve ser maior que zero");

            if (string.IsNullOrWhiteSpace(comerciante))
                throw new DomainException("Comerciante e obrigatorio");

            var novaTransacao = new Transacao
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                CartaoId = cartaoId,
                Valor = valor,
                Comerciante = comerciante.Trim(),
                Status = TransacaoStatus.Pending,
                DataTransacao = DateTime.UtcNow,
                NumeroTentativas = 0
            };

            return novaTransacao;
        }


        public void Autorizar()
        {
            if (Status != TransacaoStatus.Pending)
                throw new DomainException("Apenas transações pendentes podem ser autorizadas");

            Status = TransacaoStatus.Authorized;
            DataAutorizacao = DateTime.UtcNow;
        }

        public void Negar(string motivo)
        {
            if (Status != TransacaoStatus.Pending)
                throw new DomainException("Apenas transações pendentes podem ser negadas");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new DomainException("Motivo da negação é obrigatório");

            Status = TransacaoStatus.Denied;
            MotivoNegacao = motivo;
        }

        public void Concluir()
        {
            if (Status != TransacaoStatus.Authorized)
                throw new DomainException("Apenas transações autorizadas podem ser concluídas");

            Status = TransacaoStatus.Completed;
        }

        public void Cancelar()
        {
            if (Status == TransacaoStatus.Completed)
                throw new DomainException("Transações concluídas não podem ser canceladas");

            if (Status == TransacaoStatus.Cancelled)
                throw new DomainException("Transação já foi cancelada");

            Status = TransacaoStatus.Cancelled;
        }

        public void IncrementarTentativa()
        {
            if (NumeroTentativas >= 3)
                throw new DomainException("Limite de tentativas atingido");

            NumeroTentativas++;
        }
    }

    public enum TransacaoStatus
    {
        Pending = 1,
        Authorized = 2,
        Denied = 3,
        Completed = 4,
        Cancelled = 5
    }
}
