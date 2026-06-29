using System;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Domain.ValueObjects
{
    public class Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency = "BRL")
        {
            if (amount < 0)
                throw new DomainException("Valor não pode ser negativo");

            if (string.IsNullOrWhiteSpace(currency))
                throw new DomainException("Moeda é obrigatória");

            Amount = amount;
            Currency = currency.ToUpper();
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Não é possível somar moedas diferentes: {Currency} e {other.Currency}");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Não é possível subtrair moedas diferentes: {Currency} e {other.Currency}");

            if (Amount < other.Amount)
                throw new DomainException("Saldo insuficiente para a operação");

            return new Money(Amount - other.Amount, Currency);
        }

        public bool IsGreaterOrEqualTo(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Não é possível comparar moedas diferentes: {Currency} e {other.Currency}");

            return Amount >= other.Amount;
        }

        public bool IsLessThan(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Não é possível comparar moedas diferentes: {Currency} e {other.Currency}");

            return Amount < other.Amount;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Money other)
                return false;
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }

        public override string ToString()
        {
            return $"{Currency} {Amount:F2}";
        }
    }
}