using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuVm
{
    public abstract class Lexer
    {
        protected string _Src;
        public string Src
        {
            get { return this._Src; }
            protected set { this._Src = value; }
        }

        protected int _Idx;
        public int Idx
        {
            get { return this._Idx; }
            protected set { this._Idx = value; }
        }

        public char CurrChar
        {
            get { return this.Idx == this.Src.Length ? '\0' : this.Src[this.Idx]; }
        }

        public Lexer(string src)
        {
            this.Src = src;
            this.Idx = 0;
        }


        /// <summary>
        /// アルファベット？
        /// </summary>
        /// <returns></returns>
        public bool IsLetter()
        {
            char c = this.CurrChar;
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        public bool IsNumeric()
        {
            char c = this.CurrChar;
            return (c >= '0' && c <= '9');
        }

        public bool IsSemiColon()
        {
            char c = this.CurrChar;
            return c == ';';
        }

        /// <summary>
        /// １文字消費する
        /// </summary>
        public void Consume()
        {
            this.Idx++;
        }

        /// <summary>
        /// ホワイトスペースなら１文字消費する
        /// </summary>
        public void WhiteSpace()
        {
            char c = CurrChar;
            if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                this.Consume();
        }

        public abstract YuToken NextToken();
    }

    public class YuLexer : Lexer
    {


        public YuLexer(string s)
            : base(s)
        {
        }

        public override YuToken NextToken()
        {
            while (this.Idx < this.Src.Length)
            {
                switch (this.Src[this.Idx])
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        WhiteSpace();
                        continue;

                    case '+':
                        this.Consume();
                        return new YuToken(ETokenType.ExpAdd, "+");

                    case ';':
                        this.Consume();
                        return new YuToken(ETokenType.SemiColon, ";");

                    case '=':
                        this.Consume();
                        return new YuToken(ETokenType.Equal, "=");

                    case '{':
                        this.Consume();
                        return new YuToken(ETokenType.LBrace, "{");

                    case '}':
                        this.Consume();
                        return new YuToken(ETokenType.RBrace, "}");

                    case '(':
                        this.Consume();
                        return new YuToken(ETokenType.LParen, "(");

                    case ')':
                        this.Consume();
                        return new YuToken(ETokenType.RParen, ")");

                    default:
                        if (this.IsNumeric())
                            return this.Number();
                        if (this.IsLetter())
                        {
                            YuToken token = this.Symbol();

                            if (token.Text.ToUpper().Equals("VAR"))
                                token = new YuToken(ETokenType.Var, token.Text);
                            else if (token.Text.ToUpper().Equals("LET"))
                                token = new YuToken(ETokenType.Let, token.Text);
                            else if (token.Text.ToUpper().Equals("DEF"))
                                token = new YuToken(ETokenType.Def, token.Text);
                            else if (token.Text.ToUpper().Equals("CALL"))
                                token = new YuToken(ETokenType.Call, token.Text);

                            return token;
                        }

                        throw new YuUnKnownTokenException();
                }
            }

            return new YuToken(ETokenType.Eof, "<EOF>");
        }

        public YuToken Number()
        {
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(this.CurrChar);
                this.Consume();
            } while (this.IsNumeric());

            return new YuToken(ETokenType.Number, sb.ToString());
        }

        public YuToken Symbol()
        {
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(this.CurrChar);
                this.Consume();
            } while (this.IsLetter());

            return new YuToken(ETokenType.Symbol, sb.ToString());
        }
    }

    public class YuUnKnownTokenException : Exception
    {
    }
}
