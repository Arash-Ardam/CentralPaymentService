using Domain.Banking.Account;
using MediatR;

namespace Application.Accounting.CustomerApp.Events
{
	internal class CustomerStatusChangedEventHandler : INotificationHandler<CustomerStatusChangedEvent>
	{
		public CustomerStatusChangedEventHandler(IAccountRepository accountRepository)
		{
			_accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
		}

		public IAccountRepository _accountRepository { get; }

		public async Task Handle(CustomerStatusChangedEvent notification, CancellationToken cancellationToken)
		{
			var accounts = await _accountRepository.GetByCustomerId(notification.CustomerId);
			if (accounts != null)
			{
				accounts.ForEach(acc => acc.ChangeStatus(notification.Status));
				await _accountRepository.EditRangeAsync(accounts);
			}
		}
	}
}
