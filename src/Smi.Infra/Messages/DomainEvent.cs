namespace Smi.Infra.Messages
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using Services;

	[Serializable]
	public abstract class DomainEvent<T> : IDomainEvent, IEquatable<DomainEvent<T>>
	{
		protected DomainEvent()
		{
			Id = Guid.NewGuid();
			DateHappened = DateTime.Now;
		}

		protected DomainEvent(IDictionary<string, object> parameters)
		{
			Id = Guid.NewGuid();
			Parameters = parameters;
            DateHappened = DateTime.Now;

			if (parameters.ContainsKey("ByUsername"))
			{
				ByUser = (string) parameters["ByUsername"];
			}
		}

		public Guid Id { get; private set; }
		public Guid AggregateId { get; set; }
		public int Version { get; set; }
		public string ByUser { get; set; }
		public long Timestamp { get; set; }
		public DateTime? DateHappened { get; set; }
        
		public IDictionary<string, object> Parameters { get; set; }

		public T RehydratedParameters
		{
			get 
			{ 								
				return DictionaryAdapterMediator.GetAdapter<T>((IDictionary)Parameters);
			}
		}

		public string GetJSonSerialisation()
		{
			return JsonConvert.SerializeObject(Parameters);
		}

		public bool Equals(DomainEvent<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Id.Equals(Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj is DomainEvent<T>) return Equals((DomainEvent<T>)obj);
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}