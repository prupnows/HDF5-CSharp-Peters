using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HDF.PInvoke;

namespace HDF5CSharp.DataTypes
{

    public abstract class Hdf5ElementBase
    {
        public string Name { get; set; }

        public bool IsLazyLoading { get; }

        public Hdf5ElementBase Parent { get; set; }

        protected bool HasLoadedOnce { get; private set; }
        public abstract string GetPath();
        public abstract string GetDisplayName();
        public abstract IEnumerable<Hdf5ElementBase> GetChildSet();
        protected abstract long GetId(long fileId);
        protected abstract void CloseId(long id);

        public Hdf5ElementBase(string name, Hdf5ElementBase parent, bool isLazyLoading)
        {
            this.Name = name;
            this.Parent = parent;
            this.IsLazyLoading = isLazyLoading;
        }

    }

    public class Hdf5Element : Hdf5ElementBase
    {
        private List<Hdf5Element> children;
        public Hdf5Element(string name, Hdf5ElementBase parent, bool isLazyLoading) : base(name, parent, isLazyLoading)
        {
            children=new List<Hdf5Element>();
        }

        public override string GetPath()
        {
            return this.Name;
        }

        public override string GetDisplayName()
        {
            return this.Name;
        }

        public override IEnumerable<Hdf5ElementBase> GetChildSet()
        {
            return children.ToList();
        }

        protected override long GetId(long fileId)
        {
            return H5G.open(fileId, this.GetPath());
        }

        protected override void CloseId(long id)
        {
            if (H5I.is_valid(id) > 0)
            {
                H5G.close(id);
            }
        }

    }
}
