namespace RouteLocalizationMVC
{
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using System.Web;
	using RouteLocalizationMVC.Setup;

	public class LocalizationHttpModule : IHttpModule
	{
		public static EventHandler<CultureSelectedEventArgs> CultureSelected = (sender, e) => { };

		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.BeginRequest += BeginRequest;
		}

		protected void SetCultureFromHttpHeader(HttpContext httpContext)
		{
			// Set default culture as fallback
			string cultureName = Configuration.DefaultCulture;

			if (httpContext.Request.UserLanguages != null)
			{
				// Get language from HTTP Header
				foreach (string userLanguage in httpContext.Request.UserLanguages.Select(x => x.Split(';').First()))
				{
					try
					{
						CultureInfo userCultureInfo = new CultureInfo(userLanguage);

						// We don't can / want to support all languages
						if (!Configuration.AcceptedCultures.Contains(userCultureInfo.Name.ToLower()))
						{
							continue;
						}

						// Culture found that is supported
						cultureName = userCultureInfo.Name.ToLower();
						break;
					}
					catch
					{
						// Ignore invalid cultures
						continue;
					}
				}
			}

			// Set accepted culture
			CultureInfo cultureInfo = new CultureInfo(cultureName);

			Thread.CurrentThread.CurrentCulture = cultureInfo;
			Thread.CurrentThread.CurrentUICulture = cultureInfo;

			CultureSelected(this, new CultureSelectedEventArgs() { SelectedCulture = cultureName });
		}

		protected void BeginRequest(object sender, EventArgs e)
		{
			HttpContext httpContext = ((HttpApplication)sender).Context;

			SetCultureFromHttpHeader(httpContext);
		}
	}
}