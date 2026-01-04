using Domain.Order.Enums;

namespace Domain.Order.ValueObjects;

public class OrderSpecs
{
	public long Amount { get; }
    public string Description { get; }
    public int NumberOfTransactions { get; private set; } = 0;
	public PaymentType Type { get; }
    public OrderStatus Status { get; private set; }


	public static OrderSpecs ForSingle(long amount, string description)
	{
		return new OrderSpecs(amount, description, 1, PaymentType.Single);
	}

	public static OrderSpecs ForGrouped(long amount, string description, int count)
	{
		return new OrderSpecs(amount, description, count, PaymentType.Grouped);
	}

	private OrderSpecs(long totalAmount, string description,int numberOfTransactions,PaymentType paymentType)
	{
		if (totalAmount <= 0)
			throw new ArgumentException("Amount must be greater than zero");

		if (numberOfTransactions <= 0)
			throw new ArgumentException("NumberOfTransactions must be greater than zero");

		Amount = totalAmount;
		Description = description;
		Type = paymentType;
		Status = OrderStatus.Drafted;
		NumberOfTransactions = numberOfTransactions;
	}


	internal void SetStatus(OrderStatus status) => Status = status;
}
