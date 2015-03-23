﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;

namespace WIN32APIWapper
{
    public static class ShellHelper
    {

        #region Low/High Word

        /// <summary>
        /// Retrieves the High Word of a WParam of a WindowMessage
        /// </summary>
        /// <param name="ptr">The pointer to the WParam</param>
        /// <returns>The unsigned integer for the High Word</returns>
        public static uint HiWord(IntPtr ptr)
        {
            if (((uint)ptr & 0x80000000) == 0x80000000)
                return ((uint)ptr >> 16);
            else
                return ((uint)ptr >> 16) & 0xffff;
        }

        /// <summary>
        /// Retrieves the Low Word of a WParam of a WindowMessage
        /// </summary>
        /// <param name="ptr">The pointer to the WParam</param>
        /// <returns>The unsigned integer for the Low Word</returns>
        public static uint LoWord(IntPtr ptr)
        {
            return (uint)ptr & 0xffff;
        }

        #endregion

        #region IStream/IStorage

        /*
        public static bool GetIStream(ShellItem item, out IntPtr streamPtr, out IStream stream)
        {
            if (item.ParentItem.ShellFolder.BindToStorage(
                        item.PIDLRel.Ptr,
                        IntPtr.Zero,
                        ref Shell32Wrapper.IID_IStream,
                        out streamPtr) == Shell32Wrapper.S_OK)
            {
                stream = (IStream)Marshal.GetTypedObjectForIUnknown(streamPtr, typeof(IStream));
                return true;
            }
            else
            {
                stream = null;
                streamPtr = IntPtr.Zero;
                return false;
            }
        }
         */

        /*
        public static bool GetIStorage(ShellItem item, out IntPtr storagePtr, out IStorage storage)
        {
            if (item.ParentItem.ShellFolder.BindToStorage(
                        item.PIDLRel.Ptr,
                        IntPtr.Zero,
                        ref Shell32Wrapper.IID_IStorage,
                        out storagePtr) == Shell32Wrapper.S_OK)
            {
                storage = (IStorage)Marshal.GetTypedObjectForIUnknown(storagePtr, typeof(IStorage));
                return true;
            }
            else
            {
                storage = null;
                storagePtr = IntPtr.Zero;
                return false;
            }
        }
         */

        #endregion

        #region Drag/Drop

        /*
        /// <summary>
        /// This method will use the GetUIObjectOf method of IShellFolder to obtain the IDataObject of a
        /// ShellItem. 
        /// </summary>
        /// <param name="item">The item for which to obtain the IDataObject</param>
        /// <param name="dataObjectPtr">A pointer to the returned IDataObject</param>
        /// <returns>the IDataObject the ShellItem</returns>
        public static IntPtr GetIDataObject(ShellItem[] items)
        {
            ShellItem parent = items[0].ParentItem != null ? items[0].ParentItem : items[0];

            IntPtr[] pidls = new IntPtr[items.Length];
            for (int i = 0; i < items.Length; i++)
                pidls[i] = items[i].PIDLRel.Ptr;

            IntPtr dataObjectPtr;
            if (parent.ShellFolder.GetUIObjectOf(
                    IntPtr.Zero,
                    (uint)pidls.Length,
                    pidls,
                    ref Shell32Wrapper.IID_IDataObject,
                    IntPtr.Zero,
                    out dataObjectPtr) == Shell32Wrapper.S_OK)
            {
                return dataObjectPtr;
            }
            else
            {
                return IntPtr.Zero;
            }
        }
        */

