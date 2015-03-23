using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;

namespace YuDraw
{
    /// <summary>
    /// 図形の情報をまとめたクラス
    /// </summary>
    public class YdGeometryRenderInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Geometry _geo;
        private Brush _brush;
        private Pen _pen;

        /// <summary>
        /// 図形そのものを表す
        /// </summary>
        public Geometry Geometry
        {
            get { return this._geo; }
            set
            {
                if (this._geo != value)
                {
                    this._geo = value;
                    if (this.PropertyChanged != null)
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Geometry"));
                }
            }
        }

        /// <summary>
        /// 図形のバックグラウンド色
        /// </summary>
        public Brush Brush
        {
            get { return this._brush; }
            set
            {
                if (this._brush != value)
                {
                    this._brush = value;
                    if (this.PropertyChanged != null)
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Brush"));
                }
            }
        }

        /// <summary>
        /// 図形の枠線
        /// </summary>
        public Pen Pen
        {
            get { return this._pen; }
            set
            {
                if (this._pen != value)
                {
                    this._pen = value;
                    if (this.PropertyChanged != null)
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Pen"));
                }
            }
        }

        public YdGeometryRenderInfo(Geometry geo, Brush b, Pen p)
        {
            this._geo = geo;
            this._brush = b;
            this._pen = p;
        }

        public YdGeometryRenderInfo()
        {
            this._geo = null;
            this._brush = null;
            this._pen = null;
        }
    }
}
