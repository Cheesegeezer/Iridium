﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MediaBrowser.Library.Configuration;
using MediaBrowser.Library.Logging;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;

namespace Iridium
{
    //******************************************************************************************************************
    //  This class is used to house your configuration parameters.  It is a decendant of ModelItem and will be passed 
    //  into your config panel at runtime (see configpanel setup in plugin.cs) so that you can bind to its properties.
    //******************************************************************************************************************
    public class MyConfig : ModelItem
    {
        private FolderConfigData folderData;
        private bool isValid;
        private ConfigData _data;
        public static Choice AvailableStyles = new Choice();
        private readonly string _configFilePath = Path.Combine(ApplicationPaths.AppPluginPath, @"Configurations\IridiumSettings.xml");
        private static readonly string _configFolderPath = Path.Combine(ApplicationPaths.AppPluginPath, "Configurations");
        private static readonly string FontsFilePath = Path.Combine(ApplicationPaths.AppPluginPath, @"Iridium\Iridium_Fonts_DO_NOT_EDIT.mcml");
        private static readonly string ColorsFilePath = Path.Combine(ApplicationPaths.AppPluginPath, @"Iridium\Iridium_Colors_DO_NOT_EDIT.mcml");
        private static readonly string StyleFilePath = Path.Combine(ApplicationPaths.AppConfigPath, "Iridium_Styles.mcml");
        private static readonly string ThemeFolderPath = Path.Combine(ApplicationPaths.AppPluginPath, "Iridium");
        private static readonly string displayFolderPath = Path.Combine(ApplicationPaths.AppUserSettingsPath, "IridiumDisplay");

        public static void InitDisplayPrefs()
        {
            string[] displayFiles;
            string destFile;

            if (!Directory.Exists(displayFolderPath))
            {
                Logger.ReportInfo("Iridium - Creating display prefs folder: " + displayFolderPath);
                Directory.CreateDirectory(displayFolderPath);
            }

            // Migrate display files from root of config
            displayFiles = Directory.GetFiles(_configFolderPath, "Iridium_*.xml");

            foreach (string displayFile in displayFiles)
            {
                Logger.ReportInfo("Iridium - Moving display prefs file: " + displayFile);

                destFile = Path.Combine(displayFolderPath, Path.GetFileName(displayFile));

                try
                {
                    if (File.Exists(destFile))
                        File.Delete(displayFile);
                    else
                        File.Move(displayFile, destFile);
                }
                catch
                {
                    // NOP
                }
            }
        }

        public MyConfig()
        {
            isValid = Load();
            //CheckActiveFonts();
            //CheckActiveColors();
            CheckActiveStyle();

            ArrayList styleOptions = new ArrayList();
            string[] styleFiles;
            styleFiles = Directory.GetFiles(CustomStyles.StylesFolderPath, "*.mcml");
            foreach (string styleFile in styleFiles)
                if (styleFile != null) styleOptions.Add(Path.GetFileNameWithoutExtension(styleFile));
            styleOptions.Sort();
            CustomStyles.InstalledStyles.Options = styleOptions;

        }

        public void CheckActiveStyle()
        {
            //Logger.ReportInfo("Iridium - Checking active style: " + StyleFilePath);

            // Force Iridium_Styles.mcml to be updated with active style
            string activeStyle = ThemeStyle;
            _data.ThemeStyle = "";  // Force a style change
            ThemeStyle = activeStyle;

            // Fallback to Default if active style didn't work
            if (!File.Exists(StyleFilePath))
            {
                _data.ThemeStyle = String.Empty;  // Force a style change
                ThemeStyle = "Shaeff";
            }
        }

        private bool Load()
        {
            try
            {
                if (!Directory.Exists(ThemeFolderPath))
                {
                    Logger.ReportInfo("Iridium - Creating theme folder: " + ThemeFolderPath);
                    Directory.CreateDirectory(ThemeFolderPath);
                }

                //Create Font File
                if (!File.Exists(FontsFilePath))
                {
                    Logger.ReportInfo("Iridium - Creating basic fonts file: " + FontsFilePath);
                    File.WriteAllBytes(FontsFilePath, Resources.Fonts);
                }

                //Create Colors File
                if (!File.Exists(ColorsFilePath))
                {
                    Logger.ReportInfo("Iridium - Creating basic fonts file: " + ColorsFilePath);
                    File.WriteAllBytes(ColorsFilePath, Resources.Colors);
                }

                _data = ConfigData.FromFile(_configFilePath);
                return true;
            }
            catch (Exception exception)
            {
                if (AddInHost.Current.MediaCenterEnvironment.Dialog(exception.Message + "\nReset to default?", "Error in configuration file", DialogButtons.No | DialogButtons.Yes, 600, true) == DialogResult.Yes)
                {
                    if (!Directory.Exists(_configFolderPath))
                    {
                        Directory.CreateDirectory(_configFolderPath);
                    }
                    _data = new ConfigData(_configFilePath);
                    Save();
                    return true;
                }
                return false;
            }
        }

