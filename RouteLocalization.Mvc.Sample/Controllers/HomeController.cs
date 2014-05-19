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

		[Route("Welcome/{id:int}")]
		public virtual ActionResult Index(int id)
		{
			return View();
		}

		[Route]
		public virtual ActionResult Start()
		{
			// Redirect to localized Index
			return RedirectToAction(MVC.Home.Index(0).AddRouteValues(new { culture = Thread.CurrentThread.CurrentCulture.Name }));
		}
	}
}