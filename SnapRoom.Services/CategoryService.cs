using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SnapRoom.Common.Base;
using SnapRoom.Contract.Repositories.Entities;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Contract.Services;

namespace SnapRoom.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IAuthService _authenticationService;

		public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authenticationService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_authenticationService = authenticationService;
		}

		public async Task<BasePaginatedList<object>> GetCategories(bool? style, int pageNumber, int pageSize)
		{
			List<Category> query = await _unitOfWork.GetRepository<Category>().Entities
				.Where(c => style == null || c.Style == style).ToListAsync();
			var responseItems = query.Select(x => new
			{
				x.Id,
				x.Name
			}).ToList();
			return new BasePaginatedList<object>(responseItems, query.Count, pageNumber, pageSize);
		}


	}
}
