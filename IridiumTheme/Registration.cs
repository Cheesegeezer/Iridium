using System;
using MediaBrowser.Library.Logging;
using MediaBrowser.Library.Registration;
using MediaBrowser.Library.Threading;

namespace Iridium
{
    public class Registration
    {
        const string FeatureName = "Iridium";

        private static DateTime _expirationDate;
        private static bool _isReg;
        private static PluginStatus _pluginStatus;

        public static void CheckRegistration(System.Version version)
        {
            Async.Queue("PPing", () =>
            {
                var timeout = 30;
                var counter = 0;
                var pleaseBuy = "Please purchase a license at www.mediabrowser3.com";
                var result = MBRegistration.GetRegistrationStatus(FeatureName, version);
                
                while (!result.RegChecked && counter < timeout)
                {
                    // Do nothing but wait.
                    System.Threading.Thread.Sleep(1000);
                    counter++;
                }
                
                _expirationDate = result.ExpirationDate;
                _isReg = result.IsRegistered;

                if (!_isReg && _expirationDate > DateTime.Now)
                {
                    _pluginStatus = PluginStatus.Trial;
                    //InfoBar.DisplayAndLog(string.Format("{0} Trial Mode - {1} [{2}]", FeatureName, pleaseBuy, _expirationDate.ToLongDateString()));
                }
                else if (_isReg)
                {
                    _pluginStatus = PluginStatus.Registered;
                }
                else
                {
                    _pluginStatus = PluginStatus.Expired;

                    //InfoBar.DisplayAndLog(string.Format("{0} has expired. {1} [{2}]", FeatureName, pleaseBuy, _expirationDate.ToLongDateString()));
                }

                Logger.ReportInfo(string.Format("Registration Status: {0} [{1}]", _pluginStatus.ToString(), _expirationDate.ToLongDateString()));

                MediaBrowser.Application.CurrentInstance.SetThemeStatus(FeatureName, _pluginStatus.ToString());
            });
        }

    }
}

public enum PluginStatus
{
    Unknown = 0,
    Expired = 1,
    Trial = 2,
    Registered = 3
}
