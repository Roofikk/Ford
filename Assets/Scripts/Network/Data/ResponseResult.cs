using System.Collections.Generic;
using System.Net;

namespace Ford.WebApi.Data
{
    public class ResponseResult<T> : ResponseResult
    {
        public T Content { get; set; }

        public ResponseResult(T content, HttpStatusCode code = HttpStatusCode.OK, List<ResponseError> errors = null)
            :base(code, errors)
        {
            Content = content;
        }
    }

    public class ResponseResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public List<ResponseError> Errors { get; set; }

        public ResponseResult(HttpStatusCode code = HttpStatusCode.OK, List<ResponseError> errors = null)
        {
            StatusCode = code;
            Errors = errors ?? new List<ResponseError>();
        }
    }
}
