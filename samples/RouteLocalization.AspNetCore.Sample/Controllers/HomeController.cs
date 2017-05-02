namespace RouteLocalization.AspNetCore.Sample.Controllers
{
	using Microsoft.AspNetCore.Localization;
	using Microsoft.AspNetCore.Mvc;

	public class HomeController : Controller
	{
		[HttpGet("Welcome")]
		public ActionResult Index(string culture)
		{
			// RouteLocalization adds culture to the RouteData dictionary, can be bound as parameter or requested via the RouteData dictionary
			string cultureFromRouteData = (string)RouteData.Values["culture"];

			return View();
		}

		[HttpPost("Welcome")]
		public virtual ActionResult Index(object value)
		{
			return View();
		}

		[HttpPost("NotYetTranslated")]
		public virtual ActionResult NotYetTranslated()
		{
			return View("Index");
		}

		[HttpGet("")]
		public virtual ActionResult Start()
		{
			// Redirect to localized Index
			IRequestCultureFeature requestCultureFeature = Request.HttpContext.Features.Get<IRequestCultureFeature>();

			string action = Url.Action<HomeController>(controller => controller.Index(AspNetCore.With.Any<string>()), new
			{
				culture = requestCultureFeature.RequestCulture.Culture.Name
			});

			// Fallback if localized route does not exist
			//if (string.IsNullOrEmpty(action))
			//{
			//	action = Url.Action<HomeController>(controller => controller.Index(AspNetCore.With.Any<string>()));
			//}

			return LocalRedirect(action);
		}
	}
}