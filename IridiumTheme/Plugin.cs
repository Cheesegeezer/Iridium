using System;
using System.Collections.Generic;
using System.IO;
using MediaBrowser.Library.Localization;
using MediaBrowser.Library.Plugins;
using MediaBrowser.Library.Logging;
using MediaBrowser.Library;
using MediaBrowser.Library.Util;
using MediaBrowser.Util;
using Iridium.Fonts;

namespace Iridium
{
    class Plugin : BasePlugin
    {
        static readonly Guid IridiumGuid = new Guid("2F6788A1-12DC-4E6B-8553-14CD479BC042");
        private FontManager _fontManager;
        public static List<string> AvailableStyles = new List<string>();
        public static MyConfig config = null;
        public static List <string>ExtraViewsList = new List<string>();
        public Plugin()
        {
            Logger.ReportInfo("Iridium - Creating Theme");

            using (new Profiler("Iridium - Theme Creation"))
            {
                bool isMC = AppDomain.CurrentDomain.FriendlyName.Contains("ehExtHost");
                if (isMC)
                {
                     MyConfig.InitDisplayPrefs();
                     CustomStyles.InitStyles();

                    if (config == null)
                        config = new MyConfig();
                }
            }
        }

        /// <summary>
        /// The Init method is called when the plug-in is loaded by MediaBrowser.  You should perform all your specific initializations
        /// here - including adding your theme to the list of available themes.
        /// </summary>
        /// <param name="kernel"></param>
        public override void Init(Kernel kernel)
        {
            try
            {
                _fontManager = new FontManager();
                _fontManager.FontCollection();
                kernel.AddTheme("Iridium", "resx://Iridium/Iridium.Resources/Page#PageIridium", "resx://Iridium/Iridium.Resources/DetailMovieView#IridiumMovieView");
                
                bool isMC = AppDomain.CurrentDomain.FriendlyName.Contains("ehExtHost");
                if (isMC)
                {
                    if (config == null)
                        config = new MyConfig();
                    //_actorInfo = new ActorInfo();
                    //_actorInfo.Init();

                    //If you want to add any context menus they need to be inside this logic as well.
                    kernel.AddConfigPanel("Iridium Options", "resx://Iridium/Iridium.Resources/ConfigPanel#ConfigPanel",
                        config);
                    //Check the current active style applied
                    config.CheckActiveStyle();
                    //Append styles with custom fonts
                    CustomResourceManager.AppendFonts("Iridium", Resources.Fonts, Resources.Fonts);
                    CustomResourceManager.AppendStyles("Iridium", Resources.Colors, Resources.Colors);

                    Registration.CheckRegistration(this.Version);
                    //CustomStrings Editable by user - need to implement
                    kernel.StringData.AddStringData(MyStrings.FromFile(LocalizedStringData.GetFileName("Iridium-")));

                }
                else
                    Logger.ReportInfo(
                        "Not creating menus for Iridium.  Appear to not be in MediaCenter.  AppDomain is: " +
                        AppDomain.CurrentDomain.FriendlyName);

                //Tell the log we loaded.
                Logger.ReportInfo("Iridium Theme Loaded.");
            }
            catch (Exception ex)
            {
                string text = string.Format("Error adding theme" + Name + "- probably the Author is a mong and forgotton to do something!!!")
                ;
                ;
                Logger.ReportException(text, ex);
            }

        }

        public override string Name
        {
            //provide a short name for your theme - this will display in the configurator list box
            get { return "Iridium Theme"; }
        }

        public override string Description
        {
            //provide a longer description of your theme - this will display when the user selects the theme in the plug-in section
            get { return "A Basic Theme with rich content, perfect for XBox Extenders"; }
        }

        

        

        
    }
}
