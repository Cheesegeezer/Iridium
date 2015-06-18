using System;
using MediaBrowser.Library.Persistance;

namespace Iridium
{
    [Serializable]
    internal class FolderConfigData
    {
        #region constructor
        public FolderConfigData()
        {
        }

        public FolderConfigData(Guid folderId)
        {
            FolderId = folderId.ToString();
        }

        public FolderConfigData(Guid folderId, string file)
        {
            FolderId = folderId.ToString();
            this.file = file;
            settings = XmlSettings<FolderConfigData>.Bind(this, file);
        }
        #endregion

        [SkipField]
        public string FolderId = Guid.Empty.ToString();

        public string Orientation = "Horizontal";
        public string CFPosition = "Top";
        public string WrapEHSList = "Never";
        public string WrapItemList = "Never";
        public string FolderInfoDisplay = "Overview";
        public string FolderClearLogosList = "Logo";
        public string FolderMainArtList = "Poster";
        public bool ShowBanners = false;
        public bool ShowBackdrop = true;
        public bool ShowBackdropOverlay = true;
        public bool FolderShowNewsScroller = true;
        public bool ShowDetailsQuickList = false;
        public bool ShowFlatCoverflow = false;
        public bool ShowFullRAL = true;
        public bool ShowRALAlways = false;
        public bool ShowSelectedInfo = true;
        public bool ShowLabels = false;
        public bool EnableWatchedIndicators = true;
        public bool EnableNewItemIndicator = true;
        public bool ShowClearLogos = true;
        public string ExtraViewsList = "None";
        public bool ShowBottomInfoBar = true;
        public bool ShowFullMPAAIcons = false;
        public string TvRalOption = "NextUp";
        public string NonTVRalOption = "New";
        public string FolderStarRatingStyle = "Numeric";
        public Single FolderBackdropOverlayAlpha = 1.0f;
        public Single FolderDetailsThumbAlpha = 1.0f;
        

        #region Load / Save Data
        public static FolderConfigData FromFile(Guid folderId, string file)
        {
            return new FolderConfigData(folderId, file);
        }

        public void Save()
        {
            settings.Write();
        }

        [SkipField]
        string file;

        [SkipField]
        XmlSettings<FolderConfigData> settings;



        #endregion
    }

    [Serializable]
    public class ConfigData
    {
        [SkipField]
        private readonly XmlSettings<ConfigData> IridiumSettings;
        public bool enableVideoBackdrop;
        public Single BackdropTransitionTime;
        //public Single backdropOverlayAlpha;
        private string file;
        public bool EnableNPV;
        public object Style;
        public bool askToQuit;
        public bool enable24hrTime;
        public bool enableQuickPlay;
        public string ThemeStyle = "Shaeff";
        public string WatchedColorStyle = "Green";
        public string GameDetailPosterLayout = "Prefer Poster";
        
        public string CustomDetailPosterLayout = "Normal";
        public bool UseTVShowView;

        public ConfigData()
        {

            askToQuit = false;
            EnableNPV = false;
            enableVideoBackdrop = true;
            UseTVShowView = false;
            BackdropTransitionTime = 0.5f;
            //backdropOverlayAlpha = 0.3f;
            enable24hrTime = true;
            enableQuickPlay = true;
            ThemeStyle = "Shaeff";
            GameDetailPosterLayout = "Prefer Poster";
            CustomDetailPosterLayout = "Normal";
            WatchedColorStyle = "Green";
        }

        public ConfigData(string file)
        {
            askToQuit = false;
            EnableNPV = true;
            enableVideoBackdrop = true;
            UseTVShowView = false;
            BackdropTransitionTime = 0.5f;
            //backdropOverlayAlpha = 0.3f;
            enable24hrTime = true;
            enableQuickPlay = true;
            ThemeStyle = "Shaeff";
            GameDetailPosterLayout = "Prefer Poster";
            CustomDetailPosterLayout = "Normal";
            WatchedColorStyle = "Green";
            this.file = file;
            IridiumSettings = XmlSettings<ConfigData>.Bind(this, file);
        }

        public static ConfigData FromFile(string file)
        {
            return new ConfigData(file);
        }

        public void Save()
        {
            IridiumSettings.Write();
        }
    }
}
