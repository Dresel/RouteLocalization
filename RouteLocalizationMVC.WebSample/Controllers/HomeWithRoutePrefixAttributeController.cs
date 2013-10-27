namespace RouteLocalizationMVC.WebSample.Controllers
{
	using System.Web.Mvc;

	[RoutePrefix("HomeWithRoutePrefixAttribute")]
	public partial class HomeWithRoutePrefixAttributeController : Controller
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
	}
}