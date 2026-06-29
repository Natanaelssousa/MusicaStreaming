using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicaStreaming.Application.UseCases;
using System.Security.Claims;

namespace MusicaStreaming.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ArtistaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("criar")]
        public async Task<IActionResult> Criar([FromBody] CreateArtistaCommand command)
        {
            var resultado = await _mediator.Send(command);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return CreatedAtAction(nameof(Criar), resultado);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string termo = "")
        {
            var resultado = await _mediator.Send(new GetArtistasQuery { Termo = termo });
            if (!resultado.Sucesso) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("favoritar")]
        [Authorize]
        public async Task<IActionResult> Favoritar([FromBody] FavoritarArtistaCommand command)
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

            command.UsuarioId = Guid.Parse(usuarioId);
            var resultado = await _mediator.Send(command);
            if (!resultado.Sucesso) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpGet("favoritos")]
        [Authorize]
        public async Task<IActionResult> Favoritos()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

            var resultado = await _mediator.Send(new GetArtistasFavoritosQuery { UsuarioId = Guid.Parse(usuarioId) });
            if (!resultado.Sucesso) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}