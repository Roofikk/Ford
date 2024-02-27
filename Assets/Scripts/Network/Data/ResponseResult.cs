using System.Collections.Generic;
using System.Net;

namespace Ford.WebApi.Data
{
    public class ResponseResult<T>
    {
        public T Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<ResponseError> Errors { get; set; }

        public ResponseResult(T content, HttpStatusCode code = HttpStatusCode.OK, List<ResponseError> errors = null)
        {
            Content = content;
            StatusCode = code;
            Errors = errors ?? new List<ResponseError>();
        }
    }
}
