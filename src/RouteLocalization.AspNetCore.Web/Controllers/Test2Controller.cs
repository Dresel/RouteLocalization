namespace RouteLocalization.AspNetCore.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;

	[Route("test")]
	public class Test2Controller : Controller
	{
		// /test
		public IActionResult Test()
		{
			// Default action for attribute routed controller route
			return Content("Test!");
		}

		// /test/testone
		[HttpGet("testone")]
		public IActionResult Test1()
		{
			return Content("Test1!");
		}
	}
}