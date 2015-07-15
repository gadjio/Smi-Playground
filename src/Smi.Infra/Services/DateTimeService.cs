namespace Smi.Infra.Services
{
	using System;

	public interface IDateTimeService
	{
		DateTime UtcNow { get; }
		DateTime Now { get; }
		DateTime Today { get; }
	}

	public class DateTimeService : IDateTimeService
	{
		public DateTime UtcNow
		{
			get { return DateTime.UtcNow; }
		}

		public DateTime Now
		{
			get { return DateTime.Now; }
		}

		public DateTime Today
		{
			get { return DateTime.Today; }
		}

	}
}