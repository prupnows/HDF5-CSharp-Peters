using System;
using System.Collections.Generic;
using System.Linq;

namespace Hdf5DotnetWrapper.DataTypes
{
    public class Group : HObject
    {
        private static long serialVersionUID = 3913174542591568052L;

        /**
         * The list of members (Groups and Datasets) of this group in memory.
         */
        private List<HObject> memberList = new List<HObject>();

        /**
         * The parent group where this group is located. The parent of the root
         * group is null.
         */
        protected Group parent;

        /**
         * Total number of members of this group in file.
         */
        protected int nMembersInFile;

        public static int LINK_TYPE_HARD = 0;

        public static int LINK_TYPE_SOFT = 1;

        public static int LINK_TYPE_EXTERNAL = 64;

        public static int CRT_ORDER_TRACKED = 1;

        public static int CRT_ORDER_INDEXED = 2;


        /**
         * Constructs an instance of the group with specific name, path and parent
         * group. An HDF data object must have a name. The path is the group path
         * starting from the root. The parent group is the group where this group is
         * located.
         * <p>
         * For example, in H5Group(h5file, "grp", "/groups/", pgroup), "grp" is the
         * name of the group, "/groups/" is the group path of the group, and pgroup
         * is the group where "grp" is located.
         *
         * @param theFile
         *            the file containing the group.
         * @param grpName
         *            the name of this group, e.g. "grp01".
         * @param grpPath
         *            the full path of this group, e.g. "/groups/".
         * @param grpParent
         *            the parent of this group.
         */
        public Group(FileFormat theFile, String grpName, String grpPath, Group grpParent) : this(theFile, grpName,
            grpPath, grpParent, null)
        {

        }

        /**
         * @deprecated Not for public use in the future.<br>
         *             Using {@link #Group(FileFormat, String, String, Group)}
         *
         * @param theFile
         *            the file containing the group.
         * @param grpName
         *            the name of this group, e.g. "grp01".
         * @param grpPath
         *            the full path of this group, e.g. "/groups/".
         * @param grpParent
         *            the parent of this group.
         * @param oid
         *            the oid of this group.
         */
        [Obsolete]
        public Group(FileFormat theFile, String grpName, String grpPath, Group grpParent, long[] oid) : base(theFile,
            grpName, grpPath, oid)
        {
            this.parent = grpParent;
        }

        /**
         * Clears up member list and other resources in memory for the group. Since
         * the destructor will clear memory space, the function is usually not
         * needed.
         */
        public void clear()
        {
            memberList.Clear();
        }

        /**
         * Adds an object to the member list of this group in memory.
         *
         * @param object
         *            the HObject to be added to the member list.
         */
        public void addToMemberList(HObject obj)
        {
            if (!memberList.Contains(obj))
            {
                memberList.Add(obj);
            }
        }

        /**
         * Removes an object from the member list of this group in memory.
         *
         * @param object
         *            the HObject (Group or Dataset) to be removed from the member
         *            list.
         */
        public void removeFromMemberList(HObject obj)
        {

            memberList.Remove(obj);

        }

        /**
         * Returns the list of members of this group. The list is an java.util.List
         * containing HObjects.
         *
         * @return the list of members of this group.
         */
        public List<HObject> getMemberList()
        {
            FileFormat theFile = this.getFileFormat();

            if ((memberList == null) && (theFile != null))
            {
                int size = Math.Min(getNumberOfMembersInFile(), getFileFormat().getMaxMembers());
                memberList = new List<HObject>(size + 5); // avoid infinite loop search for groups without members

                // find the memberList from the file by checking the group path and
                // name. group may be created out of the structure tree
                // (H4/5File.loadTree()).
                if (theFile.getFID() < 0)
                {
                    try
                    {
                        theFile.open();
                    } // load the file structure;
                    catch (Exception ex)
                    {
                        ;
                    }
                }

                HObject root = theFile.getRootObject();
                if (root == null) return memberList;


                foreach (HObject uObj in ((Group)root).depthFirstMemberList())
                {
                    if (uObj is Group g)
                    {
                        if (g.getPath() != null) // add this check to get rid of null exception
                        {
                            if ((this.isRoot() && g.isRoot())
                                || (this.getPath().Equals(g.getPath()) &&
                                    g.getName().EndsWith(this.getName())))
                            {
                                memberList = g.getMemberList();
                                break;
                            }
                        }
                    }
                }
            }

            return memberList;
        }

