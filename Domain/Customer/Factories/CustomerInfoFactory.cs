using Domain.Customer.ValueObjects;

namespace Domain.Customer.Factories
{
	public class CustomerInfoFactory
	{
		internal CustomerInfoFactory() { }

		private string _firstName = string.Empty;
		private string _lastName = string.Empty;
		private string _nationalCode = string.Empty;


		public CustomerInfoFactory WithFirstName(string value)
		{
			_firstName = value; 
			return this;
		}

		public CustomerInfoFactory WithLastName(string value)
		{
			_lastName = value;
			return this;
		}

		public CustomerInfoFactory WithNationalCode(string value)
		{
			_nationalCode = value;
			return this;
		}

		public CustomerInformation Build()
		{
			return new CustomerInformation(_firstName, _lastName, _nationalCode);
		}


	}
}
