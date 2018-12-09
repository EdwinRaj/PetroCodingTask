using log4net;
using Petroineos.Reporting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Pertoineos.Reporting.Service
{
    public partial class PetroineosPositionReportingService : ServiceBase
    {
        private readonly PowerTradeManager _powerTradeManager = new PowerTradeManager();
        private readonly ILog _log;
        public PetroineosPositionReportingService()
        {
            InitializeComponent();
            _log = LogManager.GetLogger("PowerPositionReportingService");
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _log.Info("Starting the PetroineosPositionReporting Windows Service");
                Task t = _powerTradeManager.StartReporting();
                if (t.IsFaulted)
                {
                    _log.Error(t.Exception);
                }
                else
                {
                    _log.Info("PetroineosPositionReporting Windows Service successfully started");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        protected override void OnStop()
        {
            try
            {
                _log.Info("Stopping the PetroineosPositionReporting Windows Service");
                _powerTradeManager.StopReporting();

                _log.Info("PetroineosPositionReporting Windows Service Successfully stopped");
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
    }
}
