using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;

namespace YuDraw
{
    /// <summary>
    /// 図形を表示する為（ホストする為）のFrameworkElement拡張
    /// </summary>
    public class YdHost : FrameworkElement, IYdElement
    {
        /// <summary>
        /// 表示する図形
        /// </summary>
        private VisualCollection Children = null;

        public event EventHandler<SelectedEventArgs> Selected;

        /// <summary>
        /// プロパティの実態
        /// </summary>
        private YdGeometryRenderInfo _info = null;

        /// <summary>
        /// 図形の情報
        /// </summary>
        public YdGeometryRenderInfo RenderInfo
        {
            get { return this._info; }
            set
            {
                if (this.RenderInfo != value)
                {
                    this.Rendering(value);
                    this._info = value;

                    this._info.PropertyChanged -= this.PropertyChanged;
                    this._info.PropertyChanged += this.PropertyChanged;
                }
            }
        }

        public YdHost()
        {
            this.Children = new VisualCollection(this);
            this.Selected = null;
            this.MouseLeftButtonUp += YdHostMouseLeftButtonUp;
        }

        /// <summary>
        /// 自分が選択された事を知らせるイベントを発行する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void YdHostMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HitTestResultCallback cback = (x) =>
            {
                DrawingVisual dv = x.VisualHit as DrawingVisual;
                if(dv != null)
                {
                    if (this.Selected != null)
                        this.Selected(this, new SelectedEventArgs());
                }
                return HitTestResultBehavior.Stop;
            };
            VisualTreeHelper.HitTest(this.GetVisualChild(0), null, cback, new GeometryHitTestParameters(new EllipseGeometry(e.GetPosition(this), 10, 10)));
        }

        /// <summary>
        /// YdHost内を描画する
        /// </summary>
        /// <param name="info"></param>
        protected virtual void Rendering(YdGeometryRenderInfo info)
        {
            DrawingVisual v = new DrawingVisual();
            DrawingContext ctx = v.RenderOpen();
            ctx.DrawGeometry(info.Brush, info.Pen, info.Geometry);
            ctx.Close();
            this.Children.Add(v);
        }

        /// <summary>
        /// 図形情報が変更されたら再描画する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            YdGeometryRenderInfo info = sender as YdGeometryRenderInfo;
            if (info != null)
                this.Rendering(info);
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

    /// <summary>
    /// イベントの引数
    /// </summary>
    public class SelectedEventArgs : EventArgs
    {
        /// <summary>
        /// なんだっけ？ひょっとして追加選択かな
        /// </summary>
        public bool IsAdd = false;
    }
}
