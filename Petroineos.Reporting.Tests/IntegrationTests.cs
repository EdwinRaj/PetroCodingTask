using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petroineos.Reporting.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public void EndToEndHappyScenario()
        {
            PowerTradeManager manager = new PowerTradeManager();
            var startTask = manager.StartReporting();

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));
            manager.StopReporting();

        }
    }
}
