namespace RouteLocalization.AspNetCore.Test.TestData
{
	using Microsoft.AspNetCore.Mvc;

	[Route("Controller1")]
	public class Controller4
	{
		[Route("Action1")]
		public void Action1()
		{
		}

		[Route("Action1")]
		public void Action1(int parameter)
		{
		}
	}
}