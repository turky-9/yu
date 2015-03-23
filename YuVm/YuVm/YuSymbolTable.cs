using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuVm
{
    public enum ESymbolType
    {
        Var = 0,
        Proc
    }

    public class YuSymbolTableRow
    {
        private ESymbolType _SymbolType;
        public ESymbolType SymbolType
        {
            get { return this._SymbolType; }
            private set { this._SymbolType = value; }
        }

        private YuToken _SymbolToken;
        public YuToken SymbolToken
        {
            get { return this._SymbolToken; }
            private set { this._SymbolToken = value; }
        }

        private YuTreeNode _ProcTree;
        public YuTreeNode ProcTree
        {
            get { return this._ProcTree; }
            private set { this._ProcTree = value; }
        }

        public int AbsoluteAddr;

        public YuSymbolTableRow(ESymbolType stype, YuToken token)
        {
            this.SymbolType = stype;
            this.SymbolToken = token;
            this.ProcTree = null;
            this.AbsoluteAddr = -1;
        }

        public YuSymbolTableRow(ESymbolType stype, YuToken token, YuTreeNode tree)
        {
            this.SymbolType = stype;
            this.SymbolToken = token;
            this.ProcTree = tree;
            this.AbsoluteAddr = -1;
        }
    }

    public class YuSymbolTable
    {
        private List<YuSymbolTableRow> _Rows;
        public List<YuSymbolTableRow> Rows
        {
            get { return this._Rows; }
            private set { this._Rows = value; }
        }

        public YuSymbolTable()
        {
            this.Rows = new List<YuSymbolTableRow>();
        }

        public void Add(YuSymbolTableRow row)
        {
            if (this.IsContain(row))
                throw new YuSymbolIsExistException();
            this.Rows.Add(row);
        }

        public bool IsContain(YuSymbolTableRow row)
        {
            bool ret = false;
            foreach (var r in this.Rows)
            {
                if (r.SymbolType == row.SymbolType && r.SymbolToken.Text.Equals(row.SymbolToken.Text))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
    }

    public class YuSymbolIsExistException : Exception
    {
    }
}
