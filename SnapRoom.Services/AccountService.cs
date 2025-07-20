using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SnapRoom.Common.Base;
using SnapRoom.Common.Enum;
using SnapRoom.Contract.Repositories.Entities;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Contract.Services;

namespace SnapRoom.Services
{
	public class AccountService : IAccountService
	{

		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IAuthService _authService;
		private readonly IConfiguration _config;

		public AccountService(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authenticationService, IConfiguration config)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_authService = authenticationService;
			_config = config;
		}

		public async Task<BasePaginatedList<object>> GetAccounts(RoleEnum? role, int pageNumber, int pageSize)
		{
			List<Account> query = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => (role == null || a.Role == role) && a.VerificationToken == null && a.DeletedBy == null).ToListAsync();

			var responseItems = query.Select(x => new
			{
				x.Id,
				x.Name,
				x.Email,
				x.Profession,
				x.ContactNumber,
				x.Role,
				x.ApplicationUrl,
				x.AvatarSource
			}).ToList();

			return new BasePaginatedList<object>(responseItems, query.Count, pageNumber, pageSize);
		}

		public async Task<BasePaginatedList<object>> GetAwaitingDesigners(int pageNumber, int pageSize)
		{
			List<Account> query = await _unitOfWork.GetRepository<Account>().Entities
				.Where(a => a.Role == RoleEnum.Designer && a.VerificationToken != null && a.DeletedBy == null).ToListAsync();

			var responseItems = query.Select(x => new
			{
				x.Id,
				x.Name,
				x.Email,
				x.Profession,
				x.ContactNumber,
				x.Role,
				x.ApplicationUrl
			}).ToList();

			return new BasePaginatedList<object>(responseItems, query.Count, pageNumber, pageSize);
		}

	}
}