        /**
         * @return the members of this Group in breadth-first order.
         */
        public List<HObject> breadthFirstMemberList()
        {
            List<HObject> members = new List<HObject>();
            Queue<HObject> queue = new Queue<HObject>();
            HObject currentObj = this;

            foreach (HObject hObject in ((Group)currentObj).getMemberList())
            {
                queue.Enqueue(hObject);
            }

            while (queue.Any())
            {
                currentObj = queue.Dequeue();
                members.Add(currentObj);

                if (currentObj is Group g && g.getNumberOfMembersInFile() > 0)
                {
                    foreach (HObject hObject in ((Group)currentObj).getMemberList())
                    {
                        queue.Enqueue(hObject);
                    }
                }
            }

            return members;
        }

        /**
         * @return the members of this Group in depth-first order.
         */
        public List<HObject> depthFirstMemberList()
        {
            List<HObject> members = new List<HObject>();
            Stack<HObject> stack = new Stack<HObject>();
            HObject currentObj = this;

            // Push elements onto the stack in reverse order
            List<HObject> list = ((Group)currentObj).getMemberList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                stack.Push(list[i]);
            }

            while (stack.Any())
            {
                currentObj = stack.Pop();
                members.Add(currentObj);

                if (currentObj is Group g && g.getNumberOfMembersInFile() > 0)
                {
                    list = ((Group)currentObj).getMemberList();
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        stack.Push(list[i]);
                    }
                }
            }

            return members;
        }

        /**
         * Sets the name of the group.
         * <p>
         * setName (String newName) changes the name of the group in memory and
         * file.
         * <p>
         * setName() updates the path in memory for all the objects that are under
         * the group with the new name.
         *
         * @param newName
         *            The new name of the group.
         *
         * @throws Exception if the name can not be set
         */

        public void setName(string newName)
        {
            base.setName(newName);

            if (memberList != null)
            {
                int n = memberList.Count;
                HObject theObj = null;
                for (int i = 0; i < n; i++)
                {
                    theObj = memberList[i];
                    theObj.setPath(this.getPath() + newName + HObject.SEPARATOR);
                }
            }

        }

        /** @return the parent group. */
        public Group getParent()
        {
            return parent;
        }

        /**
         * Checks if it is a root group.
         *
         * @return true if the group is a root group; otherwise, returns false.
         */
        public bool isRoot()
        {
            return (parent == null);
        }

        /**
         * Returns the total number of members of this group in file.
         *
         * Current Java applications such as HDFView cannot handle files with large
         * numbers of objects (1,000,000 or more objects) due to JVM memory
         * limitation. The max_members is used so that applications such as HDFView
         * will load up to <i>max_members</i> number of objects. If the number of
         * objects in file is larger than <i>max_members</i>, only
         * <i>max_members</i> are loaded in memory.
         * <p>
         * getNumberOfMembersInFile() returns the number of objects in this group.
         * The number of objects in memory is obtained by getMemberList().size().
         *
         * @return Total number of members of this group in the file.
         */
        public int getNumberOfMembersInFile()
        {
            return nMembersInFile;
        }

        /**
         * Get the HObject at the specified index in this Group's member list.
         * @param idx The index of the HObject to get.
         * @return The HObject at the specified index.
         */
        public HObject getMember(int idx)
        {
            if (memberList.Count <= 0 || idx >= memberList.Count) return null;

            return memberList[idx];
        }
    }
}

