using System.Net;

namespace Application.Abstractions
{
	public class ApplicationResponse<T>
	{
		public T? Data { get; set; }
		public bool IsSuccess { get; set; }
		public ApplicationResultStatus Status { get; set; }
		public bool IsFailed => !IsSuccess;
		public string Message { get; set; } = string.Empty;
	}

	public class ApplicationResponse
	{
		public bool IsSuccess { get; set; }
		public bool IsFailed => !IsSuccess;
		public ApplicationResultStatus Status { get; set; }
		public string Message { get; set; } = string.Empty;
	}
}
