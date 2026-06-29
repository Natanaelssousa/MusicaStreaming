using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicaStreaming.Application.UseCases;
using System.Security.Claims;

namespace MusicaStreaming.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsuarioController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] CreateUsuarioCommand command)
        {
            var resultado = await _mediator.Send(command);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return CreatedAtAction(nameof(Registrar), resultado);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioCommand command)
        {
            var resultado = await _mediator.Send(command);

            if (!resultado.Sucesso)
                return Unauthorized(resultado);

            return Ok(resultado);
        }

        // Endpoint PRIVADO - para usuário logado
        [HttpPost("confirmar-email")]
        [Authorize]
        public async Task<IActionResult> ConfirmarEmail()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized();

            var command = new ConfirmarEmailCommand { UsuarioId = Guid.Parse(usuarioId) };
            var resultado = await _mediator.Send(command);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPost("alterar-senha")]
        [Authorize]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaCommand command)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized();

            command.UsuarioId = Guid.Parse(usuarioId);
            var resultado = await _mediator.Send(command);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }
        [HttpPost("confirmar-email-publico")]
        [AllowAnonymous]  // ← SEM AUTENTICAÇÃO
        public async Task<IActionResult> ConfirmarEmailPublico([FromBody] ConfirmarEmailCommand command)
        {
            var resultado = await _mediator.Send(command);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}