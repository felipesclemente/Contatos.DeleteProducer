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
        /// Endpoint para verificar a disponibilidade do serviço.
        /// </summary>
        [HttpGet]
        public ActionResult HealthCheck()
        {
            return Ok("Serviço online.");
        }

        /// <summary>
        /// Endpoint para apagar um contato cadastrado.
        /// </summary>
        /// <param name="id">Forneça o ID do contato a ser apagado.</param>
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
                return Ok("Exclusão de contato recepcionada com êxito.");
            }
            catch (Exception ex)
            {
                Log.Error($"DELETE para exclusão de contato falhou. Exception: {ex.GetType()}. Message: {ex.Message}.");
                return BadRequest(ex.Message);
            }
        }
    }
}
