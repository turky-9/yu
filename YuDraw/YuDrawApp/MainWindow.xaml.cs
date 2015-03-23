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
using YuDraw;

namespace YuDrawApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            /*
            this.rectangle1.MouseUp += (o, e) =>
            {
            //    MessageBox.Show("hoge");
            };

            this.canvas1.MouseLeftButtonUp += (o, e) =>
            {
                Point p = e.GetPosition(this.rectangle1);

                HitTestResultCallback cback = (x) =>
                {
                    if (object.ReferenceEquals(x.VisualHit, this.rectangle1))
                        MessageBox.Show("hit");
                    return HitTestResultBehavior.Continue;
                };
                VisualTreeHelper.HitTest(this.rectangle1, null, cback, new GeometryHitTestParameters(new EllipseGeometry(p, 10, 10)));
            };

            this.canvas1.Children.Add(new MyHost());
            */

            this.WindowStyle = System.Windows.WindowStyle.None;
            this.WindowState = System.Windows.WindowState.Maximized;
            this.Topmost = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //int a = 0;

        }
    }


    /// <summary>
    /// DrawingVisualをホストするにはFrameworkElementから継承したホストクラスが必要
    /// </summary>
    public class MyHost : FrameworkElement, IYdElement
    {
        private VisualCollection Children = null;
        private Geometry geo = null;

        public MyHost()
        {
            this.Children = new VisualCollection(this);

            DrawingVisual v = new DrawingVisual();
            DrawingContext ctx = v.RenderOpen();
            //ctx.DrawRectangle(new SolidColorBrush(Colors.Aqua), null, new Rect(new Point(10, 10), new Size(20, 50)));
            this.DrawSelectedSqure(ctx);
            ctx.Close();
            this.Children.Add(v);

            this.MouseLeftButtonUp += (o, e) =>
            {
                Point p = e.GetPosition(this);
                HitTestResult r = VisualTreeHelper.HitTest(this, p);
                if (r != null)
                {
                    DrawingVisual dv = r.VisualHit as DrawingVisual;
                    if (dv != null)
                    {
                        RectangleGeometry tmp = (RectangleGeometry)this.geo;
                        tmp.Rect = new Rect(tmp.Rect.X + 10, tmp.Rect.Y, tmp.Rect.Width, tmp.Rect.Height);
                        DrawingContext c = v.RenderOpen();
                        c.DrawRectangle(new SolidColorBrush(Colors.White), null, tmp.Rect);
                        c.Close();
                    }
                }
            };
        }

        protected void DrawSelectedSqure(DrawingContext ctx)
        {
            Brush bsh = new SolidColorBrush(Colors.White);
            Pen pen = new Pen();
            pen.Brush = new SolidColorBrush(Colors.Black);
            //pen.Thickness = 1 * (1 / PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11);
            pen.Thickness = 1;

            Rect rect = new Rect(new Point(50,10),new Size(141,94));
            this.geo = new RectangleGeometry(rect);
            /*
            GuidelineSet g = new GuidelineSet();
            g.GuidelinesX.Add(rect.Left + 0.5);
            g.GuidelinesX.Add(rect.Right+ 0.5);
            g.GuidelinesY.Add(rect.Top+ 0.5);
            g.GuidelinesY.Add(rect.Bottom+ 0.5);

            ctx.PushGuidelineSet(g);
            ctx.DrawRectangle(bsh,pen,rect);
            ctx.Pop();
            */
            ctx.DrawRectangle(bsh,pen,rect);
        }

        #region Childrenを管理する為のメンバーをoverride
        protected override int VisualChildrenCount
        {
            get { return this.Children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= this.Children.Count)
                throw new ArgumentOutOfRangeException("index");
            return this.Children[index];
        }
        #endregion
    }
}
