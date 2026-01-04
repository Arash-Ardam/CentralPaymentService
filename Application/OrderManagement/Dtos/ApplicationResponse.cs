namespace Application.OrderManagement.Dtos
{
	public class ApplicationResponse<T>
	{
		public T? Data { get; set; }
		public bool IsSuccess { get; set; }
		public bool IsFailed => !IsSuccess;
		public short StatusCode { get; set; }
		public string Message { get; set; } = string.Empty;
	}

	public class ApplicationResponse
	{
		public bool IsSuccess { get; set; }
		public bool IsFailed => !IsSuccess;
		public short StatusCode { get; set; }
		public string Message { get; set; } = string.Empty;
	}
}
