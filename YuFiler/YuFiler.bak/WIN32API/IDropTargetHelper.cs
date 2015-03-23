using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace WIN32APIWapper
{
    [ComImport]
    [GuidAttribute("4657278B-411B-11d2-839A-00C04FD918D0")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDropTargetHelper
    {
        // Notifies the drag-image manager that the drop target's IDropTarget::DragEnter method has been called
        [PreserveSig]
        Int32 DragEnter(      
            IntPtr hwndTarget,

            //å^ÇIntPtrÇ©ÇÁïœÇ¶ÇΩÅBÇ»ÇÒÇ≈è„éËÇ≠Ç¢Ç≠ÇÃÇ©ÇÕMarshalAsëÆê´Ç»ÇÃÇ©ÅH
            [In, MarshalAs(UnmanagedType.Interface)]System.Runtime.InteropServices.ComTypes.IDataObject dataObject,
            //IntPtr pDataObject,
            ref POINT ppt,
            DragDropEffects dwEffect);

        // Notifies the drag-image manager that the drop target's IDropTarget::DragLeave method has been called
        [PreserveSig]
        Int32 DragLeave();

        // Notifies the drag-image manager that the drop target's IDropTarget::DragOver method has been called
        [PreserveSig]
        Int32 DragOver(
            ref POINT ppt,
            DragDropEffects dwEffect);

        // Notifies the drag-image manager that the drop target's IDropTarget::Drop method has been called
        [PreserveSig]
        Int32 Drop(
            [In, MarshalAs(UnmanagedType.Interface)]System.Runtime.InteropServices.ComTypes.IDataObject dataObject,
            //IntPtr pDataObject,
            ref POINT ppt,
            DragDropEffects dwEffect);

        // Notifies the drag-image manager to show or hide the drag image
        [PreserveSig]
        Int32 Show(
            bool fShow);
    }
}
