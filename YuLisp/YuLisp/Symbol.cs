using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLisp
{
    public class Symbol : Atom
    {
        protected string name = null;
        protected Sexp value = null;

        internal Symbol() { }
        internal Symbol(string s) { this.name = s; }

        internal Sexp SetValue(Sexp val)
        {
            this.value = val;
            return this.value;
        }

        internal Sexp GetValue()
        {
            return this.value;
        }

        protected Sexp Unbound()
        {
            this.value = null;
            return this;
        }

        protected Sexp Intern(Env env)
        {
            return env.Put(this);
        }

        protected Sexp Unintern(Env env)
        {
            return env.Remove(this);
        }

        public override void print(TextWriter tw)
        {
            tw.Write(this.name);
        }

        public override string serialize()
        {
            return this.name;
        }
    }
}
