namespace RouteLocalization.Http.Sample.Controllers
{
	using System.Web.Http;

	[RoutePrefix("HomeWithRoutePrefixAttribute")]
	public class HomeWithRoutePrefixAttributeController : ApiController
	{
		[HttpGet]
		[Route("Book/{chapter}/{page}")]
		public string Book(int chapter, int page)
		{
			return "Hello World";
		}

		[HttpGet]
		[Route("Welcome")]
		public string Index()
		{
			return "Hello World";
		}
	}
}