namespace CentralPaymentWebApi.Abstractions
{
	public static class RouteTemplates
	{
		public const string Create = "create";
		public const string Update = "update";
		public const string Delete = "delete";
		public const string Get = "get/{id:guid}";
		public const string GetAll = "getAll";
	}
}
