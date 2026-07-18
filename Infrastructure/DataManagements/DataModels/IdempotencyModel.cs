
using Infrastructure.Services.Idempotency;

namespace Infrastructure.DataManagements.DataModels
{
	internal sealed class IdempotencyModel
	{
		public int Id { get; set; }
		public string Key { get; set; }
		public string RequestBody { get; set; }
		public string ResponseBody { get; set; } = string.Empty;
		public int StatusCode { get; set; }
		public IdempotencyStatus Status { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset ExpiresAt { get; set; }
	}

}
