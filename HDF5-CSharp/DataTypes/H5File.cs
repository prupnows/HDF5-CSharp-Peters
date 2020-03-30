using System;
using System.IO;
using System.Runtime.InteropServices;
using HDF.PInvoke;

namespace HDF5CSharp.DataTypes
{
    public class H5File : FileFormat
    {
        private static long serialVersionUID = 6247335559471526045L;

        /**
         * the file access flag. Valid values are HDF5Constants.H5F_ACC_RDONLY, HDF5Constants.H5F_ACC_RDWR and
         * HDF5Constants.H5F_ACC_CREAT.
         */
        private uint flag;

        /**
         * The index type. Valid values are HDF5Constants.H5_INDEX_NAME, HDF5Constants.H5_INDEX_CRT_ORDER.
         */
        private H5.index_t indexType = HDF5Constants.H5_INDEX_NAME;

        /**
         * The index order. Valid values are HDF5Constants.H5_ITER_INC, HDF5Constants.H5_ITER_DEC.
         */
        private H5.iter_order_t indexOrder = HDF5Constants.H5_ITER_INC;

        /**
         * The root object of the file hierarchy.
         */
        private HObject rootObject;

        /**
         * How many characters maximum in an attribute name?
         */
        private static int attrNameLen = 256;

        /**
         * The library version bounds
         */
        private int[] libver;
        public static int LIBVER_LATEST = HDF5Constants.H5F_LIBVER_LATEST;
        public static int LIBVER_EARLIEST = HDF5Constants.H5F_LIBVER_EARLIEST;
        public static int LIBVER_V18 = HDF5Constants.H5F_LIBVER_V18;
        public static int LIBVER_V110 = HDF5Constants.H5F_LIBVER_V110;

