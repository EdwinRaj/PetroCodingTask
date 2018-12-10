using log4net;
using Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Petroineos.Reporting
{
    public class PowerTradeAggregator : IPowerTradeAggregator
    {
        private readonly ILog _log;
        public PowerTradeAggregator(ILog log)
        {
            _log = log;
        }
        public ReadOnlyCollection<PowerTradePosition> AggregatePowerTrades(IEnumerable<PowerTrade> powerTrades)
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
            return powerTradeVolumes.Select(kvp => new PowerTradePosition(kvp.Key, kvp.Value)).ToList().AsReadOnly();
        }
    }

    public interface IPowerTradeAggregator
    {
        ReadOnlyCollection<PowerTradePosition> AggregatePowerTrades(IEnumerable<PowerTrade> powerTrades);
    }

    public class PowerTradePosition
    {
        public int Period { get; }

        public TimeSpan LocalTime { get; }

        public double Volume { get; }

        public PowerTradePosition(int period, double volume)
        {
            Period = period;
            Volume = volume;
            LocalTime = CalculateLocatTime(period);
        }

        private TimeSpan CalculateLocatTime(int period)
        {
            int startHour = 23;
            int resolvedHour = (startHour + (Period - 1)) % 24;
            return TimeSpan.FromHours(resolvedHour);
        }
    }
}
