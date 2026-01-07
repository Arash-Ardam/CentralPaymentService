using Domain.Order.Entities;
using Domain.Order.Enums;
using Domain.Order.ValueObjects;
namespace Domain.Order;

public class Order
{
	internal Order(string orderId)
	{
		OrderId = orderId;
	}

	public Guid Id { get; set; }
	public string OrderId { get; private set; } = string.Empty;
	public string TrackingCode { get; private set; } = string.Empty;
	public string ProviderMessage { get; private set; } = string.Empty;
	public OrderSpecs? Specifics { get; private set; }
	public Guid SourceAccountId { get; private set; }
	public SingleTransaction? SingleTransaction { get; private set; } 
	public List<GroupedTransaction> GroupedTransactions { get; private set; } = new List<GroupedTransaction>();

	public void SetSingleOrderSpecs(long totalAmount, string description)
	{
		if (Specifics != null && Specifics.Status != OrderStatus.Drafted)
			throw new InvalidOperationException("only drafted orders can changes");

		Specifics = OrderSpecs.ForSingle(totalAmount, description);
	}


	public void SetGroupedOrderSpecs(long totalAmount, string description, int numberOfTransactions)
	{
		if (Specifics != null && Specifics.Status != OrderStatus.Drafted)
			throw new InvalidOperationException("only drafted orders can changes");

		Specifics = OrderSpecs.ForGrouped(totalAmount, description, numberOfTransactions);
	}

	public void AssignToAccount(Guid accountId) => SourceAccountId = accountId;

	private void EnsureOrderType(PaymentType expected)
	{
		if (Specifics == null)
			throw new InvalidOperationException("Order specs not defined");

		if (Specifics.Type != expected)
			throw new InvalidOperationException("Invalid order type");
	}

	public void AddSingleTransaction(SingleTransaction transaction)
	{
		EnsureOrderType(PaymentType.Single);

		if (SingleTransaction is not null)
			throw new ArgumentException("In Single Order there should be at most one transaction");

		if (transaction.Specs.Amount > Specifics.Amount)
			throw new ArgumentException($"Amount entry hits totalAmount = {Specifics.Amount}, transaction: Amount = {transaction.Specs.Amount}");

		SingleTransaction = transaction;
	}

	public void AddGroupedTransactions(List<GroupedTransaction> transactions)
	{
		EnsureOrderType(PaymentType.Grouped);

		long submittedCount = GroupedTransactions.Count();
		long toSubmitCount = transactions.Count();

		long totalSubmittedAmount = GroupedTransactions.Sum(trx => trx.Specs.Amount);
		long totalSubmittedTransactions = transactions.Sum(trx => trx.Specs.Amount);

		if (submittedCount + toSubmitCount > Specifics.NumberOfTransactions)
			throw new ArgumentException($"Order hits max transactions row = {Specifics.NumberOfTransactions} , entry count ={toSubmitCount} ,already row numbers = {submittedCount}");


		else if (submittedCount + toSubmitCount < Specifics.NumberOfTransactions)
		{
			if (totalSubmittedAmount + totalSubmittedTransactions >= Specifics.Amount)
				throw new ArgumentException($"Order in less transactions count hits Amount : total = {Specifics.Amount} , entry ={totalSubmittedTransactions} ,existed= {totalSubmittedAmount}");
		}
		else
		{
			if (totalSubmittedAmount + totalSubmittedTransactions < Specifics.Amount)
				throw new ArgumentException($"target submitted amount : {totalSubmittedAmount + totalSubmittedTransactions} is not fill Order amount : {Specifics.Amount}");
		}

		GroupedTransactions.AddRange(transactions);
	}

	public void RemoveGroupedTransaction(Guid transactionId)
	{
		GroupedTransaction? targetTransaction = GroupedTransactions.FirstOrDefault(x => x.Id == transactionId);

		if (targetTransaction is null)
			throw new ArgumentException($"There is no transaction with given id = {transactionId}");

		GroupedTransactions.Remove(targetTransaction);
	}

	public void RemoveGroupedRangeTransactions(List<Guid> transactionIds)
	{
		var targetRansactions = GroupedTransactions.Where(x => transactionIds.Contains(x.Id));

		foreach (var item in targetRansactions)
		{
			GroupedTransactions.Remove(item);
		}

	}

	public void RemoveSingleTransaction()
	{
		if (SingleTransaction is null)
			throw new ArgumentException($"There is no single transaction for order to remove");

		SingleTransaction = null;
	}

	public void FinalizeGroupedOrder()
	{
		var submittedCount = GroupedTransactions.Count();

		if (submittedCount < Specifics.NumberOfTransactions)
			throw new SystemException($"the system transaction count is {Specifics.NumberOfTransactions} you submitted {submittedCount}");

		Submit();
	}

	public void FinalizeSingleOrder()
	{
		if (SingleTransaction is null)
			throw new ArgumentException($"There is no single transaction for order to submit");

		Submit();
	}

	public void EnsurePayable()
	{
		if (Specifics.Status != OrderStatus.Submited)
			throw new SystemException("target order is not submitted or has irregular status");
	}

	public void EnsureSent()
	{
		if (Specifics.Status == OrderStatus.Drafted || Specifics.Status == OrderStatus.Submited)
			throw new SystemException("target order is not Sent to provider service");
	}

	public void MarkSingleRequestAsSent(string trackingCode, string message)
	{
		TrackingCode = trackingCode;
		Specifics.SetStatus(OrderStatus.Pending);
		SingleTransaction.ProviderMessage = message;
	}

	public void MarkSingleRequestStatus(OrderStatus status, string message)
	{
		Specifics.SetStatus(status);
		SingleTransaction.ProviderMessage = message;
	}

	
	public void MarkGroupedRequestStatus(OrderStatus status,string? trackingCode, string message)
	{
		TrackingCode=trackingCode ?? string.Empty;
		Specifics.SetStatus(status);
		ProviderMessage = message ?? string.Empty;
	}

	public GroupedTransaction? GetGroupedTrasaction(Guid transactionId) => GroupedTransactions.FirstOrDefault(trx => trx.Id == transactionId);
	public GroupedTransaction? GetGroupedTrasaction(string paymentId) => GroupedTransactions.FirstOrDefault(trx => trx.Specs.PaymentId == paymentId);

	public void MarkGroupedRequestTransactionStatus(GroupedTransaction transaction, GroupedTransactionStatus status, string? trackingCode, string message)
	{
		transaction.Status = status;
		transaction.TrackingId = trackingCode ?? string.Empty;
		transaction.ProviderMessage= message ?? string.Empty;
	}
	

	private void Submit()
	{
		Specifics.SetStatus(OrderStatus.Submited);
	}


}