        private void Save()
        {
            lock (this)
            {
                _data.Save();
            }
        }

        public bool EnableVideoBackdrop
        {
            get { return _data.enableVideoBackdrop; }
            set
            {
                if (_data.enableVideoBackdrop != value)
                {
                    _data.enableVideoBackdrop = value;
                    FirePropertyChanged("EnableVideoBackdrop");
                    Save();
                }
            }
        }

        public bool EnableTVShowView
        {
            get { return _data.UseTVShowView; }
            set
            {
                if (_data.UseTVShowView != value)
                {
                    _data.UseTVShowView = value;
                    FirePropertyChanged("UseTVShowView");
                    Save();
                }
            }
        }

        public Single BackdropTransitionTime
        {
            get { return _data.BackdropTransitionTime; }
            set
            {

                if (_data.BackdropTransitionTime != value)
                {
                    _data.BackdropTransitionTime = (float)Math.Round(value, 1, MidpointRounding.ToEven);
                    FirePropertyChanged("BackdropTrnsitionTime");
                    Save();
                }
            }
        }

        /*public Single BackdropOverlayAlpha
        {
            get { return this._data.backdropOverlayAlpha; }
            set
            {

                if (this._data.backdropOverlayAlpha != value)
                {
                    this._data.backdropOverlayAlpha = (float)Math.Round(value, 1, MidpointRounding.ToEven);
                    FirePropertyChanged("BackdropOverlayAlpha");
                    this.Save();
                }
            }
        }*/

        public bool Enable24hrTime
        {
            get { return _data.enable24hrTime; }
            set
            {
                if (_data.enable24hrTime != value)
                {
                    _data.enable24hrTime = value;
                    FirePropertyChanged("enable24hrTime");
                    Save();
                }
            }
        }

        public bool EnableQuickPlay
        {
            get { return _data.enableQuickPlay; }
            set
            {
                if (_data.enableQuickPlay != value)
                {
                    _data.enableQuickPlay = value;
                    FirePropertyChanged("enableQuickPlay");
                    Save();
                }
            }
        }

        public string GameDetailPosterLayout
        {
            get
            {
                string position = "Prefer Poster";

                if (_data != null)
                    position = _data.GameDetailPosterLayout;

                return position;
            }
            set
            {
                if ((_data != null) && (_data.GameDetailPosterLayout != value))
                {
                    _data.GameDetailPosterLayout = value;
                    Save();
                    FirePropertyChanged("GameDetailPosterLayout");
                }
            }
        }

        public string WatchedColorStyle
        {
            get
            {
                string color = "Green";

                if (_data != null)
                    color = _data.WatchedColorStyle;

                return color;
            }
            set
            {
                if ((_data != null) && (_data.WatchedColorStyle != value))
                {
                    _data.WatchedColorStyle = value;
                    Save();
                    FirePropertyChanged("WatchedColorStyle");
                }
            }
        }
        
        public string CustomDetailPosterLayout
        {
            get
            {
                string position = "Normal";

                if (_data != null)
                    position = _data.CustomDetailPosterLayout;

                return position;
            }
            set
            {
                if ((_data != null) && (_data.CustomDetailPosterLayout != value))
                {
                    _data.CustomDetailPosterLayout = value;
                    Save();
                    FirePropertyChanged("CustomDetailPosterLayout");
                }
            }
        }

        #region Styles

        public Choice InstalledStyles
        {
            get { return CustomStyles.InstalledStyles; }
        }

        public string ThemeStyle
        {
            get { return _data.ThemeStyle; }
            set
            {
                if (_data.ThemeStyle != value)
                {
                    _data.ThemeStyle = value;

                    string sourceFilePath = Path.Combine(CustomStyles.StylesFolderPath, value + ".mcml");

                    //Logger.ReportInfo("Iridium - Setting active style: " + sourceFilePath);

                    File.Copy(sourceFilePath, StyleFilePath, true);

                    Save();
                    FirePropertyChanged("ThemeStyle");
                }
            }
        }

        public Choice GetAvailableStylesChoice
        {
            get
            {
                AvailableStyles.Options = Plugin.AvailableStyles;
                AvailableStyles.Chosen = _data.Style;
                return AvailableStyles;
            }
        }

