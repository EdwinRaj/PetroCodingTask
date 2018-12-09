using log4net;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petroineos.Reporting
{
    public class PowerTradeAggregator : IPowerTradeAggregator
    {

        public IEnumerable<PowerTradePosition> AggregatePowerTrades(IEnumerable<PowerTrade> powerTrades)
        {
            Dictionary<int, double> powerTradeVolumes = new Dictionary<int, double>();
            if (powerTrades != null)
            {
                foreach (var trade in powerTrades)
                {
                    foreach (var period in trade.Periods)
                    {
                        if (!powerTradeVolumes.ContainsKey(period.Period))
                        {
                            powerTradeVolumes.Add(period.Period, period.Volume);
                        }
                        else
                        {
                            powerTradeVolumes[period.Period] += period.Volume;
                        }
                    }
                }
            }
            return powerTradeVolumes.Select(kvp => new PowerTradePosition(kvp.Key, kvp.Value)).ToList();

        }
        //private void Archieve()
        //{


        //    Task<IEnumerable<PowerTrade>> getPowerTradeTask = _powerTradeProvider.GetPowerTrade();
        //    getPowerTradeTask.Start();

        //    if (getPowerTradeTask.IsCompleted && !getPowerTradeTask.IsFaulted && !getPowerTradeTask.IsCanceled)
        //    {
        //        var currentTrades = getPowerTradeTask.Result;

        //    }

        //}
    }

    public interface IPowerTradeAggregator
    {
        IEnumerable<PowerTradePosition> AggregatePowerTrades(IEnumerable<PowerTrade> powerTrades);
    }

    public class PowerTradePosition
    {
        public int Period { get;  }

        public TimeSpan LocalTime
        {
            get
            {
                int startHour = 23;
                int resolvedHour = (startHour + (Period - 1)) % 24;
                return TimeSpan.FromHours(resolvedHour);
            }
        }

        public double Volume { get;  }

        public PowerTradePosition(int period, double volume)
        {
            Period = period;
            Volume = volume;
        }
    }
}
