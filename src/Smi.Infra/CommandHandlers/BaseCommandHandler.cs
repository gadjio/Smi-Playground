namespace Smi.Infra.CommandHandlers
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using EventStore;
	using EventStore.Aggregate;
	using Extensions;
	using Handlers;
	using Smi.Infra.Bus;
	using Smi.Infra.Messages;
	using ToBeImplemented;

	public abstract class BaseCommandHandler<T> : IHandleCommand<T> where T : BaseCommand
	{
		private readonly IBus bus;
		private readonly IEventLogService eventLogService;		

		protected BaseCommandHandler(IBus bus, IEventLogService eventLogService)
		{
			this.bus = bus;
			this.eventLogService = eventLogService;			
		}

		public abstract IEnumerable<ValidationResult> ValidateCommand(T command);

		public abstract IDomainEvent EventToPublish(T command);

		public abstract Guid GetAggregateRootId(T command);

		protected void PublishEvent(IDomainEvent @event)
		{
			bus.Publish(@event);
			eventLogService.SaveEvent(@event);
		}

		public virtual void Execute(T command)
		{
			List<ValidationResult> commandValidationResult;
			command.Validate(out commandValidationResult);
			if (commandValidationResult != null && commandValidationResult.Any())
			{				
				throw new DomainValidationException(GetErrorMessage(commandValidationResult));
			}

			var validationResults = ValidateCommand(command);			
			if (validationResults != null && validationResults.Any())
			{
				throw new DomainValidationException(GetErrorMessage(validationResults.ToList())); 
			}

			var @event = EventToPublish(command);			
			if (@event != null)
			{
				@event.AggregateId = GetAggregateRootId(command);
				PublishEvent(@event);				
			}
		}

		private static string GetErrorMessage(IEnumerable<ValidationResult> commandValidationResult)
		{
			var sb = new StringBuilder();
			foreach (var result in commandValidationResult)
			{
				var prefix = "";
				if (result.MemberNames != null && result.MemberNames.Any())
				{
					prefix = result.MemberNames.Aggregate(prefix, (current, memberName) => current + memberName + " ");
					prefix = prefix + " - ";
				}

				sb.AppendLine(prefix + result.ErrorMessage);
			}
			return sb.ToString();
		}

		public void OnFail(T command)
		{			
		}
		

		protected static IDictionary<string, object> Dehydrate(object obj)
		{
			var result = new Dictionary<string, object>();
			PropertyInfo[] propertyInfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var propertyInfo in propertyInfos)
			{
				result.Add(propertyInfo.Name, propertyInfo.GetValue(obj, null));
			}

			return result;
		}
	}

	public abstract class BaseDomainCommandHandler<T> : IHandleCommand<T> where T : BaseDomainCommand
	{
		protected readonly IDomainRepository domainRepository;

		public BaseDomainCommandHandler(IDomainRepository domainRepository)
		{
			this.domainRepository = domainRepository;
		}

		public abstract void Execute(T command);

		public void OnFail(T command)
		{
			domainRepository.Evict(command.AggregateRootId);
		}

		protected static IDictionary<string, object> Dehydrate(object obj)
		{
			var result = new Dictionary<string, object>();
			PropertyInfo[] propertyInfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var propertyInfo in propertyInfos)
			{
				result.Add(propertyInfo.Name, propertyInfo.GetValue(obj, null));
			}

			return result;
		}
	}

	public abstract class BaseDomainCommandHandler<T, TAggregate> : IHandleCommand<T>
		where TAggregate : BaseAggregateRoot, new()
		where T : BaseDomainCommand
	{
		protected readonly IDomainRepository domainRepository;		

		public BaseDomainCommandHandler(IDomainRepository domainRepository)
		{
			this.domainRepository = domainRepository;			
		}

		public void Execute(T command)
		{			
			var action = new Action<TAggregate, T>(Process);
			domainRepository.ProcessAction(command, action);
		}

		public void RaiseValidationError(IEnumerable<ValidationResult> validationResults)
		{
			if (validationResults != null && validationResults.Any())
			{
				throw new DomainValidationException(GetErrorMessage(validationResults.ToList()));
			}
		}

		private static string GetErrorMessage(IEnumerable<ValidationResult> commandValidationResult)
		{
			var sb = new StringBuilder();
			foreach (var result in commandValidationResult)
			{
				var prefix = "";
				if (result.MemberNames != null && result.MemberNames.Any())
				{
					prefix = result.MemberNames.Aggregate(prefix, (current, memberName) => current + memberName + " ");
					prefix = prefix + " - ";
				}

				sb.AppendLine(prefix + result.ErrorMessage);
			}
			return sb.ToString();
		}

		public void OnFail(T command)
		{
			domainRepository.Evict(command.AggregateRootId);
		}

		public abstract void Process(TAggregate aggregate, T typeCommand);

	}
}