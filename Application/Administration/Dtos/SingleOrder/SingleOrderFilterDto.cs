using System;

namespace Application.Administration.Dtos.SingleOrder
{
	public sealed class SingleOrderFilterDto
	{
		public Guid TenantId { get; set; }
		public string? OrderId { get; set; }
		public long? Amount { get; set; }
		public string? OwnerAccount { get; set; }
		public string? DepositAccount { get; set; }
	}
}
