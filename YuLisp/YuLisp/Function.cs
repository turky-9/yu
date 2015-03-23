using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLisp
{
    public class Function : Sexp
    {
        protected Env env;
        protected Eval eval;

        public Function()
        {
            this.env = new Env();
            this.eval = new Eval(this.env);
            this.env.Put(new Symbol("LAMBDA"));
        }

        public Function(Env e)
        {
            this.env = e;
            this.eval = new Eval(this.env);
            this.env.Put(new Symbol("LAMBDA"));
        }

        public Function(Env e, Eval ev)
        {
            this.env = e;
            this.eval = ev;
            this.env.Put(new Symbol("LAMBDA"));
        }


        public virtual void print(TextWriter tw)
        {
            tw.Write(this.serialize());
        }

        public virtual string serialize()
        {
            return "<SystemFunction : " + this.GetType().Name + ">";
        }

        //テンプレートメソッド
        public virtual Sexp fun(List arg, int argnum)
        {
            return Nil.NIL;
        }

        public void RegistSystemFunctions()
        {
            this.Regist("CAR", new Car());
            this.Regist("QUOTE", new Quote());
            this.Regist("+", new Add());
            this.Regist("-", new Sub());
            this.Regist("SETQ", new Setq());
            this.Regist("DEFUN", new Defun());
            this.Regist(">=", new Ge());
            this.Regist("IF", new If());
            this.Regist("CONS", new Cons());
            this.Regist("READ", new Read());
            this.Regist("READ-FROM-STRING", new ReadFromString());
            this.Regist("EVAL", new EvalFunction());
            this.Regist("LOAD", new Load());
        }

        protected void Regist(string name, Function fun)
        {
            Symbol sym = new Symbol(name);
            env.Put(name, sym);
            sym.SetValue(fun);
        }
    }

    public class Car : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            Sexp arg1 = this.eval.eval(arg.car);
            return ((List)arg1).car;
        }
    }

    public class Quote : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            return arg.car;
        }
    }

    public class Add : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            Sexp arg1 = this.eval.eval(arg.car);
            Sexp arg2 = this.eval.eval(((List)arg.cdr).car);
            return ((LInteger)arg1).Add((LInteger)arg2);
        }
    }

    public class Sub : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            Sexp arg1 = this.eval.eval(arg.car);
            Sexp arg2 = this.eval.eval(((List)arg.cdr).car);
            return ((LInteger)arg1).Sub((LInteger)arg2);
        }
    }

    /// <summary>
    /// >=
    /// </summary>
    public class Ge : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            Sexp arg1 = this.eval.eval(arg.car);
            Sexp arg2 = this.eval.eval(((List)arg.cdr).car);
            return ((LInteger)arg1).Ge((LInteger)arg2);
        }
    }

    public class Setq : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            Sexp arg1 = arg.car;
            Sexp arg2 = ((List)arg.cdr).car;
            Symbol sym = (Symbol)arg1;
            Sexp value = eval.eval(arg2);
            sym.SetValue(value);
            return value;
        }
    }

    public class Defun : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            Sexp arg1 = arg.car;
            Sexp args = arg.cdr;

            Symbol fun = (Symbol)arg1;
            List lambda = new List();
            lambda.car = this.env.Get("LAMBDA");
            lambda.cdr = args;
            fun.SetValue(lambda);
            return fun;
        }
    }

    public class If : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            Sexp arg1 = arg.car;
            Sexp args = arg.cdr;
            Sexp arg2 = ((List)args).car;
            Sexp arg3 = ((List)((List)args).cdr).car;

            if (eval.eval(arg1) != Nil.NIL)
                return this.eval.eval(arg2);
            else
                return this.eval.eval(arg3);
        }
    }

    public class Cons : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            Sexp arg1 = this.eval.eval(arg.car);
            Sexp arg2 = this.eval.eval(((List)arg.cdr).car);
            return (Sexp)new List(arg1, arg2);
        }
    }

    public class Read : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            return (Sexp)YuLisp.lisp.read.read();
        }
    }

    public class ReadFromString : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            Sexp arg1 = this.eval.eval(arg.car);
            return (Sexp)YuLisp.lisp.read.readFromString(
                ((LString)arg1).ValueOf());
        }
    }

    public class EvalFunction : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            Sexp arg1 = eval.eval(arg.car);
            return (Sexp)eval.eval(arg1);
        }
    }

    public class Load : Function
    {
        public override Sexp fun(List arg, int argnum)
        {
            Sexp arg1 = eval.eval(arg.car);
            String fileName = ((LString)arg1).ValueOf();
            return YuLisp.lisp.read.load(fileName);
        }
    }
}
