namespace Application.OrderManagement.Dtos.SingleOrder
{
	public record SingleTransactionDto(
		Guid OrderId,
		long Amount,
		string Description,
		string FirstName,
		string LastName,
		string AccountNuumber
		);
}
