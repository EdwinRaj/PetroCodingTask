using log4net;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petroineos.Reporting
{
    public class PowerTradeProvider : IPowerTradeProvider
    {
        private readonly IPowerService _powerService;
        private readonly ILog _log;
        public PowerTradeProvider(ILog log)
        {
            _log = log;
            _powerService = new PowerService();
        }
        public async Task<IEnumerable<PowerTrade>> GetPowerTradeAsync()
        {
            int retries = 0, maxRetries = 3;
            _log.Info("Attempting to Retrieving power trades");
            while (retries < maxRetries)
            {
                try
                {
                    var results = await _powerService.GetTradesAsync(DateTime.Now);
                    _log.Info($"Power trades successfully retrieved in the {retries + 1} attempt");
                    return results;
                }
                catch (PowerServiceException serviceException)
                {
                    _log.Error(serviceException.ToString());

                    _log.Error($"Power service failed to retrieve the Power Trades at attempt No:{retries+1}");
                    
                    retries++;

                    if (retries >= maxRetries)
                    {
                        _log.Error("Max retries reached");
                    }
                    else
                    {
                        _log.Error("Sourcing the data once again after 2 seconds");
                        await Task.Delay(2000);
                    }
                }
            }
            return null;
        }
    }

    public interface IPowerTradeProvider
    {
        Task<IEnumerable<PowerTrade>> GetPowerTradeAsync();
    }
}
