namespace RouteLocalization.AspNetCore.Sample.Controllers
{
	using Microsoft.AspNetCore.Mvc;

	public class SecondController : Controller
	{
		[HttpGet("First")]
		public virtual ActionResult First()
		{
			return View("~/Views/Home/Index.cshtml");
		}

		[HttpGet("Second")]
		public virtual ActionResult Second()
		{
			return View("~/Views/Home/Index.cshtml");
		}
	}
}