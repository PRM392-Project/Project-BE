using SnapRoom.Common.Base;

namespace SnapRoom.Contract.Repositories.IUOW
{
	public interface IUnitOfWork : IDisposable
	{
		IGenericRepository<T> GetRepository<T>() where T : class;
		void Save();
		Task<int> SaveAsync();
		void BeginTransaction();
		void CommitTransaction();
		void RollBack();
		bool IsValid<T>(string id) where T : BaseEntity;
    }
}
