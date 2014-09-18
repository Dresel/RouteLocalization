namespace RouteLocalization.Mvc.Tests.Core.DifferentNamespace
{
	using System.Web.Mvc;

	public class HomeController : Controller
	{
		public virtual ActionResult Book(int chapter, int page)
		{
			return View();
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Index2()
		{
			return View();
		}
	}
}