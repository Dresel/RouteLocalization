namespace RouteLocalization.AspNetCore
{
	using System.Collections;
	using System.Collections.Generic;

	public class RouteTranslationStore : IEnumerable<RouteSelectorProcessorPair>
	{
		protected ICollection<RouteSelectorProcessorPair> RouteSelectorProcessorPairs { get; set; } =
			new List<RouteSelectorProcessorPair>();

		public void Add(RouteSelectorProcessorPair routeSelectorProcessorPair)
		{
			RouteSelectorProcessorPairs.Add(routeSelectorProcessorPair);
		}

		public IEnumerator<RouteSelectorProcessorPair> GetEnumerator()
		{
			return RouteSelectorProcessorPairs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}