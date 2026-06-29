using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using MusicaStreaming.Application.UseCases;
using System.Security.Claims;

namespace MusicaStreaming.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("musica/criar")]
        public async Task<IActionResult> CriarMusica([FromBody] CreateMusicaCommand command)
        {
            var resultado = await _mediator.Send(command);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return CreatedAtAction(nameof(CriarMusica), resultado);
        }

        [HttpGet("musicas/buscar")]
        public async Task<IActionResult> BuscarMusicas([FromQuery] string termo = "")
        {
            var query = new GetMusicasQuery { Termo = termo };
            var resultado = await _mediator.Send(query);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpGet("favoritos")]
        [Authorize]
        public async Task<IActionResult> GetFavoritos()
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized();

            var query = new GetFavoritosQuery { UsuarioId = Guid.Parse(usuarioId) };
            var resultado = await _mediator.Send(query);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPost("musica/favoritar")]
        [Authorize]
        public async Task<IActionResult> FavoritarMusica([FromBody] FavoritarMusicaCommand command)
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