using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HDF5CSharp.UnitTests
{
    internal class HDF5DataClass
    {
        public double Location { get; set; }
        public byte[] Image { get; set; }

        protected bool Equals(HDF5DataClass other)
        {
            return Location.Equals(other.Location) && Image.SequenceEqual(other.Image);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((HDF5DataClass)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Location.GetHashCode() * 397) ^ (Image != null ? Image.GetHashCode() : 0);
            }
        }
    }
    [TestClass]
    public class CommunityTests : Hdf5BaseUnitTests
    {
        [TestMethod]
        public void TestMemory()
        {
            Hdf5.Settings.EnableH5InternalErrorReporting(true);
            string fn = $"{nameof(TestMemory)}.h5";
            var data = TestCreateFile(fn);
            for (int i = 0; i < 100; i++)
            {
                var read = TestReadFile(fn);
                Assert.IsTrue(data.SequenceEqual(read));
            }
           
           

        }

        private IEnumerable<HDF5DataClass> GenerateDate()
        {
            Randomizer.Seed = new Random(8675309);
            var random = new Bogus.Randomizer();
            for (int i = 0; i < 10; i++)
            {
                yield return new HDF5DataClass
                {
                    Location = i * 10,
                    Image = random.Bytes(10048576)
                };
            }
        }

        private List<HDF5DataClass> TestCreateFile(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            var fileID = Hdf5.CreateFile(filename);
            List<HDF5DataClass> original = GenerateDate().ToList();
            for (int i = 0; i < original.Count; i++)
            {
                Hdf5.WriteObject(fileID, original[i], $"testObject{i}");
            }
            Hdf5.CloseFile(fileID);
            return original;
        }
        private List<HDF5DataClass> TestReadFile(string filename)
        {
            var fileID = Hdf5.OpenFile(filename);
            List<HDF5DataClass> read = new List<HDF5DataClass>();
            int i = 0;
            bool readOK = true;
            do
            {
                var dataClass = Hdf5.ReadObject<HDF5DataClass>(fileID, $"testObject{i++}");
                if (dataClass != null)
                {
                    read.Add(dataClass);
                }
                else
                {
                    readOK = false;
                }
            } while (readOK);

            Hdf5.CloseFile(fileID);
            return read;
        }
    }
}
