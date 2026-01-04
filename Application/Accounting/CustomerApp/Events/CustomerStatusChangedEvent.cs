using MediatR;

namespace Application.Accounting.CustomerApp.Events
{
	internal record CustomerStatusChangedEvent(Guid CustomerId,bool Status) : INotification { }
}
