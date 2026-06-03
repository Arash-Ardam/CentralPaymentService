using Application.Accounting.CustomerApp.Dtos;

namespace Application.Accounting.CustomerApp.Services;

public interface ICustomerEventService
{
	Task PublishEvent(CustomerEventDto eventDto);
}
