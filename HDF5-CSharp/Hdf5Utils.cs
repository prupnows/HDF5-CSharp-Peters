using System;
using System.Runtime.CompilerServices;
using HDF.PInvoke;

namespace HDF5CSharp
{
    public static class Hdf5Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static (bool valid,string name) GetRealName(long id,string name,string alternativeName)
        {
            string normalized = NormalizedName(name);
            if (!string.IsNullOrEmpty(normalized) && H5L.exists(id, normalized) > 0)
            {
                return (true, normalized);
            }

            normalized = NormalizedName(alternativeName);
            if (!string.IsNullOrEmpty(normalized) && H5L.exists(id, normalized) > 0)
            {
                return (true, normalized);
            }

            return (false, "");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static (bool valid, string name) GetRealAttributeName(long id, string name, string alternativeName)
        {
            string normalized = NormalizedName(name);
            if (!string.IsNullOrEmpty(normalized) && H5A.exists(id, normalized) > 0)
            {
                return (true, normalized);
            }

            normalized = NormalizedName(alternativeName);
            if (!string.IsNullOrEmpty(normalized) && H5A.exists(id, normalized) > 0)
            {
                return (true, normalized);
            }

            return (false, "");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NormalizedName(string name) => Hdf5.Hdf5Settings.LowerCaseNaming ? name.ToLowerInvariant() : name;

        public static Action<string> LogError;
        public static Action<string> LogInfo;
        public static Action<string> LogDebug;
        public static Action<string> LogCritical;
        public static Action<string> LogWarning;
    }
}
