using Microsoft.AspNetCore.Mvc;
using MediatR;
using MusicaStreaming.Application.UseCases;

namespace MusicaStreaming.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusicaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MusicaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string termo = "")
        {
            var query = new GetMusicasQuery { Termo = termo };
            var resultado = await _mediator.Send(query);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPost("favoritar")]
        public async Task<IActionResult> Favoritar([FromBody] FavoritarMusicaCommand command)
        {
            var resultado = await _mediator.Send(command);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}