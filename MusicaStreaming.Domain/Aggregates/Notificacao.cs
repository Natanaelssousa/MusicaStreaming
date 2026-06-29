using System;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Domain.Aggregates
{
    public class Notificacao
    {
        public Guid Id { get; private set; }
        public Guid TransacaoId { get; private set; }
        public TipoDestinatario Tipo { get; private set; }
        public string Destinatario { get; private set; }
        public string Conteudo { get; private set; }
        public NotificacaoStatus Status { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataEnvio { get; private set; }

        protected Notificacao() { }

        public static Notificacao Create(Guid transacaoId, TipoDestinatario tipo, string destinatario, string conteudo)
        {
            if (transacaoId == Guid.Empty)
                throw new DomainException("ID da transacao e obrigatorio");

            if (string.IsNullOrWhiteSpace(destinatario))
                throw new DomainException("Destinatario e obrigatorio");

            if (string.IsNullOrWhiteSpace(conteudo))
                throw new DomainException("Conteudo e obrigatorio");

            return new Notificacao
            {
                Id = Guid.NewGuid(),
                TransacaoId = transacaoId,
                Tipo = tipo,
                Destinatario = destinatario.Trim(),
                Conteudo = conteudo.Trim(),
                Status = NotificacaoStatus.Pendente,
                DataCriacao = DateTime.UtcNow
            };
        }

        public void MarcarComoEnviada()
        {
            if (Status == NotificacaoStatus.Enviada)
                throw new DomainException("Notificacao ja foi enviada");

            Status = NotificacaoStatus.Enviada;
            DataEnvio = DateTime.UtcNow;
        }
    }

    public enum NotificacaoStatus
    {
        Pendente = 1,
        Enviada = 2
    }

    public enum TipoDestinatario
    {
        DonoCartao = 1,
        Comerciante = 2
    }
}