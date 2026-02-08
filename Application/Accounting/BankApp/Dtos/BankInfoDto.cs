using Domain.Banking.Bank.Enums;

namespace Application.Accounting.BankApp.Dtos
{
	public class BankInfoDto()
	{
		public string Title { get; set; }
		public BankCode Code { get; set; }
		public bool Status { get; set; }
		public List<ServiceTypes> ServiceTypes { get; set; }
	}
}
