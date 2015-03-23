using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuFiler
{
    public class DatSaveInfo
    {
        public List<string> Path { get; set; }
        public List<KeyValuePair<string, string>> BookMarks;
        public int SelectedIndex { get; set; }

        public DatSaveInfo()
        {
            this.Path = new List<string>();
            this.BookMarks = new List<KeyValuePair<string, string>>();
        }
    }
}
