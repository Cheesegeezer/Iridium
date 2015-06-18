using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Iridium.APICalls;
using MediaBrowser.Library;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using MediaBrowser.Library.Util;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.News;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Querying;
using Microsoft.MediaCenter.UI;
using Application = MediaBrowser.Application;

namespace Iridium
{
    public class GetAPIItems : ModelItem
    {
        //Instantiate the APIQueries Class
        private static readonly APIQueries APIQuery = new APIQueries();
        
        public static ArrayListDataSet ResumeSet = new ArrayListDataSet();
        public static List<BaseItem> GenresSet = new List<BaseItem>();

        public static ArrayListDataSet NextUpSet = new ArrayListDataSet();
        public static ArrayListDataSet SeriesNextUp = new ArrayListDataSet();
        public static ArrayListDataSet UpComingTVSet = new ArrayListDataSet();
        public static ArrayListDataSet AcclaimedMoviesSet = new ArrayListDataSet();

        public static ArrayListDataSet RecommendedSet = new ArrayListDataSet();
        public static string BecauseYouWatchedName { get; set; }

        public static ArrayListDataSet SimilarMoviesSet = new ArrayListDataSet();

        public static ArrayListDataSet FirstAddedItem = new ArrayListDataSet();
        public static ArrayListDataSet RemainingAddedItems = new ArrayListDataSet();
        public static ArrayListDataSet SecondAddedSet = new ArrayListDataSet();
        public static ArrayListDataSet ThirdAddedSet = new ArrayListDataSet();

        public static ArrayListDataSet RecentlyWatchedSet = new ArrayListDataSet();
        public static ArrayListDataSet RemainingWatchedSet = new ArrayListDataSet();
        public static ArrayListDataSet SecondWatchedSet = new ArrayListDataSet();
        public static ArrayListDataSet ThirdWatchedSet = new ArrayListDataSet();

        public static ArrayListDataSet RecentlyUnwatchedSet = new ArrayListDataSet();
        public static ArrayListDataSet RemainingUnwatchedSet = new ArrayListDataSet();
        public static ArrayListDataSet SecondUnwatchedSet = new ArrayListDataSet();
        public static ArrayListDataSet ThirdUnwatchedSet = new ArrayListDataSet();

        public static ArrayListDataSet GenreSLSet = new ArrayListDataSet();
        public static ArrayListDataSet SpotlightAddedItemSet = new ArrayListDataSet();
        public static ArrayListDataSet YearAddedSet = new ArrayListDataSet();

        internal static ArrayListDataSet GetNextUpSet()
        {
            NextUpSet.Clear();
            //GUID taken from MBC Kernel
            Guid id = Kernel.CurrentUser.Id;

            //Enumerate thru the list Items in the API call(add the UserId to allow for custom filtering)
            //
            foreach (BaseItemDto dto in APIQuery.NextUpAPIQuery(id).Items)
            {
                Item item = GetGenericTVItem(dto);
                NextUpSet.Add(item);
            }
            return NextUpSet;
        }

        internal static ArrayListDataSet GetSeriesNextUp()
        {
            SeriesNextUp.Clear();
            //GUID taken from MBC Kernel
            Guid id = Kernel.CurrentUser.Id;
            Guid seriesId = IridiumHelper.SeriesId;
            

            //Enumerate thru the list Items in the API call(add the UserId to allow for custom filtering)
            //
            foreach (BaseItemDto dto in APIQuery.SeriesNextUpAPIQuery(id,seriesId).Items)
            {
                Item item = GetGenericTVItem(dto);
                SeriesNextUp.Add(item);
            }
            Logger.ReportInfo("=========== IRIDIUM SERIES ID = {0}", seriesId);
            return SeriesNextUp;
        }

        internal static ArrayListDataSet GetUpcomingItemsSet()
        {
            UpComingTVSet.Clear();
            Guid id = Kernel.CurrentUser.Id;

            string str = string.Empty;
            foreach (BaseItemDto dto in APIQuery.UpComingAPIQuery(id).Items)
            {
                Item item = GetGenericTVItem(dto);
                UpComingTVSet.Add(item);

            }
            return UpComingTVSet;
        }

