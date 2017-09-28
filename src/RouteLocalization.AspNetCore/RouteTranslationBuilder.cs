namespace RouteLocalization.AspNetCore
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Text.RegularExpressions;
	using Microsoft.Extensions.Logging;
	using RouteLocalization.AspNetCore.Processor;
	using RouteLocalization.AspNetCore.Selection;

	public class RouteTranslationBuilder
	{
		public RouteTranslationBuilder(RouteTranslationConfiguration routeTranslationConfiguration,
			RouteTranslationStore routeTranslationsStore, ILoggerFactory loggerFactory)
		{
			RouteTranslationConfiguration = routeTranslationConfiguration;
			RouteTranslationStore = routeTranslationsStore;
			LoggerFactory = loggerFactory;
			Logger = loggerFactory.CreateLogger<RouteTranslationBuilder>();
		}

		protected ICollection<string> CurrentCultures { get; set; }

		protected Func<IRouteSelector> CurrentRouteSelectorFunc { get; set; }

		protected ILogger Logger { get; set; }

		protected ILoggerFactory LoggerFactory { get; set; }

		protected RouteTranslationConfiguration RouteTranslationConfiguration { get; set; }

		protected RouteTranslationStore RouteTranslationStore { get; set; }

		public RouteTranslationBuilder AddDefaultTranslation()
		{
			ICollection<string> currentCultures = CurrentCultures;

			foreach (string culture in currentCultures)
			{
				CurrentCultures = new[] { culture };

				IRouteProcessor routeProcessor =
					new CopyTemplateRouteProcessor(RouteTranslationConfiguration,
						LoggerFactory.CreateLogger<CopyTemplateRouteProcessor>())
					{
						Culture = culture
					};

				RouteTranslationStore.Add(new RouteSelectorProcessorPair
				{
					Selector = CurrentRouteSelectorFunc(),
					Processor = routeProcessor
				});
			}

			CurrentCultures = currentCultures;

			return this;
		}

		// TODO: Add string version like WhereController / WhereAction
		public RouteTranslationBuilder Filter<T>()
		{
			return Filter<T>(null);
		}

		public RouteTranslationBuilder Filter<T>(Expression<Action<T>> expression)
		{
			string actionName = null;
			Type[] actionArguments = null;

			if (expression != null)
			{
				MethodCallExpression methodCall = expression.Body as MethodCallExpression;

				if (methodCall == null)
				{
					throw new ArgumentException("Expression must be a MethodCallExpression", nameof(expression));
				}

				actionName = methodCall.Method.Name;
				actionArguments = methodCall.Arguments.Select(x => x.Type).ToArray();
			}

			if (CurrentRouteSelectorFunc == null)
			{
				throw new InvalidOperationException(
					$"{typeof(FilterRouteSelector)} cannot be used before any RouteSelector is defined.");
			}

			Func<IRouteSelector> previousRouteSelectorFunc = CurrentRouteSelectorFunc;

			CurrentRouteSelectorFunc = (Func<FilterRouteSelector>)(() =>
			{
				FilterRouteSelector filterRouteSelector = new FilterRouteSelector(previousRouteSelectorFunc())
				{
					Controller = Regex.Replace(typeof(T).Name, "Controller$", string.Empty),
					ControllerNamespace = typeof(T).Namespace,
					Action = actionName,
					ActionArguments = actionArguments,
					Localizer = RouteTranslationConfiguration.Localizer,
					Cultures = CurrentCultures.ToArray()
				};

				if ((previousRouteSelectorFunc as Func<FilterRouteSelector>) != null)
				{
					FilterRouteSelector previousFilterRouteSelector = ((Func<FilterRouteSelector>)previousRouteSelectorFunc)();

					filterRouteSelector.FilterControllerOrActionWhenNoTranslatedRouteLeft = previousFilterRouteSelector
						.FilterControllerOrActionWhenNoTranslatedRouteLeft;
					filterRouteSelector.FilterControllerOrActionWhenNoUntranslatedRouteLeft = previousFilterRouteSelector
						.FilterControllerOrActionWhenNoUntranslatedRouteLeft;
				}

				if ((previousRouteSelectorFunc as Func<TranslatedRoutesRouteSelector>) != null)
				{
					filterRouteSelector.FilterControllerOrActionWhenNoTranslatedRouteLeft = true;
				}

				if ((previousRouteSelectorFunc as Func<UntranslatedRoutesRouteSelector>) != null)
				{
					filterRouteSelector.FilterControllerOrActionWhenNoUntranslatedRouteLeft = true;
				}

				return filterRouteSelector;
			});

			return this;
		}

		public RouteTranslationBuilder RemoveOriginalRoutes()
		{
			IRouteProcessor routeProcessor = new DisableOriginalRouteProcessor(RouteTranslationConfiguration,
				LoggerFactory.CreateLogger<DisableOriginalRouteProcessor>())
			{
				Cultures = CurrentCultures.ToArray()
			};

			RouteTranslationStore.Add(new RouteSelectorProcessorPair
			{
				Selector = CurrentRouteSelectorFunc(),
				Processor = routeProcessor
			});

			return this;
		}

		public RouteTranslationBuilder TranslateAction(string template)
		{
			IRouteProcessor routeProcessor = new TranslateActionRouteProcessor(RouteTranslationConfiguration,
				LoggerFactory.CreateLogger<TranslateActionRouteProcessor>())
			{
				Culture = CurrentCultures.Single(),
				Template = template
			};

			RouteTranslationStore.Add(new RouteSelectorProcessorPair
			{
				Selector = CurrentRouteSelectorFunc(),
				Processor = routeProcessor
			});

			return this;
		}

		public RouteTranslationBuilder TranslateController(string template)
		{
			IRouteProcessor routeProcessor = new TranslateControllerRouteProcessor(RouteTranslationConfiguration,
				LoggerFactory.CreateLogger<TranslateControllerRouteProcessor>())
			{
				Culture = CurrentCultures.Single(),
				Template = template
			};

			RouteTranslationStore.Add(new RouteSelectorProcessorPair
			{
				Selector = CurrentRouteSelectorFunc(),
				Processor = routeProcessor
			});

			return this;
		}

		public RouteTranslationBuilder UseCulture(string culture)
		{
			CurrentCultures = new[] { culture };

			return this;
		}

		public RouteTranslationBuilder UseCultures(string[] cultures)
		{
			CurrentCultures = cultures;

			return this;
		}

		public RouteTranslationBuilder WhereAction(string action)
		{
			return WhereAction(action, null);
		}

		public RouteTranslationBuilder WhereAction(string action, Type[] actionArguments)
		{
			SetAndConfigureDefaultRouteSelectionCriteria(basicRouteCriteriaRouteSelector =>
			{
				basicRouteCriteriaRouteSelector.Action = action;
				basicRouteCriteriaRouteSelector.ActionArguments = actionArguments;
			});

			return this;
		}

		public RouteTranslationBuilder WhereController(string controller)
		{
			return WhereController(Regex.Replace(controller, "Controller$", string.Empty), string.Empty);
		}

		public RouteTranslationBuilder WhereController(string controller, string controllerNamespace)
		{
			SetAndConfigureDefaultRouteSelectionCriteria(basicRouteCriteriaRouteSelector =>
			{
				basicRouteCriteriaRouteSelector.Action = null;
				basicRouteCriteriaRouteSelector.ActionArguments = null;
				basicRouteCriteriaRouteSelector.Controller = controller;
				basicRouteCriteriaRouteSelector.ControllerNamespace = controllerNamespace;
			});

			return this;
		}

		public RouteTranslationBuilder<T> WhereController<T>()
		{
			WhereController(Regex.Replace(typeof(T).Name, "Controller$", string.Empty), typeof(T).Namespace);

			return ToGeneric<T>();
		}

		public RouteTranslationBuilder WhereTranslated()
		{
			if (CurrentRouteSelectorFunc != null)
			{
				Logger.LogWarning($"{nameof(CurrentRouteSelectorFunc)} is not null, will be overridden.");
			}

			CurrentRouteSelectorFunc = (Func<TranslatedRoutesRouteSelector>)(() =>
			new TranslatedRoutesRouteSelector()
			{
				Cultures = CurrentCultures.ToArray(),
				Localizer = RouteTranslationConfiguration.Localizer
			});

			return this;
		}

		public RouteTranslationBuilder WhereUntranslated()
		{
			if (CurrentRouteSelectorFunc != null)
			{
				Logger.LogWarning($"{nameof(CurrentRouteSelectorFunc)} is not null, will be overridden.");
			}

			CurrentRouteSelectorFunc = (Func<UntranslatedRoutesRouteSelector>)(() => new UntranslatedRoutesRouteSelector()
			{
				Culture = CurrentCultures.Single(),
				Localizer = RouteTranslationConfiguration.Localizer
			});

			return this;
		}

		protected virtual RouteTranslationBuilder<T> ToGeneric<T>()
		{
			return new RouteTranslationBuilder<T>(RouteTranslationConfiguration, RouteTranslationStore, LoggerFactory)
			{
				CurrentRouteSelectorFunc = CurrentRouteSelectorFunc,
				CurrentCultures = CurrentCultures
			};
		}

		private void SetAndConfigureDefaultRouteSelectionCriteria(
			Action<BasicRouteCriteriaRouteSelector> basicRouteCriteriaRouteSelectorAction)
		{
			if ((CurrentRouteSelectorFunc as Func<BasicRouteCriteriaRouteSelector>) == null)
			{
				if (CurrentRouteSelectorFunc != null)
				{
					Logger.LogWarning(
						$"{nameof(CurrentRouteSelectorFunc)} is no {typeof(Func<BasicRouteCriteriaRouteSelector>)}, will be overridden.");
				}

				CurrentRouteSelectorFunc = (Func<BasicRouteCriteriaRouteSelector>)(() =>
				{
					BasicRouteCriteriaRouteSelector basicRouteCriteriaRouteSelector =
						new BasicRouteCriteriaRouteSelector(RouteTranslationConfiguration.Localizer);
					basicRouteCriteriaRouteSelectorAction(basicRouteCriteriaRouteSelector);

					return basicRouteCriteriaRouteSelector;
				});
			}
			else
			{
				Func<IRouteSelector> previousRouteSelectorFunc = CurrentRouteSelectorFunc;

				CurrentRouteSelectorFunc = (Func<BasicRouteCriteriaRouteSelector>)(() =>
				{
					BasicRouteCriteriaRouteSelector basicRouteCriteriaRouteSelector =
						((Func<BasicRouteCriteriaRouteSelector>)previousRouteSelectorFunc)();
					basicRouteCriteriaRouteSelectorAction(basicRouteCriteriaRouteSelector);

					return basicRouteCriteriaRouteSelector;
				});
			}
		}
	}
}