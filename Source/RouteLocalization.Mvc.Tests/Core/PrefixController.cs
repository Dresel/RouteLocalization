namespace RouteLocalization.Mvc.Tests.Core
{
	using System.Web.Mvc;

	[RouteArea("RouteArea")]
	[RoutePrefix("RoutePrefix")]
	public class PrefixController : ControllerBase
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}