namespace RouteLocalization.AspNetCore
{
	using System;
	using System.Collections.Generic;

	public class RouteSelection
	{
		public string Action { get; set; }

		public ICollection<Type> ActionArguments { get; set; }

		public string Controller { get; set; }

		public string ControllerNamespace { get; set; }
	}
}