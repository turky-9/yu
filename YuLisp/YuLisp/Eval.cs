using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLisp
{
    public class Eval
    {
        protected Env env;
        protected const int MaxStackSize = 65535;
        protected Sexp[] Stack = new Sexp[Eval.MaxStackSize];
        protected int StackP = 0;

        public Eval(Env e)
        {
            this.env = e;
        }

        public Sexp eval(Sexp form)
        {
            //シンボルの処理
            if (Lib.Symbol(form))
            {
                Sexp sexp = ((Symbol)form).GetValue();
                if (sexp == null)
                    throw new Error(Lib.UNBOUND, form.serialize());
                return sexp;
            }

            //アトムの処理
            if (Lib.Atom(form))
                return form;

            //リストの処理
            Sexp car = ((List)form).car;
            int argnum = ((List)form).size() - 1;
            if (Lib.Symbol(car))
            {
                Sexp fun = ((Symbol)car).GetValue();
                if (fun == null)
                    throw new Error(Lib.UNDEFINED, car.serialize());

                //carがFunctionの時
                if (Lib.Function(fun))
                {
                    Sexp arglist = (argnum == 0) ? Nil.NIL : ((List)form).cdr;
                    return ((Function)fun).fun((List)arglist, argnum);
                }

                //carがListの時
                if (Lib.List(fun))
                {
                    List cdr = (List)((List)fun).cdr;
                    List lambdalist = (List)cdr.car;
                    List body = (List)cdr.cdr;
                    if (lambdalist == Nil.NIL)
                        return this.EvalBody(body);

                    return this.SexpEval(lambdalist, body, (List)((List)form).cdr); 
                }
                throw new Error(Lib.NOTFUNCTION, car.serialize());
            }

            throw new Error(Lib.NOTSYMBOL, car.serialize());
        }

        //S式定義関数の評価
        protected Sexp SexpEval(List lambda, List body, List form)
        {
            //引数の評価
            List arglist = lambda;
            int oldstackp = this.StackP;
            while (true)
            {
                Sexp arg = form.car;
                Sexp xxx = this.eval(arg);
                this.Stack[this.StackP] = xxx;
                this.StackP++;
                if (form.cdr == Nil.NIL)
                    break;
                form = (List)form.cdr;
                arglist = (List)arglist.cdr;
            }

            //スタックへの退避
            arglist = lambda;
            int sp = oldstackp;
            while (true)
            {
                Symbol sym = (Symbol)arglist.car;
                Sexp swap = sym.GetValue();
                sym.SetValue(this.Stack[sp]);
                this.Stack[sp] = swap;
                sp++;
                if (arglist.cdr == Nil.NIL)
                    break;
                arglist = (List)arglist.cdr;
            }

            //bodyの評価
            Sexp ret = this.EvalBody(body);

            //スタックからの復帰
            arglist = lambda;
            this.StackP = oldstackp;
            while (true)
            {
                Symbol sym = (Symbol)arglist.car;
                sym.SetValue(this.Stack[oldstackp]);
                oldstackp++;
                if (arglist.cdr == Nil.NIL)
                    break;
                arglist = (List)arglist.cdr;
            }
            return ret;
        }

        //本体の評価
        protected Sexp EvalBody(List body)
        {
            Sexp ret;
            while (true)
            {
                ret = this.eval(body.car);
                if (body.cdr == Nil.NIL)
                    break;
                body = (List)body.cdr;
            }
            return ret;
        }
    }
}
