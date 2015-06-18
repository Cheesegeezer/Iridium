using System;
using Microsoft.MediaCenter.UI;

namespace Iridium
{
    public class XNewsItem : ModelItem
    {
        
        public ArrayListDataSet newsItemsSet = new ArrayListDataSet();
        private string title;
        private string id;
        private string description;
        private DateTime date;

        public ArrayListDataSet NewsItemsSet
        {
            get { return newsItemsSet; }
            
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                FirePropertyChanged("NewsTitle");
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                FirePropertyChanged("NewsId");
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                FirePropertyChanged("NewsDesc");
            }
        }

        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
                FirePropertyChanged("NewsDate");
            }
        }

        public string DateString
        {
            get
            {
                return date.ToShortDateString();
            }
        }
    }
}
