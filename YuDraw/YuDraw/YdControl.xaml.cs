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

namespace YuDraw
{
    /// <summary>
    /// 色々ひとまとめにしたもの。他から利用しやすいようにコントロールにしておく
    /// </summary>
    public partial class YdControl : UserControl
    {
        public YdControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            YdHost h = new YdHost();
            RectangleGeometry rg = new RectangleGeometry(new Rect(new Point(10, 10), new Size(20, 20)));
            YdGeometryRenderInfo ri = new YdGeometryRenderInfo();
            ri.Geometry = rg;
            ri.Brush = new SolidColorBrush(Colors.BlueViolet);
            ri.Pen = new Pen(new SolidColorBrush(Colors.RosyBrown),1.0);
            h.RenderInfo = ri;
            this.ydCanvas.Children.Add(h);
        }
    }
}
