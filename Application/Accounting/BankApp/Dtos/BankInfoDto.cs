using Domain.Banking.Bank.Enums;

namespace Application.Accounting.BankApp.Dtos
{
	public class BankInfoDto()
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public BankCode Code { get; set; }
		public bool Status { get; set; }
		public List<ServiceTypes> ServiceTypes { get; set; }
	}
}
