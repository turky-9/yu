using System;
using System.IO;

namespace YuLisp
{
    public interface Sexp
    {
        void print(TextWriter tw);
        string serialize();
    }
}
