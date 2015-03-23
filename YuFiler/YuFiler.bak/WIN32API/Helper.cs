using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;

namespace WIN32APIWapper
{
    public class Helper
    {
        /// <summary>
        /// Invokes a specific command from an IContextMenu
        /// </summary>
        /// <param name="iContextMenu">the IContextMenu containing the item</param>
        /// <param name="cmd">the index of the command to invoke</param>
        /// <param name="parentDir">the parent directory from where to invoke</param>
        /// <param name="ptInvoke">the point (in screen codinates) from which to invoke</param>
        public static void InvokeCommand(IContextMenu iContextMenu, uint cmd, string parentDir, System.Windows.Point ptInvoke)
        {
            CMINVOKECOMMANDINFOEX invoke = new CMINVOKECOMMANDINFOEX();
            invoke.cbSize = Shell32Wrapper.cbInvokeCommand;
            invoke.lpVerb = (IntPtr)cmd;
            invoke.lpDirectory = parentDir;
            invoke.lpVerbW = (IntPtr)cmd;
            invoke.lpDirectoryW = parentDir;


            //invoke.fMask = CMIC.UNICODE | CMIC.PTINVOKE |
            //    ((Control.ModifierKeys & Keys.Control) != 0 ? CMIC.CONTROL_DOWN : 0) |
            //    ((Control.ModifierKeys & Keys.Shift) != 0 ? CMIC.SHIFT_DOWN : 0);
            invoke.fMask = CMIC.UNICODE | CMIC.PTINVOKE;


            invoke.ptInvoke = new POINT() { x = (int)ptInvoke.X, y = (int)ptInvoke.Y };
            invoke.nShow = SW.SHOWNORMAL;

            iContextMenu.InvokeCommand(ref invoke);
        }


        private static void ShowContextMenuFolder(IntPtr handle, IShellFolder desktop, string parent, List<FileSystemInfo> lst, double x, double y,
            ref IContextMenu2 FolderContextMenu, ref IntPtr FolderContextPtr)
        {
            IntPtr PopupPtr = IntPtr.Zero;
            IntPtr newContextMenuPtr = IntPtr.Zero, newContextMenuPtr2 = IntPtr.Zero;
            IContextMenu newContextMenu = null;

            IntPtr PIDLParent = IntPtr.Zero;
            try
            {
                uint fcharcnt = 0;
                SFGAO fattr = SFGAO.BROWSABLE;

                //親フォルダのパス文字列からPIDLを取得
                desktop.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, parent, ref fcharcnt, out PIDLParent, ref fattr);

                //メニューを作成
                PopupPtr = User32Wrapper.CreatePopupMenu();

                #region gomi
                //コンテキストメニューに項目追加
                //不必要なのでやらない
                //viewSubMenu = User32Wrapper.CreatePopupMenu();
                //MENUITEMINFO itemInfo = new MENUITEMINFO("View");
                //itemInfo.cbSize = Shell32Wrapper.cbMenuItemInfo;
                //itemInfo.fMask = MIIM.SUBMENU | MIIM.STRING;
                //itemInfo.hSubMenu = viewSubMenu;
                //User32Wrapper.InsertMenuItem(PopupPtr, 0, true, ref itemInfo);
                //MFT rCheck = MFT.RADIOCHECK | MFT.CHECKED;
                //User32Wrapper.AppendMenu(viewSubMenu,
                //    rCheck, (uint)0, "Tiles");
                //User32Wrapper.AppendMenu(viewSubMenu,
                //    rCheck, (uint)1, "Icons");
                //User32Wrapper.AppendMenu(viewSubMenu,
                //    rCheck, (uint)2, "List");
                //User32Wrapper.AppendMenu(viewSubMenu,
                //    rCheck, (uint)3, "Details");
                #endregion

                #region New Submenu
                //IContextMenuを作成する
                if (GetNewContextMenu(PIDLParent, out newContextMenuPtr, out newContextMenu))
                {
                    //セパレータ
                    //User32Wrapper.AppendMenu(PopupPtr, MFT.SEPARATOR, 0, string.Empty);

                    //作成したIContextMenuをメニュー（ここではcontextMenu）に追加する
                    newContextMenu.QueryContextMenu(PopupPtr, 0, Shell32Wrapper.CMD_FIRST, Shell32Wrapper.CMD_LAST, CMF.NORMAL);

                    //サブメニューを取得（ここでは追加したIContextMenu?
                    //WndProcで使用される
                    FolderContextPtr = User32Wrapper.GetSubMenu(PopupPtr, 0);

                    Marshal.QueryInterface(newContextMenuPtr, ref GUIDs.IID_IContextMenu2, out newContextMenuPtr2);
                    FolderContextMenu = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(newContextMenuPtr2, typeof(IContextMenu2));

                }
                #endregion

                uint selected = User32Wrapper.TrackPopupMenuEx(PopupPtr, TPM.RETURNCMD, (int)x, (int)y, handle, IntPtr.Zero);
                if (selected >= Shell32Wrapper.CMD_FIRST)
                {
                    uint cmdidx = selected - Shell32Wrapper.CMD_FIRST;
                    Helper.InvokeCommand(newContextMenu, cmdidx, parent, new Point(x, y));
                }
            }
            #region finally
            finally
            {
                if (PopupPtr != IntPtr.Zero)
                    User32Wrapper.DestroyMenu(PopupPtr);

                if(newContextMenuPtr != IntPtr.Zero)
                {
                    Marshal.Release(newContextMenuPtr);
                    newContextMenuPtr = IntPtr.Zero;
                }

                if (newContextMenuPtr2 != IntPtr.Zero)
                {
                    Marshal.Release(newContextMenuPtr2);
                    newContextMenuPtr2 = IntPtr.Zero;
                }

                if (newContextMenu != null)
                {
                    Marshal.FinalReleaseComObject(newContextMenu);
                    newContextMenu = null;
                }

                if (FolderContextMenu != null)
                {
                    Marshal.FinalReleaseComObject(FolderContextMenu);
                    FolderContextMenu = null;
                }
            }
            #endregion

        }

