namespace Smi.Infra.Handlers
{
	using Smi.Infra.Messages;

	public interface IHandleQuery<TQuery, TResult>
		where TQuery : IQuery<TResult>
	{
		TResult Handle(TQuery query);
	}
}