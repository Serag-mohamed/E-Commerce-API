namespace E_Commerce.DTOs
{
    public class OperationResult
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();

    }

    public class OperationResult<T> : OperationResult
    {
        public T? Data { get; set; }
    }
}
