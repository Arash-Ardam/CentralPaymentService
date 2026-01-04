namespace Application.OrderManagement.Dtos.GroupedOrder
{
	public record CreateGroupedOrderDto(Guid AccountId, long TotalAmount, string Description, int NumberOfTransactions);
}
