using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
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
        public static ArrayListDataSet GenresSet = new ArrayListDataSet();
        public static ArrayListDataSet RecentlyAddedSet = new ArrayListDataSet();
        public static ArrayListDataSet RecentlyWatchedSet = new ArrayListDataSet();
        public static ArrayListDataSet RecentlyUnwatchedSet = new ArrayListDataSet();
        public static ArrayListDataSet NextUpSet = new ArrayListDataSet();
        public static ArrayListDataSet UpComingTVSet = new ArrayListDataSet();
        public static ArrayListDataSet AcclaimedMoviesSet = new ArrayListDataSet();
        public static ArrayListDataSet RecommendedSet = new ArrayListDataSet();
        public static ArrayListDataSet SimilarMoviesSet = new ArrayListDataSet();
        public static ArrayListDataSet RemainingRecentlyAddedSet = new ArrayListDataSet();
        public static string BecauseYouWatchedName { get; set; }
        
        //Keep MCML Happy
        public GetAPIItems()
        {
        }

        //
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
            Logger.ReportInfo("IRIDIUM - Attempting to Get Critically Acclaimed Movies");
            try
            {
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

        

        internal static ArrayListDataSet GetGenresSet()
        {
            Logger.ReportInfo("IRIDIUM - Attempting to Get Genres Set");
            try
            {
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                foreach (BaseItemDto dto in APIQuery.GenresAPIQuery(id, folderId).Items)
                {
                    Item item = GetGenericItem(dto);
                    GenresSet.Add(item);
                }
                
            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Genres /n {0}", fuckinShite);
            }
            return GenresSet;
        }

        internal static ArrayListDataSet GetSimilarMoviesSet()
        {
            Logger.ReportInfo("IRIDIUM - Attempting to Get Similar Movies");
            SimilarMoviesSet.Clear();
            try
            {
                
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                foreach (BaseItemDto dto in APIQuery.SimilarAPIQuery(id, folderId).Items)
                {
                    Item item = GetGenericItem(dto);
                    SimilarMoviesSet.Add(item);
                }
                
            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error {0}", fuckinShite.ToString());
            }
            return SimilarMoviesSet;
        }

        internal static ArrayListDataSet GetResumeSet()
        {
            Logger.ReportInfo("IRIDIUM - Attempting to Get Resumable Items");
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

        internal static ArrayListDataSet GetRecentlyAddedSet()
        {
            Logger.ReportInfo("IRIDIUM - Attempting to Get RecentlyAdded Items");
            try
            {
                RecentlyAddedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                foreach (BaseItemDto dto in APIQuery.GetMostRecentAddedItem(id, folderId).Items)
                {

                    Item item = GetGenericItem(dto);
                    bool showNewItems = item.ShowNewestItems;
                    if (item != null)
                    {
                        RecentlyAddedSet.Add(item);
                    }
                }
                if (RecentlyAddedSet != null)
                {
                    return RecentlyAddedSet;
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
            Logger.ReportInfo("IRIDIUM - Attempting to Get RecentlyAdded Items");
            try
            {
                RemainingRecentlyAddedSet.Clear();
                Guid id = Kernel.CurrentUser.Id;
                Guid folderId = Application.CurrentInstance.CurrentItem.Id;
                foreach (BaseItemDto dto in APIQuery.GetNext14RecentAddedItems(id, folderId).Items)
                {

                    Item item = GetGenericItem(dto);
                    if (item != null)
                    {
                        RemainingRecentlyAddedSet.Add(item);
                    }
                }
                
            }
            catch (Exception fuckinShite)
            {
                Logger.ReportError("IRIDIUM - Error Retrieving Recently Added Items", fuckinShite);
            }
            return RemainingRecentlyAddedSet;
        }

        internal static ArrayListDataSet GetRecommendedSet()
        {
            RecommendationDto rdto = null;
            Logger.ReportInfo("IRIDIUM - Attempting to Get Recommended Items for this folder");
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

            string apiUrl = Kernel.ApiClient.DashboardUrl.Split(new[] { "dashboard" }, StringSplitOptions.None)[0];
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

            string apiUrl = Kernel.ApiClient.DashboardUrl.Split(new[] { "dashboard" }, StringSplitOptions.None)[0];
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
                Date = nItem.Date,

            };
            //Logger.ReportInfo("++++++++++++++++++++MBNews: ", newsItem.Title);


            return newsItem;
        }

        public static bool IsMBIntrosInstalled()
        {
            Logger.ReportInfo("Iridium - Getting List of Plugins installed");
            //Use the standard API Prefix
            string apiUrl = Kernel.ApiClient.DashboardUrl.Split(new[] { "dashboard" }, StringSplitOptions.None)[0];
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
    }
}
