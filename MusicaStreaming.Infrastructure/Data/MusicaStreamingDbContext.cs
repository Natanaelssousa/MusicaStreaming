using Microsoft.EntityFrameworkCore;
using MusicaStreaming.Domain.Aggregates;

namespace MusicaStreaming.Infrastructure.Data
{
    public class MusicaStreamingDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Assinatura> Assinaturas { get; set; }
        public DbSet<Cartao> Cartoes { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Artista> Artistas { get; set; }
        public DbSet<Musica> Musicas { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<FavoritoArtista> FavoritosArtista { get; set; }

        public MusicaStreamingDbContext(DbContextOptions<MusicaStreamingDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigurarUsuario(modelBuilder);
            ConfigurarAssinatura(modelBuilder);
            ConfigurarCartao(modelBuilder);
            ConfigurarTransacao(modelBuilder);
            ConfigurarArtista(modelBuilder);
            ConfigurarMusica(modelBuilder);
            ConfigurarFavorito(modelBuilder);
            ConfigurarNotificacao(modelBuilder);
            ConfigurarFavoritoArtista(modelBuilder);
        }

        private static void ConfigurarUsuario(ModelBuilder modelBuilder)
        {
            var usuario = modelBuilder.Entity<Usuario>();

            usuario.HasKey(u => u.Id);
            usuario.Property(u => u.Id).ValueGeneratedNever();

            usuario.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(255);

            usuario.Property(u => u.SenhaHash)
                .IsRequired()
                .HasMaxLength(500);

            usuario.Property(u => u.Salt)
                .IsRequired()
                .HasMaxLength(500);

            usuario.Property(u => u.Status)
                .IsRequired()
                .HasConversion<int>();

            usuario.Property(u => u.EmailConfirmado)
                .IsRequired()
                .HasDefaultValue(false);

            usuario.Property(u => u.DataCriacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Value Objects
            usuario.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(255);

                email.HasIndex(e => e.Value).IsUnique();
            });

            usuario.OwnsOne(u => u.CPF, cpf =>
            {
                cpf.Property(c => c.Value)
                    .HasColumnName("CPF")
                    .IsRequired()
                    .HasMaxLength(11);

                cpf.HasIndex(c => c.Value).IsUnique();
            });

            usuario.ToTable("Usuarios");
        }

        private static void ConfigurarAssinatura(ModelBuilder modelBuilder)
        {
            var assinatura = modelBuilder.Entity<Assinatura>();

            assinatura.HasKey(a => a.Id);
            assinatura.Property(a => a.Id).ValueGeneratedNever();

            assinatura.Property(a => a.UsuarioId).IsRequired();
            assinatura.Property(a => a.Status)
                .IsRequired()
                .HasConversion<int>();

            assinatura.Property(a => a.RenovacaoAutomatica)
                .IsRequired()
                .HasDefaultValue(true);

            assinatura.Property(a => a.DataCriacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Mapeamento simplificado do Plan
            assinatura.Property(a => a.Plan)
                .HasConversion(
                    v => v.Type,
                    v => v == PlanType.Free ? Plan.Free :
                         v == PlanType.Premium ? Plan.Premium :
                         Plan.Family
                )
                .HasColumnName("PlanType")
                .IsRequired();

            assinatura.HasIndex(a => a.UsuarioId);
            assinatura.ToTable("Assinaturas");
        }

        private static void ConfigurarCartao(ModelBuilder modelBuilder)
        {
            var cartao = modelBuilder.Entity<Cartao>();

            cartao.HasKey(c => c.Id);
            cartao.Property(c => c.Id).ValueGeneratedNever();

            cartao.Property(c => c.UsuarioId).IsRequired();
            cartao.Property(c => c.Titular)
                .IsRequired()
                .HasMaxLength(255);

            cartao.Property(c => c.Status)
                .IsRequired()
                .HasConversion<int>();

            cartao.Property(c => c.DataCriacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Value Objects
            cartao.OwnsOne(c => c.Numero, numero =>
            {
                numero.Property(n => n.Value)
                    .HasColumnName("NumeroCartao")
                    .IsRequired()
                    .HasMaxLength(19);

                numero.HasIndex(n => n.Value).IsUnique();
            });

            cartao.OwnsOne(c => c.DataValidade, validade =>
            {
                validade.Property(v => v.Month)
                    .HasColumnName("MesValidade")
                    .IsRequired();

                validade.Property(v => v.Year)
                    .HasColumnName("AnoValidade")
                    .IsRequired();
            });

            cartao.OwnsOne(c => c.CVV, cvv =>
            {
                cvv.Property(v => v.Value)
                    .HasColumnName("CVV")
                    .IsRequired()
                    .HasMaxLength(4);
            });

            cartao.HasIndex(c => c.UsuarioId);
            cartao.ToTable("Cartoes");
        }

        private static void ConfigurarTransacao(ModelBuilder modelBuilder)
        {
            var transacao = modelBuilder.Entity<Transacao>();

            transacao.HasKey(t => t.Id);
            transacao.Property(t => t.Id).ValueGeneratedNever();

            transacao.Property(t => t.UsuarioId).IsRequired();
            transacao.Property(t => t.CartaoId).IsRequired();

            transacao.Property(t => t.Status)
                .IsRequired()
                .HasConversion<int>();

            transacao.Property(t => t.DataTransacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            transacao.Property(t => t.NumeroTentativas)
                .IsRequired()
                .HasDefaultValue(0);

            // Value Object Money
            transacao.OwnsOne(t => t.Valor, valor =>
            {
                valor.Property(m => m.Amount)
                    .HasColumnName("Valor")
                    .IsRequired()
                    .HasPrecision(18, 2);

                valor.Property(m => m.Currency)
                    .HasColumnName("Moeda")
                    .IsRequired()
                    .HasMaxLength(3);
            });

            transacao.Property(t => t.MotivoNegacao)
                .HasMaxLength(500); 
            transacao.Property(t => t.Comerciante)
        .IsRequired()
        .HasMaxLength(255)
        .HasDefaultValue("N/A");

            transacao.HasIndex(t => t.UsuarioId);
            transacao.HasIndex(t => t.CartaoId);
            transacao.HasIndex(t => t.Status);
            transacao.ToTable("Transacoes");
        }

        private static void ConfigurarArtista(ModelBuilder modelBuilder)
        {
            var artista = modelBuilder.Entity<Artista>();

            artista.HasKey(a => a.Id);
            artista.Property(a => a.Id).ValueGeneratedNever();

            artista.Property(a => a.Nome)
                .IsRequired()
                .HasMaxLength(255);

            artista.Property(a => a.Biografia)
                .HasMaxLength(2000);

            artista.Property(a => a.Status)
                .IsRequired()
                .HasConversion<int>();

            artista.Property(a => a.DataCriacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            artista.HasIndex(a => a.Nome).IsUnique();
            artista.HasIndex(a => a.Status);
            artista.ToTable("Artistas");
        }

        private static void ConfigurarMusica(ModelBuilder modelBuilder)
        {
            var musica = modelBuilder.Entity<Musica>();

            musica.HasKey(m => m.Id);
            musica.Property(m => m.Id).ValueGeneratedNever();

            musica.Property(m => m.ArtistaId).IsRequired();

            musica.Property(m => m.Titulo)
                .IsRequired()
                .HasMaxLength(255);

            musica.Property(m => m.DataLancamento).IsRequired();

            musica.Property(m => m.NumeroReproducoes)
                .IsRequired()
                .HasDefaultValue(0);

            musica.Property(m => m.Status)
                .IsRequired()
                .HasConversion<int>();

            musica.Property(m => m.DataCriacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Value Object Genre
            musica.OwnsOne(m => m.Genero, genero =>
            {
                genero.Property(g => g.Name)
                    .HasColumnName("Genero")
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // Value Object Duration
            musica.OwnsOne(m => m.Duracao, duracao =>
            {
                duracao.Property(d => d.Value)
                    .HasColumnName("Duracao")
                    .IsRequired();
            });

            musica.HasIndex(m => m.ArtistaId);
            musica.HasIndex(m => m.Titulo);
            musica.HasIndex(m => m.Status);
            musica.ToTable("Musicas");
        }

        private static void ConfigurarFavorito(ModelBuilder modelBuilder)
        {
            var favorito = modelBuilder.Entity<Favorito>();

            favorito.HasKey(f => f.Id);
            favorito.Property(f => f.Id).ValueGeneratedNever();

            favorito.Property(f => f.UsuarioId).IsRequired();
            favorito.Property(f => f.MusicaId).IsRequired();

            favorito.Property(f => f.DataFavoritacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            favorito.HasIndex(f => new { f.UsuarioId, f.MusicaId }).IsUnique();
            favorito.HasIndex(f => f.UsuarioId);
            favorito.HasIndex(f => f.MusicaId);
            favorito.ToTable("Favoritos");
        }

        private static void ConfigurarNotificacao(ModelBuilder modelBuilder)
        {
            var notificacao = modelBuilder.Entity<Notificacao>();

            notificacao.HasKey(n => n.Id);
            notificacao.Property(n => n.Id).ValueGeneratedNever();

            notificacao.Property(n => n.TransacaoId).IsRequired();

            notificacao.Property(n => n.Tipo)
                .IsRequired()
                .HasConversion<int>();

            notificacao.Property(n => n.Destinatario)
                .IsRequired()
                .HasMaxLength(255);

            notificacao.Property(n => n.Conteudo)
                .IsRequired()
                .HasMaxLength(1000);

            notificacao.Property(n => n.Status)
                .IsRequired()
                .HasConversion<int>();

            notificacao.Property(n => n.DataCriacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            notificacao.HasIndex(n => n.Status);
            notificacao.HasIndex(n => n.TransacaoId);
            notificacao.ToTable("Notificacoes");
        }

        private static void ConfigurarFavoritoArtista(ModelBuilder modelBuilder)
        {
            var fav = modelBuilder.Entity<FavoritoArtista>();

            fav.HasKey(f => f.Id);
            fav.Property(f => f.Id).ValueGeneratedNever();

            fav.Property(f => f.UsuarioId).IsRequired();
            fav.Property(f => f.ArtistaId).IsRequired();

            fav.Property(f => f.DataFavoritacao)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            fav.HasIndex(f => new { f.UsuarioId, f.ArtistaId }).IsUnique();
            fav.HasIndex(f => f.UsuarioId);
            fav.ToTable("FavoritosArtista");
        }
    }
}