        public List<string> StyleNames
        {
            get
            {
                return Plugin.AvailableStyles;
            }
        }

        public string UpdateStyles
        {
            set
            {
                if ((string)_data.Style != value)
                {
                    _data.Style = value;
                    AvailableStyles.Chosen = value;
                    Save();
                    FirePropertyChanged("GetAvailableStyles");
                }
            }
        }

        #endregion


        public bool AskToQuit
        {
            get
            {
                return _data.askToQuit;
            }
            set
            {
                if (_data.askToQuit != value)
                {
                    _data.askToQuit = value;
                    Save();
                    FirePropertyChanged("AskToQuit");
                }
            }
        }

        #region Folder Config Options

        private Guid folderId = Guid.Empty;
        private Guid parentFolderId = Guid.Empty;

        public Guid FolderId
        {
            get
            {
                return folderId;
            }
            set
            {
                if (folderId != value)
                {
                    folderId = value;

                    if ((folderId == Guid.Empty) || !LoadFolder())
                    {
                        folderData = null;
                    }
                }
            }
        }

        public Guid ParentFolderId
        {
            get
            {
                return parentFolderId;
            }
            set
            {
                if (parentFolderId != value)
                    parentFolderId = value;
            }
        }

        public string FolderOrientation
        {
            get
            {
                string orientation = "Vertical";

                if (folderData != null)
                    orientation = folderData.Orientation;

                return orientation;
            }
            set
            {
                if ((folderData != null) && (folderData.Orientation != value))
                {
                    folderData.Orientation = value;
                    SaveFolder();
                    FirePropertyChanged("FolderOrientation");
                }
            }
        }

        public Single FolderBackdropOverlayAlpha
        {
            get
            {
                Single alpha =  0.3f;
                if (folderData != null)
                    alpha = folderData.FolderBackdropOverlayAlpha;

                return alpha;
            }
            set
            {

                if ((folderData != null) && (folderData.FolderBackdropOverlayAlpha != value))
                {
                    folderData.FolderBackdropOverlayAlpha = (float)Math.Round(value, 1, MidpointRounding.ToEven);
                    SaveFolder();
                    FirePropertyChanged("FolderBackdropOverlayAlpha");
                }
            }
        }

        public Single FolderDetailsThumbAlpha
        {
            get
            {
                Single alpha = 0.3f;
                if (folderData != null)
                    alpha = folderData.FolderDetailsThumbAlpha;

                return alpha;
            }
            set
            {
                if ((folderData != null) && (folderData.FolderDetailsThumbAlpha != value))
                {
                    folderData.FolderDetailsThumbAlpha = (float)Math.Round(value, 1, MidpointRounding.ToEven);
                    SaveFolder();
                    FirePropertyChanged("FolderDetailsThumbAlpha");
                }
            }
        }

        public string FolderFlatCFPosition
        {
            get
            {
                string position = "Top";

                if (folderData != null)
                    position = folderData.CFPosition;

                return position;
            }
            set
            {
                if ((folderData != null) && (folderData.CFPosition != value))
                {
                    folderData.CFPosition = value;
                    SaveFolder();
                    FirePropertyChanged("FolderFlatCFPosition");
                }
            }
        }

