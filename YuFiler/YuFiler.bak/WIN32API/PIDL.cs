using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

namespace WIN32APIWapper
{
    /*
    public class PIDL : IEnumerable
    {
        private IntPtr pidl = IntPtr.Zero;

        public IEnumerator GetEnumerator()
        {
            return new PIDLEnumerator(pidl);
        }


        #region Constructors
        public PIDL(IntPtr pidl)
        {
            this.pidl = pidl;
        }
        #endregion

        // Clones the first SHITEMID structure in an ITEMIDLIST structure
        public static IntPtr ILCloneFirst(IntPtr pidl)
        {
            int size = ItemIDSize(pidl);

            byte[] bytes = new byte[size + 2];
            Marshal.Copy(pidl, bytes, 0, size);

            IntPtr newPidl = Marshal.AllocCoTaskMem(size + 2);
            Marshal.Copy(bytes, 0, newPidl, size + 2);

            return newPidl;
        }


        public static bool IsEmpty(IntPtr pidl)
        {
            if (pidl == IntPtr.Zero)
                return true;

            byte[] bytes = new byte[2];
            Marshal.Copy(pidl, bytes, 0, 2);
            int size = bytes[0] + bytes[1] * 256;
            return (size <= 2);
        }

        // Gets the next SHITEMID structure in an ITEMIDLIST structure
        public static IntPtr ILGetNext(IntPtr pidl)
        {
            int size = ItemIDSize(pidl);
            IntPtr nextPidl = new IntPtr((int)pidl + size);
            return nextPidl;
        }

        private static int ItemIDSize(IntPtr pidl)
        {
            if (!pidl.Equals(IntPtr.Zero))
            {
                byte[] buffer = new byte[2];
                Marshal.Copy(pidl, buffer, 0, 2);
                return buffer[1] * 256 + buffer[0];
            }
            else
                return 0;
        }

        public class PIDLEnumerator : IEnumerator
        {
            private IntPtr pidl;
            private IntPtr currentPidl;
            private IntPtr clonePidl;
            private bool start;

            public PIDLEnumerator(IntPtr pidl)
            {
                start = true;
                this.pidl = pidl;
                currentPidl = pidl;
                clonePidl = IntPtr.Zero;
            }

            #region IEnumerator Members

            public object Current
            {
                get
                {
                    if (clonePidl != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(clonePidl);
                        clonePidl = IntPtr.Zero;
                    }

                    clonePidl = PIDL.ILCloneFirst(currentPidl);
                    return clonePidl;
                }
            }

            public bool MoveNext()
            {
                if (clonePidl != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(clonePidl);
                    clonePidl = IntPtr.Zero;
                }

                if (start)
                {
                    start = false;
                    return true;
                }
                else
                {
                    IntPtr newPidl = ILGetNext(currentPidl);

                    if (!PIDL.IsEmpty(newPidl))
                    {
                        currentPidl = newPidl;
                        return true;
                    }
                    else
                        return false;
                }
            }

            public void Reset()
            {
                start = true;
                currentPidl = pidl;

                if (clonePidl != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(clonePidl);
                    clonePidl = IntPtr.Zero;
                }
            }

            #endregion
        }
    }
     */
}
