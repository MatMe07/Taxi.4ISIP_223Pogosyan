using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.ComponentModel;
using System.Data;
using Taxi.API.Server.Models;
using Taxi.Core.Models.Export;

namespace Taxi.API.Server.Services
{
    public class GenerationFilesService(TaxiBaseContext context)
    {
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Salary { get; set; }
        }
        private readonly TaxiBaseContext _context = context;

        public async Task<byte[]> GenerateExcel(ExportExcelRequest excelRequest)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Отчет");
                char let = 'A';
                foreach(var cel in excelRequest.ColumnNames)
                {
                    sheet.Cells[$"{let++}1"].Value = cel;
                   
                }

                sheet.Row(1).Style.Font.Bold = true;

                sheet.Cells["A2"].LoadFromCollection(excelRequest.DataEarnings, PrintHeaders: false);

                sheet.Cells.AutoFitColumns();
                return package.GetAsByteArray();
            }

        }
        [Authorize]
        public async Task<byte[]> GeneratePDF(ExportPDFRequest pDFRequest)
        {
            return null;

        }
        [HttpPost("generate-word")]
        [Authorize]
        public async Task<byte[]> GenerateWord(ExportWordRequest exportWord)
        {
            return null;
        }
    }
}
