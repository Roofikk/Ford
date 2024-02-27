using System.Collections.Generic;

namespace Ford.WebApi.Data
{
    internal class BadRequestDto
    {
        public string Uri { get; set; }
        public string Header { get; set; }
        public int Status { get; set; }
        public List<ResponseError> Errors { get; set; }
    }
}
