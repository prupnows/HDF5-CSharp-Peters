using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HDF5CSharp.DataTypes;

namespace HDF5CSharp.UnitTests.Core
{
    [Hdf5Attributes(new string[] { "some info", "more info" })]
    class AttributeClass
    {
        [Hdf5KeyValuesAttributes("Key", new[] { "NestedInfo some info", "NestedInfo more info" })]
        public class NestedInfo
        {
            public int noAttribute = 10;

            [Hdf5("some money")]
            public decimal money = 100.12M;
        }

        [Hdf5("birthdate")]
        public DateTime aDatetime = new DateTime(1969, 12, 01, 12, 00, 00, DateTimeKind.Local);

        public double noAttribute = 10.0;

        public NestedInfo nested = new NestedInfo();
    }

    class AllTypesClass
    {
        public bool aBool = true;
        public byte aByte = 10;
        public char aChar = 'a';
        public DateTime aDatetime = new DateTime(1969, 12, 01, 12, 00, 00, DateTimeKind.Local);
        public decimal aDecimal = new decimal(2.344);
        public double aDouble = 2.1;
        public short aInt16 = 10;
        public int aInt32 = 100;
        public long aInt64 = 1000;
        public sbyte aSByte = 10;
        public float aSingle = 100;
        public ushort aUInt16 = 10;
        public uint aUInt32 = 100;
        public ulong aUInt64 = 1000;
        public string aString = "test";
        public TimeSpan aTimeSpan = TimeSpan.FromHours(1);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WData
    {
        public int serial_no;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string location;
        public double temperature;
        public double pressure;

        public DateTime Time
        {
            get => new DateTime(timeTicks);
            set => timeTicks = value.Ticks;
        }

        public long timeTicks;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WData2
    {
        public int serial_no;
        [Hdf5EntryName("location1")]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string location;
        public double temperature;
        public double pressure;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string label;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Responses
    {
        public long MCID;
        public int PanelIdx;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public short[] ResponseValues;
    }

    public class TestClass : IEquatable<TestClass>
    {
        public int TestInteger { get; set; }
        public double TestDouble { get; set; }
        public bool TestBoolean { get; set; }
        public string TestString { get; set; }
        [Hdf5EntryName("Test_time")]
        public DateTime TestTime { get; set; }

        public bool Equals(TestClass other)
        {
            return other.TestInteger == TestInteger &&
        other.TestDouble == TestDouble &&
        other.TestBoolean == TestBoolean &&
                    other.TestString == TestString &&
            other.TestTime == TestTime;
        }
    }
    public class TestClassWithArray : TestClass
    {
        public double[] testDoublesField;
        public string[] testStringsField;
        public double[] TestDoubles { get; set; }
        public string[] TestStrings { get; set; }

        public bool Equals(TestClassWithArray other)
        {
            return base.Equals(other) &&
                   other.TestDoubles.SequenceEqual(TestDoubles) &&
                   other.testDoublesField.SequenceEqual(testDoublesField) &&
                   other.testStringsField.SequenceEqual(testStringsField);

        }
    }
    class TestClassWithStructs
    {
        public TestClassWithStructs()
        {
        }
        public WData[] DataList { get; set; }
    }

}
