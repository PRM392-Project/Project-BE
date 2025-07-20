using AutoMapper;

namespace SnapRoom.Contract.Repositories.Mapper
{
	public class MapperProfile : Profile
	{
		public MapperProfile() 
		{
			//CreateMap(typeof(SimpleViewDto), typeof(BaseEntity))
			//.ReverseMap()
			//.IncludeAllDerived();

			//CreateMap<Account, AccountViewDto>();
			//CreateMap<AccountUpdateDto, Account>();
			//CreateMap<CustomerUpdateDto, Account>()
			//	.ForAllMembers(opt => opt.Condition(
			//	(src, dest, srcMember) => srcMember != null &&
			//							  (srcMember is not string || !string.IsNullOrWhiteSpace(srcMember.ToString()))
			//	));

		}
	}
}
