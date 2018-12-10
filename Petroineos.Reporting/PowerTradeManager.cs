using CsvHelper;
using log4net;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Petroineos.Reporting
{
    public class PowerTradeManager
    {
        private readonly ILog _log;
        private readonly IPowerTradeProvider _tradeProvider;
        private readonly IPowerTradeAggregator _tradeAggregator;
        private string _reportFilePath;
        private CancellationTokenSource _cancellationTokenSource;
        int _reportingIntervalInMinutes = 0;

        public PowerTradeManager()
        {
            _log = LogManager.GetLogger("PowerTrade");
            _tradeProvider = new PowerTradeProvider(_log);
            _tradeAggregator = new PowerTradeAggregator(_log);

        }

        public async Task StartReporting()
        {
            if (!TryResolvingReportingFilePath() || !TryResolvingScheduleInterval())
            {
                throw new ConfigurationErrorsException("Invalid Configuration. Check If valid Reporting path and scheduled interval are specified");
            }

            _log.Info($"Reporting Path:{_reportFilePath}");
            _log.Info($"Reporting Interval is every {_reportingIntervalInMinutes} Minutes");

            _cancellationTokenSource = new CancellationTokenSource();
            await ExecuteAndRepeatAction(GeneratePowerTradePositionReportAync, TimeSpan.FromMinutes(_reportingIntervalInMinutes), _cancellationTokenSource.Token);
        }

        private bool TryResolvingScheduleInterval()
        {
            var scheduleIntervalInMinutes = ConfigurationManager.AppSettings["ScheduleIntervalInMinutes"];
            if ((IsValidConfiguration(scheduleIntervalInMinutes) && int.TryParse(scheduleIntervalInMinutes, out _reportingIntervalInMinutes)) == false)
            {
                _log.Error("Schedule Interval is not Specified");
                return false;
            }
            return true;
        }

        private bool TryResolvingReportingFilePath()
        {
            _reportFilePath = ConfigurationManager.AppSettings["PositionReportFilePath"];
            if (IsValidConfiguration(_reportFilePath))
            {
                DirectoryInfo directory = new DirectoryInfo(_reportFilePath);
                if (!directory.Exists)
                {
                    _log.Warn($"Specified directory {_reportFilePath} doesn't exists. Creating the directory");
                    directory.Create();
                    _log.Warn($"Director {_reportFilePath} successfully created");
                }
            }
            else
            {
                _log.Error("Report File System Path is not Specified");
                return false;
            }
            return true;
        }


        private async Task GeneratePowerTradePositionReportAync()
        {
            var powerTrades = await _tradeProvider.GetPowerTradeAsync();
            var powerTradePositions = _tradeAggregator.AggregatePowerTrades(powerTrades);

            var reportName = Path.Combine(_reportFilePath, $"PowerPosition_{DateTime.Now:yyyyMMdd_HHmm}.csv");

            //Generate CSV file
            WriteToCsv(powerTradePositions, reportName);

            _log.Info("Report Generation Completed");

        }

        public async Task ExecuteAndRepeatAction(Func<Task> action, TimeSpan interval, CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    _log.Info("Report Generation Initiated");
                    action();

                    _log.Info($"Going to Sleep for configured interval {_reportingIntervalInMinutes} minutes");
                    Task task = Task.Delay(interval, cancellationToken);

                    await task;
                }
            }
            catch (TaskCanceledException)
            {
                _log.Info("Task Cancellation Successful.");
            }
            catch (Exception ex)
            {
                _log.Error("Error while generating the report");
                _log.Error(ex.ToString());
            }
        }

        private void WriteToCsv(IEnumerable<PowerTradePosition> powerTradePositions, string reportName)
        {
            try
            {
                _log.Info($"Trying to persist the report named {reportName}");
                using (StreamWriter writer = new StreamWriter(reportName))
                {
                    using (CsvWriter csvWriter = new CsvWriter(writer))
                    {
                        csvWriter.Configuration.RegisterClassMap<PowerTradePositionMap>();
                        csvWriter.WriteRecords(powerTradePositions);
                    }
                }
                _log.Info($"Report {reportName} Successfully persisted");
            }
            catch (Exception)
            {
                _log.Error($"Unable to save the report {reportName}");
                _log.Error("Report Content below");
                //Todo! write report content
                _log.Error(string.Join(Environment.NewLine, powerTradePositions.Select(x => $"{x.LocalTime},{x.Volume}")));

                throw;
            }
        }

        public void StopReporting()
        {
            _log.Info("Requesting Task Cancellation");
            _cancellationTokenSource?.Cancel();
        }

        private static bool IsValidConfiguration(string configuredValue)
        {
            return configuredValue != null && configuredValue.Trim().Length > 0;
        }

    }

    public sealed class PowerTradePositionMap: CsvHelper.Configuration.ClassMap<PowerTradePosition>
    {
        public PowerTradePositionMap()
        {
            Map(m => m.LocalTime).Index(0).Name("Local Time");
            Map(m => m.Volume).Index(1).Name("Volume");
            Map(m => m.Period).Ignore();
        }
    }
}
