namespace RouteLocalization.AspNetCore.Test.TestData
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using Microsoft.AspNetCore.Mvc.Internal;

	public static class TestApplicationModel
	{
		public static ApplicationModel Instance
		{
			get
			{
				DefaultApplicationModelProvider defaultApplicationModelProvider =
					new DefaultApplicationModelProvider(new TestOptionsManager<MvcOptions>());

				ApplicationModelProviderContext applicationModelProviderContext = new ApplicationModelProviderContext(
					new List<TypeInfo>()
					{
						typeof(Controller1).GetTypeInfo(),
						typeof(Controller2).GetTypeInfo(),
						typeof(Controller3).GetTypeInfo()
					});

				defaultApplicationModelProvider.OnProvidersExecuting(applicationModelProviderContext);

				return applicationModelProviderContext.Result;
			}
		}

		public static ControllerModel Controller<T>(this ApplicationModel applicationModel)
		{
			return applicationModel.Controllers.Single(
				controller => Equals(controller.ControllerType, typeof(T).GetTypeInfo()));
		}

		public static ControllerModel Controller1(this ApplicationModel applicationModel)
		{
			return applicationModel.Controller<Controller1>();
		}

		public static ControllerModel Controller2(this ApplicationModel applicationModel)
		{
			return applicationModel.Controller<Controller2>();
		}

		public static ControllerModel Controller3(this ApplicationModel applicationModel)
		{
			return applicationModel.Controller<Controller3>();
		}
	}
}