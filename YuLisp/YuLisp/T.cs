using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLisp
{
    public class T : Atom
    {
        internal static T True = new T();

        protected T() { }


        public override void print(TextWriter tw)
        {
            tw.Write("T");
        }

        public override string serialize()
        {
            return "T";
        }
    }
}
