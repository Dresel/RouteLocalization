namespace RouteLocalization.Http.Tests.Core
{
	using System.Web.Http;

	[RoutePrefix("RoutePrefix")]
	public class PrefixController : ControllerBase
	{
		public string Index()
		{
			return string.Empty;
		}
	}
}