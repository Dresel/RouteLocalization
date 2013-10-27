namespace RouteLocalizationMVC
{
	using System;

	public class CultureSelectedEventArgs : EventArgs
	{
		public string SelectedCulture { get; set; }
	}
}