namespace Smi.Sample
{
	using System;
	using System.Globalization;
	using System.Linq;
	using AppCore.GameBoard.Commands;
	using AppCore.Player.Commands;
	using AppCore.Player.Queries;
	using Castle.Core.Logging;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.Windsor.Installer;
	using Installers;
	using Smi.Infra.Bus;
	using Smi.Infra.CommandHandlers;
	using Smi.Infra.EventStore;
	using Smi.Infra.EventStore.Aggregate;
	using Smi.Infra.Messages;

	class Program
	{
		public static IWindsorContainer Container { get; set; }

		static void Main(string[] args)
		{
			BootstrapContainer();
			//Container.AddFacility<LoggingFacility>(f => f.UseLog4Net());
			Container.Register(Component.For<ILogger>().ImplementedBy<NullLogger>());
			InitializeActiveRecord();

            Console.WriteLine("Application Started");
            Console.WriteLine("-------------------");
			
			var queryProcessor = Container.Resolve<IQueryProcessor>();

			var createPlayerCommand = new CreatePlayerCommand {PlayerId = Guid.NewGuid(), PlayerName = "Optimus Prime"};
			SendCommand(createPlayerCommand);

			var setScore = new SetPlayerInitialScoreCommand { PlayerId = createPlayerCommand.PlayerId, InitialScore = 1000 };
			SendCommand(setScore);
			

			Console.WriteLine("Player Optimus Prime has been added.");

		    var addPlayerExperienceCommand = new AddPlayerExperienceCommand
		    {
		        PlayerId = createPlayerCommand.PlayerId,
		        ExperienceToAdd = 500
		    };
            SendCommand(addPlayerExperienceCommand);

            var addPlayerVictoryCommand = new AddPlayerVictoriesCommand
            {
                PlayerId = createPlayerCommand.PlayerId,
                VictoriesToAdd = 1,
                ExperienceToAdd = 50
            };
            SendCommand(addPlayerVictoryCommand);

			var players = queryProcessor.Process(new FindAllPlayerQuery()).ToList();

			Console.WriteLine("List all Players return {0} result(s)", players.Count());
			foreach (var playerSummaryReporting in players)
			{
                Console.WriteLine("Player {0} - Score : {1} - Experience : {2} - Victories : {3}", playerSummaryReporting.PlayerName, playerSummaryReporting.PlayerScore, playerSummaryReporting.PlayerExperience, playerSummaryReporting.PlayerVictory);
			}

            Console.WriteLine(string.Empty);
            Console.WriteLine("******************************");

			var createBoard = new CreateGameBoardCommand {AggregateRootId = Guid.NewGuid(), MaximumNumberOfPlayers = 2};
			SendCommand(createBoard);

			var addPlayer1 = new AddPlayerToBoardGameCommand { AggregateRootId = createBoard.AggregateRootId, PlayerName = "Player 1" };
							
			SendCommand(addPlayer1);

            var startGame = new StartGameCommand { AggregateRootId = createBoard.AggregateRootId, StartDate = DateTime.Now };
            SendCommand(startGame);


            /*
			SendCommand(addPlayer1);

			Console.WriteLine("-------------------");
			Console.WriteLine("Retry adding player 1");
			
			var domainRepository = Container.Resolve<IDomainRepository>();
			domainRepository.Evict(createBoard.AggregateRootId);

			SendCommand(addPlayer1);
            */

			//var addPlayer2 = new AddPlayerToBoardGameCommand { AggregateRootId = createBoard.AggregateRootId, PlayerName = "Player 2" };
			//SendCommand(addPlayer2);
			
			
            Console.WriteLine("-------------------");
            Console.WriteLine("Application Stopped");
		    Console.ReadLine();
		}

		private static void SendCommand(ICommand command)
		{
			try
			{
				Container.Resolve<IBus>().Send(command);
			}
			catch (DomainValidationException ex)
			{
				Console.WriteLine(string.Format("Erreur while processing command {0}: {1}", command.GetType().Name, ex.Message));				
			}
		}


		private static void BootstrapContainer()
		{
			Container = new WindsorContainer();
			Container.AddFacility("command-handler-decoration", new CommandHandlerDecorationFacility());

			Container.Install(new IWindsorInstaller[] { new SmiSampleInstaller() });

			Container.Install(FromAssembly.InDirectory(new AssemblyFilter(".", "Smi.*.dll")));
		}

		protected static void InitializeActiveRecord()
		{
		}

		//protected static void InitializeActiveRecord()
		//{
		//	var DnaCenterEntitiesCfg = new NHibernate.Cfg.Configuration()
		//		.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, typeof(SqlClientDriver).AssemblyQualifiedName)
		//		.SetProperty(NHibernate.Cfg.Environment.Dialect, typeof(MsSql2005Dialect).AssemblyQualifiedName)
		//		.SetProperty(NHibernate.Cfg.Environment.ConnectionString, GetConnectionString())
		//		.SetProperty(NHibernate.Cfg.Environment.ProxyFactoryFactoryClass, typeof(NHibernate.ByteCode.Castle.ProxyFactoryFactory).AssemblyQualifiedName)
		//		//.SetProperty(Environment.UseQueryCache, "true")
		//		.SetProperty(NHibernate.Cfg.Environment.QuerySubstitutions, "true=1;false=0")
		//		.SetProperty(NHibernate.Cfg.Environment.Isolation, System.Data.IsolationLevel.ReadCommitted.ToString());

		//	var inPlaceConfigurationSource = new InPlaceConfigurationSource();
		//	inPlaceConfigurationSource.Add(typeof(BaseDnaCenterEntity), DnaCenterEntitiesCfg.Properties);

		//	inPlaceConfigurationSource.DefaultFlushType = DefaultFlushType.Auto;
		//	inPlaceConfigurationSource.PluralizeTableNames = false;
		//	inPlaceConfigurationSource.IsRunningInWebApp = true;

		//	Assembly[] assemblies = { typeof(DataMartEntity).Assembly };
		//	ActiveRecordStarter.Initialize(assemblies, inPlaceConfigurationSource);
		//}
	}
}
