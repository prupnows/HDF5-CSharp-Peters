using System;
using HDF.PInvoke;

namespace HDF5CSharp
{
    public static partial class Hdf5
    {
        public static Settings Hdf5Settings { get; set; }


        static Hdf5()
        {
            Hdf5Settings = new Settings { DateTimeType = DateTimeType.Ticks };

        }
    }

    public class Settings
    {
        public DateTimeType DateTimeType { get; set; }
        public bool LowerCaseNaming { get; set; }
        public bool ErrorLoggingEnable { get; private set; }

        public bool EnableErrorReporting(bool enable)
        {
            ErrorLoggingEnable = enable;   
            if (enable)
                return H5E.set_auto(H5E.DEFAULT, Hdf5Errors.ErrorDelegateMethod, IntPtr.Zero) >= 0;
            return H5E.set_auto(H5E.DEFAULT, null, IntPtr.Zero) >= 0;

        }
    }

    public enum DateTimeType
    {
        Ticks,
        UnixTimeSeconds,
        UnixTimeMilliseconds
    }
}
