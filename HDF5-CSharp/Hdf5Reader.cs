using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using HDF.PInvoke;
using HDF5CSharp.DataTypes;

namespace HDF5CSharp
{
    public partial class Hdf5
    {

        public static T ReadObject<T>(long groupId, T readValue, string groupName)
        {
            if (readValue == null)
            {
                throw new ArgumentNullException(nameof(readValue));
            }

            Type tyObject = readValue.GetType();
            //foreach (Attribute attr in Attribute.GetCustomAttributes(tyObject))
            //{
            //    if (attr is Hdf5GroupName)
            //        groupName = (attr as Hdf5GroupName).Name;
            //    if (attr is Hdf5SaveAttribute)
            //    {
            //        Hdf5SaveAttribute atLeg = attr as Hdf5SaveAttribute;
            //        if (atLeg.SaveKind == Hdf5Save.DoNotSave)
            //            return readValue;
            //    }
            //}
            bool isGroupName = !string.IsNullOrWhiteSpace(groupName);
            if (isGroupName)
                groupId = H5G.open(groupId, Hdf5Utils.NormalizedName(groupName));

            ReadFields(tyObject, readValue, groupId);
            ReadProperties(tyObject, readValue, groupId);

            if (isGroupName)
                CloseGroup(groupId);
            return readValue;
        }

        public static T ReadObject<T>(long groupId, string groupName) where T : new()
        {
            T readValue = new T();
            return ReadObject(groupId, readValue, groupName);
        }

        private static void ReadFields(Type tyObject, object readValue, long groupId)
        {
            FieldInfo[] miMembers = tyObject.GetFields(BindingFlags.DeclaredOnly |
                                                       /*BindingFlags.NonPublic |*/ BindingFlags.Public |
                                                       BindingFlags.Instance);

            foreach (FieldInfo info in miMembers)
            {
                bool nextInfo = false;
                string alternativeName = string.Empty;
                foreach (Attribute attr in Attribute.GetCustomAttributes(info))
                {
                    if (attr is Hdf5EntryNameAttribute nameAttribute)
                    {
                        alternativeName = nameAttribute.Name;
                    }

                    if (attr is Hdf5SaveAttribute attribute)
                    {
                        Hdf5Save kind = attribute.SaveKind;
                        nextInfo = (kind == Hdf5Save.DoNotSave);
                    }
                    else
                        nextInfo = false;
                }

                if (nextInfo) continue;

                Type ty = info.FieldType;
                TypeCode code = Type.GetTypeCode(ty);

                string name = info.Name;
                Hdf5Utils.LogDebug?.Invoke($"groupname: {tyObject.Name}; field name: {name}");
                bool success;
                Array values;
                if (ty.IsArray)
                {
                    var elType = ty.GetElementType();
                    TypeCode elCode = Type.GetTypeCode(elType);

                    if (elCode != TypeCode.Object)
                    {
                        (success, values) = dsetRW.ReadArray(elType, groupId, name, alternativeName);
                    }
                    else
                    {
                        (success, values) = CallByReflection<(bool, Array)>(nameof(ReadCompounds), elType,
                            new object[] {groupId, name});
                    }

                    if (success)
                        info.SetValue(readValue, values);
                }
                else if (primitiveTypes.Contains(code) || ty == typeof(TimeSpan))
                {
                    (success, values) = dsetRW.ReadArray(ty, groupId, name, alternativeName);
                    // get first value depending on rank of the matrix
                    int[] first = new int[values.Rank].Select(f => 0).ToArray();
                    if (success)
                        info.SetValue(readValue, values.GetValue(first));
                }
                else
                {
                    object value = info.GetValue(readValue);
                    if (value != null)
                        ReadObject(groupId, value, name);
                }
            }
        }

