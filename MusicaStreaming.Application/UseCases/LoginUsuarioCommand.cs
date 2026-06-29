using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.Exceptions;
using MusicaStreaming.Domain.ValueObjects;

namespace MusicaStreaming.Application.UseCases
{
    public class LoginUsuarioCommand : IRequest<LoginUsuarioResponse>
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    public class LoginUsuarioResponse
    {
        public Guid UsuarioId { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Token { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class LoginUsuarioCommandHandler : IRequestHandler<LoginUsuarioCommand, LoginUsuarioResponse>
    {
        private readonly IRepositorioUsuario _repositorio;
        private readonly IConfiguration _configuration;

        public LoginUsuarioCommandHandler(IRepositorioUsuario repositorio, IConfiguration configuration)
        {
            _repositorio = repositorio;
            _configuration = configuration;
        }

        public async Task<LoginUsuarioResponse> Handle(LoginUsuarioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await _repositorio.ObterPorEmailAsync(new Email(request.Email));

                if (usuario == null)
                {
                    return new LoginUsuarioResponse
                    {
                        Sucesso = false,
                        Mensagem = "Usuário ou senha inválidos"
                    };
                }

                usuario.FazerLogin(request.Senha);

                var token = GerarToken(usuario);

                return new LoginUsuarioResponse
                {
                    UsuarioId = usuario.Id,
                    Email = usuario.Email.Value,
                    Nome = usuario.Nome,
                    Token = token,
                    Sucesso = true,
                    Mensagem = "Login realizado com sucesso"
                };
            }
            catch (DomainException ex)
            {
                return new LoginUsuarioResponse
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new LoginUsuarioResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao fazer login: {ex.Message}"
                };
            }
        }

        private string GerarToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email.Value),
                new Claim(ClaimTypes.Name, usuario.Nome)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpirationMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}