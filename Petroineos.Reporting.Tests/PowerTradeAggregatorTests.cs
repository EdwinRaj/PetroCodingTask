using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petroineos.Reporting.Tests
{
    [TestClass]
    public class PowerTradeAggregatorTests
    {

        [TestMethod]
        public void GivenValidPowerTrades_WhenInvoked_ReturnsValidVolume()
        {
            int volumeFactor = 1;
            int tradesCount = 2;
            var testTrades = GeneratePowerPeriods(tradesCount, 24, volumeFactor);
            ILog log = LogManager.GetLogger("TradeTest");
            PowerTradeAggregator aggregator = new PowerTradeAggregator(log);
            var result = aggregator.AggregatePowerTrades(testTrades).ToList();

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(i + 1, result[i].Period);
                Assert.AreEqual(volumeFactor * tradesCount, result[i].Volume);
            }
        }

        [TestMethod]
        public void GivenInvalidPowerTrades_WhenAggregated_ReturnsException()
        {

        }

        private List<PowerTrade> GeneratePowerPeriods(int totalTrades, int totalPeriods, double volumeFactor)
        {
            List<PowerTrade> testTrades = Enumerable.Range(0, totalTrades).Select(index => PowerTrade.Create(DateTime.Now, totalPeriods)).ToList();
            foreach (var trade in testTrades)
            {
                for (int i = 1; i <= totalPeriods; i++)
                {
                    trade.Periods[i - 1].Period = i;
                    trade.Periods[i - 1].Volume = 1 * volumeFactor;
                }
            }

            return testTrades;

        }
    }
}
