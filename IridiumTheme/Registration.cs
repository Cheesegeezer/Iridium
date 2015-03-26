using System;
using System.Threading;
using MediaBrowser;
using MediaBrowser.Library.Logging;
using MediaBrowser.Library.Plugins;
using MediaBrowser.Library.Registration;
using MediaBrowser.Library.Threading;
using Version = System.Version;

namespace Iridium
{
    public class Registration
    {
        public Registration()
        {

        }

        const string FeatureName = "Iridium";

        private static DateTime _expirationDate;
        private static bool _isReg;
        private static PluginStatus _pluginStatus;
        

        public static void CheckRegistration(System.Version version)
        {
            Async.Queue("PPing", () =>
            {
                Logger.ReportInfo("==IRIDIUM== REGISTRATION CHECK IS COMMENCING");
                var timeout = 30;
                var counter = 0;
                var pleaseBuy = "Please purchase a license at www.mediabrowser3.com";
                var result = MBRegistration.GetRegistrationStatus(FeatureName, version);
                
                while (!result.RegChecked && counter < timeout)
                {
                    // Do nothing but wait.
                    Thread.Sleep(1000);
                    counter++;
                }
                
                _expirationDate = result.ExpirationDate;
                _isReg = result.IsRegistered;

                if (!_isReg && _expirationDate > DateTime.Now)
                {
                    Thread.Sleep(10000);
                    
                    _pluginStatus = PluginStatus.Trial;

                    if (Application.CurrentInstance.CurrentTheme.Name == "Iridium")
                    {
                        string text = string.Format("IRIDIUM IS IN TRAIL MODE - Expiration Date is {0} {1} ", _expirationDate.ToShortDateString(), Environment.NewLine);
                        CustomMessage.Instance.MessageBox(text);
                        //CustomMessage.MessageBox(text,true,0);
                        //Application.CurrentInstance.MessageBox(text, false, 0);
                    }
                    
                }
                else if (_isReg)
                {
                    _pluginStatus = PluginStatus.Registered;
                }
                else
                {
                    Thread.Sleep(10000);
                    _pluginStatus = PluginStatus.Expired;
                    if (Application.CurrentInstance.CurrentTheme.Name == "Iridium")
                    {
                        string text = string.Format("IRIDIUM HAS EXPIRED - Expiration Date is {0} {1}Please purchase from Server Plugin Catalogue ",_expirationDate.ToShortDateString(), Environment.NewLine);
                        CustomMessage.Instance.MessageBox(text);
                        //CustomMessage.MessageBox(text, true, 0);
                        //Application.CurrentInstance.MessageBox(text, true, 0);
                    }
                }

                Logger.ReportInfo(string.Format("++++++++++++++ IRIDIUM +++++++++++++++++ Registration Status: {0} [{1}] +++++++++++++++++", _pluginStatus, _expirationDate.ToShortDateString()));

                Application.CurrentInstance.SetThemeStatus(FeatureName, _pluginStatus.ToString());
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
