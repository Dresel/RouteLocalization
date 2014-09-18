using System.Web;
using RouteLocalization.Mvc.Setup;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStarter), "StartOnce")]

namespace RouteLocalization.Mvc.Setup
{
	using Microsoft.Web.Infrastructure.DynamicModuleHelper;

	public class PreApplicationStarter
	{
		private static bool started;

		public static void StartOnce()
		{
			if (started)
			{
				return;
			}

			started = true;

			DynamicModuleUtility.RegisterModule(typeof(CultureSensitiveHttpModule));
		}
	}
}