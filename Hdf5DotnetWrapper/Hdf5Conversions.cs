using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hdf5DotnetWrapper
{
    public static class Hdf5Conversions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long FromDatetime(DateTime time, DateTimeType type)
        {
            switch (type)
            {
                case DateTimeType.Ticks:
                    return time.Ticks;
                case DateTimeType.UnixTimeSeconds:
                    return (long)(time.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, time.Kind))).TotalSeconds;
                case DateTimeType.UnixTimeMilliseconds:
                    return (long)(time.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, time.Kind))).TotalMilliseconds;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
