using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Taxi.Core.Models.Export;

namespace Taxi.WPF.Services
{
    public class GenerationFilesService
    {
        private readonly HttpClient _httpClient;

        public GenerationFilesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> GenerateExcelAsync(ExportExcelRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/export/generate-excel", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> GeneratePdfAsync(ExportPDFRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/export/generate-pdf", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

    }
}
