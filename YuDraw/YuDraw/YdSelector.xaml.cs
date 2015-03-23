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
    /// IYdElementを囲むもの（四隅に四角形がある透明なコントロール）
    /// </summary>
    public partial class YdSelector : UserControl, IYdElement
    {
        public YdSelector()
        {
            InitializeComponent();
        }
    }
}
