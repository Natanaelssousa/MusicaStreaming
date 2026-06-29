using MediatR;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Domain.Exceptions;
using MusicaStreaming.Application.Abstractions;

namespace MusicaStreaming.Application.UseCases
{
    public class CreateUsuarioCommand : IRequest<CreateUsuarioResponse>
    {
        public string Email { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Senha { get; set; }
    }

    public class CreateUsuarioResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Mensagem { get; set; }
        public bool Sucesso { get; set; }
    }

    public class CreateUsuarioCommandHandler : IRequestHandler<CreateUsuarioCommand, CreateUsuarioResponse>
    {
        private readonly IRepositorioUsuario _repositorio;

        public CreateUsuarioCommandHandler(IRepositorioUsuario repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<CreateUsuarioResponse> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var emailJaExiste = await _repositorio.EmailJaExisteAsync(new Email(request.Email));
                if (emailJaExiste)
                {
                    return new CreateUsuarioResponse
                    {
                        Sucesso = false,
                        Mensagem = "Email já cadastrado"
                    };
                }

                var cpfJaExiste = await _repositorio.CPFJaExisteAsync(new CPF(request.CPF));
                if (cpfJaExiste)
                {
                    return new CreateUsuarioResponse
                    {
                        Sucesso = false,
                        Mensagem = "CPF já cadastrado"
                    };
                }

                var usuario = Usuario.Create(request.Email, request.Nome, request.CPF, request.Senha);
                await _repositorio.AdicionarAsync(usuario);
                await _repositorio.SalvarAsync();

                return new CreateUsuarioResponse
                {
                    Id = usuario.Id,
                    Email = usuario.Email.Value,
                    Nome = usuario.Nome,
                    Sucesso = true,
                    Mensagem = "Usuário criado com sucesso"
                };
            }
            catch (DomainException ex)
            {
                return new CreateUsuarioResponse
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new CreateUsuarioResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao criar usuário: {ex.Message}"
                };
            }
        }
    }
}
