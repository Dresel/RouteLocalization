namespace RouteLocalization.Mvc.Sample.Controllers
{
	using System.Web.Mvc;

	[RouteArea("HomeWithRouteArea")]
	public partial class HomeWithRouteAreaAttributeController : Controller
	{
		[Route("Book/{chapter}/{page}")]
		public virtual ActionResult Book(int chapter, int page)
		{
			return View();
		}

		[Route("~/WelcomeArea")]
		public virtual ActionResult Index()
		{
			return View();
		}
	}
}