        private static void ReadProperties(Type tyObject, object readValue, long groupId)
        {
            PropertyInfo[] miMembers = tyObject.GetProperties( /*BindingFlags.DeclaredOnly |*/
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo info in miMembers)
            {
                bool nextInfo = false;
                string alternativeName = string.Empty;
                foreach (Attribute attr in Attribute.GetCustomAttributes(info))
                {
                    if (attr is Hdf5SaveAttribute hdf5SaveAttribute)
                    {
                        Hdf5Save kind = hdf5SaveAttribute.SaveKind;
                        nextInfo = (kind == Hdf5Save.DoNotSave);
                    }

                    if (attr is Hdf5EntryNameAttribute hdf5EntryNameAttribute)
                    {
                        alternativeName = hdf5EntryNameAttribute.Name;
                    }
                }

                if (nextInfo) continue;
                Type ty = info.PropertyType;
                TypeCode code = Type.GetTypeCode(ty);
                string name = info.Name;

                bool success;
                Array values;
                if (ty.IsArray)
                {
                    var elType = ty.GetElementType();
                    TypeCode elCode = Type.GetTypeCode(elType);


                    if (elCode != TypeCode.Object || ty == typeof(TimeSpan[]))
                    {
                        (success, values) = dsetRW.ReadArray(elType, groupId, name, alternativeName);
                        if (success)
                            info.SetValue(readValue, values);
                    }
                    else
                    {
                        var obj = CallByReflection<IEnumerable>(nameof(ReadCompounds), elType,
                            new object[] {groupId, name});
                        var objArr = (obj).Cast<object>().ToArray();
                        values = Array.CreateInstance(elType, objArr.Length);
                        Array.Copy(objArr, values, objArr.Length);
                        info.SetValue(readValue, values);
                    }

                }
                else if (primitiveTypes.Contains(code) || ty == typeof(TimeSpan))
                {
                    (success, values) = dsetRW.ReadArray(ty, groupId, name, alternativeName);
                    if (success && values.Length > 0)
                    {
                        int[] first = new int[values.Rank].Select(f => 0).ToArray();
                        info.SetValue(readValue, values.GetValue(first));
                    }
                }
                else
                {
                    object value = info.GetValue(readValue, null);
                    if (value != null)
                    {
                        value = ReadObject(groupId, value, name);
                        info.SetValue(readValue, value);
                    }
                }
            }
        }

        public static List<Hdf5Element> ReadFileStructure(string fileName)
        {
            var structure = new List<Hdf5Element>();
            if (!File.Exists(fileName))
            {
                Hdf5Utils.LogError?.Invoke($"File {fileName} does not exist");
                return structure;
            }

            long fileId = H5F.open(fileName, H5F.ACC_RDONLY);
            if (fileId < 0)
            {
                Hdf5Utils.LogError?.Invoke($"Could not open file {fileName}");
                return structure;
            }
            StringBuilder filePath= new StringBuilder(260);
            H5F.get_name(fileId, filePath, new IntPtr(260));
            ulong idx;

            idx = 0;
            H5L.iterate(fileId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, Callback,
                Marshal.StringToHGlobalAnsi("/"));
            return structure;

            int Callback(long campaignGroupId, IntPtr intPtrName, ref H5L.info_t info, IntPtr intPtrUserData)
            {
                ulong idx2 = 0;

                long groupId = -1;
                long datasetId = -1;

                int level;

                string name;
                string fullName;
                string userData;

                H5O.type_t objectType;
                Hdf5Element currentElement;
          
                name = Marshal.PtrToStringAnsi(intPtrName);
                userData = Marshal.PtrToStringAnsi(intPtrUserData);
                fullName = CombinePath(userData, name);
                level = userData.Split("/".ToArray()).Count();

                // this is necessary, since H5Oget_info_by_name is slow because it wants verbose object header data 
                // and H5G_loc_info is not directly accessible
                // only chance is to modify source code (H5Oget_info_by_name)
                datasetId = H5D.open(campaignGroupId, name);

                if (H5I.is_valid(datasetId) > 0)
                {
                    objectType = H5O.type_t.DATASET;
                }
                else
                {
                    groupId = H5G.open(campaignGroupId, name);

                    objectType = H5I.is_valid(groupId) > 0 ? H5O.type_t.GROUP : H5O.type_t.UNKNOWN;
                }

                switch (level)
                {
                    case 1:
                    case 2:
                        break;

                    case 3:

                        if (objectType == H5O.type_t.GROUP)
                        {
                            //if (!string.IsNullOrWhiteSpace(campaignGroupPath) && fullName != campaignGroupPath)
                            //{
                            //    return 0;
                            //}

                            currentElement = structure.FirstOrDefault(e => e.Name == fullName);

                            if (currentElement == null)
                            {
                                currentElement = new Hdf5Element(fullName, null, false);
                                structure.Add(currentElement);
                            }
                        }

                        break;
                }

                if (objectType == H5O.type_t.GROUP && level < 3)
                {
                    H5L.iterate(groupId, H5.index_t.NAME, H5.iter_order_t.INC, ref idx2, Callback,
                        Marshal.StringToHGlobalAnsi(fullName));
                }

                // clean up
                if (H5I.is_valid(groupId) > 0)
                {
                    H5G.close(groupId);
                }

                if (H5I.is_valid(datasetId) > 0)
                {
                    H5D.close(datasetId);
                }

                return 0;
            }

        }

        private static string CombinePath(string path1, string path2)
        {
            return $"{ path1 }/{ path2 }".Replace("///", "/").Replace("//", "/");
        }
    }
}
