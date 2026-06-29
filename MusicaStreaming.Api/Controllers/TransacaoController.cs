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
    public class TransacaoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransacaoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("autorizar")]
        public async Task<IActionResult> Autorizar([FromBody] AutorizarTransacaoCommand command)
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
    }
}