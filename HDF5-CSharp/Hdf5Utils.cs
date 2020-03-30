using System;
using System.Runtime.CompilerServices;

namespace HDF5CSharp
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
