using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.ComponentModel;
using System.Data;
using Taxi.API.Server.Models;
using Taxi.Core.Models.Export;

namespace Taxi.API.Server.Services
{
    public class GenerationFilesService(TaxiBaseContext context)
    {
        
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


        public async Task<byte[]> GeneratePDF(ExportPDFRequest pDFRequest)
        {

            byte[] pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(16));

                    page.Header().Text(pDFRequest.TtleFile).SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);

                    page.Content().PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                    {
                        x.Spacing(10);
                        x.Item().Text(pDFRequest.WhoSform);
                        x.Item().Text($"Период: {pDFRequest.Period}");
                        x.Item().Text($"Сформировал: {pDFRequest.WhoSform}(менеджер)");
                        x.Item().Text($"Класс: {pDFRequest.ClassVehicle}");
                        x.Item().Text($"Дата формирования: {pDFRequest.isCreated}");


                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(colums =>
                            {
                                colums.RelativeColumn(2); 
                                colums.RelativeColumn(2); 
                                colums.RelativeColumn(2); 
                            });


                            table.Header(header =>
                            {
                                foreach (var colum in pDFRequest.ColumnNames)
                                {
                                    header.Cell().BorderBottom(2).Padding(5).Text(colum);
                                }
                            });

                            foreach (var dat in pDFRequest.DataEarnings)
                            {
                                table.Cell().Padding(5).AlignLeft().Text(dat.GosNumber);
                                table.Cell().Padding(5).AlignLeft().Text(dat.Earnings);
                                table.Cell().Padding(5).AlignLeft().Text(dat.totalOrder);
                            }
                        });
                    });

                    

                }


                );
            }).GeneratePdf();

            return pdfBytes;

        }


    }
}
