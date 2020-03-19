using System;

namespace Hdf5DotnetWrapper.DataTypes
{
    public class HObject
    {
        public static string SEPARATOR = "/";
        private string filename;

        /**
         * The file which contains the object
         */
        protected FileFormat fileFormat;

        /**
 * The name of the data object. The root group has its default name, a
 * slash. The name can be changed except the root group.
 */
        private string name;

        /**
         * The full path of the data object. The full path always starts with the
         * root, a slash. The path cannot be changed. Also, a path must be ended with a
         * slash. For example, /arrays/ints/
         */
        private string path;

        /** The full name of the data object, i.e. "path + name" */
        private string fullName;

        /**
 * Array of long integer storing unique identifier for the object.
 * <p>
 * HDF4 objects are uniquely identified by a (tag_id, ref_id) pair. i.e.
 * oid[0] = tag, oid[1] = ref_id.<br>
 * HDF5 objects are uniquely identified by an object reference. i.e.
 * oid[0] = obj_id.
 */
        //protected long[] oid { get; set; }
        protected IntPtr address;

        /**
   * The name of the Target Object that is being linked to.
   */
        protected string linkTargetObjName;

        /**
 * Constructs an instance of a data object without name and path.
 */
        public HObject() : this(null, null, null, IntPtr.Zero)
        {

        }

        /**
         * Constructs an instance of a data object with specific name and path.
         * <p>
         * For example, in H5ScalarDS(h5file, "dset", "/arrays"), "dset" is the name
         * of the dataset, "/arrays" is the group path of the dataset.
         *
         * @param theFile
         *            the file that contains the data object.
         * @param theName
         *            the name of the data object, e.g. "dset".
         * @param thePath
         *            the group path of the data object, e.g. "/arrays".
         */
        public HObject(FileFormat theFile, string theName, string thePath) : this(theFile, theName, thePath, IntPtr.Zero)
        {

        }

        public HObject(FileFormat theFile, string theName, string thePath, IntPtr oid)
        {
            fileFormat = theFile;
            this.address = oid;

            if (fileFormat != null)
            {
                filename = fileFormat.GetFilePath();
            }
            else
            {
                filename = null;
            }

            // file name is packed in the full path
            if ((theName == null) && (thePath != null))
            {
                if (thePath.Equals(SEPARATOR))
                {
                    theName = SEPARATOR;
                    thePath = null;
                }
                else
                {
                    // the path must starts with "/"
                    if (!thePath.StartsWith(SEPARATOR))
                    {
                        thePath = SEPARATOR + thePath;
                    }

                    // get rid of the last "/"
                    if (thePath.EndsWith(SEPARATOR))
                    {
                        thePath = thePath.Substring(0, thePath.Length - 1);
                    }

                    // separate the name and the path
                    theName = thePath.Substring(thePath.LastIndexOf(SEPARATOR) + 1);
                    thePath = thePath.Substring(0, thePath.LastIndexOf(SEPARATOR));
                }
            }
            else if ((theName != null) && (thePath == null) && (theName.IndexOf(SEPARATOR) >= 0))
            {
                if (theName.Equals(SEPARATOR))
                {
                    theName = SEPARATOR;
                    thePath = null;
                }
                else
                {
                    // the full name must starts with "/"
                    if (!theName.StartsWith(SEPARATOR))
                    {
                        theName = SEPARATOR + theName;
                    }

                    // the fullname must not end with "/"
                    int n = theName.Length;
                    if (theName.EndsWith(SEPARATOR))
                    {
                        theName = theName.Substring(0, n - 1);
                    }

                    int idx = theName.LastIndexOf(SEPARATOR);
                    if (idx < 0)
                    {
                        thePath = SEPARATOR;
                    }
                    else
                    {
                        thePath = theName.Substring(0, idx);
                        theName = theName.Substring(idx + 1);
                    }
                }
            }

            // the path must start and end with "/"
            if (thePath != null)
            {
                thePath = thePath.Replace("//", "/");
                if (!thePath.EndsWith(SEPARATOR))
                {
                    thePath += SEPARATOR;
                }
            }

            name = theName;
            path = thePath;

            Hdf5Utils.LogInfo?.Invoke($"name={name} path={path}");

            if (thePath != null)
            {
                fullName = thePath + theName;
            }
            else
            {
                if (theName == null)
                {
                    fullName = "/";
                }
                else if (theName.StartsWith("/"))
                {
                    fullName = theName;
                }
                else
                {
                    if (this is HAttribute)
                    {
                        fullName = theName;
                    }
                    else
                    {
                        fullName = "/" + theName;
                    }

                }
            }

            Hdf5Utils.LogInfo?.Invoke("fullName=" + fullName);
        }
        public FileFormat getFileFormat()
        {
            return fileFormat;
        }
        /**
  * Returns the full name (group path + object name) of the object. For
  * example, "/Images/Raster Image #2"
  *
  * @return The full name (group path + object name) of the object.
  */
        public string getFullName()
        {
            return fullName;
        }

        /**
         * Returns the group path of the object. For example, "/Images".
         *
         * @return The group path of the object.
         */
        public string getPath()
        {
            return path;
        }

        /**
         * Returns the name of the file that contains this data object.
         * <p>
         * The file name is necessary because the file of this data object is
         * uniquely identified when multiple files are opened by an application at
         * the same time.
         *
         * @return The full path (path + name) of the file.
         */
        public string getFile()
        {
            return filename;
        }
        public bool equalsOID(IntPtr theID)
        {
            return theID.Equals(address);
            //if ((oid == null))
            //{
            //    return false;
            //}

            //int n1 = theID.Length;
            //int n2 = oid.Length;

            //if (n1 == 0 || n2 == 0)
            //{
            //    return false;
            //}

            //int n = Math.Min(n1, n2);
            //bool isMatched = (theID[0] == oid[0]);

            //for (int i = 1; isMatched && (i < n); i++)
            //{
            //    isMatched = (theID[i] == oid[i]);
            //}

            //return isMatched;
        }

        public long getFID()
        {
            if (fileFormat != null)
            {
                return fileFormat.getFID();
            }
            else
            {
                return -1;
            }
        }
        /**
         * Returns the name of the object. For example, "Raster Image #2".
         *
         * @return The name of the object.
         */
        public string getName()
        {
            return name;
        }


        /**
         * Sets the path of the object.
         * <p>
         * setPath() is needed to change the path for an object when the name of a
         * group containing the object is changed by setName(). The path of the
         * object in memory under this group should be updated to the new path to
         * the group. Unlike setName(), setPath() does not change anything in file.
         *
         * @param newPath
         *            The new path of the object.
         *
         * @throws Exception if a failure occurred
         */
        public void setPath(string newPath)
        {
            if (newPath == null)
            {
                newPath = "/";
            }

            path = newPath;
        }

        /**
         * Sets the name of the object.
         *
         * setName (String newName) changes the name of the object in the file.
         *
         * @param newName
         *            The new name of the object.
         *
         * @throws Exception if name is root or contains separator
         */
        public void setName(string newName)
        {
            if (newName != null)
            {
                if (newName.Equals(SEPARATOR))
                {
                    throw new Exception("The new name cannot be the root");
                }

                if (newName.StartsWith(HObject.SEPARATOR))
                {
                    newName = newName.Substring(1);
                }

                if (newName.EndsWith(HObject.SEPARATOR))
                {
                    newName = newName.Substring(0, newName.Length - 2);
                }

                if (newName.Contains(HObject.SEPARATOR))
                {
                    throw new Exception("The new name contains the SEPARATOR character: " + HObject.SEPARATOR);
                }
            }

            name = newName;
        }

    }
}

