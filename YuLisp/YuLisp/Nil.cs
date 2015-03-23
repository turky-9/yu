using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLisp
{
    public class Nil : List
    {
        internal static Nil NIL = new Nil();
        protected Nil() { }

        public override void print(TextWriter tw)
        {
            tw.Write("NIL");
        }

        public override string serialize()
        {
            return "NIL";
        }
    }
}
