namespace Application.OrderManagement.Dtos.SingleOrder
{
	public record CreateSingleOrderDto(Guid AccountId,long Amount,string Description);
}
