namespace RouteLocalization.AspNetCore.Sample
{
	using System.Globalization;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Localization;
	using Microsoft.AspNetCore.Localization.Routing;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using RouteLocalization.AspNetCore.Sample.Controllers;

	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", false, true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug(LogLevel.Debug);

			app.UseRequestLocalization();

			app.UseStaticFiles();

			app.UseMvc();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Configure localization
			services.AddSingleton(provider =>
			{
				RequestLocalizationOptions requestLocalizationOptions = new RequestLocalizationOptions()
				{
					DefaultRequestCulture = new RequestCulture("en"),

					// Formatting numbers, dates, etc.
					SupportedCultures = new[] { new CultureInfo("en"), new CultureInfo("de") },

					// UI strings that we have localized.
					SupportedUICultures = new[] { new CultureInfo("en"), new CultureInfo("de") },
				};

				// Replaces CultureSensitiveActionFilterAttribute
				requestLocalizationOptions.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());

				return requestLocalizationOptions;
			});

			// Add framework services.
			services.AddMvc(setup =>
				{
					setup.Filters.Add(new MiddlewareFilterAttribute(typeof(LocalizationPipeline)));
				})
				.AddTypedRouting()
				.AddRouteLocalization(setup =>
				{
					setup.UseCulture("de")
						//.WhereController<HomeController>()
						//.WhereAction(action => action.Index(RouteLocalization.AspNetCore.With.Any<int>()))
						.WhereController(nameof(HomeController))
						.WhereAction(nameof(HomeController.Index))
						.TranslateAction("Willkommen");

					setup.UseCulture("de")
						.WhereController(nameof(SecondController))
						.WhereAction(nameof(SecondController.First))
						.TranslateAction("Erste");

					setup.UseCultures(new[] { "en", "de" })
						.WhereUntranslated()
						.Filter<ApiController>()
						.Filter<HomeController>(controller => controller.Start())
						.Filter<SecondController>(controller => controller.Second())
						.AddDefaultTranslation();

					setup.UseCultures(new[] { "en", "de" })
						.WhereTranslated()
						.Filter<SecondController>(controller => controller.First())
						.RemoveOriginalRoutes();
				});

			services.Configure<MvcOptions>(options => options.Conventions.Add(new CollectRoutesApplicationConvention()));
		}
	}
}