namespace RouteLocalization.AspNetCore.Sample.Controllers
{
	using Microsoft.AspNetCore.Mvc;

	// This controller should not be translated
	[Route("api/[controller]/[action]")]
	public class ApiController : Controller
	{
		public IActionResult Index()
		{
			return Content("ApiController");
		}
	}
}