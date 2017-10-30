using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DoubleX.Upload
{
    /// <summary>
    /// ListViewItem需要绑定的数据信息
    /// </summary>
    public class FtpItem : INotifyPropertyChanged
    {
        private string _Name;
        private string _Type;
        private string _Size;
        private string _Path;
        private string _ImgPath;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Type"));
                }
            }
        }
        public string Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _Size = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Size"));
                }
            }
        }
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Path"));
                }
            }
        }

        public string ImgPath
        {
            get
            {
                return _ImgPath;
            }
            set
            {
                _ImgPath = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ImgPath"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
