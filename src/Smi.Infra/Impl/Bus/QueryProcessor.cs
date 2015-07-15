namespace Smi.Infra.Impl.Bus
{
	using Castle.MicroKernel;
	using Smi.Infra.Bus;
	using Smi.Infra.Handlers;
	using Smi.Infra.Messages;

	public class QueryProcessor : IQueryProcessor
	{
		private readonly IKernel kernel;

		public QueryProcessor(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public TResult Process<TResult>(IQuery<TResult> query)
		{
			var handlerType = typeof(IHandleQuery<,>).MakeGenericType(query.GetType(), typeof(TResult));

			dynamic handler = kernel.Resolve(handlerType);

			return handler.Handle((dynamic)query);
		}
	}
}