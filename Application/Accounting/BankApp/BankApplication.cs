using Application.Accounting.BankApp.Dtos;
using Application.Accounting.BankApp.Events;
using Domain.Banking.Bank;
using Domain.Banking.Bank.Enums;
using Domain.Banking.Bank.Services;
using MediatR;

namespace Application.Accounting.BankApp
{
	internal class BankApplication
	{
		private readonly IBankRepository _bankRepository;
		private readonly IBankIdentifierService _bankIdentifierService;
		private readonly IMediator _mediator;

		public BankApplication(IBankRepository bankRepository, IBankIdentifierService bankIdentifierService, IMediator mediator)
		{
			_bankRepository = bankRepository ?? throw new ArgumentNullException(nameof(bankRepository));
			_bankIdentifierService = bankIdentifierService ?? throw new ArgumentNullException(nameof(bankIdentifierService));
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		public async Task<Guid> CreateBank(CreateBankDto bankDto)
		{
			bool isExist = await _bankIdentifierService.IsBankExists(bankDto.Name, bankDto.BankCode);

			if (isExist)
				throw new ArgumentException($"The given Bank with name:{bankDto.Name} and code:{bankDto.BankCode} already excists");

			Bank newBank = new Bank(bankDto.Name, bankDto.BankCode);

			var createdBank = await _bankRepository.AddAsync(newBank);

			return createdBank.Id;
		}

		public async Task AddPaymentServiceType(Guid bankId,ServiceTypes service)
		{
			Bank? targetBank = await _bankRepository.GetAsync(bankId);
			if (targetBank == null)
				throw new ArgumentException("given target bank is not excists");

			targetBank.AddService(service);

			await _bankRepository.EditAsync(targetBank);
		}

		public async Task RemovePaymentServiceType(Guid bankId, ServiceTypes service)
		{
			Bank? targetBank = await _bankRepository.GetAsync(bankId);
			if (targetBank == null)
				throw new ArgumentException("given target bank is not excists");

			targetBank.RemoveService(service);

			await _bankRepository.EditAsync(targetBank);
		}


		public async Task ChangeStatusAsync(Guid bankId, bool status)
		{
			Bank? targetBank = await _bankRepository.GetAsync(bankId);
			if (targetBank == null)
				throw new ArgumentException("given target bank is not excists");

			targetBank.ChangeStatus(status);

			await _bankRepository.EditAsync(targetBank);

			// raise disable related accounts event
			await _mediator.Publish(new BankStatusChangedEvent(bankId, status));
		}

	}
}
