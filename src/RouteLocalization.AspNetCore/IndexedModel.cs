namespace RouteLocalization.AspNetCore
{
	public class IndexedModel<T>
	{
		public int Index { get; set; }

		public T Model { get; set; }
	}
}