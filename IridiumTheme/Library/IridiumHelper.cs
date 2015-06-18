using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using MediaBrowser;
using MediaBrowser.Library;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Extensions;
using MediaBrowser.Library.Localization;
using MediaBrowser.Library.Logging;
using MediaBrowser.Library.Persistance;
using MediaBrowser.Library.Threading;
using MediaBrowser.Model.Querying;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using Application = MediaBrowser.Application;
using Timer = Microsoft.MediaCenter.UI.Timer;

namespace Iridium
{
    public class IridiumHelper : ModelItem
    {

        private readonly Timer OverviewTimer;
        private int NavCount;
        private Item _currentItem;
        public bool _showOverveiew;
        private static Image defaultBackdrop;
        private static Image currentBackdrop;
        public static Guid SeriesId { get; set; }
        public Item DummyItem { get; set; }
       
        private FolderModel _currentTopParent;
        private FolderModel _currentParent;
        private static string currentPage = "Page";
        private Size _bigImageSize = new Size(300,168);
        private Size _normalImageSize = new Size(296,164);

        public static IridiumHelper Instance { get; set; }
        public IridiumHelper()
        {
            OverviewTimer = new Timer();
            setupHelper();

        }

        public IridiumHelper(Item Item)
        {
            OverviewTimer = new Timer();
            CurrentItem = Item;
            setupHelper();
        }


        public Item CurrentItem
        {
            get { return _currentItem; }
            set
            {
                _currentItem = value;
                if (NavCount > 1)
                {
                    if (ShowOverview)
                    {
                        ShowOverview = false;
                    }
                    NavCount = 0;
                }
                NavCount++;
                OverviewTimer.Stop();
                OverviewTimer.Start();
                FirePropertyChanged("EndTime");
                FirePropertyChanged("CurrentItem");
                FirePropertyChanged("GenreString");
                FirePropertyChanged("PercentWatched");
                FireMusicChanges();
                FireGameChanges();
            }
        }

        public FolderModel CurrentFolder
        {
            get { return Application.CurrentInstance.CurrentFolder; }
        }

        public FolderModel SelectedChild
        {
            get { return Application.CurrentInstance.CurrentFolder.SelectedChild as FolderModel; }
        }

        public bool IsTVShowFolder
        {
            get { return SelectedChild != null && SelectedChild.CollectionType == "tvshows"; }
        }

        public bool IsMusicFolder
        {
            get { return SelectedChild != null && SelectedChild.CollectionType == "music"; }
        }

        public bool IsMovieFolder
        {
            get { return SelectedChild != null && SelectedChild.CollectionType == "movie"; }
        }

        public bool IsGameFolder
        {
            get { return SelectedChild != null && SelectedChild.CollectionType == "game"; }
        }

