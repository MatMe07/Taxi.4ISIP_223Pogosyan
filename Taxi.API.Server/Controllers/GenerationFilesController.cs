using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taxi.Core.Models.Export;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Route("api/export")]
    public class GenerationFilesController : ControllerBase
    {
        [HttpPost("generate-excel")]
        [Authorize]
        public async Task<IActionResult> GenerateExcel(ExportExcelRequest excelRequest)
        {
            return null;

        }
        [HttpPost("generate-pdf")]
        [Authorize]
        public async Task<IActionResult> GeneratePDF(ExportPDFRequest pDFRequest)
        {
            return null;

        }
        [HttpPost("generate-word")]
        [Authorize]
        public async Task<IActionResult> GenerateWord(ExportWordRequest exportWord)
        {
            return null;
        }


    }
}
