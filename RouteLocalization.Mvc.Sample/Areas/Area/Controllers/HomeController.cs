namespace RouteLocalization.Mvc.Sample.Areas.Area.Controllers
{
	using System.Web.Mvc;

	[RouteArea("Area")]
	public partial class HomeController : Controller
	{
		[Route("Book/{chapter}/{page}/{foo}")]
		public virtual ActionResult Book(int chapter, int page, int foo)
		{
			return View();
		}

		[Route("Welcome/{foo}")]
		public virtual ActionResult Index(int foo)
		{
			return View();
		}
	}
}