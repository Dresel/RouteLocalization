namespace RouteLocalization.AspNetCore.Processor
{
	using Microsoft.AspNetCore.Mvc.ActionConstraints;

	public class NeverAcceptActionContraint : IActionConstraint
	{
		public int Order { get; set; }

		public bool Accept(ActionConstraintContext context)
		{
			return false;
		}
	}
}