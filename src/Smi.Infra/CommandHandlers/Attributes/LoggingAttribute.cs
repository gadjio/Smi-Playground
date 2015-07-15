namespace Smi.Infra.CommandHandlers.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class LoggingAttribute : Attribute
	{
	}
}