        /*
        /// <summary>
        /// This method will use the GetUIObjectOf method of IShellFolder to obtain the IDropTarget of a
        /// ShellItem. 
        /// </summary>
        /// <param name="item">The item for which to obtain the IDropTarget</param>
        /// <param name="dropTargetPtr">A pointer to the returned IDropTarget</param>
        /// <returns>the IDropTarget from the ShellItem</returns>
        public static bool GetIDropTarget(ShellItem item, out IntPtr dropTargetPtr, out IDropTarget dropTarget)
        {
            ShellItem parent = item.ParentItem != null ? item.ParentItem : item;

            if (parent.ShellFolder.GetUIObjectOf(
                    IntPtr.Zero,
                    1,
                    new IntPtr[] { item.PIDLRel.Ptr },
                    ref Shell32Wrapper.IID_IDropTarget,
                    IntPtr.Zero,
                    out dropTargetPtr) == Shell32Wrapper.S_OK)
            {
                dropTarget =
                    (IDropTarget)Marshal.GetTypedObjectForIUnknown(dropTargetPtr, typeof(IDropTarget));

                return true;
            }
            else
            {
                dropTarget = null;
                dropTargetPtr = IntPtr.Zero;
                return false;
            }
        }
        */

        public static bool GetIDropTargetHelper(out IntPtr helperPtr, out IDropTargetHelper dropHelper)
        {
            if (Ole32Wrapper.CoCreateInstance(
                    ref GUIDs.CLSID_DragDropHelper,
                    IntPtr.Zero,
                    CLSCTX.INPROC_SERVER,
                    ref GUIDs.IID_IDropTargetHelper,
                    out helperPtr) == Shell32Wrapper.S_OK)
            {
                dropHelper =
                    (IDropTargetHelper)Marshal.GetTypedObjectForIUnknown(helperPtr, typeof(IDropTargetHelper));

                return true;
            }
            else
            {
                dropHelper = null;
                helperPtr = IntPtr.Zero;
                return false;
            }
        }

        /*
        public static DragDropEffects CanDropClipboard(ShellItem item)
        {
            IntPtr dataObject;
            Ole32Wrapper.OleGetClipboard(out dataObject);

            IntPtr targetPtr;
            IDropTarget target;

            DragDropEffects retVal = DragDropEffects.None;
            if (GetIDropTarget(item, out targetPtr, out target))
            {
                #region Check Copy
                DragDropEffects effects = DragDropEffects.Copy;
                if (target.DragEnter(
                    dataObject,
                    ShellAPI.MK.CONTROL,
                    new ShellAPI.POINT(0, 0),
                    ref effects) == ShellAPI.S_OK)
                {
                    if (effects == DragDropEffects.Copy)
                        retVal |= DragDropEffects.Copy;

                    target.DragLeave();
                }
                #endregion

                #region Check Move
                effects = DragDropEffects.Move;
                if (target.DragEnter(
                    dataObject,
                    MK.SHIFT,
                    new POINT(0, 0),
                    ref effects) == Shell32Wrapper.S_OK)
                {
                    if (effects == DragDropEffects.Move)
                        retVal |= DragDropEffects.Move;

                    target.DragLeave();
                }
                #endregion

                #region Check Lick
                effects = DragDropEffects.Link;
                if (target.DragEnter(
                    dataObject,
                    MK.ALT,
                    new POINT(0, 0),
                    ref effects) == Shell32Wrapper.S_OK)
                {
                    if (effects == DragDropEffects.Link)
                        retVal |= DragDropEffects.Link;

                    target.DragLeave();
                }
                #endregion

                Marshal.ReleaseComObject(target);
                Marshal.Release(targetPtr);
            }

            return retVal;
        }
        */
        #endregion

        #region QueryInfo
        /*
        public static bool GetIQueryInfo(ShellItem item, out IntPtr iQueryInfoPtr, out IQueryInfo iQueryInfo)
        {
            ShellItem parent = item.ParentItem != null ? item.ParentItem : item;

            if (parent.ShellFolder.GetUIObjectOf(
                    IntPtr.Zero,
                    1,
                    new IntPtr[] { item.PIDLRel.Ptr },
                    ref Shell32Wrapper.IID_IQueryInfo,
                    IntPtr.Zero,
                    out iQueryInfoPtr) == Shell32Wrapper.S_OK)
            {
                iQueryInfo =
                    (IQueryInfo)Marshal.GetTypedObjectForIUnknown(iQueryInfoPtr, typeof(IQueryInfo));

                return true;
            }
            else
            {
                iQueryInfo = null;
                iQueryInfoPtr = IntPtr.Zero;
                return false;
            }
        }
        */
        #endregion
    }
}
