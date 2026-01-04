namespace Domain.Customer.ValueObjects
{
	public class CustomerInformation
	{
		public string FirstName { get; private set; }
		public string LastName { get; private set; }
		public string NationalCode { get; private set; }

		internal CustomerInformation(string firstName, string lastName, string nationalCode)
		{
			FirstName = firstName;
			LastName = lastName;
			NationalCode = nationalCode;
		}

	}
}
