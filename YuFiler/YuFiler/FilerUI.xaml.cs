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
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Controls.WindowsPresentationFoundation;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace YuFiler
{
    /// <summary>
    /// FilerUI.xaml の相互作用ロジック
    /// </summary>
    public partial class FilerUI : UserControl
    {
        public IntPtr ParentHandle { get; set; }
        public IntPtr MyHandle { get; set; }

        protected string SaveFileName;

        public ObservableCollection<KeyValuePair<string,string>> BookMarks { get; set; }

        protected KeyBind KeyBind { get; set; }

        public FilerUI()
        {
            InitializeComponent();

            this.tabControl.SelectionChanged += new SelectionChangedEventHandler(TabSelectionChanged);

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

            this.BookMarks = new ObservableCollection<KeyValuePair<string, string>>();
            this.BookMarks.CollectionChanged += (o,e) =>
            {
                while (this.spUserBookMark.Children.Count != 0)
                    this.spUserBookMark.Children.RemoveAt(0);

                foreach (KeyValuePair<string, string> kv in this.BookMarks)
                {
                    Button btn = new Button();
                    btn.Content = kv.Value;
                    btn.Tag = kv.Key;

                    btn.Click += (oo, ee) =>
                    {
                        if(this.tabControl.Items.Count != 0)
                        {
                            ((TabItemUI)this.tabControl.SelectedItem).Go(ShellObject.FromParsingName((string)btn.Tag));
                        }
                    };

                    this.spUserBookMark.Children.Add(btn);
                }
            };

            /*
            this.SizeChanged += (o, e) =>
            {
                this.pathUI.Width = this.NavigateLaytout.ColumnDefinitions[2].ActualWidth;
            };
            */
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
                    break;
            }
        }

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
            if (msg == (int)EWindowMessage.KEYDOWN)
            {
                Key k = System.Windows.Input.KeyInterop.KeyFromVirtualKey(wParam.ToInt32());
                this.DataGridKeyDown(k);
            }
            return IntPtr.Zero;
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

        public virtual void ClearInitialize()
        {
            this.SaveFileName = "FilerPrevInfo_" + this.Name + ".xml";
            this.tabControl.Items.Clear();

            bool flg = true;
            if (File.Exists(this.SaveFileName))
            {
                flg = !this.Load();
            }

            if (flg)
            {
                string mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                TabItemUI ti = this.CreateTabItem((ShellObject)KnownFolders.Desktop);
                this.tabControl.Items.Add(ti);
                this.tabControl.SelectedItem = ti;
            }
        }
           

        protected virtual TabItemUI CreateTabItem(ShellObject path)
        {
            
            TabItemUI ti = new TabItemUI();
            ti.MaxWidth = 100;
            ti.Header = System.IO.Path.GetFileName(path.GetDisplayName(DisplayNameType.Default));

            ti.Go(path);

            ti.DispExpBrowser.ExplorerBrowserControl.NavigationComplete += (o, e) =>
            {
                Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser b = (Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser)o;
                b.Tag = e.NewLocation;

                TabItemUI tiui = this.tabControl.SelectedItem as TabItemUI;
                if (ti != null)
                {
                    this.pathUI.PropertyChanged -= PathPropertyChanged;
                    this.pathUI.Path = tiui.Get();
                    this.pathUI.PropertyChanged += PathPropertyChanged;

                    tiui.Header = e.NewLocation.Name;
                    tiui.DispExpBrowser.ViewMode = Microsoft.WindowsAPICodePack.Controls.ExplorerBrowserViewMode.Details;
                }
            };

            ContextMenu cm = new ContextMenu();
            MenuItem mi = new MenuItem();
            mi.Header = "Close";
            mi.Click += (o, e) =>
            {
                this.tabControl.Items.Remove(ti);
            };
            cm.Items.Add(mi);

            mi = new MenuItem();
            mi.Header = "BookMark";
            mi.Click += (o, e) =>
            {
                ShellObject shobj = (ShellObject)((TabItemUI)this.tabControl.SelectedItem).DispExpBrowser.ExplorerBrowserControl.Tag;
                string key = shobj.ParsingName;
                string val = shobj.Name;
                this.BookMarks.Add(new KeyValuePair<string, string>(key, val));
            };
            cm.Items.Add(mi);

            ti.ContextMenu = cm;

            return ti;
        }

        protected virtual void DataGridKeyDown(Key key)
        {
            /*
            List<FileSystemInfo> lst = new List<FileSystemInfo>();
            foreach (var v in ((TabItemUI)this.tabControl.SelectedItem).DispGrid.SelectedItems)
            {
                if (v is FileSystemInfo)
                    lst.Add((FileSystemInfo)v);
            }

            this.KeyBind.Execute(key, lst.ToArray());
            */
        }


        protected virtual void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(this.pathUI.Path))
            {
                string tmp = System.IO.Path.GetDirectoryName(this.pathUI.Path);
                //ルートの親はnullが帰ってくる
                if (tmp != null)
                    this.pathUI.Path = tmp;
            }
        }

        protected virtual void btnNewTab_Click(object sender, RoutedEventArgs e)
        {
            ShellObject path = null;
            if (this.tabControl.Items.Count == 0)
                path = (ShellObject)KnownFolders.Desktop;
            else
                path  = ((TabItemUI)this.tabControl.SelectedItem).DispExpBrowser.ExplorerBrowserControl.Tag as ShellObject;

            if (path != null)
            {
                TabItemUI ti = this.CreateTabItem(path);
                this.tabControl.Items.Add(ti);
                this.tabControl.SelectedItem = ti;
            }
        }

        protected virtual void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            ((TabItemUI)this.tabControl.SelectedItem).DispExpBrowser.NavigationLogIndex--;
        }

        protected virtual void btnNext_Click(object sender, RoutedEventArgs e)
        {
            ((TabItemUI)this.tabControl.SelectedItem).DispExpBrowser.NavigationLogIndex++;
        }

        private void btnDesktop_Click(object sender, RoutedEventArgs e)
        {
            ((TabItemUI)this.tabControl.SelectedItem).Go((ShellObject)KnownFolders.Desktop);
        }

        private void btnComputer_Click(object sender, RoutedEventArgs e)
        {
            ((TabItemUI)this.tabControl.SelectedItem).Go((ShellObject)KnownFolders.Computer);
        }

        public bool Load()
        {
            bool flg = true;
            try
            {
                FileStream fs = new FileStream(SaveFileName, FileMode.Open, FileAccess.Read);
                DataContractSerializer s = new DataContractSerializer(typeof(DatSaveInfo));
                DatSaveInfo info = (DatSaveInfo)s.ReadObject(fs);
                fs.Close();

                foreach (string path in info.Path)
                {
                    TabItemUI ti = this.CreateTabItem(ShellObject.FromParsingName(path));
                    this.tabControl.Items.Add(ti);
                }
                this.tabControl.SelectedIndex = info.SelectedIndex;
                foreach (KeyValuePair<string, string> kv in info.BookMarks)
                {
                    this.BookMarks.Add(kv);
                }
            }
            catch (Exception)
            {
                flg = false;
            }

            return flg;
        }

        public void Save()
        {
            try
            {
                DatSaveInfo info = new DatSaveInfo();

                foreach (TabItemUI ui in this.tabControl.Items)
                {
                    ShellObject shobj = (ShellObject)ui.DispExpBrowser.ExplorerBrowserControl.Tag;
                    info.Path.Add(shobj.ParsingName);
                }
                info.SelectedIndex = this.tabControl.SelectedIndex;
                foreach (KeyValuePair<string, string> kv in this.BookMarks)
                {
                    info.BookMarks.Add(kv);
                }


                FileStream fs = new FileStream(SaveFileName, FileMode.Create, FileAccess.Write);

                DataContractSerializer s = new DataContractSerializer(typeof(DatSaveInfo));
                s.WriteObject(fs, info);
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            this.DispExpBrowser = new ExplorerBrowser();
            this.DispExpBrowser.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.DispExpBrowser.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            this.DispExpBrowser.AdvancedQueryPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Hide;
            this.DispExpBrowser.CommandsOrganizePane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Hide;
            this.DispExpBrowser.CommandsPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Hide;
            this.DispExpBrowser.CommandsViewPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Hide;
            //this.DispExpBrowser.DetailsPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Hide;
            this.DispExpBrowser.NavigationPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Hide;
            this.DispExpBrowser.PreviewPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Hide;
            this.DispExpBrowser.QueryPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Hide;

            /*
            this.DispExpBrowser.AdvancedQueryPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Show;
            this.DispExpBrowser.CommandsOrganizePane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Show;
            this.DispExpBrowser.CommandsPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Show;
            this.DispExpBrowser.CommandsViewPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Show;
            //this.DispExpBrowser.DetailsPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Hide;
            this.DispExpBrowser.NavigationPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Show;
            this.DispExpBrowser.PreviewPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Show;
            this.DispExpBrowser.QueryPane = Microsoft.WindowsAPICodePack.Controls.PaneVisibilityState.Show;
             */

            this.Content = this.DispExpBrowser;

        }

        public ExplorerBrowser DispExpBrowser { get; set; }


        public void Go(string path)
        {
            this.Go(ShellObject.FromParsingName(path));
        }

        public void Go(ShellObject path)
        {
            this.DispExpBrowser.ExplorerBrowserControl.Navigate(path);
            this.DispExpBrowser.ExplorerBrowserControl.Tag = path;
        }

        public string Get()
        {
            ShellObject sobj = (ShellObject)this.DispExpBrowser.ExplorerBrowserControl.Tag;
            return sobj.ParsingName;
        }
    }
}
