using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Trends.ViewModels;

namespace Trends.Tests.ViewModels
{
	[TestFixture]
	public class TrendTests : AssertionHelper
	{
		private readonly DateTime january = new DateTime(2015, 1, 1);
		private readonly DateTime june = new DateTime(2015, 6, 1);

		public void Out<T>(IEnumerable<T> list)
		{
			foreach (var item in list)
				Console.WriteLine(item);
		}

		[Test]
		public void Calculate_Always_PlaceTransactionsWithRightDate()
		{
			var trend = new Trend();
            trend.Operations.Add(new Operation(100, new DateTime(2015, 1, 1), DatePeriod.FromDays(3), "Test"));

			trend.Calculate(1000, january, june);

			Expect(trend.Calendar, Has.Exactly(1).Property("Date").EqualTo(new DateTime(2015, 1, 1)));
			Expect(trend.Calendar, Has.Exactly(1).Property("Date").EqualTo(new DateTime(2015, 1, 4)));
			Expect(trend.Calendar, Has.Exactly(1).Property("Date").EqualTo(new DateTime(2015, 1, 7)));
			Expect(trend.Calendar, Has.Exactly(1).Property("Date").EqualTo(new DateTime(2015, 1, 10)));
			Expect(trend.Calendar, Has.Exactly(1).Property("Date").EqualTo(new DateTime(2015, 1, 25)));
			Expect(trend.Calendar, Has.Exactly(1).Property("Date").EqualTo(new DateTime(2015, 1, 31)));
		}


		[Test]
		public void Calculate_FromJanuaryToJune_PlaceTransactionsOnlyFromJanuaryToJune()
		{
			var trend = new Trend();
			trend.Operations.Add(new Operation(100, new DateTime(2015, 1, 1), DatePeriod.FromDays(3), "Test"));

			trend.Calculate(1000, january, june);

			Expect(trend.Calendar, Has.All.Property("Date").Property("Month").LessThan(6));
			Expect(trend.Calendar, Has.All.Property("Date").Property("Year").EqualTo(2015));
		}

		[Test]
		public void Calculate_Always_GroupsTransactionsWithSameDate()
		{
			var trend = new Trend();
			var date = new DateTime(2015, 1, 1);
			trend.Calendar.Add(new Transaction(100, date, "1"));
			trend.Calendar.Add(new Transaction(100, date, "2"));
			trend.Calendar.Add(new Transaction(100, date, "3"));

			trend.Calculate(1000, january, june);

			Expect(trend.Calendar, Count.EqualTo(1));
			Expect(trend.Calendar, All.Property("Date").EqualTo(date));
		}

		[Test]
		public void Calculate_Always_AggregateTransactionsAmmountsAndDescriptions()
		{
			var trend = new Trend();
			var date = new DateTime(2015, 1, 1);
			trend.Calendar.Add(new Transaction(100, date, "1"));
			trend.Calendar.Add(new Transaction(100, date, "2"));
			trend.Calendar.Add(new Transaction(100, date, "3"));

			trend.Calculate(1000, january, june);

			Expect(trend.Calendar, All.Property("Amount").EqualTo(300));
			Expect(trend.Calendar, All.Property("Description").EqualTo("1, 2, 3"));
		}

		[Test]
		public void Calculate_Always_SortsItemsByDate()
		{
			var trend = new Trend();
			trend.Calendar.Add(new Transaction(100, new DateTime(2015, 1, 3), "3"));
			trend.Calendar.Add(new Transaction(100, new DateTime(2015, 1, 6), "6"));
			trend.Calendar.Add(new Transaction(100, new DateTime(2015, 1, 1), "1"));
			trend.Calendar.Add(new Transaction(100, new DateTime(2015, 1, 5), "5"));
			trend.Calendar.Add(new Transaction(100, new DateTime(2015, 1, 2), "2"));
			trend.Calendar.Add(new Transaction(100, new DateTime(2015, 1, 4), "4"));

			trend.Calculate(1000, january, june);

			Expect(trend.Calendar, Is.Ordered.By("Date"));
		}

		[Test]
		public void Calculate_WithStartFunds_AppliesEachTransactionOnFundsAmount()
		{
			var trend = new Trend();

			trend.Calendar.Add(new Transaction(-100, new DateTime(2015, 1, 1), "1"));
			trend.Calendar.Add(new Transaction(-100, new DateTime(2015, 1, 2), "2"));
			trend.Calendar.Add(new Transaction(-100, new DateTime(2015, 1, 3), "3"));
			trend.Calendar.Add(new Transaction(-100, new DateTime(2015, 1, 4), "4"));
			trend.Calendar.Add(new Transaction(-100, new DateTime(2015, 1, 4), "5"));

			trend.Calculate(1000, january, june);
			var actual = trend.GetFunds().Select(funds => funds.Amount);

			decimal[] expect = {900, 800, 700, 500};
			Expect(actual, EquivalentTo(expect));
		}

		[Test]
		public void GetFunds_Always_ReturnsCorrectList()
		{
			var trend = new Trend();
			trend.Calendar.Add(new Transaction(-100, new DateTime(2015, 1, 1), "1"));
			trend.Calendar.Add(new Transaction(-100, new DateTime(2015, 1, 2), "2"));
			trend.Calendar.Add(new Transaction(-100, new DateTime(2015, 1, 3), "3"));
			trend.Calendar.Add(new Transaction(-100, new DateTime(2015, 1, 4), "4"));
			trend.Calendar.Add(new Transaction(-100, new DateTime(2015, 1, 4), "5"));

			trend.Calculate(1000, january, june);
			var actual = trend.GetFunds();

			var expected = new List<Funds>
			{
				new Funds(900, new DateTime(2015, 1, 1), "1"),
				new Funds(800, new DateTime(2015, 1, 2), "2"),
				new Funds(700, new DateTime(2015, 1, 3), "3"),
				new Funds(500, new DateTime(2015, 1, 4), "4, 5")
			};
			Expect(actual, EqualTo(expected));
		}

		[Test]
		public void Calculate_Always_DoesCorrectCalculations()
		{
			var trend = new Trend();
			var start = new DateTime(2015, 6, 1);
			trend.Operations.Add(new Operation(-100, start, DatePeriod.FromDays(3), "Test"));

			var actual = trend.Calculate(1000m, start, start.AddDays(12));

			decimal[] expected = {1000, 900, 800, 700, 600};
			Expect(actual.Select(t => t.Amount), EqualTo(expected));
		}

	}
}
