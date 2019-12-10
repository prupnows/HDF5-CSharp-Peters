using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hdf5DotnetWrapper
{
    public static class Hdf5Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NormalizedName(string name) => Hdf5.Hdf5Settings.LowerCaseNaming ? name.ToLowerInvariant() : name;
    }
}
