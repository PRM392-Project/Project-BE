using AutoMapper;
using SnapRoom.Contract.Repositories.Entities;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Contract.Services;

namespace SnapRoom.Services
{
	public class PlanService : IPlanService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IAuthService _authenticationService;

		public PlanService(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authenticationService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_authenticationService = authenticationService;
		}

		public async Task CreatePlan()
		{
			Plan plan = new Plan
			{
				Name = "Free Plan",
				Description = "This is a free plan with limited features."
			};

			await _unitOfWork.GetRepository<Plan>().InsertAsync(plan);
			await _unitOfWork.SaveAsync();
		}
	}
}
