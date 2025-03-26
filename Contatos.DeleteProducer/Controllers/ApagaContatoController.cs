using Contatos.DataContracts.Commands;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Contatos.DeleteProducer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApagaContatoController : ControllerBase
    {
        private readonly IBus _bus;

        public ApagaContatoController(IBus bus)
        {
            _bus = bus;
        }

        /// <summary>
        /// Endpoint para verificar a disponibilidade do servi�o.
        /// </summary>
        [HttpGet]
        public ActionResult HealthCheck()
        {
            return Ok("Servi�o online.");
        }

        /// <summary>
        /// Endpoint para apagar um contato cadastrado.
        /// </summary>
        /// <param name="id">Forne�a o ID do contato a ser apagado.</param>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> ApagarContato(long id)
        {
            try
            {
                await _bus.Send(new ApagarContato
                {
                    CommandId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now,
                    IdContato = id
                });
                return Ok("Exclus�o de contato recepcionada com �xito.");
            }
            catch (Exception ex)
            {
                Log.Error($"DELETE para exclus�o de contato falhou. Exception: {ex.GetType()}. Message: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }
    }
}
