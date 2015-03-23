using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLisp
{
    public class YuLisp
    {
        internal static YuLisp lisp;
        Env env;
        TextWriter pw;
        internal Reader read;
        internal Eval eval;
        const String startup = "start.lsp";

        public YuLisp()
        {
            this.init();
        }

        void init()
        {
            this.env = new Env();
            new InitYuLisp(this.env).init();
            this.pw = Console.Out;
            this.read = new Reader(env);
            this.eval = new Eval(env);

            Function funs = new Function(this.env, this.eval);
            funs.RegistSystemFunctions();
        }

        public static void Main(String[] args)
        {
            Sexp sexp;
            Console.Out.WriteLine("YuLisp");
            Console.Out.WriteLine("    if quit from system, then you type \'quit\'.");

            try
            {
                lisp = new YuLisp();
                //lisp.read.load(startup);

                //List l = new List();
                //LString s = new LString();
                //bool b = Lib.Atom(l);
                //b = Lib.Atom(s);
                //b = Lib.Atom(Nil.NIL);

                //read-eval-print-loop
                while (true)
                {
                    try
                    {
                        lisp.pw.Write("YuLisp>");
                        lisp.pw.Flush();
                        sexp = lisp.read.read();
                        if (sexp.serialize().Equals("QUIT"))
                            break;
                        sexp = lisp.eval.eval(sexp);
                        sexp.print(lisp.pw);
                        lisp.pw.WriteLine();
                        lisp.pw.Flush();
                    }
                    catch (Error e)
                    {
                        e.print(lisp.pw);
                        lisp.pw.WriteLine();
                        lisp.pw.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("An Exception is occurred");
                Console.Error.Write(e.StackTrace);
                Console.In.ReadLine();
            }
        }
    }
}
