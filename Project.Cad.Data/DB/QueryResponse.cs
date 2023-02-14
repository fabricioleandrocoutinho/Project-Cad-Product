using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Cad.Data.DB
{
    public class QueryResponse<T>
    {
        [JsonPropertyName("data")]
        public IEnumerable<T> Data { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("totalItems")]
        public long TotalItems { get; set; }

        [JsonPropertyName("totalPages")]
        public long TotalPages { get; set; }
    }
}
