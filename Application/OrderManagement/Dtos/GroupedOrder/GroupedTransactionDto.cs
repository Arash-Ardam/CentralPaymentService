namespace Application.OrderManagement.Dtos.GroupedOrder
{
	public record AddGroupedTransactionDto(Guid OrderId, List<GroupedTransactionDto> transactions);

	public record GroupedTransactionDto(
		long Amount,
		string Description,
		string FirstName,
		string LastName,
		string AccountNuumber,
		string Iban,
		string NationalId
		);
}
