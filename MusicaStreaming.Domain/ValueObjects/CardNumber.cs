using System;
using System.Security.Cryptography;
using System.Text;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Domain.ValueObjects
{
    public class CardNumber
    {
        public string Value { get; }
        private string EncryptedValue { get; }

        public CardNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Número do cartão não pode estar vazio");

            var numberClean = value.Replace(" ", "").Replace("-", "");

            if (!IsValid(numberClean))
                throw new DomainException("Número de cartão inválido");

            Value = numberClean;
            EncryptedValue = Encrypt(numberClean);
        }

        private static bool IsValid(string number)
        {
            if (number.Length < 13 || number.Length > 19)
                return false;

            if (!number.All(char.IsDigit))
                return false;

            int sum = 0;
            bool isDouble = false;

            for (int i = number.Length - 1; i >= 0; i--)
            {
                int digit = number[i] - '0';

                if (isDouble)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
                isDouble = !isDouble;
            }

            return sum % 10 == 0;
        }

        private static string Encrypt(string number)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(number));
                return Convert.ToBase64String(hash);
            }
        }

        public string GetLastFourDigits()
        {
            return Value.Substring(Value.Length - 4);
        }

        public string GetMasked()
        {
            var lastFour = GetLastFourDigits();
            return $"**** **** **** {lastFour}";
        }

        public override bool Equals(object obj)
        {
            if (obj is not CardNumber other)
                return false;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return GetMasked();
        }
    }
}