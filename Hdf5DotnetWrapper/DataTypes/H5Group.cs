using HDF.PInvoke;
using System;

namespace Hdf5DotnetWrapper.DataTypes
{
    public class H5Group : Group
    {
        private static long serialVersionUID = -951164512330444150L;
        //protected List<> attributeList;

        private int nAttributes = -1;

        private H5O.info_t obj_info;

        /**
 * Constructs an HDF5 group with specific name, path, and parent.
 *
 * @param theFile
 *            the file which containing the group.
 * @param name
 *            the name of this group, e.g. "grp01".
 * @param path
 *            the full path of this group, e.g. "/groups/".
 * @param parent
 *            the parent of this group.
 */
        public H5Group(FileFormat theFile, string name, string path, Group parent) : this(theFile, name, path, parent, IntPtr.Zero)
        {

        }

        /**
         * @deprecated Not for public use in the future.<br>
         *             Using {@link #H5Group(FileFormat, String, String, Group)}
         *
         * @param theFile
         *            the file which containing the group.
         * @param name
         *            the name of this group, e.g. "grp01".
         * @param path
         *            the full path of this group, e.g. "/groups/".
         * @param parent
         *            the parent of this group.
         * @param oid
         *            the oid of this group.
         */
        [Obsolete]
        public H5Group(FileFormat theFile, String name, String path, Group parent, IntPtr oid) : base(theFile, name, path, parent, oid)
        {
            nMembersInFile = -1;
            obj_info = new H5O.info_t();

            if ((oid == null) && (theFile != null))
            {
                // throw new Exception("??");
                // retrieve the object ID
                try
                {
                    IntPtr ptr = new IntPtr();
                    var reference = H5R.create(ptr, theFile.getFID(), this.getFullName(), H5R.type_t.OBJECT, -1);
                    //todo:
                    address = ptr;
                    //this.oid = new long[1];
                    //this.oid[0] = HDFNativeData.byteToLong(ref_buf, 0);
                }
                catch (Exception ex)
                {
                    Hdf5Utils.LogError?.Invoke("ERROR: " + ex);
                }
            }
        }

        public long open()
        {
            Hdf5Utils.LogInfo?.Invoke("open(): start");
            long gid = -1;

            try
            {
                if (isRoot())
                {
                    gid = H5G.open(getFID(), SEPARATOR, HDF5Constants.H5P_DEFAULT);
                }
                else
                {
                    gid = H5G.open(getFID(), getPath() + getName(), HDF5Constants.H5P_DEFAULT);
                }

            }
            catch (Exception ex)
            {
                gid = -1;
                Hdf5Utils.LogError?.Invoke("open(): Error:" + ex);
            }

            Hdf5Utils.LogInfo?.Invoke("open(): finish");
            return gid;
        }
        public void close(long gid)
        {
            try
            {
                H5G.close(gid);
            }
            catch (Exception ex)
            {
                Hdf5Utils.LogError?.Invoke($"close(): H5Gclose(gid {gid}): {ex}");
            }
        }
    }
}
