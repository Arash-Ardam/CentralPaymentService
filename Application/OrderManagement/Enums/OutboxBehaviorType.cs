namespace Application.OrderManagement.Enums
{
	public enum OutboxBehaviorType
	{
		Create = 1,
		Update = 2,
		Delete = 3,
		DeleteAll = 4,
		AddSpecs = 5,
		Submit = 6,
		SentToBank = 7,
		Inquiry = 8,
		AddTransactions = 9,
		RemoveTransaction = 10
	}
}
