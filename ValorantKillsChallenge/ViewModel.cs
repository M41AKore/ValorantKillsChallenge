using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValorantKillsChallenge
{
    public class ViewModel : INotifyPropertyChanged
    {
        private int killCount = 0;

        public int KillCount
        {
            get => killCount;
            set
            {
                killCount = value;
                NotifyPropertyChanged("KillCount");
            }
        }


        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
