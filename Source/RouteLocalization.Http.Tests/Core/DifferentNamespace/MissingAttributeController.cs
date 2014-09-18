namespace RouteLocalization.Http.Tests.Core.DifferentNamespace
{
	using System.Web.Http;

	public class MissingAttributeController : ApiController
	{
		public string Book(int chapter, int page)
		{
			return string.Empty;
		}

		public string Index()
		{
			return string.Empty;
		}
	}
}