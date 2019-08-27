using System;

namespace SlimShadyCore.Utilities
{
    public class SlimWin32Exception : Exception
    {
        public SlimWin32Exception(String msg)
            : base(msg + Utils.GetLastWin32Error())
        {
        }
    }
}
