using Domain.Banking.Account;
using MediatR;

namespace Application.Accounting.BankApp.Events
{
	internal class BankStatusChangedEventHandler : INotificationHandler<BankStatusChangedEvent>
	{
		public BankStatusChangedEventHandler(IAccountRepository accountRepository)
		{
			_accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
		}

		public IAccountRepository _accountRepository { get; }

		public async Task Handle(BankStatusChangedEvent notification, CancellationToken cancellationToken)
		{
			var accounts = await _accountRepository.GetByBankId(notification.BankId);

			accounts.ForEach(acc => acc.ChangeStatus(notification.Status));

			await _accountRepository.EditRangeAsync(accounts);
		}
	}
}
