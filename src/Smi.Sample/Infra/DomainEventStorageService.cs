namespace Smi.Sample.Infra
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization.Formatters.Binary;
	using Entities;
	using Model;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using Smi.Infra.EventStore;
	using Smi.Infra.Messages;
	using Smi.Infra.Services;
	using Smi.Infra.ToBeImplemented;

	public class DomainEventStorageService : IDomainEventStorage
	{
		private readonly IEntityRepository dnaEntitiesRepository;
		private readonly IDateTimeService dateTimeService;

		public DomainEventStorageService(IEntityRepository dnaEntitiesRepository, IDateTimeService dateTimeService)
		{
			this.dnaEntitiesRepository = dnaEntitiesRepository;
			this.dateTimeService = dateTimeService;
		}

		public IEnumerable<IDomainEvent> GetAllEvents(Guid eventProviderId)
		{
			List<DomainEvent> events = dnaEntitiesRepository.FindAll<DomainEvent>((x => x.EventProviderId == eventProviderId)).ToList().OrderBy(x => x.Version).ToList();
			return events.Select(x => DeSerialize(x.SerializedDomainEvent, x.Id)).ToList();
		}

		public void Save(IEventProvider eventProvider)
		{
			DomainEventProvider ep = dnaEntitiesRepository.FindAll<DomainEventProvider>((x => x.Id == eventProvider.Id)).ToList().FirstOrDefault();
			if (ep == null)
			{
				ep = new DomainEventProvider()
				{
					Id = eventProvider.Id,
					Version = 0
				};
				dnaEntitiesRepository.Save(ep);
			}

			foreach (IDomainEvent domainEvent in eventProvider.GetChanges())
			{
				domainEvent.AggregateId = ep.Id; //Assign id to events now that we have an aggregate root id				
				domainEvent.Timestamp = dateTimeService.Now.Ticks;
				SaveEvent(domainEvent, ep);
				ep.Version = domainEvent.Version;
				eventProvider.UpdateVersion(domainEvent.Version);
			}

		}

		public IEnumerable<IDomainEvent> GetAllEvents(Guid eventProviderId, int version)
		{
			List<DomainEvent> events =
				dnaEntitiesRepository.FindAll<DomainEvent>((x => x.EventProviderId == eventProviderId && x.Version > version)).ToList().OrderBy(x => x.Version).ToList();
			return events.Select(x => DeSerialize(x.SerializedDomainEvent, x.Id)).ToList();
		}


		public void Save(IDomainEvent domainEvent)
		{
			var @event = new DomainEvent
			{
				EventId = domainEvent.Id,
				EventProviderId = Guid.Empty,
				SerializedDomainEvent = Serialize(domainEvent),
				Version = domainEvent.Version,
				Type = domainEvent.GetType().AssemblyQualifiedName,
				User = domainEvent.ByUser,
				Timestamp = domainEvent.Timestamp,
				DateHappened = domainEvent.DateHappened,
				JSonDomainEvent = domainEvent.GetJSonSerialisation(),
			};

			dnaEntitiesRepository.Save<DomainEvent>(@event);
		}

		private void SaveEvent(IDomainEvent domainEvent, DomainEventProvider eventProvider)
		{
			var @event = new DomainEvent
			{
				EventId = domainEvent.Id,
				EventProviderId = eventProvider.Id,
				SerializedDomainEvent = Serialize(domainEvent),
				Version = domainEvent.Version,
				Type = domainEvent.GetType().AssemblyQualifiedName,
				User = domainEvent.ByUser,
				Timestamp = domainEvent.Timestamp,
				DateHappened = domainEvent.DateHappened,
				JSonDomainEvent = domainEvent.GetJSonSerialisation(),
			};

			dnaEntitiesRepository.Save<DomainEvent>(@event);
		}

		public byte[] Serialize(IDomainEvent domainEvent)
		{
			BinaryFormatter bf = new BinaryFormatter();

			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, domainEvent);
				return ms.GetBuffer();
			}
		}

		public IDomainEvent DeSerialize(byte[] item, long id)
		{
			BinaryFormatter bf = new BinaryFormatter();

			IDomainEvent result;

			using (MemoryStream ms = new MemoryStream())
			{
				ms.Write(item, 0, item.Length);
				ms.Position = 0;

				result = bf.Deserialize(ms) as IDomainEvent;
				return result;
			}
		}
	}

	public class SnapShotService
	{
		public SnapshotInfo Create<TAggregate>(TAggregate aggregateRoot) where TAggregate : class, IEventProvider
		{
			var result = new SnapshotInfo
			{
				Version = aggregateRoot.Version,
			};

			var t = aggregateRoot.GetType();

			//var internalValues = new Dictionary<string, object>();
			//foreach (var property in aggregateRoot.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
			//{
			//	var value = property.GetValue(aggregateRoot);
			//	if (value is IConvertible)
			//	{
			//		internalValues.Add(property.Name, value);
			//		continue;
			//	}
			//	internalValues.Add(property.Name, JsonConvert.SerializeObject(value));
			//}

			//foreach (var property in aggregateRoot.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
			//{
			//	var value = property.GetValue(aggregateRoot);
			//	if (value is IConvertible)
			//	{
			//		internalValues.Add(property.Name, value);
			//		continue;
			//	}
			//	internalValues.Add(property.Name, JsonConvert.SerializeObject(value));
			//}


			result.JsonSerialization = JsonConvert.SerializeObject(aggregateRoot, new JsonSerializerSettings() { ContractResolver = new MyContractResolver() });

			return result;
		}

		

		public TAggregate Get<TAggregate>(SnapshotInfo snapShot) where TAggregate : class, IEventProvider, new()
		{
			var aggregateRoot = JsonConvert.DeserializeObject<TAggregate>(snapShot.JsonSerialization, new JsonSerializerSettings() { ContractResolver = new MyContractResolver() });
			//var aggregateRoot = new TAggregate();

			//var internalValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(snapShot.JsonSerialization);

			//foreach (var property in aggregateRoot.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
			//{
			//	if (internalValues[property.Name] != null)
			//	{
			//		if (internalValues[property.Name] is string && ((string)internalValues[property.Name]).StartsWith("{"))
			//		{
			//			var deserialized = JsonConvert.DeserializeObject(((string) internalValues[property.Name]), property.PropertyType);
			//			property.SetValue(aggregateRoot, deserialized);
			//			continue;
			//		}

			//		if (internalValues[property.Name] is string && ((string)internalValues[property.Name]) == "null")
			//		{
			//			property.SetValue(aggregateRoot, null);
			//			continue;
			//		}

			//		var t = property.PropertyType;
			//		if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
			//		{
			//			t = t.GetGenericArguments()[0];
			//		}

			//		var value = Convert.ChangeType(internalValues[property.Name], t);			
			//		property.SetValue(aggregateRoot, value);
			//	}
			//	else
			//	{
			//		property.SetValue(aggregateRoot, null);
			//	}
			//}

			//foreach (var property in aggregateRoot.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
			//{
			//	if (internalValues[property.Name] != null)
			//	{
			//		if (internalValues[property.Name] is string && ((string)internalValues[property.Name]).StartsWith("{"))
			//		{
			//			var deserialized = JsonConvert.DeserializeObject(((string)internalValues[property.Name]), property.FieldType);
			//			property.SetValue(aggregateRoot, deserialized);
			//			continue;
			//		}

			//		if (internalValues[property.Name] is string && ((string)internalValues[property.Name]) == "null")
			//		{
			//			property.SetValue(aggregateRoot, null);
			//			continue;
			//		}

			//		var t = property.FieldType;
			//		if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
			//		{
			//			t = t.GetGenericArguments()[0];
			//		}

			//		if (internalValues[property.Name] is string)
			//		{
			//			var str = (string)internalValues[property.Name];
			//			if (string.IsNullOrEmpty(str))
			//			{
			//				property.SetValue(aggregateRoot, null);
			//				continue;
			//			}

			//			var valueFromStr = TypeDescriptor.GetConverter(t).ConvertFromInvariantString((string)internalValues[property.Name]);
			//			property.SetValue(aggregateRoot, valueFromStr);
			//			continue;
			//		}

			//		var value = Convert.ChangeType(internalValues[property.Name], t);
			//		property.SetValue(aggregateRoot, value);
			//	}
			//	else
			//	{
			//		property.SetValue(aggregateRoot, null);
			//	}
			//}

			aggregateRoot.UpdateVersion(snapShot.Version);

			return aggregateRoot;
		}

		//public SnapShot Create<TAggregate>(TAggregate aggregateRoot) where TAggregate : class, IEventProvider
		//{
		//	var result = new SnapShot
		//	{
		//		AggregateRootId = aggregateRoot.Id,
		//		Version = aggregateRoot.Version,
		//	};

		//	var t = aggregateRoot.GetType();

		//	var internalValues = new Dictionary<string, object>();
		//	foreach (var property in aggregateRoot.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
		//	{				
		//		var value = property.GetValue(aggregateRoot);
		//		internalValues.Add(property.Name, value);
		//	}

		//	result.JsonSerialization = JsonConvert.SerializeObject(internalValues);

		//	return result;
		//}

		//public TAggregate Get<TAggregate>(SnapShot snapShot) where TAggregate : class, IEventProvider, new()
		//{
		//	var aggregateRoot = new TAggregate();

		//	var internalValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(snapShot.JsonSerialization);

		//	foreach (var property in aggregateRoot.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
		//	{				
		//		var value = Convert.ChangeType(internalValues[property.Name], property.PropertyType); 
		//		property.SetValue(aggregateRoot, value);							
		//	}

		//	aggregateRoot.UpdateVersion(snapShot.Version);

		//	return aggregateRoot;
		//}
	}

	public class MyContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
	{

		
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
							.Select(p => base.CreateProperty(p, memberSerialization))
						.Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
								   .Select(f => base.CreateProperty(f, memberSerialization)))
						.ToList();
			props.ForEach(p => { p.Writable = true; p.Readable = true; });
			return props;
		}
	}

	public class SnapShot
	{
		public Guid AggregateRootId { get; set; }
		public int Version { get; set; }
		public string JsonSerialization { get; set; }
	}

	public class SnapshotInfo
	{
		public int Version { get; set; }
		public string JsonSerialization { get; set; }
	}
}