using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using GTA.Math;

namespace GTAVMemScanner
{
    public static class MemScanner
    {
        public static IntPtr FindPattern(string pattern, string mask)
        {
            return Game.FindPattern(pattern, mask);
        }

        public static IntPtr FindPatternWithStartAddr(string pattern, string mask, IntPtr start)
        {
            return Game.FindPattern(pattern, mask, start);
        }
    }
}
