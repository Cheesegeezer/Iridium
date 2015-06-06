using Microsoft.MediaCenter.UI;

namespace Iridium
{
    using System;

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
                return this.title;
            }
            set
            {
                this.title = value;
                base.FirePropertyChanged("NewsTitle");
            }
        }

        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
                FirePropertyChanged("NewsId");
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
                FirePropertyChanged("NewsDesc");
            }
        }

        public DateTime Date
        {
            get
            {
                return this.date;
            }
            set
            {
                this.date = value;
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
