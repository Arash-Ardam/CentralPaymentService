namespace Infrastructure.DataManagements
{
	internal class ORMToolsOptions
	{
		public EfCoreOptions EfCore { get; set; }
	}

	internal class EfCoreOptions
	{
		public string BaseConnectionString { get; set; }
		public string TenantConnectionString { get; set; }
		public bool isEnable { get; set; }
		public short RetryCount { get; set; }
		public TimeSpan RetryDelay { get; set; }
	}

}
