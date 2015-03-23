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
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell;
using System.IO;

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

            this.Closing += (o, e) =>
            {
                this.filerUI1.Save();
                this.filerUI2.Save();
                this.Save();
            };
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            this.Load();

            this.filerUI1.ParentHandle = helper.Handle;
            this.filerUI1.ClearInitialize();

            this.filerUI2.ParentHandle = helper.Handle;
            this.filerUI2.ClearInitialize();
        }

        //色々サンプルを書きたいのよ
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //this.hoge.ExplorerBrowserControl.Navigate((ShellObject)KnownFolders.Desktop);
                return;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private const string SaveFileName = "YuFiler.xml";
        private void Load()
        {
            if (!File.Exists(SaveFileName))
                return;

            try
            {
                FileStream fs = new FileStream(SaveFileName, FileMode.Open, FileAccess.Read);
                System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(double[]));
                double[] info = (double[])s.Deserialize(fs);
                fs.Close();

                this.Height = info[0];
                this.Width = info[1];
                this.LayoutRoot.RowDefinitions[0].Height = new GridLength(info[2]);
                //this.LayoutRoot.RowDefinitions[2].Height = new GridLength(info[3]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Save()
        {
            try
            {
                double[] info = new double[] { this.ActualHeight, this.ActualWidth, this.LayoutRoot.RowDefinitions[0].ActualHeight, this.LayoutRoot.RowDefinitions[2].ActualHeight };

                FileStream fs = new FileStream(SaveFileName, FileMode.Create, FileAccess.Write);
                System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(double[]));
                s.Serialize(fs, info);
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
