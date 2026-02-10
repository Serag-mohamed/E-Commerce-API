namespace E_Commerce.DTOs.OutputDtos
{
    public class OperationResult
    {
        public bool IsSucceeded { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = [];

    }

    public class OperationResult<T> : OperationResult
    {
        public T? Data { get; set; }
    }
}
