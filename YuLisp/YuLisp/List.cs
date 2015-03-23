using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLisp
{
    public class List : Cell, Sexp
    {
        internal List()
        {
        }

        internal List(Sexp kar, Sexp kdr)
        {
            this.car = kar;
            this.cdr = kdr;
        }

        internal Sexp SetCar(Sexp s)
        {
            return this.car = s;
        }

        internal Sexp SetCdr(Sexp s)
        {
            return this.cdr = s;
        }

        internal int size()
        {
            List list = this;
            for (int i = 1; ; i++)
            {
                if (Lib.Atom(list.cdr))
                    return i;
                list = (List)list.cdr;
            }
        }

        public virtual void print(TextWriter tw)
        {
            List list = this;
            tw.Write("(");
            while (true)
            {
                list.car.print(tw);
                if (list.cdr is Nil)
                {
                    tw.Write(")");
                    break;
                }
                else if (Lib.Atom(list.cdr))
                {
                    tw.Write(" . ");
                    list.cdr.print(tw);
                    tw.Write(")");
                    break;
                }
                else
                {
                    tw.Write(" ");
                    list = (List)list.cdr;
                }
            }
        }

        public virtual string serialize()
        {
            StringBuilder str = new StringBuilder();
            List list = this;
            str.Append("(");

            while (true)
            {
                str.Append(list.car.serialize());
                if (list.cdr is Nil)
                {
                    str.Append(")");
                    break;
                }
                else if (Lib.Atom(list.cdr))
                {
                    str.Append(" . ");
                    str.Append(list.cdr.serialize());
                    str.Append(")");
                    break;
                }
                else
                {
                    str.Append(" ");
                    list = (List)list.cdr;
                }
            }

            return str.ToString();
        }
    }
}
