using System;
using System.Threading.Tasks;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;

namespace MusicaStreaming.Application.Abstractions
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        Task<Usuario> ObterPorEmailAsync(Email email);
        Task<Usuario> ObterPorCPFAsync(CPF cpf);
        Task<bool> EmailJaExisteAsync(Email email);
        Task<bool> CPFJaExisteAsync(CPF cpf);
    }
}
