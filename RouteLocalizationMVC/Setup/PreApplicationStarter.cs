using System.Web;
using RouteLocalizationMVC.Setup;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStarter), "StartOnce")]

namespace RouteLocalizationMVC.Setup
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

			DynamicModuleUtility.RegisterModule(typeof(LocalizationHttpModule));
		}
	}
}