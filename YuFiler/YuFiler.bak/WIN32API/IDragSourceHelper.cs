using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WIN32APIWapper
{
    [ComImport]
    [Guid("DE5BF786-477A-11D2-839D-00C04FD918D0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDragSourceHelper
    {
        // Initializes the drag-image manager for a windowless control
        [PreserveSig]
        Int32 InitializeFromBitmap(
            ref SHDRAGIMAGE pshdi,
            IntPtr pDataObject);

        // Initializes the drag-image manager for a control with a window
        [PreserveSig]
        Int32 InitializeFromWindow(
            IntPtr hwnd,
            ref POINT ppt,
            IntPtr pDataObject);
    }
}
