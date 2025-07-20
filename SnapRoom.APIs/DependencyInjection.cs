using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OpenApi.Models;
using SnapRoom.APIs.Hubs;
using SnapRoom.APIs.Middleware;
using SnapRoom.Contract.Repositories.IUOW;
using SnapRoom.Contract.Repositories.Mapper;
using SnapRoom.Contract.Services;
using SnapRoom.Repositories.DatabaseContext;
using SnapRoom.Repositories.UOW;
using SnapRoom.Services;

namespace SnapRoom.APIs
{
	public static class DependencyInjection
	{
		// Main Config method
		public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
		{
			services.ConfigRoute();
			services.ConfigSwagger();
			services.AddDatabase(configuration);
			services.AddAutoMapper();
			services.AddInfrastructure(configuration);
			services.AddServices();
			services.AddCors();
			services.AddHttpContextAccessor();
		}

		public static void ApplicationSetUp(this WebApplication app)
		{
			//app.UseMiddleware<AuthMiddleware>();
			app.UseMiddleware<ExceptionMiddleware>();
			app.MapHub<ChatHub>("/chathub");
		}

		public static void ConfigRoute(this IServiceCollection services)
		{
			services.Configure<RouteOptions>(options =>
			{
				options.LowercaseUrls = true;
			});
		}

		public static void ConfigSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				c.EnableAnnotations(); // Enable annotations for Swagger
									   // Add Bearer token support to Swagger
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = "bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[] {}
					}
				});
			});

		}

		public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<SnapRoomDbContext>(options =>
			{
				options.UseLazyLoadingProxies() // Enable lazy loading
					   .UseSqlServer(configuration.GetConnectionString("Database")); // Đổi API -> Repositories
				options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
			});
		}

		public static void AddCors(this IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("AllowSpecificOrigins", policy =>
				{
					policy
						.SetIsOriginAllowed(origin =>
							origin == "https://snaproom-frontend.vercel.app" ||
							origin == "http://localhost:3000" ||
							origin.StartsWith("http://10.0.2.2")) // ✅ Allow all ports from mobile emulator
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowCredentials(); // ✅ Required if you use cookies or auth headers
				});
			});
		}

		public static void AddAutoMapper(this IServiceCollection services)
		{
			services.AddAutoMapper(typeof(MapperProfile));
		}

		public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddRepositories();
		}

		public static void AddServices(this IServiceCollection services)
		{
			//services.AddHostedService<SystemBackgroundService>();

			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<ICategoryService, CategoryService>();
			services.AddScoped<IConversationService, ConversationService>();
			services.AddScoped<IDashboardService, DashboardService>();
			services.AddScoped<IOrderService, OrderService>();
			services.AddScoped<IPlanService, PlanService>();
			services.AddScoped<IPaymentService, PaymentService>();
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<EmailService>();
			services.AddSignalR();
			services.AddMemoryCache();
		}

		public static void AddRepositories(this IServiceCollection services)
		{
			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			services.AddScoped<IUnitOfWork, UnitOfWork>();
		}
	}
}
