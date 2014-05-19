namespace RouteLocalization.Http.Tests.Core.DifferentNamespace
{
	using System.Web.Http;

	public class HomeController : ApiController
	{
		public virtual string Book(int chapter, int page)
		{
			return string.Empty;
		}

		public string Index()
		{
			return string.Empty;
		}

		public string Index2()
		{
			return string.Empty;
		}
	}
}