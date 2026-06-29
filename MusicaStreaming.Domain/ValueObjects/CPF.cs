using System;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Domain.ValueObjects
{
    public class CPF
    {
        public string Value { get; }

        public CPF(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("CPF não pode estar vazio");

            var cpfClean = value.Replace(".", "").Replace("-", "").Trim();

            if (!IsValid(cpfClean))
                throw new DomainException("CPF inválido");

            Value = cpfClean;
        }

        private static bool IsValid(string cpf)
        {
            if (cpf.Length != 11)
                return false;

            if (!cpf.All(char.IsDigit))
                return false;

            if (new string(cpf[0], 11) == cpf)
                return false;

            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (cpf[i] - '0') * (10 - i);

            int remainder = sum % 11;
            int firstDigit = remainder < 2 ? 0 : 11 - remainder;

            if ((cpf[9] - '0') != firstDigit)
                return false;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += (cpf[i] - '0') * (11 - i);

            remainder = sum % 11;
            int secondDigit = remainder < 2 ? 0 : 11 - remainder;

            if ((cpf[10] - '0') != secondDigit)
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is not CPF other)
                return false;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value[..3]}.{Value[3..6]}.{Value[6..9]}-{Value[9..]}";
        }
    }
}