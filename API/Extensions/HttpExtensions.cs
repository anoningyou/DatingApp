using System.Text.Json;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse responce,
            PaginationHeader header)
        {
            var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            responce.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            responce.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }

        public static void AddPaginationHeader<T>(this HttpResponse responce,
            PagedList<T> list)
        {
            responce.AddPaginationHeader(new PaginationHeader(list.CurrentPage,list.PageSize,
                list.TotalCount, list.TotalPages));
        }

        
    }
}