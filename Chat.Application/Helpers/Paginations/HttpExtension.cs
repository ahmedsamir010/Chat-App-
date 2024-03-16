using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Chat.Application.Helpers.Paginations
{
    public static class HttpExtension
    {
        public static void AddPaginationHeaders(this HttpResponse response, int currentPage, int totalPages, int totalSize, int itemPerPage)
        {
            PaginationHeaders headers = new(currentPage, totalPages, totalSize, itemPerPage);

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            string serializedHeaders = JsonSerializer.Serialize(headers, jsonSerializerOptions);
            response.Headers.Append("Pagination", serializedHeaders);
            response.Headers.Append("Access-Control-Expose-Headers","Pagination");
        }
    }
}
