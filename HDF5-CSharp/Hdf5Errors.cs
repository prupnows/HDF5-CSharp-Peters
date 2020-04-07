using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using HDF.PInvoke;

namespace HDF5CSharp
{
    public class Hdf5Errors
    {
        public static bool EnableErrorReporting(bool enable)
        {
            if (enable)
             return   H5E.set_auto(H5E.DEFAULT, ErrorDelegateMethod, IntPtr.Zero)>=0;
            else
                return H5E.set_auto(H5E.DEFAULT, null, IntPtr.Zero) >= 0;

        }
        private static int ErrorDelegateMethod(long estack, IntPtr client_data)
        {
            H5E.walk(estack, H5E.direction_t.H5E_WALK_DOWNWARD, WalkDelegateMethod, IntPtr.Zero);
            return 0;
        }

        private static int WalkDelegateMethod(uint n, ref H5E.error_t err_desc, IntPtr client_data)
        {
            string msg =
                $"{err_desc.desc}. (function: {err_desc.func_name}. Line:{err_desc.line}. File: {err_desc.file_name})";
           Hdf5Utils.LogError?.Invoke(msg);
            return 0;
        }
    }
}