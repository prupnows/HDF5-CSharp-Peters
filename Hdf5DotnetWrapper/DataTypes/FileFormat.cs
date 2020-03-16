using System;
using System.Collections.Generic;
using System.IO;

namespace Hdf5DotnetWrapper.DataTypes
{
    public enum FileAccess
    {
        OPEN_NEW = 1,
        READ = 2,
        WRITE = 4,
        CREATE = 8,
        FILE_CREATE_DELETE = 10,
        FILE_CREATE_OPEN = 20,
        FILE_CREATE_EARLY_LIB = 40
    }
    public abstract class FileFormat
    {   /** Key for HDF4 file format. */
        public static string FILE_TYPE_HDF4 = "HDF4";

        /** Key for HDF5 file format. */
        public static string FILE_TYPE_HDF5 = "HDF5";
        public static string FILE_OBJ_SEP = "://";

        protected static Dictionary<string, FileFormat> FileList = new Dictionary<string, FileFormat>();

        /**
         * A list of file extensions for the supported file formats. This list of
         * file extensions is not integrated with the supported file formats kept in
         * FileList, but is provided as a convenience for applications who may
         * choose to process only those files with recognized extensions.
         */
        private static string extensions = "hdf, h4, hdf5, h5, nc, fits";

        /***************************************************************************
         * Sizing information and class metadata
         **************************************************************************/
        private int max_members = 100000;      // 10,000 by default
        /**
         * Current Java applications, such as HDFView, cannot handle files with
         * large numbers of objects due to JVM memory limitations. For example,
         * 1,000,000 objects is too many. max_members is defined so that
         * applications such as HDFView will load up to <i>max_members</i> objects
         * starting with the <i>start_members</i> -th object. The implementing class
         * has freedom in its interpretation of how to "count" objects in the file.
         */
        private int start_members = 0;          // 0 by default

        /**
         * File identifier. -1 indicates the file is not open.
         */
        protected long fid = -1;

        /**
         * The absolute pathname (path+name) of the file.
         */
        protected string fullFileName = null;

        /**
         * Flag indicating if the file access is read-only.
         */
        protected bool isReadOnly = false;


        public FileFormat()
        {
            isReadOnly = false;
            fullFileName = "";
        }
        public FileFormat(string fileName)
        {
            isReadOnly = false;
            fullFileName = fileName;
            try
            {
                fullFileName = Path.GetFullPath(fileName);
            }
            catch (Exception e)
            {
                Hdf5Utils.LogError?.Invoke("Error: " + e);
            }


        }

        /**
         * Returns the root object for the file associated with this instance.
         * <p>
         * The root object is an HObject that represents the root group of a
         * file. If the file has not yet been opened, or if there is no file
         * associated with this instance, <code>null</code> will be returned.
         * <p>
         * Starting from the root, applications can descend through the tree
         * structure and navigate among the file's objects. In the tree structure,
         * internal items represent non-empty groups. Leaf items represent datasets,
         * named datatypes, or empty groups.
         *
         * @return The root object of the file, or <code>null</code> if there is no
         *         associated file or if the associated file has not yet been
         *         opened.
         * @see #open()
         */
        public abstract HObject getRootObject();
        public string GetFilePath()
        {
            return fullFileName;
        }
        public int getMaxMembers()
        {
            if (max_members < 0)
                return int.MaxValue; // load the whole file

            return max_members;
        }
        public long getFID()
        {
            return fid;
        }

        public int getStartMembers()
        {
            return start_members;
        }
        public abstract long open();
    }
}
