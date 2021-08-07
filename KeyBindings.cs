using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputRecordReplay
{
    public class KeyBindings
    {
        public string RecordButton { get; set; }
        public string StopButton { get; set; }
        public string PlayButton { get; set; }

        public KeyBindings()
        {
            RecordButton = "r";
            StopButton = "s";
            PlayButton = "p";
        }
    }
}