        public string CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; }
        }

        public string EndTime
        {
            get
            {
                string endTime = String.Empty;

                if (!String.IsNullOrEmpty(_currentItem.EndTimeString))
                {
                    endTime = _currentItem.EndTimeString.Replace(LocalizedStrings.Instance.GetString("EndsStr") + " ",
                        "");
                }

                return endTime;
            }
        }

        public string CurrentTime
        {
            get
            {
                DateTime time = DateTime.Now;
                if (Config.Enable24hrTime)
                {
                    return time.ToString("HH:mm");
                }
                return time.ToString("h:mm tt");
            }
        }

        public string CurrentDate
        {
            get
            {
                DateTime date = DateTime.Now;
                if (CurrentFolder.IsRoot)
                {
                    return date.ToLongDateString();
                }
                return null;
            }
        }

        public MyConfig ThemeConfig
        {
            get
            {
                if (Plugin.Config == null)
                    Plugin.Config = new MyConfig();

                return Plugin.Config;
            }
        }

        //private string calculateEndTime()
        //{
        //    string runtime = CurrentItem.RunningTimeString;
        //    if (runtime == "" && CurrentItem.PhysicalParent != null)
        //        runtime = CurrentItem.PhysicalParent.RunningTimeString;
        //    if(runtime != "")
        //    {
        //        runtime = runtime.Replace(" mins", "");
        //        int minutes = Int32.Parse(runtime);
        //        DateTime dt = DateTime.Now;
        //        dt = dt.AddMinutes(minutes);

        //        int Hour = dt.Hour;
        //        if(dt.Hour > 12)
        //        {
        //            Hour = Hour - 12;
        //        }

        //        if (dt.Minute > 9)
        //        {
        //            return Hour.ToString() + ":" + dt.Minute.ToString();
        //        }
        //        else
        //        {
        //            return Hour.ToString() + ":0" + dt.Minute.ToString();
        //        }


        //    }
        //    return "";
        //}

        private static bool navigatingForward = true;

        public bool NavigatingForward
        {
            get { return navigatingForward; }
            set
            {
                navigatingForward = value;
                FirePropertyChanged("NavigatingForward");
            }
        }

        public Image DefaultBackdrop
        {
            get { return defaultBackdrop; }
            set { defaultBackdrop = value; }
        }

        public Image CurrentBackdrop
        {
            get { return currentBackdrop; }
            set { currentBackdrop = value; }
        }

        private bool _isShortlistOpen;

        public bool IsShortlistOpen
        {
            get { return _isShortlistOpen; }
            set
            {
                _isShortlistOpen = value;
                FirePropertyChanged("ShortlistHasFocus");
            }
        }

        private bool isMenuOpen;
        public bool IsMenuOpen
        {
            get { return isMenuOpen; }
            set
            {
                isMenuOpen = value;
                FirePropertyChanged("IsMenuOpen");
            }
        }

        private bool buttonPanelHasFocus;
        public bool ButtonPanelHasFocus
        {
            get
            {
                return buttonPanelHasFocus;
            }
            set
            {
                buttonPanelHasFocus = value;
                base.FirePropertyChanged("ButtonPanelHasFocus");
            }
        }

        private bool _vfHasFocus = true;
        public bool VFHasFocus
        {
            get { return _vfHasFocus; }
            set
            {
                _vfHasFocus = value;
                FirePropertyChanged("VFHasFocus");
            }
        }

        private bool _configHasFocus = false;
        public bool ConfigHasFocus
        {
            get { return _configHasFocus; }
            set
            {
                _configHasFocus = value;
                FirePropertyChanged("ConfigHasFocus");
            }
        }

        private int _headerIndex;
        public int HeaderIndex
        {
            get
            {
                return this._headerIndex;
            }
            set
            {
                if (this._headerIndex != value)
                {
                    this._headerIndex = value;
                    base.FirePropertyChanged("HeaderIndex");
                }
            }
        }

        private int _panelIndex;
        public int PanelIndex
        {
            get
            {
                return this._panelIndex;
            }
            set
            {
                if (this._panelIndex != value)
                {
                    this._panelIndex = value;
                    base.FirePropertyChanged("PanelIndex");
                }
            }
        }

        public MyConfig Config { get; set; }

        public bool ShowOverview
        {
            get { return _showOverveiew; }
            set
            {
                _showOverveiew = value;
                FirePropertyChanged("ShowOverview");
            }
        }

        public string GenreString
        {
            get
            {
                string genrestring = "";
                CurrentItem.Genres.ForEach(delegate(string item) { genrestring = genrestring + item + ", "; });
                if (genrestring.Length > 0)
                {
                    genrestring = genrestring.Substring(0, genrestring.Length - 2);
                }
                return genrestring;
            }
        }

        public string WritersString
        {
            get
            {
                string writersstring = "";
                CurrentItem.Writers.ForEach(delegate(string item) { writersstring = writersstring + item + ", "; });
                if (writersstring.Length > 0)
                {
                    writersstring = writersstring.Substring(0, writersstring.Length - 2);
                }
                return writersstring;
            }
        }

        public Item TempItem { get; set; }
        public float PercentWatched()
        {
            Item item = TempItem;
            return ((item.RunTimeTicks > 0L) ? (item.PlayState.PositionTicks / (float) (item.RunTimeTicks) * 100) : 0f);
        }

        public string PercentWatchedString
        {
            get { return PercentWatched().ToString("n1") + (" %"); }
        }

        public string TimeRemaining()
        {
            Item item = TempItem;
            int tot = item.RunningTime;
            int pos = new TimeSpan(item.PlayState.PositionTicks).Minutes;
            
            int rem = (tot) - (pos);
            
            return rem + ("mins");
            
        }

        public Size GetBigImageSize
        {
            get
            {
                return new Size(BigImageSize.Width * 2, BigImageSize.Height * 2);
            }
        }

        public Size BigImageSize
        {
            get
            {
                return _bigImageSize;
            }
        }

        public Size NormalImageSize
        {
            get
            {
                return _normalImageSize;
            }
        }

        private readonly Inset _steppedInset;
        public Inset LogoInsetCalc
        {
            get
            {
                return new Inset(SteppedInset.Left, SteppedInset.Top, SteppedInset.Right*2, SteppedInset.Bottom);
            }
            
        }

        public Inset SteppedInset
        {
            get { return _steppedInset; }
        }

        
        private bool _ralHasFocus;

        public bool RALHasFocus
        {
            get { return _ralHasFocus; }
            set
            {
                _ralHasFocus = value;
                FirePropertyChanged("RALHasFocus");
            }
        }

        public int SelectedIndex { get; set; }

        public bool IsSongBackdropPlaying
        {
            get
            {
                if (Application.CurrentInstance.IsPlayingVideo)
                {
                    return false;
                }
                return true;
            }
        }

        public bool VideoBackdropApplicable
        {
            get
            {
                var conf = new MyConfig();
                if (!conf.EnableVideoBackdrop)
                {
                    return false;
                }
                Type pc = Application.CurrentInstance.PlaybackController.GetType();
                if (pc != typeof (PlaybackController))
                {
                    return false;
                }
                if (!Application.CurrentInstance.PlaybackController.IsPlaying)
                {
                    return false;
                }
                return true;
            }
        }

        #region New Item Notification

        private string _dateStr;
        private Item _newItem;
        private bool _showNewItemPopout;
        


        public bool ShowNewItemPopout
        {
            get { return _showNewItemPopout; }
            set
            {
                if (_showNewItemPopout != value)
                {
                    _showNewItemPopout = value;
                    FirePropertyChanged("ShowNewItemPopout");
                }
            }
        }

        public Item NewItem
        {
            get { return _newItem; }
            set
            {
                if (_newItem != value)
                {
                    _newItem = value;
                    FirePropertyChanged("NewItem");
                }
            }
        }

        public string Date
        {
            get { return _dateStr; }
            set
            {
                if (_dateStr != value)
                {
                    _dateStr = value;
                    FirePropertyChanged("Date");
                }
            }
        }

        public bool Display24Hr { get; set; }

        #endregion

        #region Prevent Quit from EHS

        public void AskToQuit()
        {
            MediaCenterEnvironment mediaCenterEnvironment = AddInHost.Current.MediaCenterEnvironment;
            const string text = "Are you sure you want to quit MediaBrowser?";
            const string caption = "Quit MediaBrowser";
            if (mediaCenterEnvironment.Dialog(text, caption, DialogButtons.Cancel | DialogButtons.Ok, 0, true) ==
                DialogResult.Ok)
            {
                Application.CurrentInstance.BackOut();
            }
        }

        #endregion

        #region Music Metadata

        public string Duration
        {
            get
            {
                string duration = GetDynamicProperty("Duration");

                if (duration != null)
                {
                    return duration;
                }
                return "";
            }
        }

        public string Album
        {
            get
            {
                string album = GetDynamicProperty("Album");

                if (album != null)
                {
                    return album;
                }
                return "";
            }
        }

        public string Artist
        {
            get
            {
                string artist = GetDynamicProperty("Artist");

                if (artist != null)
                {
                    return artist;
                }
                return "";
            }
        }


        public string ProductionYear
        {
            get
            {
                string productionYear = GetDynamicProperty("ProductionYear");


                if (productionYear != null)
                {
                    return productionYear;
                }
                return "";
            }
        }

        private void FireMusicChanges()
        {
            FirePropertyChanged("Album");
            FirePropertyChanged("Artist");
            FirePropertyChanged("ProductionYear");
            FirePropertyChanged("Duration");
        }


        private string GetDynamicProperty(string PropertyName)
        {
            try
            {
                return CurrentItem.DynamicProperties[PropertyName] as string;
            }
            catch (Exception)
            {
                return "";
            }
        }

        #endregion

        #region Game Metadata

        public string Players
        {
            get
            {
                string players = GetDynamicProperty("Players");

                if (players != null)
                {
                    return players;
                }
                return "";
            }
        }


        public Single TgdbRating
        {
            get
            {
                string rating = GetDynamicProperty("TgdbRating");

                if (!String.IsNullOrEmpty(rating))
                {
                    return Convert.ToSingle(rating);
                }
                return 0;
            }
        }


        public string EsrbRating
        {
            get
            {
                string rating = GetDynamicProperty("EsrbRating");

                if (rating != null)
                {
                    return rating;
                }

                return "";
            }
        }

        public string ConsoleReleaseYear
        {
            get
            {
                int year = GetDynamicPropertyAsInt("ReleaseYear");

                if (year != 0)
                {
                    return year.ToString();
                }

                return "";
            }
        }

        public string Company
        {
            get
            {
                string company = GetDynamicProperty("Manufacturer");

                if (company != null)
                {
                    return company;
                }

                return "";
            }
        }

        private void FireGameChanges()
        {
            FirePropertyChanged("Players");
            FirePropertyChanged("TgdbRating");
            FirePropertyChanged("ProductionYear");
            FirePropertyChanged("EsrbRating");
            FirePropertyChanged("Company");
            FirePropertyChanged("ConsoleReleaseYear");
        }

        private int GetDynamicPropertyAsInt(string PropertyName)
        {
            try
            {
                return Convert.ToInt32(CurrentItem.DynamicProperties[PropertyName]);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion

        public void setupHelper()
        {
            SelectedIndex = 0;
            Config = new MyConfig();
            ShowOverview = false;
            OverviewTimer.AutoRepeat = false;
            OverviewTimer.Interval = 1000;
            OverviewTimer.Tick += delegate
            {
                if ((((this.CurrentItem.Overview != "") && !this.ShowOverview) &&
                     ((this.CurrentItem.ItemTypeString != "Season") &&
                      (this.CurrentItem.ItemTypeString != "Album"))) &&
                    (((this.CurrentItem.ItemTypeString != "ArtistAlbum") &&
                      (this.CurrentItem.ItemTypeString != "Folder")) &&
                     (this.CurrentItem.ItemTypeString != "MusicFolder")))

                    ShowOverview = true;
                NavCount = 0;
            };
        }

        //Item curritem = Application.CurrentInstance.CurrentItem
        public bool getProperty(string propertyname)
        {
            return ShowOverview;
        }

        private float CalculatePercentWatched()
        {
            float num = 0f;
            if (!String.IsNullOrEmpty(CurrentItem.RunningTimeString))
            {
                int num2 =
                    Int32.Parse(CurrentItem.RunningTimeString.Substring(0, CurrentItem.RunningTimeString.IndexOf(' ')));
                var totalMinutes = (int) CurrentItem.WatchedTime.TotalMinutes;
                if ((num2 > 0) && (totalMinutes > 0))
                {
                    num = totalMinutes/((float) num2);
                }
            }
            Logger.ReportInfo("PercentageWatched = {0}", num.ToString(CultureInfo.CurrentCulture));
            return num;
        }

        public Rotation getRotation(int Index)
        {
            int indexDifference = SelectedIndex - Index;
            var rot = new Rotation();
            if (indexDifference == 0)
            {
                rot.AngleDegrees = 0;
                return rot;
            }
            if (indexDifference <= 0)
            {
                if (Math.Abs(indexDifference) == 1)
                {
                    rot.AngleDegrees = 15;
                }
                else if (Math.Abs(indexDifference) == 2)
                {
                    rot.AngleDegrees = 30;
                }
                else if (Math.Abs(indexDifference) == 3)
                {
                    rot.AngleDegrees = 45;
                }
                else
                {
                    rot.AngleDegrees = 55;
                }
            }
            else
            {
                rot.AngleDegrees = 0;
            }
            return rot;
        }

        public int getIndexdif(int Index)
        {
            return SelectedIndex - Index;
        }

        public Vector3 getScale(int Index)
        {
            int indexDifference = SelectedIndex - Index;
            var scale = new Vector3();
            if (indexDifference == 0)
            {
                scale.X = 1.6f;
                scale.Y = 1.6f;
                scale.Z = 1.6f;
                return scale;
            }
            if (indexDifference <= 0)
            {
                if (Math.Abs(indexDifference) == 1)
                {
                    scale.X = 1.4f;
                    scale.Y = 1.4f;
                    scale.Z = 1.4f;
                }
                else if (Math.Abs(indexDifference) == 2)
                {
                    scale.X = 1.25f;
                    scale.Y = 1.25f;
                    scale.Z = 1.25f;
                }
                else if (Math.Abs(indexDifference) == 3)
                {
                    scale.X = 1.1f;
                    scale.Y = 1.1f;
                    scale.Z = 1.1f;
                }
                else
                {
                    scale.X = 1f;
                    scale.Y = 1f;
                    scale.Z = 1f;
                }
            }
            else
            {
                scale.X = 1.4f;
                scale.Y = 1.4f;
                scale.Z = 1.4f;
            }
            return scale;
        }

        public float getPosition(int Index)
        {
            int indexDifference = SelectedIndex - Index;
            if (Math.Abs(indexDifference) > 1)
            {
                return 60*indexDifference;
            }
            return 0;
        }


        public Inset getMargin(int Index)
        {
            int indexDifference = SelectedIndex - Index;

            indexDifference = Math.Abs(indexDifference);
            if (indexDifference > 1)
            {
                return new Inset(-15, 0, -15, 0);
            }
            return new Inset(0, 0, 0, 0);
        }


        #region FolderConfig Helper

        public int Negate(int number)
        {
            return (-number);
        }

        public Guid GetFolderPrefsId(FolderModel folder)
        {
            Guid id = Guid.Empty;

            if (folder != null)
            {
                id = folder.Id;

                if (MediaBrowser.Config.Instance.EnableSyncViews)
                {
                    if (folder.BaseItem is Folder && folder.BaseItem.GetType() != typeof (Folder))
                    {
                        id = folder.BaseItem.GetType().FullName.GetMD5();
                    }
                }

            }

            return (id);
        }

        public FolderModel CurrentParent
        {
            get { return (_currentParent); }
            set
            {
                _currentParent = value;
                _currentTopParent = null;

                if (_currentParent != null)
                {
                    FolderModel curParent = _currentParent;

                    while (true)
                    {
                        if ((curParent.PhysicalParent == null) || curParent.PhysicalParent.IsRoot)
                        {
                            _currentTopParent = curParent;
                            break;
                        }
                        curParent = curParent.PhysicalParent;
                    }
                }

                FirePropertyChanged("CurrentTopParent");
            }
        }

        public FolderModel CurrentTopParent
        {
            get { return _currentTopParent; }
        }

        public void LoadDisplayPrefs(FolderModel folder)
        {
            if (folder != null)
            {
                // Referencing folder.DisplayPrefs causes it to be loaded if
                // it doesn't already exist
                Guid id = folder.DisplayPrefs.Id;
            }
        }

        #endregion

        #region MediaInfo

        public string Resolution
        {
            get { return CurrentItem.MediaInfo.Width + "x" + CurrentItem.MediaInfo.Height; }
        }

        public string AudioBitRate
        {
            get { return (CurrentItem.MediaInfo.AudioBitRate/1000) + "Kbps"; }
        }

        public string VideoBitRate
        {
            get { return (CurrentItem.MediaInfo.VideoBitRate/1000) + "Kbps"; }
        }

        public string VideoCodec
        {
            get { return CurrentItem.MediaInfo.VideoCodec; }
        }

        public string AudioFormat
        {
            get { return CurrentItem.MediaInfo.AudioFormat; }
        }

        public string AudioStreamCount
        {
            get { return CurrentItem.MediaInfo.AudioStreamCount.ToString(); }
        }

        public string AudioChannelCount
        {
            get { return CurrentItem.MediaInfo.AudioChannelCount; }
        }

        public string VideoFPS
        {
            get { return CurrentItem.MediaInfo.VideoFPS; }
        }

        #endregion

        #region Custom Views

        public string LayoutSeries { get { return "resx://Iridium/Iridium.Resources/TVShowsLayout#TVShowsLayout"; } }
        public string LayoutRoot
        {
            get { return "resx://Iridium/Iridium.Resources/LayoutRoot#LayoutRoot"; }
        }

        /*public string LayoutCoverFlow2
        {
            get { return "resx://Iridium/Iridium.Resources/LayoutCV2#IridiumLayoutCV2"; }
        }

        public string LayoutDetailsQuickList
        {
            get { return "resx://Iridium/Iridium.Resources/LayoutDetailsQuickList#IridiumLayoutDetailsQuickList"; }
        }

        public string LayoutBannerView
        {
            get { return "resx://Iridium/Iridium.Resources/LayoutBannerView#IridiumLayoutBannerView"; }
        }*/

        public string LayoutCoverflow
        {
            get { return "resx://Iridium/Iridium.Resources/LayoutCoverflow#IridiumLayoutCoverflow"; }
        }

        public string LayoutDetails
        {
            get { return "resx://Iridium/Iridium.Resources/LayoutDetails#IridiumLayoutDetails"; }
        }

        public string LayoutPoster
        {
            get { return "resx://Iridium/Iridium.Resources/LayoutPoster#IridiumLayoutPoster"; }
        }

        public string LayoutThumb
        {
            get { return "resx://Iridium/Iridium.Resources/LayoutThumb#IridiumLayoutThumb"; }
        }

        public string LayoutThumbstrip
        {
            get { return "resx://Iridium/Iridium.Resources/LayoutThumbStrip#IridiumLayoutThumbStrip"; }
        }

        #endregion


        public Item GetQuickListItem(FolderModel folder, int index)
        {
            Item item = null;

            if ((folder != null) && (index >= 0) && (index < folder.QuickListItems.Count))
                item = folder.QuickListItems[index];

            return (item);
        }


        public ArrayListDataSet GetActorLocalCredits(Item item)
        {
            ArrayListDataSet itemSet = new ArrayListDataSet();
            ActorItemWrapper aiw = GetAPIItems.GetPersonDtoStream(item).Item;

            Item credItem = aiw.Item;
            itemSet.Add(credItem);
            return itemSet;
        }

        #region APICalls for MCML

        public int ResumeSetCount
        {
            get { return GetAPIItems.ResumeSet.Count; }
        }

        public int SpotlightCount
        {
            get { return GetAPIItems.SpotlightAddedItemSet.Count; }
        }

        public int GenresSetCount
        {
            get { return GetAPIItems.GenresSet.Count; }
        }
        public int AcclaimedMoviesSetCount
        {
            get { return GetAPIItems.AcclaimedMoviesSet.Count; }
        }
        public int RecentlyAddedSetCount
        {
            get { return GetAPIItems.FirstAddedItem.Count; }
        }
        public int RemainingRecentlyAddedSetCount
        {
            get { return GetAPIItems.RemainingAddedItems.Count; }
        }
        public int RecentlyWatchedSetCount
        {
            get { return GetAPIItems.RecentlyWatchedSet.Count; }
        }
        public int RecentlyUnwatchedSetCount
        {
            get { return GetAPIItems.RecentlyUnwatchedSet.Count; }
        }
        public int NextUpSetCount
        {
            get { return GetAPIItems.NextUpSet.Count; }
        }
        public int UpcomingTVSetCount
        {
            get { return GetAPIItems.UpComingTVSet.Count; }
        }
        public int RecommendedSetCount
        {
            get { return GetAPIItems.RecommendedSet.Count; }
        }

        //SERIES NEXT UP
        public ArrayListDataSet GetSeriesNextUp()
        {
            return GetAPIItems.GetSeriesNextUp();
        }

        public ArrayListDataSet GetRALItems()
        {
            return GetAPIItems.GetAppRAL();
        }
        //RECENTLY ADDED LISTS
        public ArrayListDataSet ShowFirstNewItem()
        {
            return GetAPIItems.GetRecentlyAddedSet();
        }
        public ArrayListDataSet ShowSecondNewItem()
        {
            return GetAPIItems.GetSecondAddedSet();
        }
        public ArrayListDataSet ShowThirdNewItem()
        {
            return GetAPIItems.GetThirdAddedSet();
        }
        public ArrayListDataSet ShowRemainingNewItems()
        {
            return GetAPIItems.GetRemainingRecentlyAddedSet();
        }

        public List<Item> QuickListItems { get; set; }

        public ArrayListDataSet FirstNewItem
        {
            get { return GetAPIItems.FirstAddedItem; }
        }
        public ArrayListDataSet SecondNewItem
        {
            get { return GetAPIItems.SecondAddedSet; }
        }
        public ArrayListDataSet ThirdNewItem
        {
            get { return GetAPIItems.ThirdAddedSet; }
        }
        public ArrayListDataSet RemainingNewItems
        {
            get { return GetAPIItems.RemainingAddedItems; }
        }

        // WATCHED LISTS
        public ArrayListDataSet ShowFirstWatchedItem()
        {
            return GetAPIItems.GetFirstWatchedSet();
        }
        public ArrayListDataSet ShowSecondWatchedItem()
        {
            return GetAPIItems.GetSecondWatchedSet();
        }
        public ArrayListDataSet ShowThirdWatchedItem()
        {
            return GetAPIItems.GetThirdWatchedSet();
        }
        public ArrayListDataSet ShowRemainingWatchedItems()
        {
            return GetAPIItems.GetRemainingWatchedSet();
        }

        public ArrayListDataSet FirstWatchedItem
        {
            get { return GetAPIItems.RecentlyWatchedSet; }
        }
        public ArrayListDataSet SecondWatchedItem
        {
            get { return GetAPIItems.SecondWatchedSet; }
        }
        public ArrayListDataSet ThirdWatchedItem
        {
            get { return GetAPIItems.ThirdWatchedSet; }
        }
        public ArrayListDataSet RemainingWatchedItems
        {
            get { return GetAPIItems.RemainingWatchedSet; }
        }

        // UNWATCHED LISTS
        public ArrayListDataSet ShowFirstUnwatchedItem()
        {
            return GetAPIItems.GetFirstUnwatchedSet();
        }
        public ArrayListDataSet ShowSecondUnwatchedItem()
        {
            return GetAPIItems.GetSecondUnwatchedSet();
        }
        public ArrayListDataSet ShowThirdUnwatchedItem()
        {
            return GetAPIItems.GetThirdUnwatchedSet();
        }
        public ArrayListDataSet ShowRemainingUnwatchedItems()
        {
            return GetAPIItems.GetRemainingUnwatchedSet();
        }

        public ArrayListDataSet FirstUnwatchedItem
        {
            get { return GetAPIItems.RecentlyUnwatchedSet; }
        }
        public ArrayListDataSet SecondUnwatchedItem
        {
            get { return GetAPIItems.SecondUnwatchedSet; }
        }
        public ArrayListDataSet ThirdUnwatchedItem
        {
            get { return GetAPIItems.ThirdUnwatchedSet; }
        }
        public ArrayListDataSet RemainingUnwatchedItems
        {
            get { return GetAPIItems.RemainingUnwatchedSet; }
        }

        
        //TV LISTS
        public ArrayListDataSet GetNextUpEpisodes()
        {
            return GetAPIItems.GetNextUpSet();
        }
        public ArrayListDataSet GetUpcomingTV()
        {
            return GetAPIItems.GetUpcomingItemsSet();
        }

        //REMAINING LISTS
        public ArrayListDataSet GetResumeItems
        {
            get { return GetAPIItems.GetResumeSet(); }
        }

        public ArrayListDataSet GetAcclaimedMovies()
        {
            return GetAPIItems.GetAcclaimedSet();
        }

        public ArrayListDataSet ShuffleAcclaimedMovies()
        {
            return ShuffleList(GetAPIItems.AcclaimedMoviesSet);
        }

        public string RecommendedReason()
        {
            return GetAPIItems.BecauseYouWatchedName;
        }
        public ArrayListDataSet GetRecommendedSet()
        {
            return GetAPIItems.GetRecommendedSet();
        }
        

        

        public ArrayListDataSet GetSimilar()
        {
            return GetAPIItems.GetSimilarMoviesSet();
        }
        private List<BaseItem> genresList;

        public void GetGenres()
        {
            GetAPIItems.GetGenresSet();
            if (GetAPIItems.GenresSet != null)
            {
                ShowGenres();
            }
            else Logger.ReportInfo("************** IRIDIUM NO GENRES FOUND");
        }

        public void ShowGenres()
        {

            foreach (var genre in GetAPIItems.GenresSet)
            {
                if (genre == null)
                {
                    return;
                }
                genresList.Add(genre);
                Microsoft.MediaCenter.UI.Application.DeferredInvoke(
                _ => Application.CurrentInstance.Navigate(ItemFactory.Instance.Create(genresList.First())));
            }
            
        }

        

        public ArrayListDataSet GetNewsItems()
        {
            return GetAPIItems.NewsItemsList();
        }

        //Spotlight
        public ArrayListDataSet GetSpotlightAddedItem()
        {
            return GetAPIItems.GetSpotlightAddedItem();
        }

        private static ArrayListDataSet ShuffleList(ArrayListDataSet arrList)
        {
            ArrayListDataSet randomList = new ArrayListDataSet();

            Random r = new Random();
            for (int cnt = 0; cnt < arrList.Count; cnt++)
            {
                object tmp = arrList[cnt];
                int idx = r.Next(arrList.Count - cnt) + cnt;
                arrList[cnt] = arrList[idx];
                arrList[idx] = tmp;
            }

            return arrList;
        }

        #endregion

        public string PluginUpdatesString()
        {
            string pluginUpdateString = String.Empty;
            if (Application.CurrentInstance.PluginUpdatesAvailable)
            {
                pluginUpdateString = "Restart to Update Plugins";
            }
            if (!Application.CurrentInstance.PluginUpdatesAvailable)
            {
                pluginUpdateString = "";
            }
            return pluginUpdateString;
        }

        public void NavigateToGenre(string itemType)
        {
            switch (Application.CurrentInstance.CurrentItem.BaseItem.GetType().Name)
            {
                case "Series":
                case "Season":
                case "Episode":
                    itemType = "Series";
                    break;

                case "MusicAlbum":
                case "MusicArtist":
                case "MusicGenre":
                case "Song":
                    itemType = "MusicAlbum";
                    break;

                case "Game":
                    itemType = "Game";
                    break;
            }

            Async.Queue("Genre navigation", () =>
            {
                
                ItemQuery query = new ItemQuery
                {
                    UserId = Kernel.CurrentUser.Id.ToString(),
                    Fields = MB3ApiRepository.StandardFields,
                    ParentId = Application.CurrentInstance.CurrentItem.Id.ToString(),
                    IncludeItemTypes = new[] { itemType },
                    Recursive = true
                };
                var index = new SearchResultFolder(Kernel.Instance.MB3ApiRepository.RetrieveItems(query).ToList()) ;
                
                Microsoft.MediaCenter.UI.Application.DeferredInvoke(_ => Application.CurrentInstance.Navigate(ItemFactory.Instance.Create(index)));
            });
        }

        private Folder GetStartingFolder(BaseItem item)
        {
            var currentIndex = item as Index;
            return currentIndex ?? (Folder)RootFolder;
        }

        public AggregateFolder RootFolder
        {
            get
            {
                return Kernel.Instance.RootFolder;
            }
        }

        public string PluginStatus()
        {
            Thread.Sleep(7500);
            return Registration.Status;
        }

        #region Actor Bio Page and Collection Scroller

        public static ArrayListDataSet ActorCollection = new ArrayListDataSet();
        private PluginStatus _pluginStatus;


        public ActorItemWrapper GetActor(Item item, int index)
        {
            ActorItemWrapper wrapper = null;
            if (((item != null) && (index >= 0)) && (index < item.Actors.Count))
            {
                wrapper = item.Actors[index];
            }
            return wrapper;
        }

        public void OpenActorPage(Item item)
        {
            ActorInfo pInfo = GetAPIItems.GetPersonDtoStream(item);
            var properties = new Dictionary<string, object>();
            properties.Add("Application", Application.CurrentInstance);
            properties.Add("Item", item);
            properties.Add("Person", pInfo);
            Application.CurrentInstance.OpenMCMLPage("resx://Iridium/Iridium.Resources/ActorPopup#ActorPopup",
                properties);
        }

        public void OpenTVShowsView(Item item)
        {
            var properties = new Dictionary<string, object>();
            properties.Add("Application", Application.CurrentInstance);
            properties.Add("Folder", item.PhysicalParent );
            properties.Add("ThemeHelper", this);
            Application.CurrentInstance.OpenMCMLPage("resx://Iridium/Iridium.Resources/TVShowsLayout#TVShowsLayout",
                properties);
        }

        public ArrayListDataSet GetActorCollection()
        {
            return ActorCollection;
        }


        public void NavigateToActorCollection(Item item)
        {
            ActorCollection.Clear();
            if (item.BaseItem is Person)
            {
                NavigateToActor(item);
            }
            else
            {
                Logger.ReportInfo("**Iridium** - Unable to Navigate to Actor Collection");
            }
        }

        private void NavigateToActor(Item item)
        {
            Person person = item.BaseItem as Person;
            if (person != null)
            {
                GetCollectionItems(person.Name, new[] {"Actor"}, person.Id.ToString());
                Logger.ReportInfo("*******************    NAVIGATE TO ACTOR NAME ================= {0}", person.Name);
                Logger.ReportInfo("*******************    NAVIGATE TO ACTOR ID ================= {0}", person.Id);
            }
        }

        private static void GetCollectionItems(string name, string[] personTypes, string id)
        {
            Async.Queue("Person navigation", () =>
            {
                ItemQuery query = new ItemQuery 
                {
                    UserId = Kernel.CurrentUser.Id.ToString(),
                    Fields = MB3ApiRepository.StandardFields,
                    PersonIds = new [] {id},
                    PersonTypes = personTypes,
                    Recursive = true
                };
                Logger.ReportInfo("PERSONIDS =======------/////////  ========== {0}", query.PersonIds.ToString());
                Person person = Kernel.Instance.MB3ApiRepository.RetrievePerson(name) ?? new Person();
                var index = new SearchResultFolder(Kernel.Instance.MB3ApiRepository.RetrieveItems(query).ToList())
                {
                    Name = person.Name,
                    Overview = person.Overview
                    
                };
                foreach (BaseItem collectionItem in index.Children)
                {
                    if (collectionItem == null)
                    {
                        return;
                    }
                    Item item = GetActorCollectionItem(collectionItem);
                    //Logger.ReportInfo("*************ACTOR COLLECTION ************** ITEM ADDED = {0}", collectionItem.Name);
                    ActorCollection.Add(item);
                }

                //Microsoft.MediaCenter.UI.Application.DeferredInvoke(_ => Navigate(ItemFactory.Instance.Create(index)));
            });
        }
        #endregion

        private static Item GetActorCollectionItem(BaseItem collectionItem)
        {
            BaseItem baseItem = Kernel.Instance.MB3ApiRepository.RetrieveItem(new Guid(collectionItem.Id.ToString()));
            if (baseItem == null)
            {
                return null;
            }
            Item item = ItemFactory.Instance.Create(baseItem);
            return item;
        }

        public void AddWeatherMenu()
        {
            var kernel = new Kernel();
            Guid weatherFolderGuid = new Guid("F4F97F4C-7512-41A7-B50E-AE08740C158E");
            //Create Weather collection
            BaseItem weatherFolder = new BaseItem()
            {

                Id = weatherFolderGuid,
                Name = "Weather"
            };
            RootFolder.AddVirtualChild(weatherFolder);
        }

        public void OpenNewsPage()
        {
            ArrayListDataSet news = GetAPIItems.NewsItemsList();
            var properties = new Dictionary<string, object>
            {
                {"Application", Application.CurrentInstance},
                {"NewsItem", news}
            };
            Application.CurrentInstance.OpenMCMLPage("resx://Iridium/Iridium.Resources/NewsScroller#NewsScroller", properties);
        }
    }
}
