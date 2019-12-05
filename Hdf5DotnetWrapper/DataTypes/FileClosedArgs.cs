using System;

namespace Hdf5DotnetWrapper.DataTypes
{
    public class FileClosedArgs : EventArgs
    {
        public string ClosedFile { get; }
        public bool CancelRequested { get; set; }

        public FileClosedArgs(string fileName)
        {
            ClosedFile = fileName;
        }
    }
}
