using Application.Abstractions;

namespace CentralPaymentWebApi.Abstractions
{
	public class ApiResponse
	{
		public ApplicationResponse AppResponse { get; set; }
		public IResult HttpResult { get; set; }
	}
	public class ApiResponse<T>
	{
		public ApplicationResponse<T> AppResponse { get; set; }
		public IResult HttpResult { get; set; }
	}
}
