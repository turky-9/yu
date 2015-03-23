using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLisp
{
    public abstract class Number : Sexp
    {
        abstract public void print(System.IO.TextWriter tw);

        abstract public string serialize();
    }
}
