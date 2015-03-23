using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLisp
{
    public class Env
    {
        protected Dictionary<string, Symbol> tab;
        internal Env()
        {
            this.tab = new Dictionary<string, Symbol>();
        }

        internal Symbol Put(Symbol val)
        {
            this.tab.Add(val.serialize(), val);
            return val;
        }

        internal Symbol Put(string nm, Symbol val)
        {
            if (this.tab.ContainsKey(nm))
                this.tab.Remove(nm);

            this.tab.Add(nm, val);
            return val;
        }

        internal Sexp Get(Symbol sym)
        {
            return this.Get(sym.serialize());
        }

        internal Sexp Get(string nm)
        {
            if (this.tab.ContainsKey(nm))
            {
                return this.tab[nm];
            }
            return Nil.NIL;
        }

        internal Sexp Remove(Symbol sym)
        {
            if (this.tab.ContainsKey(sym.serialize()))
            {
                this.tab.Remove(sym.serialize());
                return sym;
            }
            return Nil.NIL;
        }
    }
}
