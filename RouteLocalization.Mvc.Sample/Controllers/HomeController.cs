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
		public virtual ActionResult Index()
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