using HDF.PInvoke;
using System;
using System.Collections.Generic;

namespace Hdf5DotnetWrapper
{
    using hid_t = Int64;

    public static partial class Hdf5
    {

        public static int CloseGroup(long groupId)
        {
            return H5G.close(groupId);
        }

        public static long CreateGroup(long groupId, string groupName)
        {

            return (GroupExists(groupId, groupName))
                ? H5G.open(groupId, Hdf5Utils.NormalizedName(groupName))
                : H5G.create(groupId, Hdf5Utils.NormalizedName(groupName));
        }




        /// <summary>
        /// creates a structure of groups at once
        /// </summary>
        /// <param name="groupOrfileId"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static long CreateGroupRecursively(long groupOrfileId, string groupName)
        {
            IEnumerable<string> grps = groupName.Split('/');
            long gid = groupOrfileId;
            groupName = "";
            foreach (var name in grps)
            {
                groupName = string.Concat(groupName, "/", name);
                gid = CreateGroup(gid, groupName);
            }
            return gid;
        }

        public static bool GroupExists(long groupId, string groupName)
        {
            bool exists = false;
            try
            {
                H5G.info_t info = new H5G.info_t();
                var gid = H5G.get_info_by_name(groupId, Hdf5Utils.NormalizedName(groupName), ref info);
                exists = gid == 0;
            }
            catch (Exception)
            {
            }
            return exists;
        }

        public static ulong NumberOfAttributes(int groupId, string groupName)
        {
            H5O.info_t info = new H5O.info_t();
            var gid = H5O.get_info(groupId, ref info);
            return info.num_attrs;
        }

        public static H5G.info_t GroupInfo(long groupId)
        {
            H5G.info_t info = new H5G.info_t();
            var gid = H5G.get_info(groupId, ref info);
            return info;
        }
    }
}
