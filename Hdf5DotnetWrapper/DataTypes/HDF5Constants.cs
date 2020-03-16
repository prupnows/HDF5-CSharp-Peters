using HDF.PInvoke;

namespace Hdf5DotnetWrapper.DataTypes
{
    public static class HDF5Constants
    {
        public static int H5_INDEX_NAME = 1;
        public static int H5_ITER_INC = 2;
        public static int H5F_LIBVER_LATEST = (int)H5F.libver_t.LATEST;
        public static int H5F_LIBVER_EARLIEST = (int)H5F.libver_t.EARLIEST;
        public static int H5F_LIBVER_V18 = (int)H5F.libver_t.V18;
        public static int H5F_LIBVER_V110 = (int)H5F.libver_t.V110;
        public static long H5P_DEFAULT = H5P.DEFAULT;
        public static uint H5F_ACC_CREAT = H5F.ACC_CREAT;
        public static uint H5F_ACC_TRUNC = H5F.ACC_TRUNC;
        public static H5F.scope_t H5F_SCOPE_LOCAL = H5F.scope_t.LOCAL;
        public static uint H5F_ACC_RDWR = H5F.ACC_RDWR;
        public static uint H5F_ACC_RDONLY = H5F.ACC_RDONLY;
    }
}
