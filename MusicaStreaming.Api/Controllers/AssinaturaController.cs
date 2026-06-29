using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using MusicaStreaming.Application.UseCases;
using System.Security.Claims;

namespace MusicaStreaming.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssinaturaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AssinaturaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("criar")]
        public async Task<IActionResult> Criar([FromBody] CreateAssinaturaCommand command)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized();

            command.UsuarioId = Guid.Parse(usuarioId);
            var resultado = await _mediator.Send(command);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return CreatedAtAction(nameof(Criar), resultado);
        }

        [HttpPost("renovar")]
        public async Task<IActionResult> Renovar()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized();

            var command = new RenovarAssinaturaCommand { UsuarioId = Guid.Parse(usuarioId) };
            var resultado = await _mediator.Send(command);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}
