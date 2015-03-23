using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLisp
{
    public class LString : Atom
    {
        private string Value { get; set; }

        public LString()
        {
            this.Value = string.Empty;
        }

        public LString(string s)
        {
            this.Value = s;
        }

        public string ValueOf()
        {
            return this.Value;
        }


        public override void print(TextWriter tw)
        {
            tw.Write(this.serialize());
        }

        public override string serialize()
        {
            return "\"" + this.Value + "\"";
        }
    }
}
