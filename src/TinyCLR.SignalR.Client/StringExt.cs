using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace TinyCLR.SignalR.Client
{
    public static class StringExt
    {
        public static bool StartsWith(this string s, string value)
        {
            return s.IndexOf(value) == 0;
        }

        public static bool Contains(this string s, string value)
        {
            return s.IndexOf(value) > 0;
        }
    }
}
