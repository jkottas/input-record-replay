using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputRecordReplay.Models
{
    public class KeyBindings
    {
        public string RecordBeginButton { get; set; }
        public string RecordEndButton { get; set; }
        public string PlaybackBeginButton { get; set; }
        public string PlaybackEndButton { get; set; }

        public KeyBindings()
        {
            RecordBeginButton = "KEY_R";
            RecordEndButton = "KEY_S";
            PlaybackBeginButton = "KEY_P";
            PlaybackEndButton = "KEY_S";
        }
    }
}
