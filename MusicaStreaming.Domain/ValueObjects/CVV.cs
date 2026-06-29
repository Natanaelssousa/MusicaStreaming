using System;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Domain.ValueObjects
{
    public class CVV
    {
        public string Value { get; }

        public CVV(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("CVV não pode estar vazio");

            var cvvClean = value.Trim();

            if (cvvClean.Length < 3 || cvvClean.Length > 4)
                throw new DomainException("CVV deve ter 3 ou 4 dígitos");

            if (!cvvClean.All(char.IsDigit))
                throw new DomainException("CVV deve conter apenas dígitos");

            Value = cvvClean;
        }

        public override bool Equals(object obj)
        {
            if (obj is not CVV other)
                return false;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return "***";
        }
    }
}