namespace Smi.Infra.CommandHandlers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Security;
	using Castle.MicroKernel;
	using Smi.Infra.CommandHandlers.Attributes;
	using Smi.Infra.CommandHandlers.Decoration;

	public class CommandHandlerChainBuilder
	{
		private readonly IKernel kernel;

		public CommandHandlerChainBuilder(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public object BuildChain(object instance)
		{
			var commandHandlerType = instance.GetType();

			instance = Encapsulate(typeof(TransactionalCommandHandler<>), instance);

			LoggingAttribute loggingAttribute;
			if (TryGetAttribute(commandHandlerType, out loggingAttribute))
			{
				instance = Encapsulate(typeof(LoggedCommandHandler<>), loggingAttribute, instance);
			}
			

			return instance;
		}

		private object Encapsulate(Type encapsulateType, object instance)
		{
			ConstructorInfo constructorInfo = GetFirstConstructorForGenericType(encapsulateType, GetGenericArgumentsFromFirstInterface(instance));
			var parameters = new List<object>();
			foreach (var parameterInfo in constructorInfo.GetParameters())
			{
				if (parameterInfo.ParameterType.IsAssignableFrom(instance.GetType()))
				{
					parameters.Add(instance);
					continue;
				}
				parameters.Add(kernel.Resolve(parameterInfo.ParameterType));
			}

			return constructorInfo.Invoke(parameters.ToArray());
		}

		private object Encapsulate(Type encapsulateType, object attribute, object instance)
		{
			var constructorInfo = GetFirstConstructorForGenericType(encapsulateType, GetGenericArgumentsFromFirstInterface(instance));
			var parameters = new List<object>();
			foreach (var parameterInfo in constructorInfo.GetParameters())
			{
				if (parameterInfo.ParameterType.IsAssignableFrom(instance.GetType()))
				{
					parameters.Add(instance);
					continue;
				}
				if (parameterInfo.ParameterType.IsAssignableFrom(attribute.GetType()))
				{
					parameters.Add(attribute);
					continue;
				}
				parameters.Add(kernel.Resolve(parameterInfo.ParameterType));
			}

			return constructorInfo.Invoke(parameters.ToArray());
		}

		private static ConstructorInfo GetFirstConstructorForGenericType(Type genericType, Type[] genericArguments)
		{
			var x = genericType.MakeGenericType(genericArguments);
			var ctors = x.GetConstructors();
			return ctors.First();
		}

		private static Type[] GetGenericArgumentsFromFirstInterface(object instance)
		{
			return instance.GetType().GetInterfaces().First().GetGenericArguments();
		}

		private static bool TryGetAttribute<T>(Type type, out T attribute) where T : class
		{
			var attributes = type.GetCustomAttributes(typeof(T), true);
			if (attributes.Length > 0)
			{
				attribute = attributes[0] as T;
				return true;
			}
			attribute = null;
			return false;
		}

		private static bool TryGetAttributeFailOnTooMany<T>(Type type, out T attribute) where T : class
		{
			var attributes = type.GetCustomAttributes(typeof(T), true);
			if (attributes.Length > 1)
			{
				throw new SecurityException("Trop d'attributs de sécurité pour " + type.FullName);
			}
			if (attributes.Length == 1)
			{
				attribute = attributes[0] as T;
				return true;
			}
			attribute = null;
			return false;
		}
	}
}