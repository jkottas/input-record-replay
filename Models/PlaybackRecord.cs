using InputRecordReplay.InputHooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputRecordReplay.Models
{
    public struct PlaybackRecord
    {
        public TimeSpan when;
        public INPUT input;
        //public uint msg;
        //public uint wParam;
        //public uint lParam;
        //#region keyboard only attributes
        //public char type;
        //public byte bVk;
        //public byte bScan;
        //#endregion
        //#region mouse only attributes
        //public int dx;
        //public int dy;
        //public uint dwData;
        //#endregion
        //#region shared attributes
        //public uint dwFlags;
        //public int dwExtraInfo;
        //#endregion
    }
}
