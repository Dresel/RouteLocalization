namespace RouteLocalization.Mvc.Tests.Core
{
	using System.Web.Mvc;

	[RouteArea("ShouldBeIgnored")]
	[RoutePrefix("ShouldBeIgnored")]
	public class ControllerBase : Controller
	{
	}
}