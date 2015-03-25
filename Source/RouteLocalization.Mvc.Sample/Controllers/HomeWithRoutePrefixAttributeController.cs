namespace RouteLocalization.Mvc.Sample.Controllers
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

		[Route("~/WelcomePrefix")]
		public virtual ActionResult Index()
		{
			return View();
		}
	}
}