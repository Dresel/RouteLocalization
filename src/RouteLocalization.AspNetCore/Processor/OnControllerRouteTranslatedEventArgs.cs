namespace RouteLocalization.AspNetCore.Processor
{
	using System;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class OnControllerRouteTranslatedEventArgs : EventArgs
	{
		public ControllerModel OriginalControllerModel { get; set; }

		public SelectorModel OriginalSelectorModel { get; set; }

		public ControllerModel LocalizedControllerModel { get; set; }

		public SelectorModel LocalizedSelectorModel { get; set; }
	}
}