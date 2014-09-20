# The addon for simple route localization

RouteLocalization is a lightweight extension for Asp.Net Mvc / Web Api attribute routing to enable simple route localization.

It can be installed via Nuget:

  * ASP.NET MVC: [RouteLocalization.Mvc](http://nuget.org/packages/RouteLocalization.Mvc)
  * ASP.NET Web API: [RouteLocalization.WebApi](http://nuget.org/packages/RouteLocalization.WebApi)

## What can RouteLocalization do for you?

RouteLocalization has the following features:

* Allows localization of your attribute routes with fluent interfaces
* Localized routes are culture sensitive - that means if you request an english route, the thread culture is set to "en", if you request a german route it is set to "de", etc.
* Link generation is culture sensitive too - if you generate a link for a specific action the corresponding translated version for this url is generated, if existing

## Where to start?

Read the [Getting Started](Documentation/GettingStarted.md) document section.

Examine the sample projects:

* [ASP.NET MVC](Source/RouteLocalization.Mvc.Sample/App_Start/RouteConfig.cs)
* [ASP.NET Web API](Source/RouteLocalization.Http.Sample/App_Start/WebApiConfig.cs)

## Documentation

* [Getting Started](Documentation/GettingStarted.md)
* [Selecting and Translating Routes](Documentation/SelectingAndTranslatingRoutes.md)
* [The Configuration Class](Documentation/TheConfigurationClass.md)
* Link Generation - TODO
* Various Use Cases - TODO