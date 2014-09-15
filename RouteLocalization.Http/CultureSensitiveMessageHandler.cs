namespace RouteLocalization.Http
{
	using System;
	using System.Globalization;
	using System.Net.Http;
	using System.Threading;

	public class CultureSensitiveMessageHandler : DelegatingHandler
	{
		public CultureSensitiveMessageHandler()
		{
			SetCurrentCulture = true;
			SetCurrentUICulture = true;

			GetCultureFromHttpRequestMessageDelegate = context => null;
		}

		public event EventHandler<CultureSelectedEventArgs> CultureSelected = (sender, e) => { };

		public Func<HttpRequestMessage, CultureInfo> GetCultureFromHttpRequestMessageDelegate { get; set; }

		public bool SetCurrentCulture { get; set; }

		public bool SetCurrentUICulture { get; set; }

		protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			CultureInfo cultureInfo = GetCultureFromHttpRequestMessageDelegate(request);

			if (cultureInfo == null)
			{
				return base.SendAsync(request, cancellationToken);
			}

			if (SetCurrentCulture)
			{
				Thread.CurrentThread.CurrentCulture = cultureInfo;
			}

			if (SetCurrentUICulture)
			{
				Thread.CurrentThread.CurrentUICulture = cultureInfo;
			}

			CultureSelected(this, new CultureSelectedEventArgs() { SelectedCulture = cultureInfo.Name, HttpObject = request });

			return base.SendAsync(request, cancellationToken);
		}
	}
}