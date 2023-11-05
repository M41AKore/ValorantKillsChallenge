using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ValorantKillsChallenge
{
    [XmlRoot("Settings")]
    public class Settings
    {
        [XmlElement("Hotkey")]
        public Keys Hotkey { get; set; }

        [XmlElement("ChallengeLength")]
        public float ChallengeLength { get; set; }

        [XmlElement("KillRegMatchString")]
        public string KillRegMatchString { get; set; }
    }
}
