namespace Ford.WebApi.Data
{
    public class ResponseError
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public ResponseError(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public ResponseError()
        {
            Title = string.Empty;
            Message = string.Empty;
        }
    }
}
