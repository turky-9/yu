using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media;
using WIN32APIWapper;

namespace YuFiler
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return IconDictionary.Get(value.ToString());

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class IconDictionary
    {
        protected static Dictionary<string, BitmapFrame> IconDic = new Dictionary<string, BitmapFrame>();
        protected static BitmapFrame FolderIcon = null;

        protected static BitmapFrame GetFolder(string nm)
        {
            if (FolderIcon != null)
                return FolderIcon;

            Icon icon = IconFunction.GetFileAssociatedIcon(nm, EIconSize.Small, true);
            FolderIcon = Icon2BitmapFrame(icon);
            return FolderIcon;
        }

        public static BitmapFrame Get(string nm)
        {
            if (Directory.Exists(nm))
                return GetFolder(nm);

            string ext = System.IO.Path.GetExtension(nm);
            if (IconDic.ContainsKey(ext))
                return IconDic[ext];

            Icon icon = IconFunction.GetFileAssociatedIcon(nm, EIconSize.Small, true);
            if (icon == null)
            {
                IconDic.Add(ext, null);
                return null;
            }

            IconDic.Add(ext, Icon2BitmapFrame(icon));
            return IconDic[ext];
        }

        protected static BitmapFrame Icon2BitmapFrame(Icon icon)
        {
            using (var stream = new MemoryStream())
            {
                Bitmap bmp = icon.ToBitmap();
                System.Drawing.Color c = System.Drawing.Color.FromName("Black");
                bmp.MakeTransparent(c);
                bmp.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }
    }
}
