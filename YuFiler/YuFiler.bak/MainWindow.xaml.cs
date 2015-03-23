using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WIN32APIWapper;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell;

namespace YuFiler
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            //this.filerUI.ParentHandle = helper.Handle;
            //this.filerUI.ClearInitialize();
        }

        //色々サンプルを書きたいのよ
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.hoge.ExplorerBrowserControl.Navigate((ShellObject)KnownFolders.Desktop);
                return;
                IntPtr PDesktop;
                Shell32Wrapper.SHGetDesktopFolder(out PDesktop);

                IShellFolder SHDesktop = (IShellFolder)Marshal.GetTypedObjectForIUnknown(PDesktop, typeof(IShellFolder));

                IntPtr strr = Marshal.AllocCoTaskMem(Shell32Wrapper.MAX_PATH * 2 + 4);
                Marshal.WriteInt32(strr, 0, 0);

                IntPtr ptrPid;
                Shell32Wrapper.SHGetSpecialFolderLocation(IntPtr.Zero, (int)SpecialFolderID.CSIDL_DRIVES, out ptrPid);

                //なんかDesktop自身の名前は取得方法がわからん
                //if (SHDesktop.GetDisplayNameOf(desktop, SHGNO.NORMAL, strr) == Shell32Wrapper.S_OK)
                if (SHDesktop.GetDisplayNameOf(ptrPid, SHGNO.NORMAL, strr) == Shell32Wrapper.S_OK)
                {
                    StringBuilder buf = new StringBuilder(Shell32Wrapper.MAX_PATH);
                    ShlwapiWrapper.StrRetToBuf(strr, PDesktop, buf, Shell32Wrapper.MAX_PATH);
                    MessageBox.Show(buf.ToString());
                }
                Marshal.FreeCoTaskMem(strr);


                //対象ファイルの親のシェルフォルダの PIDL を取得する
                IntPtr PFolder;
                uint fcharcnt = 0;
                SFGAO fattr = SFGAO.BROWSABLE;
                if (SHDesktop.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, @"C:\temp", ref fcharcnt, out PFolder, ref fattr) == Shell32Wrapper.S_OK)
                {
                    //対象ファイルの親のシェルフォルダのポインタを取得する
                    IntPtr PPV;
                    if (SHDesktop.BindToObject(PFolder, IntPtr.Zero, GUIDs.IID_IShellFolder, out PPV) == Shell32Wrapper.S_OK)
                    {
                        //対象ファイルの親のシェルフォルダ IShellFolder を取得する
                        IShellFolder SHFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(PPV, typeof(IShellFolder));

                        //対象ファイルの PIDL (親のシェルフォルダからの相対 PIDL)を取得する
                        IntPtr PFile;
                        if (SHFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, "a.vbs", ref fcharcnt, out PFile, ref fattr) == Shell32Wrapper.S_OK)
                        {
                            //対象ファイルの IContextMenu へのポインタを取得する
                            IntPtr[] children = new IntPtr[]{PFile};
                            IntPtr PContext;
                            if (SHFolder.GetUIObjectOf(IntPtr.Zero, (uint)children.Length, children, GUIDs.IID_IContextMenu, IntPtr.Zero, out PContext) == Shell32Wrapper.S_OK)
                            {
                                //対象ファイルの IContextMenu を取得する
                                IContextMenu CContext = (IContextMenu)Marshal.GetTypedObjectForIUnknown(PContext, typeof(IContextMenu));

                                //対象ファイルの IContextMenu2, IContextMenu3 のポインタを取得する
                                IntPtr PContext2,PContext3;
                                Marshal.QueryInterface(PContext, ref GUIDs.IID_IContextMenu2, out PContext2);
                                Marshal.QueryInterface(PContext, ref GUIDs.IID_IContextMenu3, out PContext3);

                                IContextMenu2 CContext2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(PContext2, typeof(IContextMenu2));
                                IContextMenu3 CContext3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(PContext3, typeof(IContextMenu3));

                                //ポップアップメニューを作成する
                                IntPtr PPopup = User32Wrapper.CreatePopupMenu();

                                //ポップアップメニューに、コンテキストメニュー IContextMenu を追加する
                                CMF ContextMenuFlag = CMF.EXPLORE | CMF.CANRENAME;
                                CContext.QueryContextMenu(PPopup, 0, Shell32Wrapper.CMD_FIRST, Shell32Wrapper.CMD_LAST, ContextMenuFlag);

                                //ポップアップメニューを表示する
                                //呼び出しをブロックします
                                uint selected = User32Wrapper.TrackPopupMenuEx(PPopup, TPM.RETURNCMD, 0, 0, new System.Windows.Interop.WindowInteropHelper(this).Handle, IntPtr.Zero);
                                if (selected >= Shell32Wrapper.CMD_FIRST)
                                {
                                    uint cmdidx = selected - Shell32Wrapper.CMD_FIRST;


                                    Helper.InvokeCommand(CContext, cmdidx, @"C:\temp", new Point(0, 0));
                                }

                                User32Wrapper.DestroyMenu(PPopup);

                                Marshal.ReleaseComObject(CContext3);
                                Marshal.ReleaseComObject(CContext2);

                                Marshal.Release(PContext3);
                                Marshal.Release(PContext2);

                                Marshal.ReleaseComObject(CContext);
                            }
                            Marshal.Release(PContext);
                        }
                        Marshal.FreeCoTaskMem(PFile);
                        Marshal.ReleaseComObject(SHFolder);
                    }
                    Marshal.Release(PPV);
                }
                //なんでかExceptionが発生するのでコメントアウト　←　解放するメソッドを間違えていた
                //Marshal.Release(PIDLParent);
                Marshal.FreeCoTaskMem(PFolder);
                Marshal.ReleaseComObject(SHDesktop);
                Marshal.Release(PDesktop);



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

    }
}
