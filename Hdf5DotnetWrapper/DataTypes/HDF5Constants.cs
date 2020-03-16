using HDF.PInvoke;

namespace Hdf5DotnetWrapper.DataTypes
{
    public static class HDF5Constants
    {
        public static H5.index_t H5_INDEX_NAME = H5.index_t.NAME;
        public static H5.iter_order_t H5_ITER_INC = H5.iter_order_t.INC;
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
        public static byte[] H5R_OBJECT = new byte[H5R.OBJ_REF_BUF_SIZE];
        public static int H5O_TYPE_GROUP = (int)H5O.type_t.GROUP;
        public static int H5O_TYPE_DATASET = (int)H5O.type_t.DATASET;
        public static H5T.class_t H5T_ARRAY = H5T.class_t.ARRAY;
        public static H5T.class_t H5T_VLEN = H5T.class_t.VLEN;
        public static H5T.class_t H5T_COMPOUND = H5T.class_t.COMPOUND;
        public static int H5O_TYPE_NAMED_DATATYPE = (int)H5O.type_t.NAMED_DATATYPE;
        public static int H5O_TYPE_UNKNOWN = (int)H5O.type_t.UNKNOWN;
    }
}
