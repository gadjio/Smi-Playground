namespace Smi.Infra.EventPublishing
{
	public interface IPublisher
	{
		void Publish(string topic, object @event);
	}	
}