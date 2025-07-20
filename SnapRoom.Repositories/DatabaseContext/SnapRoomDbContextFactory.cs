using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace SnapRoom.Repositories.DatabaseContext
{
	namespace SnapRoom.Repositories.DatabaseContext
	{
		public class SnapRoomDbContextFactory : IDesignTimeDbContextFactory<SnapRoomDbContext>
		{
			public SnapRoomDbContext CreateDbContext(string[] args)
			{
				var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\SnapRoom.Apis");
				IConfigurationRoot configuration = new ConfigurationBuilder()
					.SetBasePath(basePath)
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.Build();

				var builder = new DbContextOptionsBuilder<SnapRoomDbContext>();
				var connectionString = configuration.GetConnectionString("Database");
				builder.UseSqlServer(connectionString);

				return new SnapRoomDbContext(builder.Options);
			}
		}
	}
}
