using System;
using MusicaStreaming.Domain.Exceptions;
using MusicaStreaming.Domain.ValueObjects;

namespace MusicaStreaming.Domain.Aggregates
{
    public class Assinatura
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Plan Plan { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }
        public DateTime? DataRenovacao { get; private set; }
        public SubscriptionStatus Status { get; private set; }
        public bool RenovacaoAutomatica { get; private set; }
        public DateTime DataCriacao { get; private set; }

        protected Assinatura() { }

        public static Assinatura Create(Guid usuarioId, Plan plan)
        {
            if (usuarioId == Guid.Empty)
                throw new DomainException("ID do usuário é obrigatório");

            if (plan == null)
                throw new DomainException("Plano é obrigatório");

            var dataInicio = DateTime.UtcNow;
            var dataFim = dataInicio.AddMonths(1);

            var novaAssinatura = new Assinatura
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Plan = plan,
                DataInicio = dataInicio,
                DataFim = dataFim,
                Status = SubscriptionStatus.Active,
                RenovacaoAutomatica = true,
                DataCriacao = DateTime.UtcNow
            };

            return novaAssinatura;
        }

        public void Renovar()
        {
            if (Status == SubscriptionStatus.Cancelled)
                throw new DomainException("Assinatura cancelada não pode ser renovada");

            if (Status == SubscriptionStatus.Expired)
            {
                Status = SubscriptionStatus.Active;
            }

            DataFim = DateTime.UtcNow.AddMonths(1);
            DataRenovacao = DateTime.UtcNow;
        }

        public void Cancelar(string motivo)
        {
            if (Status == SubscriptionStatus.Cancelled)
                throw new DomainException("Assinatura já foi cancelada");

            Status = SubscriptionStatus.Cancelled;
            RenovacaoAutomatica = false;
        }

        public void AlterarPlan(Plan novoPlan)
        {
            if (novoPlan == null)
                throw new DomainException("Novo plano é obrigatório");

            if (Plan.Type == novoPlan.Type)
                throw new DomainException("Novo plano não pode ser igual ao atual");

            if (Status != SubscriptionStatus.Active)
                throw new DomainException("Apenas assinaturas ativas podem ter o plano alterado");

            Plan = novoPlan;
        }

        public bool EstaProximaDeExpirar()
        {
            var diasParaExpiracao = (DataFim - DateTime.UtcNow).TotalDays;
            return diasParaExpiracao <= 7 && diasParaExpiracao > 0;
        }

        public bool EstaExpirada()
        {
            return DateTime.UtcNow > DataFim && Status != SubscriptionStatus.Cancelled;
        }
    }

    public enum SubscriptionStatus
    {
        Active = 1,
        Expired = 2,
        Cancelled = 3
    }

    public class Plan
    {
        public PlanType Type { get; }
        public Money Price { get; }
        public int DownloadLimit { get; }
        public bool HasAds { get; }
        public int MaxSimultaneousDevices { get; }

        public static readonly Plan Free = new(
            PlanType.Free,
            new Money(0),
            10,
            hasAds: true,
            maxSimultaneousDevices: 1
        );

        public static readonly Plan Premium = new(
            PlanType.Premium,
            new Money(19.90m),
            999,
            hasAds: false,
            maxSimultaneousDevices: 2
        );

        public static readonly Plan Family = new(
            PlanType.Family,
            new Money(34.90m),
            999,
            hasAds: false,
            maxSimultaneousDevices: 6
        );

        public Plan(PlanType type, Money price, int downloadLimit, bool hasAds, int maxSimultaneousDevices)
        {
            if (downloadLimit < 0)
                throw new DomainException("Limite de baixadas não pode ser negativo");

            if (maxSimultaneousDevices < 1)
                throw new DomainException("Limite de dispositivos simultâneos deve ser no mínimo 1");

            Type = type;
            Price = price ?? throw new ArgumentNullException(nameof(price));
            DownloadLimit = downloadLimit;
            HasAds = hasAds;
            MaxSimultaneousDevices = maxSimultaneousDevices;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Plan other)
                return false;
            return Type == other.Type;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }

    public enum PlanType
    {
        Free = 1,
        Premium = 2,
        Family = 3
    }
}
