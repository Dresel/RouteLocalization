namespace RouteLocalization.Http.Sample.Controllers
{
	using System.Web.Http;

	[Route("ControllerLevelAttribute/{action=Index}")]
	public partial class ControllerLevelAttributeController : ApiController
	{
		[HttpGet]
		[Route("ControllerLevelAttribute/Book/{chapter}/{page}")]
		public virtual string Book(int chapter, int page)
		{
			return "Hello World!";
		}

		[HttpGet]
		public virtual string Index()
		{
			return "Hello World!";
		}

		[HttpGet]
		public virtual string Index2()
		{
			return "Hello World!";
		}
	}
}