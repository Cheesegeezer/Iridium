using Microsoft.MediaCenter.UI;

namespace Iridium
{
    public class DetailsItem : ModelItem
    {
        public DetailsItem()
        { }

        public Command Focus
        {
            get;
            set;
        }
        public Command Click
        {
            get;
            set;
        }

    }
}
