using System;
using System.Collections.Generic;
using System.Text;

namespace BinarDataGenerator
{
    public static class Extensions
    {
        public static int ToFreqKey(this double d)
        {
            return (int)d * 10000;
        }
        public static int ToDepKey(this double d)
        {
            return (int)d * 100;
        }
        public static int ToPolKey(this string p)
        {
            return p.GetHashCode();
        }
    }
}
