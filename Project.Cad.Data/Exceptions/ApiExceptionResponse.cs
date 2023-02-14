using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Cad.Data.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ApiExceptionResponse
    {
        public string Type { get; set; }
        public string Error { get; set; }
        public string Detail { get; set; }
        public string TraceId { get; set; }
    }
}
