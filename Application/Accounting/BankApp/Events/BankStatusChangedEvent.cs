using MediatR;

namespace Application.Accounting.BankApp.Events
{
	internal record BankStatusChangedEvent(Guid BankId,bool Status) : INotification { }
}
