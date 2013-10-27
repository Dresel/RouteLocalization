namespace RouteLocalizationMVC.Tests.Core
{
	using System.Web.Mvc;

	internal class MissingAttributeController : Controller
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