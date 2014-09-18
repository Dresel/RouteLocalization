namespace RouteLocalization.Mvc.Sample.Controllers
{
	using System.Threading;
	using System.Web.Mvc;

	public partial class HomeController : Controller
	{
		[Route("Book/{chapter}/{page}", Name = "Book")]
		public virtual ActionResult Book(int chapter, int page)
		{
			return View();
		}

		[Route("Welcome")]
		[HttpGet]
		public virtual ActionResult Index(string culture)
		{
			// Culture is automatically set as default (route.Defaults) for localized routes,
			// it can also be requested via the route data values
			string cultureFromRouteData = (string)RouteData.Values["culture"];

			return View();
		}

		[Route("Welcome")]
		[HttpPost]
		public virtual ActionResult Index(object value)
		{
			return View();
		}

		[Route]
		public virtual ActionResult Start()
		{
			// Redirect to localized Index
			return RedirectToAction(MVC.Home.Index().AddRouteValues(new { culture = Thread.CurrentThread.CurrentCulture.Name }));
		}
	}
}