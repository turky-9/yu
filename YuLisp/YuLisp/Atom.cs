
using System.IO;
namespace YuLisp
{
    public abstract class Atom : Sexp
    {
        public abstract void print(TextWriter tw);
        public abstract string serialize();
    }
}
