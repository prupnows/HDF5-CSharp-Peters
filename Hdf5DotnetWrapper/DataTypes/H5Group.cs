using HDF.PInvoke;
using System.Collections.Generic;

namespace Hdf5DotnetWrapper.DataTypes
{
    public class H5Group : Group
    {
        private static long serialVersionUID = -951164512330444150L;
        protected List<> attributeList;

        private int nAttributes = -1;

        private H5O.info_t obj_info;

    }
}