        public static bool GetNewContextMenu(IntPtr pidl, out IntPtr iContextMenuPtr, out IContextMenu iContextMenu)
        {
            //アンマネージなIContextMenuを作成する
            if (Ole32Wrapper.CoCreateInstance(
                    ref GUIDs.CLSID_NewMenu,
                    IntPtr.Zero,
                    CLSCTX.INPROC_SERVER,
                    ref GUIDs.IID_IContextMenu,
                    out iContextMenuPtr) == Shell32Wrapper.S_OK)
            {
                //アンマネージからマネージなオブジェクトを取得
                iContextMenu = Marshal.GetTypedObjectForIUnknown(iContextMenuPtr, typeof(IContextMenu)) as IContextMenu;

                //アンマネージなIContextMenuからアンマネージなIShellExtInitを取得する
                IntPtr iShellExtInitPtr;
                if (Marshal.QueryInterface(
                    iContextMenuPtr,
                    ref GUIDs.IID_IShellExtInit,
                    out iShellExtInitPtr) == Shell32Wrapper.S_OK)
                {
                    //アンマネージからマネージなオブジェクトを取得
                    IShellExtInit iShellExtInit = Marshal.GetTypedObjectForIUnknown(iShellExtInitPtr, typeof(IShellExtInit)) as IShellExtInit;

                    //とりあえず初期化する
                    //マナーのようです
                    int hresult = iShellExtInit.Initialize(pidl, IntPtr.Zero, 0);

                    Marshal.ReleaseComObject(iShellExtInit);
                    Marshal.Release(iShellExtInitPtr);

                    return true;
                }
                else
                {
                    if (iContextMenu != null)
                    {
                        Marshal.ReleaseComObject(iContextMenu);
                        iContextMenu = null;
                    }

                    if (iContextMenuPtr != IntPtr.Zero)
                    {
                        Marshal.Release(iContextMenuPtr);
                        iContextMenuPtr = IntPtr.Zero;
                    }

                    return false;
                }
            }
            else
            {
                iContextMenuPtr = IntPtr.Zero;
                iContextMenu = null;
                return false;
            }
        }

