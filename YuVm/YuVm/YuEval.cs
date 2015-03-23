using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuVm
{
    public class YuEval
    {
        private List<VMOp> OpLst;
        private List<Tuple<VMOp, YuToken>> NeedResolvLst;

        private VM _Vm;
        public VM Vm
        {
            get { return this._Vm; }
            private set { this._Vm = value; }
        }

        private YuSymbolTable _SymbolTable;
        public YuSymbolTable SymbolTable
        {
            get { return this._SymbolTable; }
            private set { this._SymbolTable = value; }
        }


        public YuEval()
        {
            this.OpLst = new List<VMOp>();
            this.NeedResolvLst = new List<Tuple<VMOp, YuToken>>();
            this.SymbolTable = new YuSymbolTable();
        }

        public void TranslateTree(YuTreeNode node)
        {
            if (node.Token.Type == ETokenType.ExpAdd)
            {
                this.RecTree(node);
                
            }else if (node.Token.Type == ETokenType.Var)
            {
                YuSymbolTableRow sr = new YuSymbolTableRow(ESymbolType.Var, node.Children[0].Token);
                this.SymbolTable.Add(sr);
            }
            else if (node.Token.Type == ETokenType.Equal)
            {
                /*
                if (node.Children[1].Children[0].Token.Type == ETokenType.Number)
                {
                    this.OpLst.Add(new VMOp(EOpCode.Push, int.Parse(node.Children[1].Children[0].Token.Text)));
                    this.OpLst.Add(new VMOp(EOpCode.Push, int.Parse(node.Children[1].Children[1].Token.Text)));
                    this.OpLst.Add(new VMOp(EOpCode.Add, 0));
                }
                */
                this.RecTree(node.Children[1]);

                YuSymbolTableRow row = new YuSymbolTableRow(ESymbolType.Var, node.Children[0].Token);
                if (this.SymbolTable.IsContain(row) == false)
                    throw new YuSymbolUndefinedException();

                VMOp op = new VMOp(EOpCode.Pop, -1);

                this.OpLst.Add(op);
                this.NeedResolvLst.Add(new Tuple<VMOp, YuToken>(op, node.Children[0].Token));
            }
            else if (node.Token.Type == ETokenType.Def)
            {
                YuSymbolTableRow sr = new YuSymbolTableRow(ESymbolType.Proc, node.Children[0].Token, node);
                this.SymbolTable.Add(sr);
            }
            else if (node.Token.Type == ETokenType.Call)
            {
                VMOp op = new VMOp(EOpCode.Call, -1);
                this.OpLst.Add(op);
                this.NeedResolvLst.Add(new Tuple<VMOp, YuToken>(op, node.Children[0].Token));
            }
            else if (node.Token.Type == ETokenType.Eof)
            {
                this.OpLst.Add(new VMOp(EOpCode.End, 0));
            }
        }

        private void RecTree(YuTreeNode node)
        {
            if (node.Children[0].Token.Type == ETokenType.ExpAdd)
                this.RecTree(node.Children[0]);
            else
            {
                if (node.Children[0].Token.Type == ETokenType.Symbol)
                {
                    YuSymbolTableRow row = new YuSymbolTableRow(ESymbolType.Var, node.Children[0].Token);
                    if (this.SymbolTable.IsContain(row) == false)
                        throw new YuSymbolUndefinedException();

                    VMOp op = new VMOp(EOpCode.PushAddr, -1);
                    this.OpLst.Add(op);
                    this.NeedResolvLst.Add(new Tuple<VMOp, YuToken>(op, node.Children[0].Token));
                }
                else
                    this.OpLst.Add(new VMOp(EOpCode.Push, int.Parse(node.Children[0].Token.Text)));
            }

            if (node.Children[1].Token.Type == ETokenType.ExpAdd)
                this.RecTree(node.Children[1]);
            else
            {
                if (node.Children[1].Token.Type == ETokenType.Symbol)
                {
                    YuSymbolTableRow row = new YuSymbolTableRow(ESymbolType.Var, node.Children[1].Token);
                    if (this.SymbolTable.IsContain(row) == false)
                        throw new YuSymbolUndefinedException();

                    VMOp op = new VMOp(EOpCode.PushAddr, -1);
                    this.OpLst.Add(op);
                    this.NeedResolvLst.Add(new Tuple<VMOp, YuToken>(op, node.Children[1].Token));
                }
                else
                    this.OpLst.Add(new VMOp(EOpCode.Push, int.Parse(node.Children[1].Token.Text)));
            }

            this.OpLst.Add(new VMOp(EOpCode.Add, 0));
        }

        public void Eval()
        {
            this.ResolveProc();
            this.ResolveVar();
            this.Vm = new VM(this.OpLst);
            this.Vm.Run();
        }

        private void ResolveVar()
        {
            foreach (var row in this.SymbolTable.Rows)
            {
                //変数のアドレスを決定
                if (row.SymbolType == ESymbolType.Var)
                {
                    this.OpLst.Add(new VMOp(EOpCode.Data, 0));
                    row.AbsoluteAddr = this.OpLst.Count - 1;

                    //変数のアドレスをオペランドに設定
                    foreach (Tuple<VMOp, YuToken> op in this.NeedResolvLst)
                    {
                        if (op.Item1.OpCd != EOpCode.Call)
                        {
                            if (row.SymbolToken.Text.Equals(op.Item2.Text))
                                op.Item1.OpRnd = row.AbsoluteAddr;
                        }
                    }
                }
            }
        }

        private void ResolveProc()
        {
            foreach (var row in this.SymbolTable.Rows)
            {
                //手続きのアドレスを決定
                if (row.SymbolType == ESymbolType.Proc)
                {
                    row.AbsoluteAddr = this.OpLst.Count;
                    for (int i = 1; i < row.ProcTree.Children.Count; i++)
                    {
                        this.TranslateTree(row.ProcTree.Children[i]);
                    }
                    this.OpLst.Add(new VMOp(EOpCode.Return, -1));

                    //手続きのアドレスをオペランドに設定
                    foreach (Tuple<VMOp, YuToken> op in this.NeedResolvLst)
                    {
                        if (op.Item1.OpCd == EOpCode.Call)
                        {
                            if (row.SymbolToken.Text.Equals(op.Item2.Text))
                                op.Item1.OpRnd = row.AbsoluteAddr;
                        }
                    }
                }
            }
        }
    }

    public class YuSymbolUndefinedException : Exception
    {
    }
}
