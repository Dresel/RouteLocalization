namespace RouteLocalization.AspNetCore.Processor
{
	using System;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class OnTranslatedControllerModelCreatedEventArgs : EventArgs
	{
		public ControllerModel OriginalControllerModel { get; set; }

		public ControllerModel TranslatedControllerModel { get; set; }
	}
}