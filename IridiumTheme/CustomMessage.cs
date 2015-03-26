using System;
using System.Threading;
using MediaBrowser;
using MediaBrowser.Library.Threading;
using Microsoft.MediaCenter.UI;
using Application = Microsoft.MediaCenter.UI.Application;

namespace Iridium
{
    public class CustomMessage : ModelItem
    {
        private static CustomMessage customMessage;
        public static CustomMessage _instance; 

        public static CustomMessage Instance
        {
            get
            {
                if (_instance == null)
                {
                    CustomMessage instance = new CustomMessage();
                    _instance = instance;
                }
                return _instance;
            }
            
        }
        public CustomMessage()
        {
        }

        private string _messageUI = "resx://Iridium/Iridium.Resources/CustomMessage#MessageBox";
        public string MessageResponse { get; set; }
        private string _messageText = "";
        private bool _showMessage = false;
        

        public string MessageBox(string msg)
        {
            return this.MessageBox(msg, true, 0, null);
        }
        public string MessageBox(string msg, bool modal, int timeout)
        {
            return this.MessageBox(msg, modal, timeout, null);
        }

        private string MessageBox(string msg, bool modal, int timeout, string ui)
        {
            Action action;
            this.MessageUI = !string.IsNullOrEmpty(ui) ? ui : "resx://Iridium/Iridium.Resources/CustomMessage#MessageBox";
            this.MessageResponse = "";
            this.MessageText = msg;
            this.ShowMessage = true;
            if (timeout > 0)
            {
                if (modal)
                {
                    this.WaitForMessage(timeout);
                }
                else
                {
                    action = delegate
                    {
                        this.WaitForMessage(timeout);
                    };
                    Async.Queue("Iri Mes", action);
                }
            }
            return this.MessageResponse;
        }

        public string MessageUI
        {
            get
            {
                return this._messageUI;
            }
            set
            {
                DeferredHandler method = null;
                if (this._messageUI != value)
                {
                    this._messageUI = value;
                    if (method == null)
                    {
                        method = _ => base.FirePropertyChanged("MessageUI");
                    }
                    Application.DeferredInvoke(method);
                }
            }
        }

        public string MessageText
        {
            get
            {
                return this._messageText;
            }
            set
            {
                DeferredHandler method = null;
                if (this._messageText != value)
                {
                    this._messageText = value;
                    if (method == null)
                    {
                        method = _ => base.FirePropertyChanged("MessageText");
                    }
                    Application.DeferredInvoke(method);
                }
            }
        }

        public bool ShowMessage
        {
            get
            {
                return this._showMessage;
            }
            set
            {
                DeferredHandler method = null;
                if (this._showMessage != value)
                {
                    this._showMessage = value;
                    if (method == null)
                    {
                        method = _ => base.FirePropertyChanged("ShowMessage");
                    }
                    Application.DeferredInvoke(method);
                }
            }
        }

        protected void WaitForMessage(int timeout)
        {
            DateTime now = DateTime.Now;
            while (this._showMessage && ((DateTime.Now - now).TotalMilliseconds < timeout))
            {
                Thread.Sleep(250);
            }
            this.ShowMessage = false;
        }

        //Add Custom MBInfo Messages
        public static void InfoDisplay(string message)
        {
            InfomationItem info = new InfomationItem(message);
            MediaBrowser.Application.CurrentInstance.Information.AddInformation(info);
        }

        public void ProgressBox(string msg)
        {
            MessageBox(msg, false, 0, "resx://Iridium/Iridium.Resources/CustomMessage#ProgressBox");
        }

        public string YesNoBox(string msg)
        {
            return MessageBox(msg, true, 0, "resx://Iridium/Iridium.Resources/CustomMessage#YesNoBox");
        }
    }
}
