using System;
using System.Security.Cryptography;
using System.Text;
using MusicaStreaming.Domain.Exceptions;
using MusicaStreaming.Domain.ValueObjects;

namespace MusicaStreaming.Domain.Aggregates
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public Email Email { get; private set; }
        public string Nome { get; private set; }
        public CPF CPF { get; private set; }
        public string SenhaHash { get; private set; }
        public string Salt { get; private set; }
        public UserStatus Status { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataUltimoLogin { get; private set; }
        public bool EmailConfirmado { get; private set; }

        protected Usuario() { }

        public static Usuario Create(string email, string nome, string cpf, string senha)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email é obrigatório");

            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("Nome é obrigatório");

            if (string.IsNullOrWhiteSpace(cpf))
                throw new DomainException("CPF é obrigatório");

            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
                throw new DomainException("Senha deve ter no mínimo 8 caracteres");

            var novoUsuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Email = new Email(email),
                Nome = nome.Trim(),
                CPF = new CPF(cpf),
                Status = UserStatus.Active,
                DataCriacao = DateTime.UtcNow,
                EmailConfirmado = false
            };

            novoUsuario.Salt = GenerateSalt();
            novoUsuario.SenhaHash = HashPassword(senha, novoUsuario.Salt);

            return novoUsuario;
        }

        public void ConfirmarEmail()
        {
            if (EmailConfirmado)
                throw new DomainException("Email já foi confirmado");

            EmailConfirmado = true;
        }

        public void FazerLogin(string senhaFornecida)
        {
            if (Status != UserStatus.Active)
                throw new DomainException("Usuário não está ativo");

            if (!EmailConfirmado)
                throw new DomainException("Email não foi confirmado");

            if (!VerificarSenha(senhaFornecida))
                throw new DomainException("Senha incorreta");

            DataUltimoLogin = DateTime.UtcNow;
        }

        public void AlterarSenha(string senhaAtual, string novaSenha)
        {
            if (!VerificarSenha(senhaAtual))
                throw new DomainException("Senha atual incorreta");

            if (string.IsNullOrWhiteSpace(novaSenha) || novaSenha.Length < 8)
                throw new DomainException("Nova senha deve ter no mínimo 8 caracteres");

            if (senhaAtual == novaSenha)
                throw new DomainException("Nova senha não pode ser igual à anterior");

            SenhaHash = HashPassword(novaSenha, Salt);
        }

        public void Suspender(string motivo)
        {
            if (Status == UserStatus.Deleted)
                throw new DomainException("Usuário deletado não pode ser suspenso");

            if (Status == UserStatus.Suspended)
                throw new DomainException("Usuário já está suspenso");

            Status = UserStatus.Suspended;
        }

        public void Reativar()
        {
            if (Status != UserStatus.Suspended)
                throw new DomainException("Apenas usuários suspensos podem ser reativados");

            Status = UserStatus.Active;
        }

        public void Deletar()
        {
            if (Status == UserStatus.Deleted)
                throw new DomainException("Usuário já foi deletado");

            Status = UserStatus.Deleted;
        }

        private bool VerificarSenha(string senhaFornecida)
        {
            var senhaFornecidaHash = HashPassword(senhaFornecida, Salt);
            return SenhaHash == senhaFornecidaHash;
        }

        private static string GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        private static string HashPassword(string senha, string salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(senha, Encoding.UTF8.GetBytes(salt), 10000, HashAlgorithmName.SHA256))
            {
                var hash = pbkdf2.GetBytes(20);
                return Convert.ToBase64String(hash);
            }
        }
    }

    public enum UserStatus
    {
        Active = 1,
        Suspended = 2,
        Deleted = 3
    }
}