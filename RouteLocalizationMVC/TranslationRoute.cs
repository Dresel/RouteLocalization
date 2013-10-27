namespace RouteLocalizationMVC
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Web.Routing;
	using RouteLocalizationMVC.Extensions;

	public class TranslationRoute : Route
	{
		public TranslationRoute(string url, IRouteHandler routeHandler)
			: base(url, routeHandler)
		{
			TranslationRouteRoot = this;
			TranslatedRoutes = new Dictionary<string, TranslationRoute>();
		}

		public TranslationRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
			: base(url, defaults, routeHandler)
		{
			TranslationRouteRoot = this;
			TranslatedRoutes = new Dictionary<string, TranslationRoute>();
		}

		public TranslationRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
			IRouteHandler routeHandler)
			: base(url, defaults, constraints, routeHandler)
		{
			TranslationRouteRoot = this;
			TranslatedRoutes = new Dictionary<string, TranslationRoute>();
		}

		public TranslationRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
			RouteValueDictionary dataTokens, IRouteHandler routeHandler)
			: base(url, defaults, constraints, dataTokens, routeHandler)
		{
			TranslationRouteRoot = this;
			TranslatedRoutes = new Dictionary<string, TranslationRoute>();
		}

		public string Culture { get; set; }

		public bool IsRoot
		{
			get { return TranslationRouteRoot == null || TranslationRouteRoot == this; }
		}

		public IDictionary<string, TranslationRoute> TranslatedRoutes { get; set; }

		public TranslationRoute TranslationRouteRoot { get; set; }

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			string currentCulture = Thread.CurrentThread.CurrentUICulture.Name.ToLower();

			// If specific path is requested, override culture and remove RouteValue
			if (values.ContainsKey("Culture"))
			{
				currentCulture = (string)values["Culture"];
			}

			if (string.IsNullOrEmpty(Culture) || Culture == currentCulture ||
				!TranslationRouteRoot.TranslatedRoutes.ContainsKey(currentCulture))
			{
				VirtualPathData virtualPathData = base.GetVirtualPath(requestContext, values).RemoveCulture();

				if (virtualPathData != null)
				{
					values.Remove("Culture");

					virtualPathData = base.GetVirtualPath(requestContext, values).RemoveCulture();
				}

				return virtualPathData;
			}

			VirtualPathData virtualPathDataTranslation =
				TranslationRouteRoot.TranslatedRoutes[currentCulture].GetVirtualPath(requestContext, values).RemoveCulture();

			if (virtualPathDataTranslation != null)
			{
				values.Remove("Culture");

				virtualPathDataTranslation =
					TranslationRouteRoot.TranslatedRoutes[currentCulture].GetVirtualPath(requestContext, values).RemoveCulture();
			}

			return virtualPathDataTranslation;
		}
	}
}