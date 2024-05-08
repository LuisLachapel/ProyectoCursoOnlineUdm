using Microsoft.AspNetCore.Mvc;
using System.IO;
using Aplicacion.Cursos;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [AllowAnonymous]
    public class ExportarDocumentoController : MiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Stream>> GetTask()
        {
            return await Mediator.Send(new ExportPDF.Consulta());
        }

    }
}
