namespace RouteLocalizationMVC
{
	using System;
	using System.Globalization;
	using System.Threading;
	using System.Web;

	public class LocalizationHttpModule : IHttpModule
	{
		public static EventHandler<CultureSelectedEventArgs> CultureSelected = (sender, e) => { };

		public static Func<HttpContext, CultureInfo> GetCultureFromHttpContextDelegate = context => null;

		static LocalizationHttpModule()
		{
			SetCurrentCulture = true;
			SetCurrentUICulture = true;
		}

		public static bool SetCurrentCulture { get; set; }

		public static bool SetCurrentUICulture { get; set; }

		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.BeginRequest += BeginRequest;
		}

		protected void SetCultureFromHttpContext(HttpContext httpContext)
		{
			CultureInfo cultureInfo = GetCultureFromHttpContextDelegate(httpContext);

			if (cultureInfo == null)
			{
				return;
			}

			if (SetCurrentCulture)
			{
				Thread.CurrentThread.CurrentCulture = cultureInfo;
			}

			if (SetCurrentUICulture)
			{
				Thread.CurrentThread.CurrentUICulture = cultureInfo;
			}

			CultureSelected(this, new CultureSelectedEventArgs() { SelectedCulture = cultureInfo.Name.ToLower() });
		}

		protected void BeginRequest(object sender, EventArgs e)
		{
			HttpContext httpContext = ((HttpApplication)sender).Context;

			SetCultureFromHttpContext(httpContext);
		}
	}
}