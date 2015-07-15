namespace Smi.Infra.Messages
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	[Serializable]
	public class BaseDomainCommand : BaseCommand
	{
		public Guid Id { get; private set; }

		public Guid AggregateRootId { get; set; }
	
		public BaseDomainCommand()
		{

		}

	}


	[Serializable]
	public class BaseCommand : ICommand
	{		
		public string ByUsername { get; set; }

		public BaseCommand()
		{

		}

		public IDictionary<string, object> DehydrateCommand()
		{
			var result = new Dictionary<string, object>();
			PropertyInfo[] propertyInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var propertyInfo in propertyInfos)
			{
				result.Add(propertyInfo.Name, propertyInfo.GetValue(this, null));
			}

			return result;
		}
	}
}