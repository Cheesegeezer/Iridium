using MediaBrowser.Library;
using Microsoft.MediaCenter.UI;

namespace Iridium.Actors
{
    public class ActorCreditItem : ModelItem
    {
        private string _character;
        private bool _have;
        private static string _id;
        private Item _item;
        private string _release = "";
        private string _title;

        public string Character
        {
            get
            {
                return _character;
            }
            set
            {
                _character = value;
                FirePropertyChanged("Character");
            }
        }

        public bool Have
        {
            get
            {
                return _have;
            }
            set
            {
                _have = value;
                FirePropertyChanged("Have");
            }
        }

        public static string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                
            }
        }

        public Item Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                FirePropertyChanged("Item");
            }
        }

        public string Release
        {
            get
            {
                return _release;
            }
            set
            {
                _release = value;
                FirePropertyChanged("Release");
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                FirePropertyChanged("Title");
            }
        }
    }
}
