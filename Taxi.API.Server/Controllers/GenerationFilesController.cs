using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taxi.API.Server.Services;
using Taxi.Core.Models.Export;

namespace Taxi.API.Server.Controllers
{
    [ApiController]
    [Route("api/export")]
    public class GenerationFilesController : ControllerBase
    {
        private readonly GenerationFilesService generationFilesService;
        public GenerationFilesController(GenerationFilesService generationFilesService)
        {
            this.generationFilesService = generationFilesService;
        }

        [HttpPost("generate-excel")]
        [Authorize]
        public async Task<IActionResult> GenerateExcel(ExportExcelRequest excelRequest)
        {

            var d = await generationFilesService.GenerateExcel(excelRequest);
            return File(d, "application/excel", excelRequest.fileName);

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
