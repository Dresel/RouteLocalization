namespace RouteLocalization.AspNetCore.Test.TestData
{
	using Microsoft.AspNetCore.Mvc;

	[Route("Controller1")]
	[Route("Controller11")]
	public class Controller1
	{
		[Route("Action1")]
		[Route("Action11")]
		public void Action1()
		{
		}

		[Route("Action2")]
		public void Action2()
		{
		}

		[Route("Action2")]
		public void Action2(int value)
		{
		}

		public void Action3(int argument1)
		{
		}

		public void Action4(double argument1)
		{
		}
	}
}