        public H5File()
        {
            FileList.Add("hdf.object.h5.H5File", this);
        }
        public override HObject getRootObject()
        {
            return rootObject;
        }
        public void ReadFileStructure(string filename)
        {
            if (File.Exists(filename))
            {
                fullFileName = filename;
                // var fileId = Hdf5.OpenFile(filename, readOnly: true);
                open(true);
                rootObject = new H5Group(this, "/", null, null);
                depth_first(rootObject, 0);
            }
        }
        private int depth_first(HObject parentObject, int nTotal)
        {

            Hdf5Utils.LogInfo?.Invoke($"depth_first({parentObject}): start");

            int nelems;
            string fullPath = null;
            string ppath = null;
            long gid = -1;

            H5Group pgroup = (H5Group)parentObject;
            ppath = pgroup.getPath();

            if (ppath == null)
            {
                fullPath = HObject.SEPARATOR;
            }
            else
            {
                fullPath = ppath + pgroup.getName() + HObject.SEPARATOR;
            }

            nelems = 0;
            try
            {
                gid = pgroup.open();
                var info = new H5G.info_t();
                H5G.get_info(gid, ref info);
                nelems = (int)info.nlinks;
            }
            catch (Exception ex)
            {
                nelems = -1;
                Hdf5Utils.LogError?.Invoke($"depth_first({parentObject}): H5Gget_info(gid {gid}) failure: {ex}");
            }

            if (nelems <= 0)
            {
                pgroup.close(gid);
                Hdf5Utils.LogInfo?.Invoke($"depth_first({parentObject}): nelems <= 0");
                Hdf5Utils.LogInfo?.Invoke($"depth_first({parentObject}): finish");
                return nTotal;
            }

            // since each call of H5.H5Gget_objname_by_idx() takes about one second.
            // 1,000,000 calls take 12 days. Instead of calling it in a loop,
            // we use only one call to get all the information, which takes about
            // two seconds
            int[] objTypes = new int[nelems];
            //long[] fNos = new long[nelems];
            //long[] objRefs = new long[nelems];
            string[] objNames = new string[nelems];
            H5L.info_t[] infos = new H5L.info_t[nelems];
            try
            {
                int i = 0;
                int callback(long group, IntPtr name, ref H5L.info_t info, IntPtr op_data)
                {
                    string realName = Marshal.PtrToStringAuto(name);
                    objTypes[i] = (int)info.type;
                    objNames[i] = realName;
                    infos[i] = info;

                    return i++;
                }

                ulong pos = 0;
                H5L.iterate(gid, indexType, indexOrder, ref pos, callback, IntPtr.Zero);

                //for (ulong i = 0; i < (ulong)nelems; i++)
                //{


                //    H5G.info_t info = new H5G.info_t();
                //    H5G.get_info_by_idx(fid, fullPath, indexType, indexOrder, i, ref info);
                //    infos[i] = info;


                //}

                // H5.H5Gget_obj_info_full(fid, fullPath, objNames, objTypes, null, fNos, objRefs, indexType, indexOrder);
            }
            catch (Exception ex)
            {
                Hdf5Utils.LogError?.Invoke($"depth_first({parentObject}): failure: {ex}");
                Hdf5Utils.LogError?.Invoke($"depth_first({parentObject}): finish");
                return nTotal;
            }

            int nStart = getStartMembers();
            int nMax = getMaxMembers();

            string obj_name;
            int obj_type;

            // Iterate through the file to see members of the group
            for (int i = 0; i < nelems; i++)
            {
                obj_name = objNames[i];
                obj_type = objTypes[i];
                Hdf5Utils.LogInfo?.Invoke($"depth_first({parentObject}): obj_name={obj_name}, obj_type={obj_type}");

                if (obj_name == null)
                {
                    Hdf5Utils.LogInfo?.Invoke($"depth_first({parentObject}): continue after null obj_name");
                    continue;
                }

                nTotal++;

                if (nMax > 0)
                {
                    if ((nTotal - nStart) >= nMax)
                        break; // loaded enough objects
                }

                bool skipLoad = (nTotal > 0) && (nTotal < nStart);

                // create a new group
                if (obj_type == HDF5Constants.H5O_TYPE_GROUP)
                {
                    H5Group g = new H5Group(this, obj_name, fullPath, pgroup);

                    pgroup.addToMemberList(g);

                    // detect and stop loops
                    // a loop is detected if there exists object with the same
                    // object ID by tracing path back up to the root.
                    bool hasLoop = false;
                    H5Group tmpObj = (H5Group)parentObject;

                    while (tmpObj != null)
                    {
                        if (tmpObj.equalsOID(new IntPtr((int)infos[i].u.address)) && (tmpObj.getPath() != null))
                        {
                            hasLoop = true;
                            break;
                        }
                        else
                        {
                            tmpObj = (H5Group)tmpObj.getParent();
                        }
                    }

                    // recursively go through the next group
                    // stops if it has loop.
                    if (!hasLoop)
                    {
                        nTotal = depth_first(g, nTotal);
                    }
                }
                else if (skipLoad)
                {
                    continue;
                }
                else if (obj_type == HDF5Constants.H5O_TYPE_DATASET)
                {
                    long did = -1;
                    long tid = -1;
                    H5T.class_t tclass = H5T.class_t.NO_CLASS;
                    try
                    {
                        did = H5D.open(fid, fullPath + obj_name, HDF5Constants.H5P_DEFAULT);
                        if (did >= 0)
                        {
                            tid = H5D.get_type(did);

                            tclass = H5T.get_class(tid);
                            if ((tclass == HDF5Constants.H5T_ARRAY) || (tclass == HDF5Constants.H5T_VLEN))
                            {
                                // for ARRAY, the type is determined by the base type
                                long btid = H5T.get_super(tid);

                                tclass = H5T.get_class(btid);

                                try
                                {
                                    H5T.close(btid);
                                }
                                catch (Exception ex)
                                {
                                    Hdf5Utils.LogInfo?.Invoke($"depth_first({parentObject})[{i}] dataset {obj_name} H5Tclose(btid {btid}) failure: {ex}");
                                }
                            }
                        }
                        else
                        {
                            Hdf5Utils.LogError?.Invoke($"depth_first({parentObject})[{i}] {obj_name} dataset open failure");
                        }
                    }
                    catch (Exception ex)
                    {
                        Hdf5Utils.LogError?.Invoke($"depth_first({parentObject})[{i}] {obj_name} dataset access failure: {ex}");
                    }
                    finally
                    {
                        try
                        {
                            H5T.close(tid);
                        }
                        catch (Exception ex)
                        {
                            Hdf5Utils.LogError?.Invoke($"depth_first({parentObject})[{i}] dataset {obj_name} H5Tclose(tid {tid}) failure: {ex}");
                        }
                        try
                        {
                            H5D.close(did);
                        }
                        catch (Exception ex)
                        {
                            Hdf5Utils.LogInfo?.Invoke($"depth_first({parentObject})[{i}] dataset {obj_name} H5Dclose(did {did}) failure: {ex}");
                        }
                    }
                    //todo:
                    //Dataset d = null;
                    //if (tclass == HDF5Constants.H5T_COMPOUND)
                    //{
                    //    // create a new compound dataset
                    //    d = new H5CompoundDS(this, obj_name, fullPath, oid); // deprecated!
                    //}
                    //else
                    //{
                    //    // create a new scalar dataset
                    //    d = new H5ScalarDS(this, obj_name, fullPath, oid); // deprecated!
                    //}

                    // pgroup.addToMemberList(d);
                }
                else if (obj_type == HDF5Constants.H5O_TYPE_NAMED_DATATYPE)
                {
                    //Datatype t = new H5Datatype(this, obj_name, fullPath, oid); // deprecated!

                    //pgroup.addToMemberList(t);
                }
                else if (obj_type == HDF5Constants.H5O_TYPE_UNKNOWN)
                {
                    //H5Link link = new H5Link(this, obj_name, fullPath, oid);

                    // pgroup.addToMemberList(link);
                    continue; // do the next one, if the object is not identified.
                }
            } // ( i = 0; i < nelems; i++)

            pgroup.close(gid);

            Hdf5Utils.LogInfo?.Invoke($"depth_first({parentObject}): finish");
            return nTotal;
        }

