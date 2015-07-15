namespace Smi.Sample.Installers
{
	using System.Reflection;
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.Windsor;
	using Infra.Entities;
	using Infra.Services;
	using Smi.Infra.Bus;
	using Smi.Infra.Impl.Bus;

	public class SmiSampleInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<IBus>().ImplementedBy<DirectBus>());
			container.Register(Component.For<IQueryProcessor>().ImplementedBy<QueryProcessor>());
			container.Register(Component.For<IEntityRepository>().ImplementedBy<InMemoryEntityRepository>());

			var infraAssembly = Assembly.GetAssembly(typeof (IBus));
			container.Register(AllTypes.FromAssembly(infraAssembly).Where(t => t.Namespace.EndsWith("Services")).WithService.FirstInterface());



			container.Register(Component.For<IHiLoGenerator>().ImplementedBy<HiLoGeneratorNHibernate>());

			container.Register(AllTypes.FromThisAssembly().Where(t => t.Name.EndsWith("Handler")).WithService.FirstInterface());
			container.Register(AllTypes.FromThisAssembly().Where(t => t.Name.EndsWith("Builder")).WithService.FirstInterface());

			container.Register(AllTypes.FromThisAssembly().Where(t => t.Name.EndsWith("Service")).WithService.FirstInterface());
			container.Register(AllTypes.FromThisAssembly().Where(t => t.Name.EndsWith("Provider")).WithService.FirstInterface());
		
		}
	}
}