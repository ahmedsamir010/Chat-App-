namespace Chat.API.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        private static readonly Dictionary<int, string> DefaultMessages = new Dictionary<int, string>
        {
            [200] = "Success: Request completed successfully.",
            [400] = "Error: Invalid request.",
            [401] = "Unauthorized: Access denied.",
            [404] = "Not Found: Resource not available.",
            [500] = "Server Error: Oops! Something went wrong."
        };
        public ApiResponse(int statusCode, string? message = null, List<string>? errors = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessage(statusCode);
            Errors = errors ?? new List<string>();
        }
        private string GetDefaultMessage(int statusCode)
        {
            return DefaultMessages.TryGetValue(statusCode, out string? defaultMessage) ? defaultMessage : "An error occurred";
        }
    }
}
