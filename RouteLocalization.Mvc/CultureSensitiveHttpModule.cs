namespace RouteLocalization.Mvc
{
	using System;
	using System.Globalization;
	using System.Threading;
	using System.Web;

	public class CultureSensitiveHttpModule : IHttpModule
	{
		static CultureSensitiveHttpModule()
		{
			SetCurrentCulture = true;
			SetCurrentUICulture = true;

			GetCultureFromHttpContextDelegate = context => null;
		}

		public static event EventHandler<CultureSelectedEventArgs> CultureSelected = (sender, e) => { };

		public static Func<HttpContext, CultureInfo> GetCultureFromHttpContextDelegate { get; set; }

		public static bool SetCurrentCulture { get; set; }

		public static bool SetCurrentUICulture { get; set; }

		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.BeginRequest += BeginRequest;
		}

		protected void BeginRequest(object sender, EventArgs e)
		{
			HttpContext httpContext = ((HttpApplication)sender).Context;

			SetCultureFromHttpContext(httpContext);
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

			CultureSelected(this, new CultureSelectedEventArgs() { SelectedCulture = cultureInfo.Name, Context = new HttpContextWrapper(httpContext) });
		}
	}
}