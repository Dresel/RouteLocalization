namespace RouteLocalization.AspNetCore.Web
{
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using RouteLocalization.AspNetCore.Web.Controllers;

	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			IConfigurationBuilder builder =
				new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
					.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseMvc(routes => { /*routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");*/ });
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc(
				setup =>
				{
					string defaultCulture = "en";

					setup.Conventions.Add(new RouteLocalizationConvention(
						convention =>
						{
							// Configuration not used at the moment
							convention.Configure(configuration =>
							{
								// TODO: DefaultCulture still needed / make sense?
								// configuration.DefaultCulture = "en";

								configuration.AcceptedCultures = new HashSet<string>() { defaultCulture, "de" };
							});

							convention
								.UseCulture("de")
								.WhereController<TestController>()
								.WhereAction(x => x.Test1())
								.TranslateAction("testeins")
								.WhereAction(x => x.Test2(0))
								.TranslateAction("testzwei/{parameter:int:min(2)}")
								.WhereAction(x => x.Test3(0))
								.TranslateAction("testdrei/{parameter:int:min(2)}");

							convention.UseCulture("de")
								.WhereController<Test2Controller>()
								.TranslateController("test")
								.WhereAction(x => x.Test1())
								.TranslateAction("testeins");

							// Not working, proposed api

							// If you do nothing, this would result in a similar configuration as AttributeRouteProcessing.AddAsNeutralRoute

							// The call below should result in a similar configuration as AttributeRouteProcessing.AddAsDefaultCultureRoute
							//convention
							//	.UseCulture(defaultCulture)
							//	.WhereUntranslated()
							//	.ApplyTranslation();

							// The call below should result in a similar configuration as AttributeRouteProcessing.AddAsNeutralAndDefaultCultureRoute
							//convention
							//	.UseCulture(defaultCulture)
							//	.WhereUntranslated()
							//	.AddTranslation();

							// The call below should result in a similar configuration as AttributeRouteProcessing.AddAsNeutralAndDefaultCultureRoute
							//convention
							//	.UseCulture(defaultCulture)
							//	.WhereTranslated()
							//	.RemoveOriginalRoutes();
						}));
				});
		}
	}
}