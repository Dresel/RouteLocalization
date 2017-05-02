namespace RouteLocalization.AspNetCore
{
	public static class With
	{
		public static TParameter Any<TParameter>()
		{
			return default(TParameter);
		}
	}
}