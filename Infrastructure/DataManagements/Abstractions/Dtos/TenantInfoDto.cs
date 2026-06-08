namespace Infrastructure.DataManagements.Abstractions.Dtos
{
	internal sealed class TenantInfoDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string? ConnectionString { get; set; }
	}
}
