namespace RouteLocalizationMVC.WebSample.Controllers
{
	using System.Web.Mvc;

	public partial class HomeController : Controller
	{
		[Route("Book/{chapter}/{page}")]
		public virtual ActionResult Book(int chapter, int page)
		{
			return View();
		}

		[Route("Welcome")]
		public virtual ActionResult Index()
		{
			return View();
		}

		[Route]
		public virtual ActionResult Start()
		{
			// Redirect to localized Index
			return RedirectToRoute(MVC.Home.Index().GetRouteValueDictionary());
		}
	}
}