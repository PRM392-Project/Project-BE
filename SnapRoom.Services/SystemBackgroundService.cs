using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SnapRoom.Services
{
	public class SystemBackgroundService : BackgroundService
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;
		public SystemBackgroundService(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{

		}

	}
}
