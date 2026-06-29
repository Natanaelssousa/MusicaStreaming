using System;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Domain.ValueObjects
{
    public class Duration
    {
        public TimeSpan Value { get; }

        public Duration(TimeSpan value)
        {
            if (value <= TimeSpan.Zero)
                throw new DomainException("Duração deve ser maior que zero");

            if (value.TotalHours > 10)
                throw new DomainException("Duração não pode exceder 10 horas");

            Value = value;
        }

        public Duration(int minutes, int seconds = 0)
            : this(TimeSpan.FromMinutes(minutes).Add(TimeSpan.FromSeconds(seconds)))
        {
        }

        public int TotalSeconds => (int)Value.TotalSeconds;

        public override bool Equals(object obj)
        {
            if (obj is not Duration other)
                return false;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value.Minutes:D2}:{Value.Seconds:D2}";
        }
    }
}