namespace Smi.Infra.Bus
{
	using Smi.Infra.Messages;

	public interface IQueryProcessor
	{
		TResult Process<TResult>(IQuery<TResult> query);
	}
}