        public override long open()
        {
            Hdf5Utils.LogInfo?.Invoke("open()");
            return open(true);
        }
        private long open(bool loadFullHierarchy)
        {
            long the_fid = -1;

            long plist = HDF5Constants.H5P_DEFAULT;

            // BUG: HDF5Constants.H5F_CLOSE_STRONG does not flush cache
            /*
             * try { //All open objects remaining in the file are closed // then file is closed plist =
             * H5.H5Pcreate (HDF5Constants.H5P_FILE_ACCESS); H5.H5Pset_fclose_degree ( plist,
             * HDF5Constants.H5F_CLOSE_STRONG); } catch (Exception ex) {} the_fid = open(loadFullHierarchy,
             * plist); try { H5.H5Pclose(plist); } catch (Exception ex) {}
             */

            Hdf5Utils.LogInfo?.Invoke("open(): loadFull=" + loadFullHierarchy);
            the_fid = open(loadFullHierarchy, plist);
            return the_fid;
        }


        /**
         * Opens access to this file.
         *
         * @param loadFullHierarchy
         *            if true, load the full hierarchy into memory; otherwise just opens the file identifier.
         *
         * @return the file identifier if successful; otherwise returns negative value.
         *
         * @throws Exception
         *            If there is a failure.
         */
        private long open(bool loadFullHierarchy, long plist)
        {
            Hdf5Utils.LogInfo?.Invoke($"open(loadFullHierarchy = {loadFullHierarchy}, plist = {plist}): start");
            if (fid > 0)
            {
                Hdf5Utils.LogInfo?.Invoke("open(): FID already opened");
                Hdf5Utils.LogInfo?.Invoke("open(): finish");
                return fid; // file is opened already
            }

            // The cwd may be changed at Dataset.read() by System.setProperty("user.dir", newdir)
            // to make it work for external datasets. We need to set it back
            // before the file is closed/opened.
            string rootPath = Environment.CurrentDirectory;
            if (!File.Exists(fullFileName))
            {
                Hdf5Utils.LogInfo?.Invoke($"open(): File {fullFileName} does not exist");
                Hdf5Utils.LogInfo?.Invoke("open(): finish");
                throw new Exception("File does not exist -- " + fullFileName);
            }
            else if ((flag == HDF5Constants.H5F_ACC_RDONLY) && new FileInfo(fullFileName).IsReadOnly)
            {
                Hdf5Utils.LogInfo?.Invoke($"open(): Cannot read file {fullFileName}");
                Hdf5Utils.LogInfo?.Invoke("open(): finish");
                throw new Exception("Cannot read file -- " + fullFileName);
            }
            // check for valid file access permission
            else if (flag < 0)
            {
                Hdf5Utils.LogInfo?.Invoke("open(): Invalid access identifier -- " + flag);
                Hdf5Utils.LogInfo?.Invoke("open(): finish");
                throw new Exception("Invalid access identifer -- " + flag);
            }
            else if (HDF5Constants.H5F_ACC_CREAT == flag)
            {
                // create a new file
                Hdf5Utils.LogInfo?.Invoke("open(): create file");
                fid = H5F.create(fullFileName, HDF5Constants.H5F_ACC_TRUNC, HDF5Constants.H5P_DEFAULT,
                        HDF5Constants.H5P_DEFAULT);
                H5F.flush(fid, HDF5Constants.H5F_SCOPE_LOCAL);
                H5F.close(fid);
                flag = HDF5Constants.H5F_ACC_RDWR;
            }

            else if (((flag == HDF5Constants.H5F_ACC_RDWR) || (flag == HDF5Constants.H5F_ACC_CREAT)) && new FileInfo(fullFileName).IsReadOnly)
            {
                Hdf5Utils.LogInfo?.Invoke($"open(): Cannot write file {fullFileName}");
                Hdf5Utils.LogInfo?.Invoke("open(): finish");
                throw new Exception("Cannot write file, try opening as read-only -- " + fullFileName);
            }


            try
            {
                Hdf5Utils.LogInfo?.Invoke("open(): open file");
                fid = H5F.open(fullFileName, flag, plist);
            }
            catch (Exception ex)
            {
                try
                {
                    Hdf5Utils.LogInfo?.Invoke("open(): open failed, attempting to open file read-only");
                    fid = H5F.open(fullFileName, HDF5Constants.H5F_ACC_RDONLY, HDF5Constants.H5P_DEFAULT);
                    isReadOnly = true;
                }
                catch (Exception ex2)
                {
                    Hdf5Utils.LogError?.Invoke("open(): open failed:" + ex2);

                }
            }

            if ((fid >= 0) && loadFullHierarchy)
            {
                // load the hierarchy of the file
                Hdf5Utils.LogInfo?.Invoke("open(loadFullHeirarchy): load the hierarchy");
                loadIntoMemory();
            }

            Hdf5Utils.LogInfo?.Invoke($"open(loadFullHeirarchy = {loadFullHierarchy}, plist = {plist}): finish");
            return fid;
        }

        private void loadIntoMemory()
        {
            Hdf5Utils.LogInfo?.Invoke("loadIntoMemory(): start");
            if (fid < 0)
            {
                Hdf5Utils.LogError?.Invoke("loadIntoMemory(): Invalid FID");
                return;
            }

            /*
             * TODO: Root group's name should be changed to 'this.getName()' and all
             * previous accesses of this field should now use getPath() instead of getName()
             * to get the root group. The root group actually does have a path of "/". The
             * depth_first method will have to be changed to setup other object paths
             * appropriately, as it currently assumes the root path to be null.
             */
            rootObject = new H5Group(this, "/", null, null);
            Hdf5Utils.LogInfo?.Invoke("loadIntoMemory(): depth_first on root");
            depth_first(rootObject, 0);
            Hdf5Utils.LogInfo?.Invoke("loadIntoMemory(): finish");
        }

    }
}
