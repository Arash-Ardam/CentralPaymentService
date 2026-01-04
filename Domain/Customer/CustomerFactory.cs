using Domain.Customer.Factories;

namespace Domain.Customer
{
	public static class CustomerFactory
	{
		public static CustomerInfoFactory GetInformationFactory() => new CustomerInfoFactory();
	}
}
