using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuVm
{
    public abstract class Parser
    {
        protected Lexer _Lex;
        public Lexer Lex
        {
            get{return this._Lex;}
            protected set{this._Lex = value;}
        }

        protected YuToken _CurrToken;
        public YuToken CurrToken
        {
            get { return this._CurrToken; }
            protected set { this._CurrToken = value; }
        }

        public Parser(Lexer lex)
        {
            this.Lex = lex;
            this.CurrToken = this.Lex.NextToken();
        }

        /// <summary>
        /// マッチしていればトークンを消費する
        /// </summary>
        /// <param name="type"></param>
        public void Match(ETokenType type)
        {
            if (this.CurrToken.Type == type)
                this.Consume();
            else
                throw new YuUnKnownTokenException();
        }

        /// <summary>
        /// トークンを消費する
        /// </summary>
        public void Consume()
        {
            this.CurrToken = this.Lex.NextToken();
        }
    }

    public class YuParser : Parser
    {
        public YuParser(Lexer lex)
            : base(lex)
        {
        }

        public YuTreeNode NextTree()
        {
            if (this.CurrToken.Type == ETokenType.Eof)
                return new YuTreeNode(this.CurrToken);
            return this.Statement();
        }

        public YuTreeNode Statement()
        {
            if (this.CurrToken.Type == ETokenType.Var)
                return this.VarDefStat();
            else if (this.CurrToken.Type == ETokenType.Def)
                return this.ProcDefStat();
            else if (this.CurrToken.Type == ETokenType.Let)
                return this.AssignStat();
            else if (this.CurrToken.Type == ETokenType.Call)
                return this.CallStat();
            else
                return this.ExpStat();
            /*
            else if (this.CurrToken.Type == ETokenType.Symbol)
                return this.VCalcStat();
            else
                return this.CalcStat();
            */
        }

        public YuTreeNode CallStat()
        {
            YuToken a = this.Call();
            YuToken b = this.Symbol();
            this.LParen();
            this.RParen();
            this.SemiColon();

            YuTreeNode ret = new YuTreeNode(a);
            ret.Children.Add(new YuTreeNode(b));

            return ret;
        }

        public YuTreeNode ProcDefStat()
        {
            YuToken a = this.Def();
            YuToken b = this.Symbol();

            YuTreeNode ret = new YuTreeNode(a);
            ret.Children.Add(new YuTreeNode(b));
            this.LBrace();

            while (this.CurrToken.Type != ETokenType.RBrace && this.CurrToken.Type != ETokenType.Eof)
            {
                ret.Children.Add(this.Statement());
            }

            this.RBrace();

            return ret;
        }

        public YuTreeNode ExpStat()
        {
            YuToken a = this.Term();
            YuToken exp = null;
            YuToken b = null;
            YuTreeNode ret = null;

            while (this.CurrToken.Type != ETokenType.SemiColon && this.CurrToken.Type != ETokenType.Eof)
            {
                exp = this.ExpAdd();
                b = this.Term();

                if (ret == null)
                {
                    ret = new YuTreeNode(exp);
                    ret.Children.Add(new YuTreeNode(a));
                    ret.Children.Add(new YuTreeNode(b));
                }
                else
                {
                    YuTreeNode tmp = new YuTreeNode(exp);
                    tmp.Children.Add(ret);
                    tmp.Children.Add(new YuTreeNode(b));
                    ret = tmp;
                }
            }
            this.SemiColon();

            if (exp == null)
                ret = new YuTreeNode(a);

            return ret;
        }

        /*
        public YuTreeNode CalcStat()
        {
            YuToken a = this.Number();
            YuToken b = this.ExpAdd();
            YuToken c = this.Number();
            this.SemiColon();

            YuTreeNode ret = new YuTreeNode(b);
            ret.Children.Add(new YuTreeNode(a));
            ret.Children.Add(new YuTreeNode(c));

            return ret;
        }

        public YuTreeNode VCalcStat()
        {
            YuToken a = this.Symbol();
            YuToken b = this.ExpAdd();
            YuToken c = this.Number();
            this.SemiColon();

            YuTreeNode ret = new YuTreeNode(b);
            ret.Children.Add(new YuTreeNode(a));
            ret.Children.Add(new YuTreeNode(c));

            return ret;
        }
        */

        public YuTreeNode VarDefStat()
        {
            YuToken a = this.Var();
            YuToken b = this.Symbol();
            this.SemiColon();

            YuTreeNode ret = new YuTreeNode(a);
            ret.Children.Add(new YuTreeNode(b));

            return ret;
        }

        public YuTreeNode AssignStat()
        {
            this.Let();
            YuToken a = this.Symbol();
            YuToken b = this.Equal();
            YuTreeNode n = this.ExpStat();

            /*
            if (this.CurrToken.Type == ETokenType.Number)
                n = this.CalcStat();
            else
                n = this.VCalcStat();
            */


            YuTreeNode ret = new YuTreeNode(b);
            ret.Children.Add(new YuTreeNode(a));
            ret.Children.Add(n);

            return ret;
        }

        public YuToken Number()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.Number);
            return ret;
        }

        public YuToken ExpAdd()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.ExpAdd);
            return ret;
        }

        public YuToken SemiColon()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.SemiColon);
            return ret;
        }

        public YuToken Equal()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.Equal);
            return ret;
        }

        public YuToken Var()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.Var);
            return ret;
        }

        public YuToken Symbol()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.Symbol);
            return ret;
        }

        public YuToken Let()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.Let);
            return ret;
        }

        public YuToken Def()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.Def);
            return ret;
        }

        public YuToken Call()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.Call);
            return ret;
        }

        public YuToken LBrace()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.LBrace);
            return ret;
        }

        public YuToken RBrace()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.RBrace);
            return ret;
        }

        public YuToken LParen()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.LParen);
            return ret;
        }

        public YuToken RParen()
        {
            YuToken ret = this.CurrToken;
            this.Match(ETokenType.RParen);
            return ret;
        }

        public YuToken Term()
        {
            YuToken ret = this.CurrToken;
            if (ret.Type == ETokenType.Number)
                this.Match(ETokenType.Number);
            else
                this.Match(ETokenType.Symbol);
            return ret;
        }

    }
}
