namespace RouteLocalization.Http.Sample.Controllers
{
	using System.Threading;
	using System.Web.Http;
	using System.Web.Http.Results;

	public class HomeController : ApiController
	{
		[HttpGet]
		[Route("Book/{chapter}/{page}", Name = "Book")]
		public string Book(int chapter, int page)
		{
			return "Hello World!";
		}

		[HttpGet]
		[Route("Welcome", Name = "Index")]
		public string Index(string culture)
		{
			// Culture is automatically set as default (route.Defaults) for localized routes,
			// it can also be requested via the route data values
			string cultureFromRouteData = (string)ControllerContext.RouteData.Values["culture"];

			return "Hello World!";
		}

		[Route("Welcome")]
		[HttpPost]
		public string Index(object value)
		{
			return "Hello World!";
		}

		[HttpGet]
		[Route]
		public RedirectResult Start()
		{
			// Redirect to localized Index
			return Redirect(Url.Link("Index", new { culture = Thread.CurrentThread.CurrentCulture.Name }));
		}
	}
}