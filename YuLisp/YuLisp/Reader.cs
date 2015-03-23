using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLisp
{
    public class Reader
    {
        // 文字処理バッファのサイズ
        private const int CharBuffSize = 256;
        // 文字処理バッファ
        private char[] charBuff = null;
        // 1文字バッファ
        private String line;
        // 1行の文字数
        private int lineLength = 0;

        // 1文字バッファ
        private char ch;
        // 1行内の位置
        private int indexOfLine = 0;

        // 環境
        private Env env = null;

        private TextReader br = null;

        internal Reader(Env environment)
        {
            this.env = environment;
            this.br = Console.In;
            init();
        }

        private void init()
        {
            //charBuff = new char[CharBuffSize];
        }

        internal Sexp read()
        {
            this.getSexpPrepare();
            return this.getSexp();
        }

        internal Sexp readFromString(String instr)
        {
            this.getSexpPrepareString(instr);
            return this.getSexp();
        }

        private void getSexpPrepareString(String instr)
        {
            this.line = instr;
            this.gSP();
        }
        private void getSexpPrepare()
        {
            this.line = this.br.ReadLine();
            this.gSP();
        }
        private void gSP()
        {
            this.indexOfLine = 0;
            this.lineLength = line.Length;
            this.line += '\0';
            this.charBuff = this.line.ToCharArray(0, this.line.Length);
            //this.charBuff[lineLength] = '\0';
            this.getChar();
        }

        private void getChar()
        {
            this.ch = charBuff[indexOfLine];
            indexOfLine++;
        }

        //一文字先読み
        //ただしthis.chがスペースの場合、空白を読み飛ばしthis.chを進める。
        private char nextChar()
        {
            char nch = charBuff[indexOfLine];
            while (char.IsWhiteSpace(this.ch))
            {
                if (nch == ')')
                {
                    getChar();
                    break;
                };
                if (indexOfLine >= lineLength)
                    break;
                this.ch = nch;
                indexOfLine++;
                nch = charBuff[indexOfLine];
            }
            return nch;
        }

        private Sexp getSexp()
        {
            for (; indexOfLine <= lineLength; this.getChar())
            {
                switch (this.ch)
                {
                    case '(':
                        return makeList();
                    case '\'':
                        return makeQuote();
                    case '-':
                        return makeMinusNumber();
                    case '\"':
                        return makeString();
                    default:
                        if (char.IsWhiteSpace(this.ch))
                            break;
                        if (char.IsDigit(this.ch))
                            return makeNumber();
                        return makeSymbol();
                }
            }
            return Nil.NIL;
        }

        private Sexp makeNumber()
        {
            StringBuilder str = new StringBuilder();
            for (; indexOfLine <= lineLength; this.getChar())
            {
                if (this.ch == '(' || this.ch == ')')
                    break;
                if (char.IsWhiteSpace(this.ch))
                    break;
                if (!char.IsDigit(this.ch))
                {
                    this.indexOfLine--;
                    return makeSymbolInternal(str);
                }
                str.Append(this.ch);
            }
            int value = int.Parse(str.ToString());
            return (Sexp)new LInteger(value);
        }

        private Sexp makeMinusNumber()
        {
            char nch = charBuff[indexOfLine];
            if (char.IsDigit(nch) == false)
                return makeSymbolInternal(new StringBuilder().Append(this.ch));
            return makeNumber();
        }

        private Sexp makeSymbol()
        {
            this.ch = char.ToUpper(this.ch);
            StringBuilder str = new StringBuilder().Append(this.ch);
            return makeSymbolInternal(str);
        }

        private Sexp makeSymbolInternal(StringBuilder str)
        {
            while (indexOfLine < lineLength)
            {
                this.getChar();
                if (this.ch == '(' || this.ch == ')')
                    break;
                if (char.IsWhiteSpace(this.ch))
                    break;
                this.ch = char.ToUpper(this.ch);
                str.Append(this.ch);
            }

            String symStr = "" + str;

            if (symStr.Equals("T"))
                return T.True;
            if (symStr.Equals("NIL"))
                return Nil.NIL;

            Sexp sym = this.env.Get(symStr);
            if (sym == Nil.NIL)
                return this.env.Put(new Symbol(symStr));
            return sym;
        }

        private Sexp makeList()
        {
            this.getChar();
            this.nextChar();
            if (this.ch == ')')
            {
                getChar();
                return Nil.NIL;
            }

            List top = new List();
            List list = top;
            while (true)
            {
                list.SetCar(this.getSexp());
                this.nextChar();
                if (this.ch == ')')
                    break;
                if (indexOfLine == lineLength)
                    return Nil.NIL;
                if (this.ch == '.')
                {
                    this.getChar();
                    list.SetCdr(this.getSexp());
                    this.getChar();
                    return top;
                }
                list.SetCdr((Sexp)new List());
                list = (List)list.cdr;
            }

            this.getChar();
            return top;
        }

        private Sexp makeQuote()
        {
            List top = new List();
            List list = top;
            list.SetCar((Symbol)env.Get("QUOTE"));
            list.SetCdr((Sexp)new List());
            list = (List)list.cdr;
            this.getChar();
            list.SetCar(this.getSexp());
            return top;
        }

        private Sexp makeString()
        {
            StringBuilder str = new StringBuilder();
            while (this.indexOfLine < this.lineLength)
            {
                this.getChar();
                if (this.ch == '\"')
                    break;
                str.Append(this.ch);
            }
            LString lstr = new LString("" + str);
            return lstr;
        }

        internal Sexp load(String fileName)
        {
            TextReader oldBr = this.br;
            try
            {
                FileStream iin = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                this.br = new StreamReader(iin);
                init();
                for (; ; )
                {
                    String str = br.ReadLine();
                    if (str == null)
                        break;
                    Sexp sexp = readFromString(str);
                    Sexp ret = YuLisp.lisp.eval.eval(sexp);
                }
                this.br.Close();
                return T.True;
            }
            catch (Exception)
            {
                this.br = oldBr;
                return Nil.NIL;
            }
        }
    }
}
