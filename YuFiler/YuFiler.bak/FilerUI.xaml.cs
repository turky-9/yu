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
using System.IO;
using System.Drawing;
using System.Diagnostics;
using WIN32APIWapper;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace YuFiler
{
    /// <summary>
    /// FilerUI.xaml の相互作用ロジック
    /// </summary>
    public partial class FilerUI : UserControl
    {
        public IntPtr ParentHandle { get; set; }
        public IntPtr MyHandle { get; set; }

        public IShellFolder SHDesktop { get; set; }

        protected KeyBind KeyBind { get; set; }

        public FilerUI()
        {
            InitializeComponent();

            this.tabControl.SelectionChanged += new SelectionChangedEventHandler(TabSelectionChanged);
            this.SHDesktop = Helper.GetDesktop();

            this.InitKeyBind();

            this.Loaded += (o, e) =>
            {
                HwndSource source = HwndSource.FromVisual(this) as HwndSource;
                if (source != null)
                {
                    this.MyHandle = source.Handle;

                    //コンテキストメニューの「新規作成」の為に必要
                    source.AddHook(this.WndProc);
                }
            };
        }

        private void InitKeyBind()
        {
            this.KeyBind = new KeyBind();
            this.KeyBind.InternalKeyBindExecute += InternalKeyBindExecute;

            KeyBindItem item = new KeyBindItem();
            item.Command = @"C:\Program Files\Hidemaru\Hidemaru.exe";
            item.PushDownKey = Key.E;
            item.Modifiers = ModifierKeys.None;
            item.ExecKbn = EKeyBindExecKbn.External;
            this.KeyBind.AddItem(item);

            item = new KeyBindItem();
            item.PushDownKey = Key.F5;
            item.Modifiers = ModifierKeys.None;
            item.ExecKbn = EKeyBindExecKbn.Internal;
            item.InternalKbn = EKeyBindInternalKbn.ReDisp;
            this.KeyBind.AddItem(item);
        }

        private void InternalKeyBindExecute(object sender, KeyBindInternalEventArgs e)
        {
            switch (e.Kbn)
            {
                case EKeyBindInternalKbn.ReDisp:
                    TabItemUI ti = this.tabControl.SelectedItem as TabItemUI;
                    if (ti != null)
                        ti.ReDisp();
                    break;
            }
        }

        private IContextMenu2 FolderContextMenu;
        private IntPtr FolderContextPtr;
        /// <summary>
        /// とりあえず属性つけとかないとWindowsからメッセージが来まくるのでデバッグがしんどくなる。
        /// 適宜外して下さい。
        /// </summary>
        /// <param name="hwnm"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private IntPtr WndProc(IntPtr hwnm, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (FolderContextMenu != null &&
               ((msg == (int)WM.INITMENUPOPUP && wParam == FolderContextPtr) ||
                msg == (int)WM.MEASUREITEM ||
                msg == (int)WM.DRAWITEM))
            {
                if (FolderContextMenu.HandleMenuMsg(
                    (uint)msg, wParam, lParam) == Shell32Wrapper.S_OK)
                    return IntPtr.Zero;
            }

            if (msg == (int)WM.KEYDOWN)
            {
                Key k = System.Windows.Input.KeyInterop.KeyFromVirtualKey(wParam.ToInt32());
                this.DataGridKeyDown(k);
            }
            return IntPtr.Zero;
        }

        ~FilerUI()
        {
            if (this.SHDesktop != null)
                Helper.FinalReleaseComObject(this.SHDesktop);
        }

        protected virtual void TabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItemUI ti = this.tabControl.SelectedItem as TabItemUI;
            if(ti != null)
            {
                this.pathUI.PropertyChanged -= PathPropertyChanged;
                this.pathUI.Path = ti.Get();
                this.pathUI.PropertyChanged += PathPropertyChanged;
            }
        }

        protected virtual void PathPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            TabItemUI ti = this.tabControl.SelectedItem as TabItemUI;
            if (ti != null)
            {
                ti.Header = System.IO.Path.GetFileName(this.pathUI.Path);
                if (string.IsNullOrEmpty((string)ti.Header))
                    ti.Header = this.pathUI.Path;

                ti.Go(this.pathUI.Path);
            }
        }

        public  virtual void ClearInitialize()
        {
            this.tabControl.Items.Clear();

            string mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            TabItemUI ti = this.CreateTabItem(mydoc);
            this.tabControl.Items.Add(ti);
            this.tabControl.SelectedItem = ti;

            this.TreeViewInitialize(mydoc);
        }

        protected virtual void TreeViewInitialize(string path)
        {
            string[] ele = path.Split('\\');

            TreeViewItemUI target = null;
            TreeViewItemUI desktop = new TreeViewItemUI();
            desktop.Header = "Desktop";
            desktop.Tag = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            TreeViewItemUI mycom = new TreeViewItemUI();
            mycom.Header = "MyComputer";
            //mycom.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            mycom.IsScaned = true;
            desktop.Items.Add(mycom);

            foreach (var d in Directory.GetLogicalDrives())
            {
                TreeViewItemUI t = new TreeViewItemUI() { Header = d };
                t.Tag = d;
                if(char.ToUpper(ele[0][0]) == char.ToUpper(d[0]))
                    target = t;
                mycom.Items.Add(t);
            }


            if (target != null)
            {
                string fullpath = (string)target.Header;
                this.MakeTree(target, fullpath);
                for (int i = 1; i < ele.Length; i++)
                {
                    for(int j = 0; j < target.Items.Count;j++)
                    {
                        if (ele[i].Equals((string)((TreeViewItemUI)target.Items[j]).Header))
                        {
                            fullpath = System.IO.Path.Combine(fullpath, ele[i]);
                            this.MakeTree((TreeViewItemUI)target.Items[j], fullpath);
                            target = (TreeViewItemUI)target.Items[j];
                            break;
                        }
                    }
                }
            }

            this.tvControl.Items.Add(desktop);

            this.tvControl.SelectedItemChanged += TreeSelectedItemChanged;
        }

        protected virtual void TreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string s = (string)((TreeViewItemUI)this.tvControl.SelectedItem).Tag;
            this.pathUI.Path = s;
            if (((TreeViewItemUI)this.tvControl.SelectedItem).IsScaned == false)
                this.MakeTree((TreeViewItemUI)this.tvControl.SelectedItem, s);
            e.Handled = true;
        }

        protected virtual void MakeTree(TreeViewItemUI parent, string path)
        {
            try
            {
                foreach (var ele in Directory.GetDirectories(path))
                {
                    TreeViewItemUI t = new TreeViewItemUI() { Header = System.IO.Path.GetFileName(ele) };
                    t.Tag = System.IO.Path.Combine(path, ele);
                    parent.Items.Add(t);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("アクセスできません", "Unauthorized", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
            parent.IsScaned = true;
        }

        protected virtual TabItemUI CreateTabItem(string path)
        {
            TabItemUI ti = new TabItemUI();
            ti.MaxWidth = 100;
            ti.Header = System.IO.Path.GetFileName(path);

            ti.DispGrid = this.CreateDataGrid();
            ti.Go(path);
            ti.Content = ti.DispGrid;

            return ti;
        }

        protected virtual DataGrid CreateDataGrid()
        {
            DataGrid dg = new FileListDataGrid();
            dg.CanUserAddRows = false;
            dg.AutoGenerateColumns = false;
            dg.IsReadOnly = true;
            dg.SelectionMode = DataGridSelectionMode.Extended;
            dg.SelectionUnit = DataGridSelectionUnit.FullRow;
            dg.GridLinesVisibility = DataGridGridLinesVisibility.None;
            dg.MouseDoubleClick += DataGridDoubleClick;
            //データグリッドのSelectionChangedがbubble upしてタブのSlectionChangedがfireされないようHandled=trueにする
            dg.SelectionChanged += (o, e) =>
            {
                e.Handled = true;
            };

            //なんかデータグリッドでなにも選択してない時に、キーイベントをどうやっても拾えなかった。
            //頭来たからWndProcで拾うようにした
            //dg.KeyDown += DataGridKeyDown;

            //コンテキストはWin32APIを使う
            //dg.ContextMenu = this.CreateContextMenu();
            dg.MouseRightButtonUp += DataGridMouseRightButtonUp;

            //DataGridTemplateColumn tmpc = (DataGridTemplateColumn)this.Resources["IconColumn"];
            //tmpc.Width = 30;
            //dg.Columns.Add(tmpc);
            DataTemplate dtmpl = (DataTemplate)this.Resources["Dtmpl"];
            DataGridTemplateColumn tmpc = new DataGridTemplateColumn();
            tmpc.CellTemplate = dtmpl;
            tmpc.Width = 20;
            tmpc.MaxWidth = 20;
            dg.Columns.Add(tmpc);


            DataGridTextColumn tc = new DataGridTextColumn();
            tc.Width = 180;
            Binding bin = new Binding("Name");
            bin.Mode = BindingMode.OneWay;
            tc.Header = "Name";
            tc.Binding = bin;
            dg.Columns.Add(tc);

            tc = new DataGridTextColumn();
            tc.Width = 80;
            bin = new Binding("LastWriteTime");
            bin.Mode = BindingMode.OneWay;
            bin.StringFormat = "yyyy/MM/dd hh:mm:ss";
            tc.Header = "Modified";
            tc.Binding = bin;
            dg.Columns.Add(tc);

            tc = new DataGridTextColumn();
            tc.Width = 80;
            bin = new Binding("Length");
            bin.Mode = BindingMode.OneWay;
            bin.Converter = (SizeConverter)this.Resources["Sconv"];
            tc.Header = "Size";
            tc.Binding = bin;
            Style styl = new System.Windows.Style();
            styl.TargetType = typeof(TextBlock);
            Setter ster = new Setter();
            ster.Property = TextBlock.HorizontalAlignmentProperty;
            ster.Value = System.Windows.HorizontalAlignment.Right;
            styl.Setters.Add(ster);
            tc.ElementStyle = styl;
            dg.Columns.Add(tc);

            dg.AllowDrop = true;
            dg.Drop += this.ListViewDrop;

            IntPtr drophelpPtr;
            IDropTargetHelper dropHelper;
            ShellHelper.GetIDropTargetHelper(out drophelpPtr, out dropHelper);
            dg.DragEnter += (o, e) =>
            {
                if (dropHelper != null)
                {
                    //マネージオブジェクトのIntPtrを取得する方法
                    //でもblittableでないとか言って怒られたorz
                    //DragEnterの第２引数の型をIntPtrから変更した
                    //GCHandle gch = GCHandle.Alloc(e.Data, GCHandleType.Pinned);
                    //IntPtr p = gch.AddrOfPinnedObject();
                    //gch.Free();
                    POINT p = new POINT();
                    p.x = (int)e.GetPosition(this).X;
                    p.y = (int)e.GetPosition(this).Y;
                    dropHelper.DragEnter(this.MyHandle, (System.Runtime.InteropServices.ComTypes.IDataObject)e.Data, ref p, e.Effects);
                }
            };

            dg.DragOver += (o, e) =>
            {
                if (dropHelper != null)
                {
                    POINT p = new POINT();
                    p.x = (int)e.GetPosition(this).X;
                    p.y = (int)e.GetPosition(this).Y;
                    dropHelper.DragOver(p, e.Effects);
                }
            };

            dg.DragLeave += (o, e) =>
            {
                if (dropHelper != null)
                {
                    dropHelper.DragLeave();
                }
            };

            //todo
            //列と列の間はBorderの様なので、列と列の間をクリックすると変な動作してカッコ悪い。
            //あと、選択を解除した後に現在のセル？なのか黒い四角が残るのがかっこ悪い。
            dg.MouseLeftButtonUp += (o, e) =>
            {
                if (!(((DataGrid)o).InputHitTest(e.GetPosition(dg)) is TextBlock))
                {
                    dg.SelectedItem = null;
                }
            };
          
            return dg;
        }

        /// <summary>
        /// ファイル一覧にドラッグされた時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ListViewDrop(object sender, DragEventArgs e)
        {
            IntPtr drophelpPtr;
            IDropTargetHelper dropHelper;
            ShellHelper.GetIDropTargetHelper(out drophelpPtr, out dropHelper);
            if (dropHelper != null)
            {
                 POINT p = new POINT();
                 p.x = (int)e.GetPosition(this).X;
                 p.y = (int)e.GetPosition(this).Y;
                 dropHelper.Drop((System.Runtime.InteropServices.ComTypes.IDataObject)e.Data, p, e.Effects);
            }
            FileListDataGrid dg = sender as FileListDataGrid;
            if (dg == null)
                return;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                dg.DragMouseButton = DragDropKeyStates.None;
                return;
            }

            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null)
            {
                dg.DragMouseButton = DragDropKeyStates.None;
                return;
            }

            string itemprefix = files[0].Substring(0, 2).ToUpper();
            string myprefix = this.pathUI.Path.Substring(0, 2).ToUpper();
            if (dg.DragMouseButton == DragDropKeyStates.LeftMouseButton)
            {
                if (itemprefix.Equals(@"\\") || myprefix.Equals(@"\\"))
                    this.FileCopy(files);
                else
                {
                    if (itemprefix.Equals(myprefix))
                        this.FileMove(files);
                    else
                        this.FileCopy(files);
                }
            }
            else if (dg.DragMouseButton == DragDropKeyStates.RightMouseButton)
            {
                #region
                ContextMenu cm = new ContextMenu();
                MenuItem mi = new MenuItem();
                mi.Header = "Copy";
                mi.Click += (o, arg) =>
                {
                    cm.IsOpen = false;
                    this.FileCopy(files);
                };
                cm.Items.Add(mi);

                mi = new MenuItem();
                mi.Header = "Move";
                mi.Click += (o, arg) =>
                {
                    cm.IsOpen = false;
                    this.FileMove(files);
                };
                cm.Items.Add(mi);
                cm.IsOpen = true;
                #endregion
            }
        }

        protected virtual void FileCopy(string[] files)
        {
            SHFILEOPSTRUCT info = new SHFILEOPSTRUCT();
            info.hwnd = this.ParentHandle;
            info.wFunc = FOFunc.FO_COPY;

            //ファイル名の区切りは'\0'で最後にはもう一つ'\0'が必要
            foreach (string f in files)
                info.pFrom = f + '\0';
            info.pFrom = info.pFrom + '\0';
            info.pTo = this.pathUI.Path;

            Shell32Wrapper.SHFileOperation(ref info);

            TabItemUI ti = this.tabControl.SelectedItem as TabItemUI;
            if (ti != null)
                ti.ReDisp();
        }

        protected virtual void FileMove(string[] files)
        {
            SHFILEOPSTRUCT info = new SHFILEOPSTRUCT();
            info.hwnd = this.ParentHandle;
            info.wFunc = FOFunc.FO_MOVE;

            //ファイル名の区切りは'\0'で最後にはもう一つ'\0'が必要
            foreach (string f in files)
                info.pFrom = f + '\0';
            info.pFrom = info.pFrom + '\0';
            info.pTo = this.pathUI.Path;

            Shell32Wrapper.SHFileOperation(ref info);

            TabItemUI ti = this.tabControl.SelectedItem as TabItemUI;
            if (ti != null)
                ti.ReDisp();
        }

        protected virtual void DataGridMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            List<FileSystemInfo> lst = new List<FileSystemInfo>();
            foreach (var v in ((TabItemUI)this.tabControl.SelectedItem).DispGrid.SelectedItems)
            {
                if (v is FileSystemInfo)
                    lst.Add((FileSystemInfo)v);
            }

            if (lst.Count == 0)
                lst = null;

            var p = this.PointToScreen(e.GetPosition(this));
            Helper.ShowContextMenu(this.ParentHandle, this.SHDesktop, this.pathUI.Path, lst, p.X, p.Y, ref this.FolderContextMenu, ref this.FolderContextPtr);

            //TabItemUI ti = this.tabControl.SelectedItem as TabItemUI;
            //if (ti != null)
            //    ti.ReDisp();
        }

        protected virtual void DataGridKeyDown(Key key)
        {
            List<FileSystemInfo> lst = new List<FileSystemInfo>();
            foreach (var v in ((TabItemUI)this.tabControl.SelectedItem).DispGrid.SelectedItems)
            {
                if (v is FileSystemInfo)
                    lst.Add((FileSystemInfo)v);
            }

            this.KeyBind.Execute(key, lst.ToArray());
        }

        /// <summary>
        /// 今使ってない
        /// </summary>
        /// <returns></returns>
        protected virtual ContextMenu CreateContextMenu()
        {
            ContextMenu cm = new ContextMenu();
            MenuItem mi = new MenuItem();
            mi.Header = "Property";
            mi.Click += (o, e) =>
                {
                    TabItemUI ti = this.tabControl.SelectedItem as TabItemUI;
                    if (ti != null)
                    {
                        List<string> lst = new List<string>();
                        foreach (var obj in ti.DispGrid.SelectedItems)
                        {
                            FileSystemInfo fsi = obj as FileSystemInfo;
                            if (fsi != null)
                                lst.Add(fsi.FullName);
                        }
                        if (lst.Count == 1)
                            Shell32Wrapper.ShowProperty(this.ParentHandle, lst[0]);
                        else if (lst.Count > 1)
                        {
                            //動かん
                            //Shell32Wrapper.ShowMultiFileProperties(lst.ToArray());

                        }

                    }
                };
            cm.Items.Add(mi);

            return cm;
        }

        protected virtual void DataGridDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                DirectoryInfo di = dg.SelectedItem as DirectoryInfo;
                if (di != null)
                {
                    this.pathUI.Path = di.FullName;
                }

                FileInfo fi = dg.SelectedItem as FileInfo;
                if (fi != null)
                {
                    try
                    {
                        ProcessStartInfo psinfo = new ProcessStartInfo();
                        psinfo.FileName = fi.FullName;
                        psinfo.CreateNoWindow = false;
                        psinfo.UseShellExecute = true;
                        Process proc = Process.Start(psinfo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

      

        protected virtual void btnUp_Click(object sender, RoutedEventArgs e)
        {
            string tmp = System.IO.Path.GetDirectoryName(this.pathUI.Path);
            //ルートの親はnullが帰ってくる
            if(tmp != null)
                this.pathUI.Path = tmp;
        }

        protected virtual void btnNewTab_Click(object sender, RoutedEventArgs e)
        {
            TabItemUI ti = this.CreateTabItem(this.pathUI.Path);
            this.tabControl.Items.Add(ti);
            this.tabControl.SelectedItem = ti;
        }

        protected virtual void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            TabItemUI ti = this.tabControl.SelectedItem as TabItemUI;
            if (ti != null)
            {
                string tmp = ti.PrevPath();
                if (!string.IsNullOrEmpty(tmp))
                {
                    this.pathUI.PropertyChanged -= PathPropertyChanged;
                    this.pathUI.Path = tmp;
                    this.pathUI.PropertyChanged += PathPropertyChanged;
                }
            }
        }

        protected virtual void btnNext_Click(object sender, RoutedEventArgs e)
        {
            TabItemUI ti = this.tabControl.SelectedItem as TabItemUI;
            if (ti != null)
            {
                string tmp = ti.NextPath();
                if (!string.IsNullOrEmpty(tmp))
                {
                    this.pathUI.PropertyChanged -= PathPropertyChanged;
                    this.pathUI.Path = tmp;
                    this.pathUI.PropertyChanged += PathPropertyChanged;
                }
            }
        }

    }

    public class FileListDataGrid : DataGrid
    {
        static FileListDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileListDataGrid), new FrameworkPropertyMetadata(typeof(DataGrid)));
        }

        public DragDropKeyStates DragMouseButton { get; set; }

        public FileListDataGrid()
        {
            this.DragMouseButton = DragDropKeyStates.None;

            this.DragEnter += (o, e) =>
            {
                this.DragMouseButton = e.KeyStates;
            };

            this.DragOver += (o, e) =>
            {
                this.DragMouseButton = e.KeyStates;
            };
            this.DragLeave += (o, e) =>
            {
                this.DragMouseButton = DragDropKeyStates.None;
            };
        }

      
    }


    public class TabItemUI : TabItem
    {
        static TabItemUI()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItemUI), new FrameworkPropertyMetadata(typeof(TabItem)));
        }

        public TabItemUI()
        {
            this.DispGrid = null;
            this.PathHistory = new List<string>();
            this.HistoryIdx = -1;
        }

        public DataGrid DispGrid { get; set; }
        protected  List<string> PathHistory { get; set; }
        public int HistoryIdx { get; protected set; }

        public string Get()
        {
            if (this.HistoryIdx == -1)
                return null;
            return this.PathHistory[this.HistoryIdx];
        }

        public void ReDisp()
        {
            if (this.HistoryIdx == -1)
                return;
            if (this.DispGrid != null)
                this.DispGrid.ItemsSource = this.GetItems(this.Get());

        }

        public void Go(string path)
        {
            if (this.HistoryIdx + 1 == this.PathHistory.Count)
                this.PathHistory.Add(path);
            else
                this.PathHistory[this.HistoryIdx + 1] = path;

            this.HistoryIdx++;

            if(this.DispGrid != null)
                this.DispGrid.ItemsSource = this.GetItems(path);
        }

        public string PrevPath()
        {
            if (this.HistoryIdx <= 0)
                return null;
            else
            {
                this.HistoryIdx--;
                if(this.DispGrid != null)
                    this.DispGrid.ItemsSource = this.GetItems(this.PathHistory[this.HistoryIdx]);
                return this.PathHistory[this.HistoryIdx];
            }
        }
        public string NextPath()
        {
            if (this.HistoryIdx + 1 == this.PathHistory.Count)
                return null;
            else
            {
                this.HistoryIdx++;
                if(this.DispGrid != null)
                    this.DispGrid.ItemsSource = this.GetItems(this.PathHistory[this.HistoryIdx]);
                return this.PathHistory[this.HistoryIdx];
            }
        }

        protected virtual List<FileSystemInfo> GetItems(string path)
        {
            List<FileSystemInfo> ret = new List<FileSystemInfo>();
            try
            {
                foreach (string dir in Directory.GetDirectories(path))
                    ret.Add(new DirectoryInfo(dir));

                foreach (string fl in Directory.GetFiles(path))
                    ret.Add(new FileInfo(fl));
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("アクセスできません", "Unauthorized", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return ret;
        }
    }

    public class TreeViewItemUI : TreeViewItem
    {
        static TreeViewItemUI()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewItemUI), new FrameworkPropertyMetadata(typeof(TreeViewItem)));
        }

        public bool IsScaned { get; set; }

        public TreeViewItemUI()
        {
            this.IsScaned = false;
        }
    }
}
