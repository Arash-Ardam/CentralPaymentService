namespace Application.OrderManagement.Ports.Abstractions
{
	public class ProviderResponse<T>
	{
		public T? Data { get; set; }
		public bool IsSuccess { get; set; }
		public bool IsFailed => !IsSuccess;
		public short StatusCode { get; set; }
		public string Message { get; set; } = string.Empty;
	}
}
