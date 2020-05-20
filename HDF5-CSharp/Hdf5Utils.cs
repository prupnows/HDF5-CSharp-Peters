using HDF.PInvoke;
using HDF5CSharp.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HDF5CSharp
{
    public static class Hdf5Utils
    {
        public static Action<string> LogError;
        public static Action<string> LogInfo;
        public static Action<string> LogDebug;
        public static Action<string> LogWarning;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static (bool valid, string name) GetRealName(long id, string name, string alternativeName)
        {
            string normalized = NormalizedName(name);
            if (!String.IsNullOrEmpty(normalized) && H5L.exists(id, normalized) > 0)
            {
                return (true, normalized);
            }

            normalized = NormalizedName(alternativeName);
            if (!String.IsNullOrEmpty(normalized) && H5L.exists(id, normalized) > 0)
            {
                return (true, normalized);
            }

            return (false, "");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static (bool valid, string name) GetRealAttributeName(long id, string name, string alternativeName)
        {
            string normalized = NormalizedName(name);
            if (!String.IsNullOrEmpty(normalized) && H5A.exists(id, normalized) > 0)
            {
                return (true, normalized);
            }

            normalized = NormalizedName(alternativeName);
            if (!String.IsNullOrEmpty(normalized) && H5A.exists(id, normalized) > 0)
            {
                return (true, normalized);
            }

            return (false, "");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NormalizedName(string name) => Hdf5.Settings.LowerCaseNaming ? name.ToLowerInvariant() : name;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void LogMessage(string msg, Hdf5LogLevel level)
        {
            if (!Hdf5.Settings.ErrorLoggingEnable) return;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ItemExists(long groupId, string groupName, Hdf5ElementType type)
        {
            switch (type)
            {
                case Hdf5ElementType.Group:
                case Hdf5ElementType.Dataset:
                    return H5L.exists(groupId, Hdf5Utils.NormalizedName(groupName)) > 0;
                case Hdf5ElementType.Attribute:
                    return H5A.exists(groupId, Hdf5Utils.NormalizedName(groupName)) > 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        internal static long GetDatasetId(long parentId, string name, long dataType, long spaceId)
        {
            return GetId(parentId, name, dataType, spaceId, Hdf5ElementType.Dataset);
        }
        internal static long GetAttributeId(long parentId, string name, long dataType, long spaceId)
        {
            return GetId(parentId, name, dataType, spaceId, Hdf5ElementType.Attribute);
        }

        private static long GetId(long parentId, string name, long dataType, long spaceId, Hdf5ElementType type)
        {
            string normalizedName = Hdf5Utils.NormalizedName(name);
            bool exists = Hdf5Utils.ItemExists(parentId, normalizedName, type);
            if (exists)
            {
                Hdf5Utils.LogMessage($"{normalizedName} already exists", Hdf5LogLevel.Debug);
                if (!Hdf5.Settings.OverrideExistingData)
                {
                    if (Hdf5.Settings.ThrowOnError)
                        throw new Hdf5Exception($"{normalizedName} already exists");
                    return -1;
                }
            }

            var datasetId = -1L;
            switch (type)
            {
                case Hdf5ElementType.Unknown:
                    break;
                case Hdf5ElementType.Group:
                case Hdf5ElementType.Dataset:
                    if (exists)
                    {
                        H5L.delete(parentId, normalizedName);
                        // datasetId = H5D.open(parentId, normalizedName);
                    }
                    datasetId = H5D.create(parentId, normalizedName, dataType, spaceId);
                    break;
                case Hdf5ElementType.Attribute:
                    if (exists)
                    {
                        H5A.delete(parentId, normalizedName);
                    }

                    datasetId = H5A.create(parentId, normalizedName, dataType, spaceId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            if (datasetId == -1L)
            {
                string error = $"Unable to create dataset for {normalizedName}";
                Hdf5Utils.LogMessage($"{normalizedName} already exists", Hdf5LogLevel.Error);
                if (Hdf5.Settings.ThrowOnError)
                    throw new Hdf5Exception(error);
            }
            return datasetId;
        }

        public static Type GetEnumerableType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            var iface = (type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))).FirstOrDefault();

            if (iface == null)
                throw new ArgumentException($"{type} Does not represent an enumerable type.", type.Name);

            return GetEnumerableType(iface);
        }
    }
}
