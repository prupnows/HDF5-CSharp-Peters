using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hdf5DotnetWrapper
{
    public static class Hdf5Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NormalizedName(string name) => Hdf5.Hdf5Settings.LowerCaseNaming ? name.ToLowerInvariant() : name;

        public static Action<string> LogError;
        public static Action<string> LogInfo;
        public static Action<string> LogDebug;
        public static Action<string> LogCritical;

    }
}
