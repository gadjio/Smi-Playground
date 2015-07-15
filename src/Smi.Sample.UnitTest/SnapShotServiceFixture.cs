namespace Smi.Sample.UnitTest
{
	using System;
	using System.Collections.Generic;
	using AppCore.GameBoard.Commands;
	using AppCore.GameBoard.Domain;
	using AppCore.GameBoard.Events;
	using Infra;
	using Infra.Entities;
	using Microsoft.Win32.SafeHandles;
	using Newtonsoft.Json;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Smi.Infra.Bus;
	using Smi.Infra.EventStore;
	using Smi.Infra.EventStore.Aggregate;
	using Smi.Infra.EventStore.Services;
	using Smi.Infra.Services;

	[TestFixture]
	public class SnapShotServiceFixture
	{
		private SnapShotService sut;

		private IDomainRepository domainRepository;
		private IBus bus;
		private IEventConvertor eventConvertor;

		[SetUp]
		public void SetUp()
		{
			sut = new SnapShotService();

			bus = MockRepository.GenerateMock<IBus>();
			eventConvertor = MockRepository.GenerateMock<IEventConvertor>();

			var entityRepository = new InMemoryEntityRepository();
			domainRepository = new DomainRepository(new DomainEventStorageService(entityRepository, new DateTimeService()), bus, eventConvertor);
		}

		[Test]
		public void Test()
		{
			var id = Guid.NewGuid();
			var createCommand = new CreateGameBoardCommand
			{
				FriendlyName = "Transformers",
				MinimumNumberOfPlayers = 4,
				MaximumNumberOfPlayers = 16,
				MaxValidStartDate = new DateTime(2015, 02, 01)
			};

			var aggregate = new GameBoardAggregateRoot(id, createCommand.DehydrateCommand());
			domainRepository.Save(aggregate);

			var savedAggregate = domainRepository.GetById<GameBoardAggregateRoot>(id);

			//ACT
			var result = sut.Create(savedAggregate);

			//ASSERT
			Assert.That(result.Version, Is.EqualTo(1));

			var json = JsonConvert.DeserializeObject<IDictionary<string, object>>(result.JsonSerialization);
			Assert.That(json.ContainsKey("ActualNumberOfPlayers"));
			Assert.That(json.ContainsKey("MinimumNumberOfPlayers"));
			Assert.That(json.ContainsKey("MaximumNumberOfPlayers"));
			Assert.That(json.ContainsKey("StartDate"));

			Assert.That(json["ActualNumberOfPlayers"], Is.EqualTo(0));
			Assert.That(json["MinimumNumberOfPlayers"], Is.EqualTo(4));
			Assert.That(json["MaximumNumberOfPlayers"], Is.EqualTo(16));
			Assert.That(json["StartDate"], Is.EqualTo(new DateTime(2015, 02, 01)));

			Assert.That(json.Keys.Count, Is.EqualTo(4));
		}


		[Test]
		public void RecreateFromSnapshot()
		{
			var id = Guid.NewGuid();
			var createCommand = new CreateGameBoardCommand
			{
				FriendlyName = "Transformers",
				MinimumNumberOfPlayers = 4,
				MaximumNumberOfPlayers = 16,
				MaxValidStartDate = new DateTime(2015, 02, 01)
			};

			var aggregate = new GameBoardAggregateRoot(id, createCommand.DehydrateCommand());
			domainRepository.Save(aggregate);

			var savedAggregate = domainRepository.GetById<GameBoardAggregateRoot>(id);

			var snapshot = sut.Create(savedAggregate);

			var rebuiltAggregate = sut.Get<GameBoardAggregateRoot>(snapshot);

			//ACT
			var result = sut.Create(rebuiltAggregate);

			//ASSERT
			Assert.That(result.Version, Is.EqualTo(1));

			var json = JsonConvert.DeserializeObject<IDictionary<string, object>>(result.JsonSerialization);
			Assert.That(json.ContainsKey("ActualNumberOfPlayers"));
			Assert.That(json.ContainsKey("MinimumNumberOfPlayers"));
			Assert.That(json.ContainsKey("MaximumNumberOfPlayers"));
			Assert.That(json.ContainsKey("StartDate"));

			Assert.That(json["ActualNumberOfPlayers"], Is.EqualTo(0));
			Assert.That(json["MinimumNumberOfPlayers"], Is.EqualTo(4));
			Assert.That(json["MaximumNumberOfPlayers"], Is.EqualTo(16));
			Assert.That(json["StartDate"], Is.EqualTo(new DateTime(2015, 02, 01)));

			Assert.That(json.Keys.Count, Is.EqualTo(4));
		}


		[Test]
		public void CreateComplexAggregate()
		{
			var id = Guid.NewGuid();

			var optimusPrime = new TestEntity("OptimusPrime", 5, new List<TestItem>());
			var bumbleBee = new TestEntity("BumBleBee", null, new List<TestItem>() { new TestItem("sub1", false), new TestItem("sub2", true) });
			var hotRod = new TestEntity("HotRod", 5, new List<TestItem> { new TestItem("hr1", true), new TestItem("hr2", false) });

			var aggregate = new TestAggregateRoot(id, "Transformers", new List<TestEntity> { bumbleBee, hotRod }, optimusPrime);			

			//ACT
			var result = sut.Create(aggregate);

			//ASSERT
			Assert.That(result.Version, Is.EqualTo(1));

			var json = JsonConvert.DeserializeObject<IDictionary<string, object>>(result.JsonSerialization);
			Assert.That(json.ContainsKey("ActualNumberOfPlayers"));
			Assert.That(json.ContainsKey("MinimumNumberOfPlayers"));
			Assert.That(json.ContainsKey("MaximumNumberOfPlayers"));
			Assert.That(json.ContainsKey("StartDate"));

			Assert.That(json.ContainsKey("name"));
			Assert.That(json.ContainsKey("entities"));
			Assert.That(json.ContainsKey("mainEntity"));

			Assert.That(json["ActualNumberOfPlayers"], Is.EqualTo(0));
			Assert.That(json["MinimumNumberOfPlayers"], Is.EqualTo(0));
			Assert.That(json["MaximumNumberOfPlayers"], Is.EqualTo(0));

			Assert.That(json["name"], Is.EqualTo("Transformers"));

			Console.WriteLine("Json keys and values");
			foreach (var key in json.Keys)
			{
				Console.WriteLine(key + " : " + json[key]);
			}
			
			// Contains 4 backingField
			Assert.That(json.Keys.Count, Is.EqualTo(13));
		}

		[Test]
		public void RecreateComplexAggregateFromSnapshot()
		{
			var id = Guid.NewGuid();
			

			var optimusPrime = new TestEntity("OptimusPrime", 5, new List<TestItem>());
			var bumbleBee = new TestEntity("BumBleBee", null, new List<TestItem>() {new TestItem("sub1", false), new TestItem("sub2", true)});
			var hotRod = new TestEntity("HotRod", 5, new List<TestItem> {new TestItem("hr1", true), new TestItem("hr2", false)});			

			var aggregate = new TestAggregateRoot(id, "Transformers", new List<TestEntity>{bumbleBee, hotRod}, optimusPrime );
			
			var snapshot = sut.Create(aggregate);

			var rebuiltAggregate = sut.Get<TestAggregateRoot>(snapshot);

			//ACT
			var result = sut.Create(rebuiltAggregate);

			//ASSERT
			Assert.That(result.Version, Is.EqualTo(1));

			Assert.That(rebuiltAggregate.GetName(), Is.EqualTo("Transformers"));
			Assert.That(rebuiltAggregate.GetEntitiesList().Count, Is.EqualTo(2));			
		}

		[Test]
		public void Recreate_V2_From_V1_Snapshot()
		{
			var id = Guid.NewGuid();
			var createCommand = new CreateGameBoardCommand
			{
				FriendlyName = "Transformers",
				MinimumNumberOfPlayers = 4,
				MaximumNumberOfPlayers = 16,
				MaxValidStartDate = new DateTime(2015, 02, 01)
			};

			var aggregate = new GameBoardAggregateRoot(id, createCommand.DehydrateCommand());
			domainRepository.Save(aggregate);

			var savedAggregate = domainRepository.GetById<GameBoardAggregateRoot>(id);

			var snapshot = sut.Create(savedAggregate);

			
			Exception receivedException = null;
			try
			{
				sut.Get<GameBoardAggregateRoot_V2>(snapshot);
			}
			catch (Exception exception)
			{
				receivedException = exception;
			}

			Assert.That(receivedException, Is.Not.Null);
			
		}

		[Test]
		public void Recreate_v2_typeChanged_From_V1_Snapshot()
		{
			var id = Guid.NewGuid();
			var createCommand = new CreateGameBoardCommand
			{
				FriendlyName = "Transformers",
				MinimumNumberOfPlayers = 4,
				MaximumNumberOfPlayers = 16,
				MaxValidStartDate = new DateTime(2015, 02, 01)
			};

			var aggregate = new GameBoardAggregateRoot(id, createCommand.DehydrateCommand());
			domainRepository.Save(aggregate);

			var savedAggregate = domainRepository.GetById<GameBoardAggregateRoot>(id);

			var snapshot = sut.Create(savedAggregate);


			Exception receivedException = null;
			try
			{
				sut.Get<GameBoardAggregateRoot_V2TypeChanged>(snapshot);
			}
			catch (Exception exception)
			{
				receivedException = exception;
			}

			Assert.That(receivedException, Is.Not.Null);

		}
    }


	public class GameBoardAggregateRoot_V2 : GameBoardAggregateRoot
	{
		private string TestValueAdded { get; set; }
	}

	public class GameBoardAggregateRoot_V2TypeChanged : BaseAggregateRoot
	{
		private int ActualNumberOfPlayers { get; set; }
		private int MinimumNumberOfPlayers { get; set; }
		private int MaximumNumberOfPlayers { get; set; }
		private int StartDate { get; set; }

		public GameBoardAggregateRoot_V2TypeChanged()
		{
		}
		

		public override void RegisterEvents()
		{

		}
	}

	public class TestAggregateRoot : BaseAggregateRoot
	{
		private int ActualNumberOfPlayers { get; set; }
		private int MinimumNumberOfPlayers { get; set; }
		private int MaximumNumberOfPlayers { get; set; }
		private int StartDate { get; set; }

		private string name;
		private IList<TestEntity> entities;
		private TestEntity mainEntity;


		public TestAggregateRoot()
		{
		}

		public TestAggregateRoot(Guid id, string name, IList<TestEntity> entities, TestEntity mainEntity)
		{
			base.Id = id;
			base.Version = 1;

			this.name = name;
			this.entities = entities;
			this.mainEntity = mainEntity;
		}

		public override void RegisterEvents()
		{
			
		}

		public string GetName()
		{
			return name;
		}
	
		public IList<TestEntity> GetEntitiesList()
		{
			return entities;
		}


		public TestEntity GetMainEntity()
		{
			return mainEntity;
		}
		
	}

	public class TestEntity
	{
		private readonly string name;
		private int? pieces;
		private IList<TestItem> items;

		public TestEntity(string name, int? pieces, IList<TestItem> items)
		{
			this.name = name;
			this.pieces = pieces;
			this.items = items;
		}

		public string Name
		{
			get { return name; }
		}

		public IList<TestItem> Items
		{
			get { return items; }
		}

		public int? Pieces
		{
			get { return pieces; }
			set { pieces = value; }
		}
	}

	public class TestItem
	{
		private string name;
		private bool isDefault;

		public TestItem(string name, bool isDefault)
		{
			this.name = name;
			this.isDefault = isDefault;
		}

		public string Name
		{
			get { return name; }
		}

		public bool IsDefault
		{
			get { return isDefault; }
		}
	}
}
