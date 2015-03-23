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
using System.Collections.Specialized;

namespace YuDraw
{
    /// <summary>
    /// IYdElementを含むコレクション
    /// </summary>
    public class YdElementCollection : UIElementCollection, INotifyCollectionChanged
    {
        public YdElementCollection(UIElement visualParent, FrameworkElement logicalParent)
            : base(visualParent, logicalParent)
        {
        }

        public override void RemoveRange(int index, int count)
        {
            if (this.Count > (index + count) && index >= 0)
            {
                if (this.CollectionChanged != null)
                {
                    List<UIElement> lst = new List<UIElement>();
                    for (int i = index; i < (index + count); i++)
                        lst.Add(this[i]);
                    this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, lst));
                }
            }
            base.RemoveRange(index, count);
        }

        public override void RemoveAt(int index)
        {
            if (this.Count > index && index >= 0)
            {
                if (this.CollectionChanged != null)
                    this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, this[index]));
            }

            base.RemoveAt(index);
        }

        public override void Remove(UIElement element)
        {
            if (base.Contains(element))
            {
                if(this.CollectionChanged != null)
                    this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, element));
            }

            base.Remove(element);
        }

        public override int Add(UIElement element)
        {
            if (element is IYdElement)
            {
                if(this.CollectionChanged != null)
                    this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element));

                return base.Add(element);
            }

            throw new ArgumentException("element is not implement IYdElement");
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    /// <summary>
    /// IYdElementのみを含む事が出来るCanvas
    /// </summary>
    public class YdCanvas : Canvas
    {
        /// <summary>
        /// 現在選択されているIYdElementを囲むSelectorのList
        /// </summary>
        protected List<YdSelector> Selector;

        public YdCanvas()
        {
            this.Selector = new List<YdSelector>();
            this.MouseLeftButtonUp += YdMouseLeftButtonUp;
            this.Background = new SolidColorBrush(Colors.SlateGray);
        }

        /// <summary>
        /// マウスカーソル位置にIYdElementが存在しなければ何も選択されていない状態にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void YdMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            bool flg = false;
            HitTestResultCallback cback = (x) =>
            {
                DrawingVisual tmp = x.VisualHit as DrawingVisual;
                if (tmp != null)
                {
                    flg = true;
                    return HitTestResultBehavior.Stop;
                }
                return HitTestResultBehavior.Continue;
            };
            VisualTreeHelper.HitTest(this, null, cback, new GeometryHitTestParameters(new EllipseGeometry(e.GetPosition(this), 10, 10)));

            if (flg == false)
                this.ClearSelector();
        }

        /// <summary>
        /// IYdElementのみを含むようにする為の拡張（親を上書き）
        /// </summary>
        /// <param name="logicalParent"></param>
        /// <returns></returns>
        protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            YdElementCollection col = new YdElementCollection(this, logicalParent);
            col.CollectionChanged += CollectionChanged;

            return col;
        }

        /// <summary>
        /// IYdElementが追加、削除された場合の処理。
        /// YdHostがクリックされた事を意味するイベントのイベントハンドラを設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                YdHost host = e.NewItems[0] as YdHost;
                if (host != null)
                {
                    host.Selected += GeometorySelected;
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var x in e.OldItems)
                {
                    YdHost host = x as YdHost;
                    if (host != null)
                        host.Selected -= GeometorySelected;
                }
            }
        }

        /// <summary>
        /// イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void GeometorySelected(object sender, SelectedEventArgs e)
        {
            if (e.IsAdd)
            {
            }
            else
            {
                this.ClearSelector();

                //IYdElementを囲むSelectorを表示する
                YdSelector sel = new YdSelector();
                this.Selector.Add(sel);
                this.Children.Add(sel);
            }
        }

        /// <summary>
        /// IYdElementを囲むSelectorをクリアする
        /// </summary>
        protected void ClearSelector()
        {
            //SelectorもCanvasの子なのでCanvasから削除する
            foreach (YdSelector s in this.Selector)
                this.Children.Remove(s);

            this.Selector = new List<YdSelector>();
        }
    }
}
