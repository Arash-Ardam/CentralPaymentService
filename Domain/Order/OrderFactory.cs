using Domain.Order.Builders;
using Domain.Order.Enums;
namespace Domain.Order;

public class OrderFactory
{

	public static Order CreateSingle(Guid accountId,long totalAmount, string description)
	{
		string orderId = Random.Shared.Next(1000000, 9999999).ToString();

		Order order = new Order(orderId);

		order.SetSingleOrderSpecs(totalAmount, description);
		order.AssignToAccount(accountId);

		return order;
	}

	public static Order CreateGroup(Guid accountId, long totalAmount, string description,int numberOfTransactions)
	{
		string orderId = Random.Shared.Next(1000000, 9999999).ToString();

		Order order = new Order(orderId);

		order.SetGroupedOrderSpecs(totalAmount, description, numberOfTransactions);
		order.AssignToAccount(accountId);

		return order;
	}

	public static SingleTransactionBuilder GetSingleTransactionBuilder() => new SingleTransactionBuilder();
	public static GroupedTransactionBuilder GetGroupedTransactionBuilder() => new GroupedTransactionBuilder();
}
