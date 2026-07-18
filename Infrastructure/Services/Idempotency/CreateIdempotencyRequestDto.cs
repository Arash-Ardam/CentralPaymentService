namespace Infrastructure.Services.Idempotency
{
	public class CreateIdempotencyRequestDto
	{
		public string Key { get; set; }
		public string RequestBody { get; set; }
	}
}
