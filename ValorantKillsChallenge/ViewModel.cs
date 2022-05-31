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
        private string challengeSeconds;

        public int KillCount
        {
            get => killCount;
            set
            {
                killCount = value;
                NotifyPropertyChanged("KillCount");
            }
        }
        public string ChallengeSeconds
        {
            get => challengeSeconds;
            set
            {
                challengeSeconds = value;
                NotifyPropertyChanged("ChallengeSeconds");
            }
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
