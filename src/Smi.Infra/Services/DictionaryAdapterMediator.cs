namespace Smi.Infra.Services
{
	using System.Collections;
	using Castle.Components.DictionaryAdapter;

	public static class DictionaryAdapterMediator
	{
		private static readonly DictionaryAdapterFactory dictionaryAdapterFactory = new DictionaryAdapterFactory();

		public static T GetAdapter<T>(IDictionary dictionary)
		{
			return dictionaryAdapterFactory.GetAdapter<T>(dictionary);
		}


	}
}