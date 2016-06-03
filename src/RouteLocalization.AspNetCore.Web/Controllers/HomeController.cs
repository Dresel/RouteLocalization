namespace RouteLocalization.AspNetCore.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;

	public class HomeController : Controller
	{
		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Error()
		{
			return View();
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}