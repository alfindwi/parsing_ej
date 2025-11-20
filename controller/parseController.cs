using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using parsing_jrn_Ej.Services;

namespace parsing_jrn_Ej.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParseController : ControllerBase
    {
        private readonly JrnParserService _service;

        public ParseController(JrnParserService service)
        {
            _service = service;
        }


        [HttpPost("process")]
        public async Task<IActionResult> ProcessPending()
        {
            await _service.ProcessPendingFilesAsync();
            return Ok(new { message = "All pending JRN files processed successfully." });
        }

        [HttpGet("ej")]
        public async Task<IActionResult> GetAllTransaksi([FromQuery] int page = 1)
        {
            var result = await _service.getAllTransaksi(page);
            return Ok(result);
        }



        [HttpGet("error")]
        public async Task<IActionResult> GetPesanErrorFiles()
        {
            var pendingFiles = await _service.getPesanError();
            return Ok(pendingFiles);
        }
    }
}
