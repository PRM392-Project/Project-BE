using Microsoft.EntityFrameworkCore;
using SnapRoom.Common.Base;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Repositories.DatabaseContext;

namespace SnapRoom.Repositories.UOW
{
	public class UnitOfWork(SnapRoomDbContext dbContext) : IUnitOfWork
	{
		private bool disposed = false;
		private readonly SnapRoomDbContext _dbContext = dbContext;
		public void BeginTransaction()
		{
			_dbContext.Database.BeginTransaction();
		}

		public void CommitTransaction()
		{
			_dbContext.Database.CommitTransaction();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					_dbContext.Dispose();
				}
			}
			disposed = true;
		}

		public void RollBack()
		{
			_dbContext.Database.RollbackTransaction();
		}

		public void Save()
		{
			_dbContext.SaveChanges();
		}

        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }


        public IGenericRepository<T> GetRepository<T>() where T : class
		{
			return new GenericRepository<T>(_dbContext);
		}

		public bool IsValid<T>(string id) where T : BaseEntity
		{
			var entity = GetRepository<T>().GetById(id);

			return (entity is not null && entity.DeletedBy is null);
		}
    }
}
