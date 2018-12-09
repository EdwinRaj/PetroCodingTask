using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using System.Linq;
using System.Collections.Generic;

namespace Petroineos.Reporting.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Random random = new Random();
            var y = Enumerable.Range(0, random.Next(1, 20));
            var data = Enumerable.Range(0, random.Next(1, 20)).Select<int, PowerTrade>((Func<int, PowerTrade>)(x => PowerTrade.Create(DateTime.Now, x))).ToArray<PowerTrade>();
            var testResult = GetTradesImpl(DateTime.Now);
        }

        private static readonly TimeZoneInfo GmtTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        private IEnumerable<PowerTrade> GetTradesImpl(DateTime date)
        {
            DateTime dateTime1 = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified).Date.AddHours(-1.0);
            DateTime dateTime2 = dateTime1.AddDays(1.0);
            DateTime utc1 = TimeZoneInfo.ConvertTimeToUtc(dateTime1, GmtTimeZoneInfo);
            DateTime utc2 = TimeZoneInfo.ConvertTimeToUtc(dateTime2, GmtTimeZoneInfo);
            int numberOfPeriods = (int)utc2.Subtract(utc1).TotalHours;
            Random random = new Random();
            var isTest = true;
            PowerTrade[] array = Enumerable.Range(0, isTest ? 2 : random.Next(1, 20)).Select<int, PowerTrade>((Func<int, PowerTrade>)(_ => PowerTrade.Create(date, numberOfPeriods))).ToArray<PowerTrade>();
            int index = 0;
            for (DateTime dateTime3 = utc1; dateTime3 < utc2; dateTime3 = dateTime3.AddHours(1.0))
            {
                foreach (PowerTrade powerTrade in array)
                {
                    double num = isTest ? (double)(index + 1) : random.NextDouble() * 1000.0;
                    powerTrade.Periods[index].Volume = num;
                }
                ++index;
            }
            return (IEnumerable<PowerTrade>)array;
        }
    }
}
