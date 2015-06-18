using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;

namespace Iridium.Library
{
    partial class IridiumHelper : ModelItem
    {
        private int _headerIndex;
        private int _folderIndex;
        private int _tilesIndex;

        public int CurrentHeaderIndex
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

        public int FolderIndex
        {
            get
            {
                return this._folderIndex;
            }
            set
            {
                if (this._folderIndex != value)
                {
                    this._folderIndex = value;
                    base.FirePropertyChanged("FolderIndex");
                }
            }
        }

        public int TileGroupIndex
        {
            get
            {
                return this._tilesIndex;
            }
            set
            {
                if (this._tilesIndex != value)
                {
                    this._tilesIndex = value;
                    base.FirePropertyChanged("TilesIndex");
                }
            }
        }

        
    }
}
