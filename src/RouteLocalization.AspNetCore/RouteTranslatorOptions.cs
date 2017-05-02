namespace RouteLocalization.AspNetCore
{
	using System;

	public class RouteTranslatorOptions
	{
		public Action<RouteTranslator> RouteTranslatorAction { get; set; }
	}
}