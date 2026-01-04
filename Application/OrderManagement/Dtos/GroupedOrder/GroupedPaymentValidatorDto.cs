namespace Application.OrderManagement.Dtos.GroupedOrder
{
	public class GroupedPaymentValidatorDto
	{
		public long? AllowedTotalAmount { get; set; }
		public long AllowedNumberOfTransactions { get; set; }

		public long EntryTotalAmount { get; set; }
		public long EntryNumberOfTransactions { get; set; }
	}
}
