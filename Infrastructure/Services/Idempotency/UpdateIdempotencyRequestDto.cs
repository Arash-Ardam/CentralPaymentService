namespace Infrastructure.Services.Idempotency
{
	public class UpdateIdempotencyRequestDto
	{
		public int Id { get; set; }
		public string Key { get; set; }
		public string RequestBody { get; set; }
		public string ResponseBody { get; set; } = string.Empty;
		public int StatusCode { get; set; }
		public IdempotencyStatus Status { get; set; }
	}
}
