using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLisp
{
    public class InitYuLisp
    {
        Env env;

        internal InitYuLisp()
        {
            this.env = new Env();
        }

        internal InitYuLisp(Env e)
        {
            this.env = e;
        }

        public Env init()
        {
            this.env.Put(new Symbol("QUOTE"));
            return this.env;
        }
    }
}
