using System;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Domain.ValueObjects
{
    public class CardExpiryDate
    {
        public int Month { get; }
        public int Year { get; }

        public CardExpiryDate(int month, int year)
        {
            if (month < 1 || month > 12)
                throw new DomainException("Mês deve estar entre 1 e 12");

            if (year < DateTime.Now.Year)
                throw new DomainException("Cartão expirado");

            if (year == DateTime.Now.Year && month < DateTime.Now.Month)
                throw new DomainException("Cartão expirado");

            Month = month;
            Year = year;
        }

        public bool IsExpired()
        {
            var now = DateTime.Now;
            var lastDayOfMonth = DateTime.DaysInMonth(Year, Month);
            var expiryDate = new DateTime(Year, Month, lastDayOfMonth, 23, 59, 59);
            return expiryDate < now;
        }

        public override bool Equals(object obj)
        {
            if (obj is not CardExpiryDate other)
                return false;
            return Month == other.Month && Year == other.Year;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Month, Year);
        }

        public override string ToString()
        {
            return $"{Month:D2}/{Year}";
        }
    }
}