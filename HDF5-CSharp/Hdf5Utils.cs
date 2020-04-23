using System;
using System.Runtime.CompilerServices;
using HDF.PInvoke;
using HDF5CSharp.DataTypes;

namespace HDF5CSharp
{
    public static class Hdf5Utils
    {
        public static Action<string> LogError;
        public static Action<string> LogInfo;
        public static Action<string> LogDebug;
        public static Action<string> LogWarning;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void LogMessage(string msg, Hdf5LogLevel level)
        {
            if (!Hdf5.Hdf5Settings.ErrorLoggingEnable) return;
            switch (level)
            {
                case Hdf5LogLevel.Debug:
                    LogDebug?.Invoke(msg);
                    break;
                case Hdf5LogLevel.Info:
                    LogInfo?.Invoke(msg);
                    break;
                case Hdf5LogLevel.Warning:
                    LogWarning?.Invoke(msg);
                    break;
                case Hdf5LogLevel.Error:
                    LogError?.Invoke(msg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
     
    }
}
