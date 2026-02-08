using Domain.Banking.Bank.Enums;

namespace CentralPaymentWebApi.Dtos.BankApi
{
	public class AssignServiceDto
	{
		public Guid BankId { get; set; }
		public List<ServiceTypes> ServiceTypes { get; set; }
	}
}
