using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petroineos.Reporting.Tests
{
    [TestClass]
    public class PowerTradeVolumeTests
    {
        [TestMethod]
        [DataRow(1,23)]
        [DataRow(2, 0)]
        [DataRow(3, 1)]
        [DataRow(4, 2)]
        [DataRow(22, 20)]
        [DataRow(23, 21)]
        [DataRow(24, 22)]
        public void GivenPeriod_WhenInvoked_ReturnResolvedLocalTime(int period, int hour)
        {
            TimeSpan expectedHour = TimeSpan.FromHours(hour);
            PowerTradePosition powerTradeVolume = new PowerTradePosition(period, 1);
            Assert.AreEqual(expectedHour, powerTradeVolume.LocalTime);
        }
    }
}
