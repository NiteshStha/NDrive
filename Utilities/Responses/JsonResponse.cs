using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Responses
{
    public class JsonResponse<T>
    {
        public int StatusCode { get; set; }
        public int? Count { get; set; }
        public T Data { get; set; }

        public JsonResponse(int statusCode, int? count , T data)
        {
            StatusCode = statusCode;
            Count = count;
            Data = data;
        }
    }
}
