using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLisp
{
    public class LInteger : Number
    {
        private int value;

        public LInteger() { this.value = 0; }
        public LInteger(int i) { this.value = i; }

        public int valuOf() { return this.value; }

        //四則演算
        public LInteger Add(LInteger i) { return new LInteger(this.value + i.valuOf()); }
        public LInteger Sub(LInteger i) { return new LInteger(this.value - i.valuOf()); }
        public LInteger Mul(LInteger i) { return new LInteger(this.value * i.valuOf()); }
        public LInteger Div(LInteger i) { return new LInteger(this.value / i.valuOf()); }

        //比較
        public Sexp Ge(LInteger i) { return (this.value >= i.valuOf()) ? (Sexp)T.True : Nil.NIL; }
        public Sexp Le(LInteger i) { return (this.value <= i.valuOf()) ? (Sexp)T.True : Nil.NIL; }
        public Sexp Gt(LInteger i) { return (this.value > i.valuOf()) ? (Sexp)T.True : Nil.NIL; }
        public Sexp Lt(LInteger i) { return (this.value < i.valuOf()) ? (Sexp)T.True : Nil.NIL; }
        public Sexp Eq(LInteger i) { return (this.value == i.valuOf()) ? (Sexp)T.True : Nil.NIL; }

        public override void print(TextWriter tw)
        {
            tw.Write(this.serialize());
        }

        public override string serialize()
        {
            return this.value.ToString();
        }
    }
}
