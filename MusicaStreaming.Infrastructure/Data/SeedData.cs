using Microsoft.EntityFrameworkCore;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;
using System.Runtime.CompilerServices;

namespace MusicaStreaming.Infrastructure.Data
{
    public static class SeedData
    {
        public static void PopularBancoDados(MusicaStreamingDbContext context)
        {
            if (context.Artistas.Any())
                return;

            // Criar Artistas
            var beatles = Artista.Create("The Beatles", "Lendária banda britânica de rock dos anos 60");
            var pinkFloyd = Artista.Create("Pink Floyd", "Banda de rock progressivo inglesa");
            var queen = Artista.Create("Queen", "Banda de rock britânica");
            var bowie = Artista.Create("David Bowie", "Músico e produtor inglês inovador");
            var elis = Artista.Create("Elis Regina", "Musa da Bossa Nova brasileira");
            var tomJobim = Artista.Create("Tom Jobim", "Compositor e pianista brasileiro");
            var gilberto = Artista.Create("Gilberto Gil", "Cantor e compositor brasileiro");
            var caetano = Artista.Create("Caetano Veloso", "Cantor e compositor brasileiro");
            var miles = Artista.Create("Miles Davis", "Trompetista de jazz americano");
            var coltrane = Artista.Create("John Coltrane", "Saxofonista de jazz americano");

            context.Artistas.AddRange(beatles, pinkFloyd, queen, bowie, elis, tomJobim, gilberto, caetano, miles, coltrane);
            context.SaveChanges();

            // Inserir músicas via SQL (sem interpolação de string)
            var sql = @"
                INSERT INTO Musicas (Id, ArtistaId, Titulo, Genero, Duracao, DataLancamento, NumeroReproducoes, Status, DataCriacao)
                VALUES 
                (NEWID(), '" + beatles.Id + @"', 'Hey Jude', 'Rock', '00:07:11', '1968-08-26', 0, 1, GETUTCDATE()),
                (NEWID(), '" + beatles.Id + @"', 'Let It Be', 'Rock', '00:03:50', '1970-03-06', 0, 1, GETUTCDATE()),
                (NEWID(), '" + beatles.Id + @"', 'Imagine', 'Rock', '00:03:03', '1971-09-09', 0, 1, GETUTCDATE()),
                (NEWID(), '" + pinkFloyd.Id + @"', 'Comfortably Numb', 'Rock', '00:06:23', '1979-11-30', 0, 1, GETUTCDATE()),
                (NEWID(), '" + pinkFloyd.Id + @"', 'Shine On You Crazy Diamond', 'Rock', '00:13:31', '1975-09-08', 0, 1, GETUTCDATE()),
                (NEWID(), '" + queen.Id + @"', 'Bohemian Rhapsody', 'Rock', '00:05:55', '1975-10-31', 0, 0, GETUTCDATE()),
                (NEWID(), '" + queen.Id + @"', 'We Are the Champions', 'Rock', '00:02:59', '1977-10-07', 0, 1, GETUTCDATE()),
                (NEWID(), '" + bowie.Id + @"', 'Space Oddity', 'Rock', '00:05:15', '1969-07-11', 0, 1, GETUTCDATE()),
                (NEWID(), '" + bowie.Id + @"', 'Heroes', 'Rock', '00:06:10', '1977-10-21', 0, 1, GETUTCDATE()),
                (NEWID(), '" + elis.Id + @"', 'Águas de Março', 'MPB', '00:04:32', '1972-03-15', 0, 1, GETUTCDATE()),
                (NEWID(), '" + elis.Id + @"', 'Fascinação', 'MPB', '00:03:28', '1974-10-10', 0, 1, GETUTCDATE()),
                (NEWID(), '" + tomJobim.Id + @"', 'Garota de Ipanema', 'MPB', '00:03:23', '1962-06-01', 0, 1, GETUTCDATE()),
                (NEWID(), '" + tomJobim.Id + @"', 'A Noite e o Dia', 'MPB', '00:04:15', '1980-07-20', 0, 1, GETUTCDATE()),
                (NEWID(), '" + gilberto.Id + @"', 'Festa para um Rei Negro', 'MPB', '00:03:45', '1977-12-10', 0, 1, GETUTCDATE()),
                (NEWID(), '" + caetano.Id + @"', 'Sampa', 'MPB', '00:05:38', '1978-12-01', 0, 1, GETUTCDATE()),
                (NEWID(), '" + miles.Id + @"', 'So What', 'Jazz', '00:09:02', '1959-03-02', 0, 1, GETUTCDATE()),
                (NEWID(), '" + coltrane.Id + @"', 'A Love Supreme', 'Jazz', '00:07:45', '1964-12-10', 0, 1, GETUTCDATE())
            ";

            context.Database.ExecuteSql(FormattableStringFactory.Create(sql));
        }
    }
}