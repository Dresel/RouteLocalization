namespace RouteLocalization.AspNetCore.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;

	public class TestController : Controller
	{
		// /test/test
		public IActionResult Test()
		{
			// None attribute routed action route, should not be translated
			return Content("Test!");
		}

		// /testone
		[HttpGet("testone")]
		public IActionResult Test1()
		{
			return Content("Test1!");
		}

		// /testtwo/1
		[HttpGet("testtwo/{parameter:int:min(1)}")]
		public IActionResult Test2(int parameter)
		{
			return Content("Test2!");
		}

		// /testthree/1
		[HttpGet("testthree/{parameter:int:min(1)}")]
		public IActionResult Test3(int parameter)
		{
			return Content("Test3!");
		}

		// /testthree/1
		[HttpPost("testthree/{parameter:int:min(1)}")]
		public IActionResult Test3(int parameter, object viewModel)
		{
			return Content("Test3!");
		}

		// /testfour/
		[HttpGet("testfour")]
		public IActionResult Test4()
		{
			return Content("Test4!");
		}
	}
}