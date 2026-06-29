using System;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Domain.ValueObjects
{
    public class Genre
    {
        public string Name { get; }

        public static readonly Genre Rock = new("Rock");
        public static readonly Genre Pop = new("Pop");
        public static readonly Genre Jazz = new("Jazz");
        public static readonly Genre CountryMusic = new("Country Music");
        public static readonly Genre Electronic = new("Electronic");
        public static readonly Genre HipHop = new("Hip-Hop");
        public static readonly Genre Samba = new("Samba");
        public static readonly Genre MPB = new("MPB");
        public static readonly Genre Classical = new("Classical");
        public static readonly Genre Reggae = new("Reggae");

        public Genre(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Gênero não pode estar vazio");

            Name = name.Trim();
        }

        public override bool Equals(object obj)
        {
            if (obj is not Genre other)
                return false;
            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.ToLower().GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
