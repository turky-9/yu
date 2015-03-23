using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLisp
{
    public static class Lib
    {
        public static bool Atom(Sexp s)
        {
            return !(s is List);
        }

        public static bool Symbol(Sexp s)
        {
            return s is Symbol;
        }

        public static bool Function(Sexp s)
        {
            return s is Function;
        }

        public static bool List(Sexp s)
        {
            return s is List;
        }

        public const int UNBOUND = 1;
        public const int UNDEFINED = 2;
        public const int NOTFUNCTION = 3;
        public const int NOTSYMBOL = 4;
    }
}
