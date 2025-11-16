using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using parsing_jrn_Ej.Services;

namespace parsing_jrn_Ej.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParseController : ControllerBase
    {
        private readonly JrnParserService _parser;

        public ParseController(JrnParserService parser)
        {
            _parser = parser;
        }


        [HttpPost("process")]
        public async Task<IActionResult> ProcessPending()
        {
            await _parser.ProcessPendingFilesAsync();
            return Ok(new { message = "All pending JRN files processed successfully." });
        }

        [HttpGet("ej")]
        public async Task<IActionResult> GetPendingFiles()
        {
            var pendingFiles = await _parser.getAllTransaksi();
            return Ok(pendingFiles);
        }
    }
}
