using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuVm
{
    public enum ETokenType
    {
        Eof = -1,
        SemiColon = 0,

        Symbol = 1,

        Var = 10,
        Let = 11,
        Def = 12,
        Call = 13,

        ExpAdd = 50,
        Equal = 51,

        Number = 100,

        LBrace = 110,
        RBrace = 111,
        LParen = 112,
        RParen = 113,
    }

    public class YuToken
    {
        private ETokenType _Type;
        public ETokenType Type
        {
            get { return this._Type; }
            private set{this._Type = value;}
        }

        private string _Text;
        public string Text
        {
            get { return this._Text; }
            private set{this._Text = value;}
        }

        public YuToken(ETokenType type, string txt)
        {
            this.Type = type;
            this.Text = txt;
        }

        public override string ToString()
        {
            return "<'" + this.Text + "', '" + this.Type.ToString() + "'>";
        }
    }
}
