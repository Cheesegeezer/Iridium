using System.Threading;
using MediaBrowser.Library;
using MediaBrowser.Library.Logging;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using Application = MediaBrowser.Application;

namespace Iridium
{
    public class MyShortlist : ModelItem
    {
        //Adds or Removes the Item from the Shortlist
        public void AddItemToShortlist(Item item)
        {
            if (_shortlist.Contains(item)) //Remove if Item Exists
            {
                
                Logger.ReportInfo("**Iridium SHORTLIST** ATTEMPTING TO REMOVE ITEM --{0}-- TO SHORTLIST", item.Name);
                _shortlist.Remove(item);
                Logger.ReportInfo("**Iridium SHORTLIST** REMOVED --{0}-- FROM SHORTLIST", item.Name);
                ShortlistOptionText();
                ItemInShortlist();
                FirePropertyChanged("AddItemToShortlist");
            }
            else
            {
                //Add if Item does not Exist
                Logger.ReportInfo("**Iridium SHORTLIST** ATTEMPTING TO ADD ITEM --{0}-- TO SHORTLIST", item.Name);
                _shortlist.Add(item);
                Logger.ReportInfo("**Iridium SHORTLIST** ADDED --{0}-- TO SHORTLIST", item.Name);
                ShortlistOptionText();
                ItemInShortlist();
                FirePropertyChanged("AddItemToShortlist");
            }
        }

        //Shortlist to be Displayed
        private static ArrayListDataSet _shortlist = new ArrayListDataSet();
        public ArrayListDataSet Shortlist
        {
            get { return _shortlist; }
            set
            {
                _shortlist = value;
                FirePropertyChanged("Shortlist");
            }
        }

        //Determines if the Shortlist has any Items in it
        public bool HasShortlistItems
        {
            get
            {
                if (_shortlist.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }

        //Determines if the Current Item is in the Shortlist
        private bool _isItemInShortlist;
        public bool IsItemInShortlist
        {
            get
            {
                return _isItemInShortlist;
            }
            set
            {
                _isItemInShortlist = value;
                FirePropertyChanged("IsItemInShortlist");
            }
        }

        public void ItemInShortlist()
        {
            Item currItem = Application.CurrentInstance.CurrentItem;
            if (Shortlist.Contains(currItem))
            {
                //Logger.ReportInfo("+++++++++++++++++++++++++++ item Name = {0}",Application.CurrentInstance.CurrentItem.Name);
                IsItemInShortlist = true;
                FirePropertyChanged("ItemInShortlist");
            }
            else
            {
                //Logger.ReportInfo("---------------------------- item Name = {0}",Application.CurrentInstance.CurrentItem.Name);
                IsItemInShortlist = false;
                FirePropertyChanged("ItemInShortlist");
            }
        }

        //Gets the Option Text for the Context Menu and changes it when the user add/removes and item.
        private string _shortlistOption;
        public string ShortlistOption
        {
            get
            {
                if (_shortlistOption == null)
                {
                    _shortlistOption = Kernel.Instance.StringData.GetString("AddToShortlistOptionsLabel");
                }
                return _shortlistOption;
            }
            set
            {
                _shortlistOption = value;
                FirePropertyChanged("ShortlistOption");
            }
        }

        

        public void ShortlistOptionText()
        {
            Item currItem = Application.CurrentInstance.CurrentItem;
            if (_shortlist.Contains(currItem))
            {
                ShortlistOption = Kernel.Instance.StringData.GetString("RemoveFromShortlistOptionsLabel");
            }
            else
            {
                ShortlistOption = Kernel.Instance.StringData.GetString("AddToShortlistOptionsLabel");
            }
        }

        public void NoShortlistMessage()
        {
            string text = "You have NO items in Your Shortlist";
            
            CustomMessage.Instance.MessageBox(text);
            //Application.CurrentInstance.MessageBox(text, true, 0);
        }

        public void ClearShortlist()
        {
            MediaCenterEnvironment mediaCenterEnvironment = AddInHost.Current.MediaCenterEnvironment;
            string text = "SHORTLIST: Are you sure you want clear your Shortlist?";
            const string caption = "SHORTLIST";

            //CustomMessage.Instance.YesNoBox(text);

            if (CustomMessage.Instance.YesNoBox(text) == "Y")
            {
                _shortlist.Clear();
                Thread.Sleep(1000);
                CustomMessage.Instance.MessageBox("Your Shortlist has been Emptied");
            }
            else if (CustomMessage.Instance.YesNoBox(text) == "N")
            {
                
            }


            /*if (mediaCenterEnvironment.Dialog(text, caption, DialogButtons.Cancel | DialogButtons.Ok, 0, true) ==
                DialogResult.Ok)
            {
                _shortlist.Clear();
                Application.CurrentInstance.MessageBox("Shortlist has been Emptied", true, 0);
            }*/
        }
    }
}