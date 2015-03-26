using System;
using System.Collections.Generic;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Library;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;

namespace Iridium.APICalls
{
    class APIQueries : BaseApiClient 
    {
        //Required for BaseApiClient - No requirement for any methods
        protected override void SetAuthorizationHeader(string header)
        {
            //throw new NotImplementedException();
        }

        //The API address - taken from MBC
        private string APIUrl()
        {
                return Kernel.ApiClient.DashboardUrl.Split(new string[] { "dashboard" }, StringSplitOptions.None)[0];
        }
        
        //Guid = UserId
        public ItemsResult NextUpAPIQuery(Guid userId)
        {
            string query = "&Limit=10&Fields=Name%2COverview%2CIsEpisode%2COfficialRating%2CStatus%2CPrimaryImageAspectRatio&format=Json"; //must include "&format=Json" in order to allow for the items to be read.
            string queryUrl = string.Format("{0}Shows/NextUp?UserId={1}{2}", APIUrl(), userId, query); //Query Format taken from Swagger
            Logger.Info("***************************** UserId = ", userId);
            return Kernel.ApiClient.GenericApiQuery(queryUrl);//Interrogate the API based on the query string.
        }

        public ItemsResult UpComingAPIQuery(Guid userId)
        {
            string query = "&Fields=Name%2CId%2CSeriesName%2CIndexNumber%2C%20ParentIndexNumber%2CAirTime%2CSeriesStudio%2C%20PremiereDate%2CParentBackdropItemId&format=Json"; //must include "&format=Json" in order to allow for the items to be read.
            string queryUrl = string.Format("{0}Shows/Upcoming?UserId={1}{2}", APIUrl(), userId, query); //Query Format taken from Swagger
            return Kernel.ApiClient.GenericApiQuery(queryUrl);//Interrogate the API based on the query string.
        }

        public ItemsResult AcclaimedAPIQuery(Guid userId, Guid folderId)
        {
            string query = string.Format("&ParentId={0}&format=Json",folderId); //must include "&format=Json" in order to allow for the items to be read.
            string queryUrl = string.Format("{0}Items?MinCriticRating=80&Limit=10&Recursive=true&UsereId={1}{2}", APIUrl(), userId, query); //Query Format taken from Swagger
            return Kernel.ApiClient.GenericApiQuery(queryUrl);//Interrogate the API based on the query string.
        }
        
        public ItemsResult GenresAPIQuery(Guid userId, Guid folderId) 
        {
            string query = string.Format("&ParentId={0}&format=Json", folderId); //must include "&format=Json" in order to allow for the items to be read.
            string queryUrl = string.Format("{0}Genres?UserId={1}{2}", APIUrl(), userId, query); //Query Format taken from Swagger
            return Kernel.ApiClient.GenericApiQuery(queryUrl);//Interrogate the API based on the query string.
        }
        
        public ItemsResult SimilarAPIQuery(Guid userId, Guid itemId)
        {
            string query = string.Format("&Limit=10&format=Json"); //must include "&format=Json" in order to allow for the items to be read.
            string queryUrl = string.Format("{0}Movies/{1}/Similar?UserId={2}{3}", APIUrl(), itemId, userId, query); //Query Format taken from Swagger
            return Kernel.ApiClient.GenericApiQuery(queryUrl);//Interrogate the API based on the query string.
        }

        internal ItemsResult GetResumableItems(Guid userId, Guid folderId)
        {
            string query = string.Format("&ParentId={0}&Filters=IsResumable&format=Json", folderId); //must include "&format=Json" in order to allow for the items to be read.
            string queryUrl = string.Format("{0}Items?Limit=10&Recursive=true&UserId={1}{2}", APIUrl(), userId, query); //Query Format taken from Swagger
            return Kernel.ApiClient.GenericApiQuery(queryUrl);//Interrogate the API based on the query string.
        }

        internal ItemsResult GetRecentAddedItem(Guid userId, Guid folderId,int start, int limit)
        {
            string query = string.Format("&ParentId={0}&Filters=IsNotFolder&SortBy=DateCreated&format=Json", folderId); //must include "&format=Json" in order to allow for the items to be read.
            string queryUrl = string.Format("{0}Items?StartIndex={1}&Limit={2}&Recursive=true&SortOrder=Descending&UserId={3}{4}", APIUrl(), start, limit, userId, query); //Query Format taken from Swagger
            return Kernel.ApiClient.GenericApiQuery(queryUrl);//Interrogate the API based on the query string.
        }

        /*internal ItemsResult GetNext14RecentAddedItems(Guid userId, Guid folderId)
        {
            string query = string.Format("&ParentId={0}&Filters=IsNotFolder&SortBy=DateCreated&format=Json", folderId); //must include "&format=Json" in order to allow for the items to be read.
            string queryUrl = string.Format("{0}Items?StartIndex=1&Limit=10&Recursive=true&SortOrder=Descending&UserId={1}{2}", APIUrl(), userId, query); //Query Format taken from Swagger
            return Kernel.ApiClient.GenericApiQuery(queryUrl);//Interrogate the API based on the query string.
        }*/

        public IEnumerable<RecommendationDto> RecommendationDtoQuery(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            using (var stream = Kernel.ApiClient.GetSerializedStream(url))
            {
                return DeserializeFromStream<IEnumerable<RecommendationDto>>(stream);
            }
        }

        public IEnumerable<RecommendationDto> GetRecommendedItems(Guid userId, Guid folderId)
        {
            string query = string.Format("&ParentId={0}&format=Json", folderId); //Add additional filters to this line
            string queryUrl = string.Format("{0}Movies/Recommendations?Category=1&ItemLimit=8&UserId={1}{2}", APIUrl(), userId, query); //Query Format taken from Swagger
            return RecommendationDtoQuery(queryUrl);//Interrogate the API based on the query string.
        }

        // http://localhost:8096/mediabrowser/Items?Limit=1&Recursive=true&SortOrder=Descending&ParentId=c8ca023cdbb96c78e4de5fcb477df512&Filters=IsNotFolder&SortBy=DateCreated //Return First item for RAL
        // http://localhost:8096/mediabrowser/Items?StartIndex=1&Limit=14&Recursive=true&SortOrder=Descending&ParentId=c8ca023cdbb96c78e4de5fcb477df512&Filters=IsNotFolder&SortBy=DateCreated //Return next 14 items

    }
}