       public bool FolderShowBottomInfoBar
        {
            get
            {
                bool show = false;

                if (folderData != null)
                    show = folderData.ShowBottomInfoBar;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.ShowBottomInfoBar != value))
                {
                    folderData.ShowBottomInfoBar = value;
                    SaveFolder();
                    FirePropertyChanged("FolderShowBottomInfoBar");
                }
            }
        }

       
       public bool FolderShowFullMPAAIcons
        {
            get
            {
                bool show = false;

                if (folderData != null)
                    show = folderData.ShowFullMPAAIcons;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.ShowFullMPAAIcons != value))
                {
                    folderData.ShowFullMPAAIcons = value;
                    SaveFolder();
                    FirePropertyChanged("FolderShowFullMPAAIcons");
                }
            }
        }

        public string FolderWrapEHSList
        {
            get
            {
                string wrap = "When Too Big";

                if (folderData != null)
                    wrap = folderData.WrapEHSList;

                return wrap;
            }
            set
            {
                if ((folderData != null) && (folderData.WrapEHSList != value))
                {
                    folderData.WrapEHSList = value;
                    SaveFolder();
                    FirePropertyChanged("FolderWrapEHSList");
                }
            }
        }

        public string FolderWrapItemList
        {
            get
            {
                string wrap = "When Too Big";

                if (folderData != null)
                    wrap = folderData.WrapItemList;

                return wrap;
            }
            set
            {
                if ((folderData != null) && (folderData.WrapItemList != value))
                {
                    folderData.WrapItemList = value;
                    SaveFolder();
                    FirePropertyChanged("FolderWrapItemList");
                }
            }
        }
        public string ExtraViewsList
        {
            get
            {
                string list = "None";

                if (folderData != null)
                    list = folderData.ExtraViewsList;

                return list;
            }
            set
            {
                if ((folderData != null) && (folderData.ExtraViewsList != value))
                {
                    folderData.ExtraViewsList = value;
                    SaveFolder();
                    FirePropertyChanged("ExtraViewsList");
                }
            }
        }

        public string FolderStarRatingStyle
        {
            get
            {
                string list = "Numeric";

                if (folderData != null)
                    list = folderData.FolderStarRatingStyle;

                return list;
            }
            set
            {
                if ((folderData != null) && (folderData.FolderStarRatingStyle != value))
                {
                    folderData.FolderStarRatingStyle = value;
                    SaveFolder();
                    FirePropertyChanged("FolderStarRatingStyle");
                }
            }
        }

        //Choice for selecting Logos/ClearArt & Thumbs
        public string FolderClearLogosList
        {
            get
            {
                string show = "Logo";

                if (folderData != null)
                    show = folderData.FolderClearLogosList;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.FolderClearLogosList != value))
                {
                    folderData.FolderClearLogosList = value;
                    SaveFolder();
                    FirePropertyChanged("FolderClearLogosList");
                }
            }
        }

        public string TvRalOption
        {
            get
            {
                string show = "NextUp";

                if (folderData != null)
                    show = folderData.TvRalOption;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.TvRalOption != value))
                {
                    folderData.TvRalOption = value;
                    SaveFolder();
                    FirePropertyChanged("TvRalOption");
                }
            }
        }

        public string NonTVRalOption
        {
            get
            {
                string show = "New";

                if (folderData != null)
                    show = folderData.NonTVRalOption;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.NonTVRalOption != value))
                {
                    folderData.NonTVRalOption = value;
                    SaveFolder();
                    FirePropertyChanged("NonTVRalOption");
                }
            }
        }

        public string FolderMainArtList
        {
            get
            {
                string show = "Poster";

                if (folderData != null)
                    show = folderData.FolderMainArtList;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.FolderMainArtList != value))
                {
                    folderData.FolderMainArtList = value;
                    SaveFolder();
                    FirePropertyChanged("FolderClearLogosList");
                }
            }
        }

        public string FolderInfoDisplay
        {
            get
            {
                string display = "Overview";

                if (folderData != null)
                    display = folderData.FolderInfoDisplay;

                return display;
            }
            set
            {
                if ((folderData != null) && (folderData.FolderInfoDisplay != value))
                {
                    folderData.FolderInfoDisplay = value;
                    SaveFolder();
                    FirePropertyChanged("FolderInfoDisplay");
                }
            }
        }

        public bool FolderShowBackdrop
        {
            get
            {
                bool show = true;

                if (folderData != null)
                    show = folderData.ShowBackdrop;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.ShowBackdrop != value))
                {
                    folderData.ShowBackdrop = value;
                    SaveFolder();
                    FirePropertyChanged("FolderShowBackdrop");
                }
            }
        }

        public bool FolderShowNewsScroller
        {
            get
            {
                bool show = true;

                if (folderData != null)
                    show = folderData.FolderShowNewsScroller;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.FolderShowNewsScroller != value))
                {
                    folderData.FolderShowNewsScroller = value;
                    SaveFolder();
                    FirePropertyChanged("FolderShowNewsScroller");
                }
            }
        }

        public bool FolderShowBackdropOverlay
        {
            get
            {
                bool show = false;

                if (folderData != null)
                    show = folderData.ShowBackdropOverlay;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.ShowBackdropOverlay != value))
                {
                    folderData.ShowBackdropOverlay = value;
                    SaveFolder();
                    FirePropertyChanged("FolderShowBackdropOverlay");
                }
            }
        }

        public bool FolderShowBanners
        {
            get
            {
                bool show = true;

                if (folderData != null)
                    show = folderData.ShowBanners;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.ShowBanners != value))
                {
                    folderData.ShowBanners = value;
                    SaveFolder();
                    FirePropertyChanged("FolderShowBanners");
                }
            }
        }

        public bool FolderShowFullRAL
        {
            get
            {
                bool show = true;

                if (folderData != null)
                    show = folderData.ShowFullRAL;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.ShowFullRAL != value))
                {
                    folderData.ShowFullRAL = value;
                    SaveFolder();
                    FirePropertyChanged("FolderShowFullRAL");
                }
            }
        }

        public bool FolderShowRALAlways
        {
            get
            {
                bool show = true;

                if (folderData != null)
                    show = folderData.ShowRALAlways;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.ShowRALAlways != value))
                {
                    folderData.ShowRALAlways = value;
                    SaveFolder();
                    FirePropertyChanged("FolderShowRALAlways");
                }
            }
        }

        public bool FolderShowSelectedInfo
        {
            get
            {
                bool show = true;

                if (folderData != null)
                    show = folderData.ShowSelectedInfo;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.ShowSelectedInfo != value))
                {
                    folderData.ShowSelectedInfo = value;
                    SaveFolder();
                    FirePropertyChanged("FolderShowSelectedInfo");
                }
            }
        }

        public bool FolderEnableWatchedIndicators
        {
            get
            {
                bool enabled = true;

                if (folderData != null)
                    enabled = folderData.EnableWatchedIndicators;

                return enabled;
            }
            set
            {
                if ((folderData != null) && (folderData.EnableWatchedIndicators != value))
                {
                    folderData.EnableWatchedIndicators = value;
                    SaveFolder();
                    FirePropertyChanged("FolderEnableWatchedIndicators");
                }
            }
        }

        public bool FolderEnableNewItemIndicator
        {
            get
            {
                bool enabled = true;

                if (folderData != null)
                    enabled = folderData.EnableNewItemIndicator;

                return enabled;
            }
            set
            {
                if ((folderData != null) && (folderData.EnableNewItemIndicator != value))
                {
                    folderData.EnableNewItemIndicator = value;
                    SaveFolder();
                    FirePropertyChanged("FolderEnableNewItemIndicator");
                }
            }
        }

        public bool FolderShowDetailsQuickList
        {
            get
            {
                bool show = true;

                if (folderData != null)
                    show = folderData.ShowDetailsQuickList;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.ShowDetailsQuickList != value))
                {
                    folderData.ShowDetailsQuickList = value;
                    SaveFolder();
                    FirePropertyChanged("FolderShowDetailsQuickList");
                }
            }
        }

        public bool FolderShowFlatCoverflow
        {
            get
            {
                bool show = true;

                if (folderData != null)
                    show = folderData.ShowFlatCoverflow;

                return show;
            }
            set
            {
                if ((folderData != null) && (folderData.ShowFlatCoverflow != value))
                {
                    folderData.ShowFlatCoverflow = value;
                    SaveFolder();
                    FirePropertyChanged("FolderShowFlatCoverflow");
                }
            }
        }

        #endregion

        #region Save / Load Folder Configuration

        private void SaveFolder()
        {
            if ((folderData != null) && (folderData.FolderId != Guid.Empty.ToString()))
            {
                lock (this)
                    folderData.Save();
            }
        }

        private bool LoadFolder()
        {
            if (folderId != Guid.Empty)
            {
                string folderFilePath = Path.Combine(displayFolderPath, "Iridium_" + folderId + ".xml");

                if (!File.Exists(folderFilePath) && (parentFolderId != Guid.Empty))
                {
                    string parentFolderFilePath = Path.Combine(displayFolderPath, "Iridium_" + parentFolderId + ".xml");

                    if (File.Exists(parentFolderFilePath))
                    {
                        try
                        {
                            File.Copy(parentFolderFilePath, folderFilePath);
                        }
                        catch (Exception)
                        {
                            // NOP
                        }
                    }
                }

                try
                {
                    folderData = FolderConfigData.FromFile(folderId, folderFilePath);

                    if (folderData.WrapItemList == "True")
                    {
                        folderData.WrapItemList = "When Too Big";
                        SaveFolder();
                    }
                    else if (folderData.WrapItemList == "False")
                    {
                        folderData.WrapItemList = "Never";
                        SaveFolder();
                    }

                    return true;
                }
                catch (Exception)
                {
                    if (!Directory.Exists(_configFilePath))
                        Directory.CreateDirectory(_configFolderPath);

                    folderData = new FolderConfigData(folderId, folderFilePath);
                    SaveFolder();
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region ActorsPage
        /*public void OpenActorPage(Item children, Item item)
        {
            FolderModel model = children as FolderModel;
            if (model != null)
            {
                ActorInfo info = new ActorInfo();
                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties.Add("Application", Application.CurrentInstance);
                properties.Add("Item", item);
                properties.Add("Person", info.getPerson(item));
                info.Dispose();
                Application.CurrentInstance.CurrentFolder = model;
                Application.CurrentInstance.OpenMCMLPage(
                    "resx://Iridium/Iridium.Resources/ActorPopup#ActorPopup", properties);
            }
        }*/
        #endregion

    }
}
