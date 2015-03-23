using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuVm
{
    public enum EOpCode
    {
        Nop = 0,
        Push,
        PushAddr,
        Pop,
        Peek,
        Add,
        Out,
//        Jmp,
        Call,
        Return,

        Data = -1,
        End = -100
    }

    public class VMOp
    {
        private EOpCode _OpCd;
        public EOpCode OpCd
        {
            get { return this._OpCd; }
        }

        private int _OpRnd;
        public int OpRnd
        {
            get { return this._OpRnd; }
            set { this._OpRnd = value; }
        }

        public VMOp(EOpCode cd, int rnd)
        {
            this._OpCd = cd;
            this._OpRnd = rnd;
        }

        public override string ToString()
        {
            return this.OpCd.ToString() + " : " + this.OpRnd.ToString();
        }
    }

    public class VMStack
    {
        private List<VMOp> _Stack;
        public List<VMOp> Stack
        {
            get { return this._Stack; }
            private set { this._Stack = value; }
        }

        private int _StackP;
        public int StackP
        {
            get { return this._StackP; }
            private set { this._StackP = value; }
        }

        public VMStack()
        {
            this.Stack = new List<VMOp>();
            this.StackP = -1;
        }

        public void Push(VMOp x)
        {
            if(this.Stack.Count == 0)
            {
                this.Stack.Add(x);
                this.StackP = 0;
            }
            else if ((this.StackP + 1) == this.Stack.Count)
            {
                this.Stack.Add(x);
                this.StackP++;
            }
            else
            {
                this.StackP++;
                this.Stack[this.StackP] = x;
            }
        }

        public VMOp Peek()
        {
            if (this.StackP == -1)
                throw new YuStackUnderFlowException();

            return this.Stack[this.StackP];
        }

        public VMOp Pop()
        {
            VMOp ret = this.Peek();
            this.Stack[this.StackP] = null;
            this.StackP--;
            return ret;
        }

        internal void ClearStack()
        {
            List<VMOp> tmp = this.Stack;
            this.Stack = new List<VMOp>();
            for (int i = 0; i <= this.StackP; i++)
                this.Stack.Add(tmp[i]);
        }
    }

    public class YuStackUnderFlowException : Exception
    {
    }
}
