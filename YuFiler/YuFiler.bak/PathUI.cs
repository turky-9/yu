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
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace YuFiler
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:YuFiler"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:YuFiler;assembly=YuFiler"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:PathUI/>
    ///
    /// </summary>
    public class PathUI : Control, INotifyPropertyChanged
    {
        public static DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(PathUI), new PropertyMetadata(string.Empty, OnPathPropertyChanged), IsValidPath);

        protected static bool IsValidPath(object value)
        {
            string str = value as string;
            if (str != null)
            {
                if (!string.IsNullOrWhiteSpace(str) && !Directory.Exists(str))
                    return false;

                return true;
            }
            else
                return false;
        }

        protected static void OnPathPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PathUI obj = o as PathUI;
            if (obj != null)
            {
                if (!((string)e.NewValue).Equals((string)e.OldValue))
                {
                    obj.SetPath((string)e.NewValue);
                    obj.OnPropertyChanged("Path");
                }
            }
        }

        public string Path
        {
            get { return (string)this.GetValue(PathProperty); }
            set { this.SetValue(PathProperty, value); }
        }

        protected TextBox InputBox { get; set; }
        protected Grid DispUI { get; set; }


        protected List<PathUIButton> ButtonList { get; set; }

        static PathUI()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathUI), new FrameworkPropertyMetadata(typeof(PathUI)));
        }

        public PathUI()
        {
            this.InputBox = null;
            this.DispUI = null;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextBox box = this.GetTemplateChild("txtInp") as TextBox;
            if (box != null)
            {
                this.InputBox = box;
                this.InputBox.LostFocus += new RoutedEventHandler(CompleteDirectInput);
                this.InputBox.KeyDown += (o, e) =>
                    {
                        if (e.Key == Key.Enter)
                            this.CompleteDirectInput(null, null);
                    };
            }

            Grid gd = this.GetTemplateChild("gdDisp") as Grid;
            if (gd != null)
                this.DispUI = gd;
        }

    

        protected virtual void SetPath(string path)
        {
            this.ClearDispUI();
            if (this.DispUI != null)
            {
                string tmp;
                if (string.IsNullOrWhiteSpace(path))
                    tmp = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                else
                {
                    if (path.EndsWith("\\"))
                        tmp = path.Substring(0, path.Length - 1);
                    else
                        tmp = path;
                }
               

                string[] ele = tmp.Split('\\');

                this.ButtonList = new List<PathUIButton>();
                ColumnDefinition coldef;
                GridSplitter gsp;
                PathUIButton btn;
                for (int i = 0; i < ele.Length; i++)
                {
                    //要素を追加
                    coldef = new ColumnDefinition();
                    coldef.MinWidth = 10;
                    this.DispUI.ColumnDefinitions.Add(coldef);

                    btn = new PathUIButton() { Content = " " + ele[i] + "  " };
                    btn.Click += ElementButtonClick;
                    this.ButtonList.Add(btn);
                    this.DispUI.Children.Add(btn);

                    btn.Measure(new Size(1024, 100));
                    btn.MinWidth = 10;
                    coldef.Width = new GridLength(btn.DesiredSize.Width, GridUnitType.Auto);
                    Grid.SetColumn(btn, i * 2);

                    //セパレータを追加
                    double sepwidth = 2;
                    coldef = new ColumnDefinition();
                    coldef.Width = new GridLength(sepwidth);
                    this.DispUI.ColumnDefinitions.Add(coldef);

                    gsp = new GridSplitter();
                    gsp.ResizeDirection = GridResizeDirection.Columns;
                    LinearGradientBrush brush = new LinearGradientBrush();
                    brush.StartPoint = new Point(0, 0);
                    brush.EndPoint = new Point(0, 1);
                    brush.GradientStops.Add(new GradientStop() { Color = Colors.LightGray, Offset = 0 });
                    brush.GradientStops.Add(new GradientStop() { Color = Colors.Gray, Offset = 1 });
                    gsp.Background = brush;
                    gsp.Width = sepwidth;
                    gsp.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    gsp.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    gsp.ResizeBehavior = GridResizeBehavior.BasedOnAlignment;
                    this.DispUI.Children.Add(gsp);

                    Grid.SetColumn(gsp, (i * 2) + 1);
                }

                coldef = new ColumnDefinition();
                this.DispUI.ColumnDefinitions.Add(coldef);

                btn = new PathUIButton();
                btn.MinWidth = 10;
                btn.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                this.DispUI.Children.Add(btn);

                Grid.SetColumn(btn, ele.Length * 2);


                btn.Click += new RoutedEventHandler(DirectInput);
            }
        }

        protected virtual void ElementButtonClick(object sender, RoutedEventArgs e)
        {
            PathUIButton btn = sender as PathUIButton;
            if (btn != null)
            {
                List<string> buff = new List<string>();
                for (int i = 0; i < this.ButtonList.Count; i++)
                {
                    string tmp = (string)this.ButtonList[i].Content;
                    tmp = tmp.Trim();
                    if (i == 0)
                        tmp += "\\";
                    buff.Add(tmp.Trim());
                    if (object.ReferenceEquals(btn, this.ButtonList[i]))
                        break;
                }
                this.Path = System.IO.Path.Combine(buff.ToArray());
            }
        }

        protected virtual void DirectInput(object sender, RoutedEventArgs e)
        {
            this.DispUI.Visibility = System.Windows.Visibility.Collapsed;
            this.InputBox.Visibility = System.Windows.Visibility.Visible;
            this.InputBox.Select(0, this.InputBox.Text.Length);
            this.InputBox.Focus();
        }

        protected virtual void CompleteDirectInput(object sender, RoutedEventArgs e)
        {
            this.InputBox.Visibility = System.Windows.Visibility.Collapsed;
            this.DispUI.Visibility = System.Windows.Visibility.Visible;
        }

        protected void ClearDispUI()
        {
            if (this.DispUI != null)
            {
                this.DispUI.ColumnDefinitions.Clear();
                this.DispUI.Children.Clear();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string nm)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(nm));
        }
    }

    public class PathUIButton : Button
    {
        static PathUIButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathUIButton), new FrameworkPropertyMetadata(typeof(PathUIButton)));
        }
    }
}
