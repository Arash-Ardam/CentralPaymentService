namespace Application.Accounting.CustomerApp.Dtos
{
	public record InformationDto(Guid CustomerId,string FirstName, string LastName, string NationalCode,bool isEnable,string ConnectionString);
}
