namespace RouteLocalization.Mvc.Tests.Core.DifferentNamespace
{
	using System.Web.Mvc;

	public class MissingAttributeController : Controller
	{
		public ActionResult Book(int chapter, int page)
		{
			return View();
		}

		public ActionResult Index()
		{
			return View();
		}
	}
}