        public static void ShowContextMenu(IntPtr handle, IShellFolder desktop, string parent, List<FileSystemInfo> lst, double x, double y, ref IContextMenu2 newContextMenu2, ref IntPtr newSubmenuPtr)
        {
            if (lst == null)
            {
                ShowContextMenuFolder(handle, desktop, parent, lst, x, y, ref newContextMenu2, ref newSubmenuPtr);
                return;
            }

            IntPtr PPopup = IntPtr.Zero, PIDLParent = IntPtr.Zero, PSHParent = IntPtr.Zero;
            IntPtr PContext = IntPtr.Zero, PContext2 = IntPtr.Zero, PContext3 = IntPtr.Zero;
            List<IntPtr> ChildrenList = null;
            IContextMenu CContext = null;
            IContextMenu2 CContext2 = null;
            IContextMenu3 CContext3 = null;
            IShellFolder SHParent = null;
            try
            {
                //親フォルダの PIDL を取得する
                uint fcharcnt = 0;
                SFGAO fattr = SFGAO.BROWSABLE;
                if (desktop.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, parent, ref fcharcnt, out PIDLParent, ref fattr) == Shell32Wrapper.S_OK)
                {
                    //親フォルダのシェルフォルダのポインタを取得する
                    
                    if (desktop.BindToObject(PIDLParent, IntPtr.Zero, GUIDs.IID_IShellFolder, out PSHParent) == Shell32Wrapper.S_OK)
                    {
                        //親フォルダのIShellFolder を取得する
                        SHParent = (IShellFolder)Marshal.GetTypedObjectForIUnknown(PSHParent, typeof(IShellFolder));

                        ChildrenList = new List<IntPtr>();
                        //対象ファイルの PIDL (親のシェルフォルダからの相対 PIDL)を取得する
                        foreach (var files in lst)
                        {
                            IntPtr PFile;
                            if (SHParent.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, files.Name, ref fcharcnt, out PFile, ref fattr) == Shell32Wrapper.S_OK)
                                ChildrenList.Add(PFile);
                        }

                        //対象ファイルの IContextMenu へのポインタを取得する
                        IntPtr[] children = ChildrenList.ToArray();
                        if (SHParent.GetUIObjectOf(IntPtr.Zero, (uint)children.Length, children, GUIDs.IID_IContextMenu, IntPtr.Zero, out PContext) == Shell32Wrapper.S_OK)
                        {
                            //対象ファイルの IContextMenu を取得する
                            CContext = (IContextMenu)Marshal.GetTypedObjectForIUnknown(PContext, typeof(IContextMenu));

                            //対象ファイルの IContextMenu2, IContextMenu3 のポインタを取得する
                            Marshal.QueryInterface(PContext, ref GUIDs.IID_IContextMenu2, out PContext2);
                            Marshal.QueryInterface(PContext, ref GUIDs.IID_IContextMenu3, out PContext3);

                            CContext2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(PContext2, typeof(IContextMenu2));
                            CContext3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(PContext3, typeof(IContextMenu3));

                            //ポップアップメニューを作成する
                            PPopup = User32Wrapper.CreatePopupMenu();

                            //ポップアップメニューに、コンテキストメニュー IContextMenu を追加する
                            CMF ContextMenuFlag = CMF.EXPLORE | CMF.CANRENAME;
                            CContext.QueryContextMenu(PPopup, 0, Shell32Wrapper.CMD_FIRST, Shell32Wrapper.CMD_LAST, ContextMenuFlag);

                            //ポップアップメニューを表示する
                            //呼び出しをブロックします
                            uint selected = User32Wrapper.TrackPopupMenuEx(PPopup, TPM.RETURNCMD, (int)x, (int)y, handle, IntPtr.Zero);
                            if (selected >= Shell32Wrapper.CMD_FIRST)
                            {
                                uint cmdidx = selected - Shell32Wrapper.CMD_FIRST;
                                Helper.InvokeCommand(CContext, cmdidx, parent, new Point(x, y));
                            }

                           
                        }
                       
                    }
                 
                }
            }
            #region finally
            finally
            {
                if (PPopup != null)
                    User32Wrapper.DestroyMenu(PPopup);

                if (CContext3 != null)
                {
                    Marshal.FinalReleaseComObject(CContext3);
                    CContext3 = null;
                }

                if (CContext2 != null)
                {
                    Marshal.FinalReleaseComObject(CContext2);
                    CContext2 = null;
                }

                if (CContext != null)
                {
                    Marshal.FinalReleaseComObject(CContext);
                    CContext = null;
                }

                if (ChildrenList != null)
                {
                    foreach (var child in ChildrenList)
                        Marshal.FreeCoTaskMem(child);
                    ChildrenList = null;
                }

                if (SHParent != null)
                {
                    Marshal.FinalReleaseComObject(SHParent);
                    SHParent = null;
                }

                if(PIDLParent != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(PIDLParent);

                if (PSHParent != IntPtr.Zero)
                    Marshal.Release(PSHParent);

                if (PContext != IntPtr.Zero)
                    Marshal.Release(PContext);

                if (PContext2 != IntPtr.Zero)
                    Marshal.Release(PContext2);

                if (PContext3 != IntPtr.Zero)
                    Marshal.Release(PContext3);
            }
            #endregion
        }

        public static IShellFolder GetDesktop()
        {
            IntPtr PDesktop;
            Shell32Wrapper.SHGetDesktopFolder(out PDesktop);
            IShellFolder SHDesktop = (IShellFolder)Marshal.GetTypedObjectForIUnknown(PDesktop, typeof(IShellFolder));
            return SHDesktop;
        }

        public static void FinalReleaseComObject(object o)
        {
            Marshal.FinalReleaseComObject(o);
        }
    }
}
