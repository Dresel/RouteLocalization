namespace RouteLocalization.AspNetCore.Processor
{
	using System;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class OnActionRouteTranslatedEventArgs : EventArgs
	{
		public ActionModel OriginalActionModel { get; set; }

		public SelectorModel OriginalSelectorModel { get; set; }

		public ActionModel LocalizedActionModel { get; set; }

		public SelectorModel LocalizedSelectorModel { get; set; }
	}
}