namespace RouteLocalization.Mvc.Sample.Controllers
{
	using System.Web.Mvc;

	[Route("ControllerLevelAttribute/{action=Index}")]
	public partial class ControllerLevelAttributeController : Controller
	{
		[Route("ControllerLevelAttribute/Book/{chapter}/{page}")]
		public virtual ActionResult Book(int chapter, int page)
		{
			return View();
		}

		public virtual ActionResult Index()
		{
			return View();
		}

		public virtual ActionResult Index2()
		{
			return View();
		}
	}
}