using MediatR;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Exceptions;

namespace MusicaStreaming.Application.UseCases
{
    public class AlterarSenhaCommand : IRequest<AlterarSenhaResponse>
    {
        public Guid UsuarioId { get; set; }
        public string SenhaAtual { get; set; }
        public string NovaSenha { get; set; }
    }

    public class AlterarSenhaResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }

    public class AlterarSenhaCommandHandler : IRequestHandler<AlterarSenhaCommand, AlterarSenhaResponse>
    {
        private readonly IRepositorioUsuario _repositorio;

        public AlterarSenhaCommandHandler(IRepositorioUsuario repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<AlterarSenhaResponse> Handle(AlterarSenhaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await _repositorio.ObterPorIdAsync(request.UsuarioId);

                if (usuario == null)
                {
                    return new AlterarSenhaResponse
                    {
                        Sucesso = false,
                        Mensagem = "Usuário não encontrado"
                    };
                }

                usuario.AlterarSenha(request.SenhaAtual, request.NovaSenha);
                await _repositorio.SalvarAsync();

                return new AlterarSenhaResponse
                {
                    Sucesso = true,
                    Mensagem = "Senha alterada com sucesso"
                };
            }
            catch (DomainException ex)
            {
                return new AlterarSenhaResponse
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new AlterarSenhaResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao alterar senha: {ex.Message}"
                };
            }
        }
    }
}