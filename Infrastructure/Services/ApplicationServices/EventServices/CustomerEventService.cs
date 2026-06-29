using Application.Accounting.CustomerApp.Dtos;
using Application.Accounting.CustomerApp.Services;
using AutoMapper;
using Infrastructure.DataManagements.Abstractions.ORMs;
using Infrastructure.DataManagements.DataModels;

namespace Infrastructure.Services.ApplicationServices.EventServices
{
	internal sealed class CustomerEventService
	{
		private readonly AdminEfCoreDbContext _adminDb;
		private readonly IMapper _mapper;
		public CustomerEventService(AdminEfCoreDbContext adminDb, IMapper mapper)
		{
			_adminDb = adminDb ?? throw new ArgumentNullException(nameof(adminDb));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}
		//public Task PublishEvent(CustomerEventDto eventDto)
		//{
		//	var eventModel = _mapper.Map<CustomerEventModel>(eventDto);
		//	return _adminDb.CustomerEvents.AddAsync(eventModel).AsTask();
		//}

	}
}