        private static Item GetGenericTVItem(BaseItemDto dto)
        {
            //Retrieves the item based on the items guid
            BaseItem baseItem = Kernel.Instance.MB3ApiRepository.RetrieveItem(new Guid(dto.Id));
            //If the call to the api returns empty, catch it here.
            if (baseItem == null)
            {
                return null;
            }
            //Ensure that we only return episodes
            //Lets tell the Kernel what we want to return and create that item.
            Item episodeItem = ItemFactory.Instance.Create(baseItem);
            if (episodeItem.BaseItem is Episode)
            {
                TVHelper.CreateEpisodeParents(episodeItem);
            }
            return episodeItem;
        }

        internal static ArrayListDataSet GetAcclaimedSet()
        {
            AcclaimedMoviesSet.Clear();
            try
            {
                //limit is set to 10
                //GUID taken from MBC Kernel
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                


                //Enumerate thru the list Items in the API call(add the UserId to allow for custom filtering)
                //
                foreach (BaseItemDto dto in APIQuery.AcclaimedAPIQuery(id, folderId).Items)
                {
                    Item item = GetGenericItem(dto);
                    AcclaimedMoviesSet.Add(item);
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Critically Acclaimed Movies", fuckinShite.ToString());
            }
            return AcclaimedMoviesSet;
        }

        internal static List<BaseItem> GetGenresSet()
        {
            try
            {
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                foreach (BaseItemDto dto in APIQuery.GenresAPIQuery(id, folderId).Items)
                {
                    BaseItem item = GetGenreItem(dto);
                    GenresSet.Add(item);
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Genres /n {0}", fuckinShite);
            }
            return GenresSet;
        }
        private static BaseItem GetGenreItem(BaseItemDto dto)
        {
            //Retrieves the item based on the items guid
            BaseItem baseItem = Kernel.Instance.MB3ApiRepository.RetrieveItem(new Guid(dto.Id));
            //If the call to the api returns empty, catch it here.

            if (baseItem == null)
            {
                return null;
            }
            return baseItem;

        }

        internal static ArrayListDataSet GetSimilarMoviesSet()
        {
            SimilarMoviesSet.Clear();
            try
            {
                Guid id = Kernel.CurrentUser.Id;
                Item currItem = Application.CurrentInstance.CurrentItem;
                if (currItem.ItemTypeString != "Episode")
                {
                    foreach (BaseItemDto dto in APIQuery.SimilarAPIQuery(id, currItem.Id).Items)
                    {
                        Item item = GetGenericItem(dto);
                        SimilarMoviesSet.Add(item);
                    }
                }
                else if (currItem.ItemTypeString == "Episode")
                {
                    foreach (BaseItemDto dto in APIQuery.SimilarTVAPIQuery(id, currItem.PhysicalParent.PhysicalParent.Id).Items)
                    {
                        Item item = GetGenericItem(dto);
                        SimilarMoviesSet.Add(item);
                    }
                }
                else return null;

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error {0}", fuckinShite.ToString());
            }
            return SimilarMoviesSet;
        }

        internal static ArrayListDataSet GetResumeSet()
        {
            try
            {
                ResumeSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                foreach (BaseItemDto dto in APIQuery.GetResumableItems(id, folderId).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        ResumeSet.Add(item);
                    }
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Resumable Items{0}", fuckinShite.ToString());
            }
            return ResumeSet;
        }

        internal static List<Item> GetRecentItems()
        {
            return new List<Item>(Application.CurrentInstance.CurrentFolderModel.QuickListItems);
        }

        internal static ArrayListDataSet GetAppRAL()
        {
            try
            {
                FirstAddedItem.Clear();
                SecondAddedSet.Clear();
                ThirdAddedSet.Clear();
                RemainingAddedItems.Clear();
                List<Item> ralList = GetRecentItems();
                Logger.ReportInfo("");

                if (ralList != null)
                {
                    Item firstItem = ralList[0];
                    if (firstItem != null)
                    {
                        FirstAddedItem.Add(firstItem);
                        Logger.ReportInfo("*---------***-- First newest Item = {0}", firstItem.Name);
                    }
                    Item secItem = ralList[1];
                    if (secItem != null)
                    {
                        SecondAddedSet.Add(secItem);
                        Logger.ReportInfo("*---------***-- Second newest Item = {0}", secItem.Name);
                    }
                    Item thirdItem = ralList[2];
                    if (thirdItem != null)
                    {
                        ThirdAddedSet.Add(thirdItem);
                        Logger.ReportInfo("*---------***-- Third newest Item = {0}", thirdItem.Name);
                    }
                    if (FirstAddedItem != null && SecondAddedSet != null && ThirdAddedSet != null)
                    {
                        ralList.RemoveAt(0);
                        ralList.RemoveAt(1);
                        ralList.RemoveAt(2);
                        foreach (var item in ralList)
                        {
                            RemainingAddedItems.Add(item);
                        }
                    }

                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Recently Added Items {0}", fuckinShite);
            }
            return null;
        }

        internal static ArrayListDataSet GetRecentlyAddedSet()
        {
            try
            {
                FirstAddedItem.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 0;
                int limit = 1;

                foreach (BaseItemDto dto in APIQuery.GetRecentAddedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        FirstAddedItem.Add(item);
                    }
                }
                if (FirstAddedItem != null)
                {
                    return FirstAddedItem;
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Recently Added Items {0}", fuckinShite);
            }
            return null;
        }

        internal static ArrayListDataSet GetSecondAddedSet()
        {
            try
            {
                SecondAddedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 1;
                int limit = 1;

                foreach (BaseItemDto dto in APIQuery.GetRecentAddedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        SecondAddedSet.Add(item);
                    }
                }
                if (SecondAddedSet != null)
                {
                    return SecondAddedSet;
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Recently Added Items {0}", fuckinShite);
            }
            return null;
        }

        internal static ArrayListDataSet GetThirdAddedSet()
        {
            try
            {
                ThirdAddedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 2;
                int limit = 1;
                foreach (BaseItemDto dto in APIQuery.GetRecentAddedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        ThirdAddedSet.Add(item);
                    }
                }
                if (ThirdAddedSet != null)
                {
                    return ThirdAddedSet;
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Recently Added Items {0}", fuckinShite);
            }
            return null;
        }

        internal static ArrayListDataSet GetRemainingRecentlyAddedSet()
        {
            try
            {
                RemainingAddedItems.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 3;
                int limit = 10;
                foreach (BaseItemDto dto in APIQuery.GetRecentAddedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        RemainingAddedItems.Add(item);
                    }
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Recently Added Items", fuckinShite);
            }
            return RemainingAddedItems;
        }

        internal static ArrayListDataSet GetRecommendedSet()
        {
            RecommendationDto rdto = null;
            try
            {
                RecommendedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                const string category = "1";
                const string limit = "5";

                //BecauseYouWatchedName = dto.BaselineItemName;
                rdto = APIQuery.GetRecommendedItems(id, folderId).FirstOrDefault();
                if (rdto != null)
                {
                    BecauseYouWatchedName = rdto.BaselineItemName;
                    //Logger.ReportInfo("************ IRIDIUM - RecommendedTypes = {0}", dto.RecommendationType.ToString());

                    foreach (BaseItemDto i in rdto.Items)
                    {
                        {
                            Item item = GetGenericItem(i);
                            if (item != null)
                            {
                                RecommendedSet.Add(item);
                            }
                        }
                    }
                }

                //Random rand = new Random();
                //int r = rand.Next(RecommendedSet.Count);
                //return RecommendedSet[r] as ArrayListDataSet;

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Movie Recommendations List {0}", fuckinShite.ToString());
            }
            return RecommendedSet;
        }

        private static Item GetGenericItem(BaseItemDto dto)
        {
            //Retrieves the item based on the items guid
            BaseItem baseItem = Kernel.Instance.MB3ApiRepository.RetrieveItem(new Guid(dto.Id));
            //If the call to the api returns empty, catch it here.

            if (baseItem == null)
            {
                return null;
            }
            Item item = ItemFactory.Instance.Create(baseItem);
            if (item.BaseItem is Episode)
            {
                TVHelper.CreateEpisodeParents(item);
                return item;
            }
            return item;

        }

        public static ActorInfo GetPersonDtoStream(Item item)
        {
            Logger.ReportInfo("Iridium - Getting Actor info for {0}", item.Name);

            string apiUrl = Kernel.ApiClient.DashboardUrl.Split(new[] {"dashboard"}, StringSplitOptions.None)[0];
            //Use the standard API Prefix
            string queryUrl = string.Format("{0}Persons/{1}", apiUrl, Uri.EscapeUriString(item.Name));
            //Query Format taken from Swagger

            BaseItemDto dto;

            //get the Json Stream
            using (Stream stream = Kernel.ApiClient.GetSerializedStream(queryUrl))
            {
                //Deserialize the Json stream and put in local variable
                dto = Kernel.ApiClient.DeserializeFromStream<BaseItemDto>(stream);
                if (dto == null)
                    Logger.ReportInfo("***** Iridium ***** Unable to get Actor Info for {0}", item.Name);
            }
            //return the dto
            return SaveActorInfo(dto);
        }

        internal static ActorInfo SaveActorInfo(BaseItemDto personDto)
        {
            ActorInfo info = new ActorInfo
            {
                Id = personDto.Id, //string
                Name = personDto.Name, //string
                Born = personDto.PremiereDate.HasValue ? personDto.PremiereDate.Value.ToString("D") : null, //string
                Died = personDto.EndDate.HasValue ? personDto.EndDate.Value.ToString("d MMM yyyy") : null, //string
                Bio = personDto.Overview, //string
                BirthPlaceLocations = personDto.ProductionLocations, //List<string>
                BornDate = personDto.PremiereDate, //DateTime
                DiedDate = personDto.EndDate //DateTime
            };
            info.HasLoaded = true;
            /*Logger.ReportInfo(
                "Name = {0} || DTO BORN = {6} ||Born = {1} , Aged {4} || Died = {2} || Birthplace = {3} || Biography: {5}",
                info.Name, info.Born, info.Died, info.BirthPlace, info.Age, info.Bio, personDto.PremiereDate);*/
            return info;
        }



        public static ArrayListDataSet NewsItemsList()
        {
            Logger.ReportInfo("Iridium - Getting News From MB Blog");

            string apiUrl = Kernel.ApiClient.DashboardUrl.Split(new[] {"dashboard"}, StringSplitOptions.None)[0];
            string queryUrl = string.Format("{0}/News/Product?Limit=10", apiUrl);

            QueryResult<NewsItem> nItem;
            XNewsItem xNewsItem = new XNewsItem();

            using (Stream stream = Kernel.ApiClient.GetSerializedStream(queryUrl))
            {
                nItem = Kernel.ApiClient.DeserializeFromStream<QueryResult<NewsItem>>(stream);
                if (nItem != null)
                {
                    foreach (NewsItem item in nItem.Items)
                    {
                        XNewsItem newsItem = GetNewsItemInfo(item);
                        xNewsItem.newsItemsSet.Add(newsItem);
                    }
                }
                else
                {
                    Logger.ReportInfo("***** Iridium ***** Unable to get News Info for {0}");
                }
            }
            return xNewsItem.newsItemsSet;
        }

        private static XNewsItem GetNewsItemInfo(NewsItem nItem)
        {
            XNewsItem newsItem = new XNewsItem
            {
                Id = nItem.Guid,
                Title = nItem.Title,
                Description = nItem.Description,
                Date = nItem.Date

            };
            //Logger.ReportInfo("++++++++++++++++++++MBNews: ", newsItem.Title);


            return newsItem;
        }

        internal static ArrayListDataSet GetFirstWatchedSet()
        {
            try
            {
                RecentlyWatchedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 0;
                int limit = 1;

                foreach (BaseItemDto dto in APIQuery.GetRecentWatchedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    bool showNewItems = item.ShowNewestItems;
                    if (item != null)
                    {
                        RecentlyWatchedSet.Add(item);
                    }
                }
                if (RecentlyWatchedSet != null)
                {
                    return RecentlyWatchedSet;
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Recently Added Items {0}", fuckinShite);
            }
            return null;
        }

        internal static ArrayListDataSet GetSecondWatchedSet()
        {
            try
            {
                SecondWatchedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 1;
                int limit = 1;

                foreach (BaseItemDto dto in APIQuery.GetRecentWatchedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        SecondWatchedSet.Add(item);
                    }
                }
                if (SecondWatchedSet != null)
                {
                    return SecondWatchedSet;
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Recently Added Items {0}", fuckinShite);
            }
            return null;
        }

        internal static ArrayListDataSet GetThirdWatchedSet()
        {
            try
            {
                ThirdWatchedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 2;
                int limit = 1;
                foreach (BaseItemDto dto in APIQuery.GetRecentWatchedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        ThirdWatchedSet.Add(item);
                    }
                }
                if (ThirdWatchedSet != null)
                {
                    return ThirdWatchedSet;
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Recently Added Items {0}", fuckinShite);
            }
            return null;
        }

        internal static ArrayListDataSet GetRemainingWatchedSet()
        {
            try
            {
                RemainingWatchedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 3;
                int limit = 10;
                foreach (BaseItemDto dto in APIQuery.GetRecentWatchedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        RemainingWatchedSet.Add(item);
                    }
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Recently Added Items", fuckinShite);
            }
            return RemainingWatchedSet;
        }

        internal static ArrayListDataSet GetFirstUnwatchedSet()
        {
            try
            {
                RecentlyUnwatchedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 0;
                int limit = 1;

                foreach (BaseItemDto dto in APIQuery.GetRecentUnwatchedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    bool showNewItems = item.ShowNewestItems;
                    if (item != null)
                    {
                        RecentlyUnwatchedSet.Add(item);
                    }
                }
                if (RecentlyUnwatchedSet != null)
                {
                    return RecentlyUnwatchedSet;
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Unwatched Items {0}", fuckinShite);
            }
            return null;
        }

        internal static ArrayListDataSet GetSecondUnwatchedSet()
        {
            try
            {
                SecondUnwatchedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 1;
                int limit = 1;

                foreach (BaseItemDto dto in APIQuery.GetRecentUnwatchedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        SecondUnwatchedSet.Add(item);
                    }
                }
                if (SecondUnwatchedSet != null)
                {
                    return SecondUnwatchedSet;
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Unwatched Items {0}", fuckinShite);
            }
            return null;
        }

        internal static ArrayListDataSet GetThirdUnwatchedSet()
        {
            try
            {
                ThirdUnwatchedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 2;
                int limit = 1;
                foreach (BaseItemDto dto in APIQuery.GetRecentUnwatchedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        ThirdUnwatchedSet.Add(item);
                    }
                }
                if (ThirdUnwatchedSet != null)
                {
                    return ThirdUnwatchedSet;
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Unwatched Items {0}", fuckinShite);
            }
            return null;
        }

        internal static ArrayListDataSet GetRemainingUnwatchedSet()
        {
            try
            {
                RemainingUnwatchedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 3;
                int limit = 10;
                foreach (BaseItemDto dto in APIQuery.GetRecentUnwatchedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        RemainingUnwatchedSet.Add(item);
                    }
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Unwatched Items", fuckinShite);
            }
            return RemainingUnwatchedSet;
        }

        public static bool IsMBIntrosInstalled()
        {
            Logger.ReportInfo("Iridium - Getting List of Plugins installed");
            //Use the standard API Prefix
            string apiUrl = Kernel.ApiClient.DashboardUrl.Split(new[] {"dashboard"}, StringSplitOptions.None)[0];
            //Query Format taken from Swagger
            string queryUrl = string.Format("{0}Plugins", apiUrl);
            //get the Json Stream
            using (Stream stream = Kernel.ApiClient.GetSerializedStream(queryUrl))
            {
                if (stream == null)
                {
                    Logger.ReportInfo("****** Iridium ****** Plugin Retreival Failed");
                }
                //Deserialize the Json stream and put in local variable
                string MBIntros = "MBIntros.dll";
                PluginInfo info = Kernel.ApiClient.DeserializeFromStream<PluginInfo>(stream);
                if (info == null)
                    Logger.ReportInfo("******* Iridium ******* Plugin list installed is {0}", info);

            }
            return false;
        }

        internal static ArrayListDataSet GetSpotlightAddedItem()
        {
            try
            {
                SpotlightAddedItemSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                int start = 0;
                int limit = 10;

                foreach (BaseItemDto dto in APIQuery.GetRecentUnwatchedItem(id, folderId, start, limit).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        SpotlightAddedItemSet.Add(item);
                    }
                }
                if (SpotlightAddedItemSet != null)
                { 
                    List<ArrayListDataSet> shuffled = ShuffleList(SpotlightAddedItemSet);
                    return shuffled.First();
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Spotlight Item {0}", fuckinShite);
            }
            return null;
        }

        internal static ArrayListDataSet GetYearAddedSet()
        {
            try
            {
                YearAddedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;

                foreach (BaseItemDto dto in APIQuery.YearAPIQuery(id, folderId).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        YearAddedSet.Add(item);
                    }
                }
                if (YearAddedSet != null)
                {
                    return YearAddedSet;
                }

            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Year Set Items {0}", fuckinShite);
            }
            return null;
        }

        private static List<ArrayListDataSet> ShuffleList(ArrayListDataSet arrList)
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
            return new List<ArrayListDataSet> {arrList};
        }
    }
}
