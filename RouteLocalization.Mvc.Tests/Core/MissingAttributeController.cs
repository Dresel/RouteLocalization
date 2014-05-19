namespace RouteLocalization.Mvc.Tests.Core
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

		public ActionResult Index(int parameter1)
		{
			return View();
		}
	}
}