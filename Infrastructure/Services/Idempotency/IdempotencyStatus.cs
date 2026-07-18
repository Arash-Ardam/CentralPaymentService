namespace Infrastructure.Services.Idempotency;

public enum IdempotencyStatus
{
	Pending = 0,
	Completed = 1,
	Rejected = 2,
	Failed = 3
}
