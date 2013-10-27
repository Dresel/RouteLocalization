namespace RouteLocalizationMVC.Extensions
{
	using System.Web.Routing;

	public static class VirtualPathDataExtensions
	{
		public static VirtualPathData RemoveCulture(this VirtualPathData virtualPathData)
		{
			return virtualPathData;
		}
	}
}