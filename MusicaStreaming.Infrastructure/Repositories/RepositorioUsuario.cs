using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicaStreaming.Application.Abstractions;
using MusicaStreaming.Domain.Aggregates;
using MusicaStreaming.Domain.ValueObjects;
using MusicaStreaming.Infrastructure.Data;

namespace MusicaStreaming.Infrastructure.Repositories
{
    public class RepositorioUsuario : RepositorioGenerico<Usuario>, IRepositorioUsuario
    {
        public RepositorioUsuario(MusicaStreamingDbContext context) : base(context) { }

        public async Task<Usuario> ObterPorEmailAsync(Email email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email.Value == email.Value);
        }

        public async Task<Usuario> ObterPorCPFAsync(CPF cpf)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.CPF.Value == cpf.Value);
        }

        public async Task<bool> EmailJaExisteAsync(Email email)
        {
            return await _dbSet.AnyAsync(u => u.Email.Value == email.Value);
        }

        public async Task<bool> CPFJaExisteAsync(CPF cpf)
        {
            return await _dbSet.AnyAsync(u => u.CPF.Value == cpf.Value);
        }
    }
}