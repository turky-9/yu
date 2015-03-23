using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLisp
{
    public class Cell
    {
        internal Sexp car;
        internal Sexp cdr;

        public Cell()
        {
            this.car = Nil.NIL;
            this.cdr = Nil.NIL;
        }
    }
}
