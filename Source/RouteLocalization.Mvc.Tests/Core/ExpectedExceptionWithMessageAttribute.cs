namespace RouteLocalization.Mvc.Tests.Core
{
	using System;
	using System.Reflection;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	public class ExpectedExceptionWithMessageAttribute : ExpectedExceptionBaseAttribute
	{
		public ExpectedExceptionWithMessageAttribute(Type exceptionType)
			: this(exceptionType, null)
		{
		}

		public ExpectedExceptionWithMessageAttribute(Type exceptionType, string expectedMessage)
			: this(exceptionType, expectedMessage, false)
		{
		}

		public ExpectedExceptionWithMessageAttribute(Type exceptionType, string expectedMessage, bool containing)
		{
			ExceptionType = exceptionType;
			ExpectedMessage = expectedMessage;
			Containing = containing;
		}

		public ExpectedExceptionWithMessageAttribute(Type exceptionType, Type resourcesType, string resourceName)
			: this(exceptionType, resourcesType, resourceName, false)
		{
		}

		public ExpectedExceptionWithMessageAttribute(Type exceptionType, Type resourcesType, string resourceName,
			bool containing)
		{
			ExceptionType = exceptionType;
			ExpectedMessage = ExpectedMessage;
			ResourcesType = resourcesType;
			ResourceName = resourceName;
			Containing = containing;
		}

		public bool Containing { get; set; }

		public Type ExceptionType { get; set; }

		public string ExpectedMessage { get; set; }

		public bool IgnoreCase { get; set; }

		public string ResourceName { get; set; }

		public Type ResourcesType { get; set; }

		protected override void Verify(Exception e)
		{
			if (e.GetType() != ExceptionType)
			{
				Assert.Fail(
					"ExpectedExceptionWithMessageAttribute failed. Expected exception type: <{0}>. Actual exception type: <{1}>. Exception message: <{2}>",
					ExceptionType.FullName, e.GetType().FullName, e.Message);
			}

			string actualMessage = e.Message.Trim();

			string expectedMessage = ExpectedMessage;

			if (expectedMessage == null)
			{
				if (ResourcesType != null && ResourceName != null)
				{
					PropertyInfo resourceProperty = ResourcesType.GetProperty(ResourceName,
						BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
					if (resourceProperty != null)
					{
						string resourceValue = null;

						try
						{
							resourceValue = resourceProperty.GetMethod.Invoke(null, null) as string;
						}
						finally
						{
							if (resourceValue != null)
							{
								expectedMessage = resourceValue;
							}
							else
							{
								Assert.Fail(
									"ExpectedExceptionWithMessageAttribute failed. Could not get resource value. ResourceName: <{0}> ResourcesType<{1}>.",
									ResourceName, ResourcesType.FullName);
							}
						}
					}
					else
					{
						Assert.Fail(
							"ExpectedExceptionWithMessageAttribute failed. Could not find static resource property on resources type. ResourceName: <{0}> ResourcesType<{1}>.",
							ResourceName, ResourcesType.FullName);
					}
				}
				else
				{
					Assert.Fail("ExpectedExceptionWithMessageAttribute failed. Both ResourcesType and ResourceName must be specified.");
				}
			}

			StringComparison stringComparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

			if (Containing)
			{
				if (actualMessage.IndexOf(expectedMessage, stringComparison) == -1)
				{
					Assert.Fail(
						"ExpectedExceptionWithMessageAttribute failed. Expected message: <{0}>. Actual message: <{1}>. Exception type: <{2}>",
						expectedMessage, e.Message, e.GetType().FullName);
				}
			}
			else
			{
				if (!string.Equals(expectedMessage, actualMessage, stringComparison))
				{
					Assert.Fail(
						"ExpectedExceptionWithMessageAttribute failed. Expected message to contain: <{0}>. Actual message: <{1}>. Exception type: <{2}>",
						expectedMessage, e.Message, e.GetType().FullName);
				}
			}
		}
	}
}