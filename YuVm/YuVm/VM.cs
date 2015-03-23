using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuVm
{
    public class VM
    {
        private int _Pc;
        public int Pc
        {
            get { return this._Pc; }
            private set { this._Pc = value; }
        }

        private List<VMOp> _Mem;
        public List<VMOp> Mem
        {
            get { return this._Mem; }
            private set { this._Mem = value; }
        }

        private VMStack _Stack;
        public VMStack Stack
        {
            get { return this._Stack; }
            private set { this._Stack = value; }
        }

        public VM(List<VMOp> mem)
        {
            this.Mem = mem;
            this.Pc = 0;

            this.Stack = new VMStack();
        }

        public void Run()
        {
            while (this.RunStep() == true) ;
        }

        public bool RunStep()
        {
            bool ret = true;

            if (this.Pc >= this.Mem.Count)
            {
                this.Stack.ClearStack();
                return false;
            }

            VMOp currop = this.Mem[this.Pc];
            switch (currop.OpCd)
            {
                case EOpCode.Nop :
                    break;

                case EOpCode.Push:
                    this.Stack.Push(new VMOp(EOpCode.Data, currop.OpRnd));
                    break;

                case EOpCode.PushAddr:
                    this.Stack.Push(this.Mem[currop.OpRnd]);
                    break;

                case EOpCode.Pop:
                    this.Mem[currop.OpRnd] = this.Stack.Pop();
                    break;

                case EOpCode.Peek:
                    this.Mem[currop.OpRnd] = this.Stack.Peek();
                    break;

                case EOpCode.End:
                    this.Stack.ClearStack();
                    ret = false;
                    break;

                case EOpCode.Add:
                    int a = this.Stack.Pop().OpRnd;
                    int b = this.Stack.Pop().OpRnd;
                    this.Stack.Push(new VMOp(EOpCode.Data, a + b));
                    break;

                case EOpCode.Call:
                case EOpCode.Return:
                    break;

                case EOpCode.Data:
                    throw new YuDataExecutionException();

                case EOpCode.Out:
                    throw new NotImplementedException();

                default:
                    throw new YuUnKnownOpCodeException();
            }

            if(currop.OpCd == EOpCode.Call)
            {
                this.Stack.Push(new VMOp(EOpCode.Data, this.Pc + 1));
                this.Pc = currop.OpRnd;
            }
            else if (currop.OpCd == EOpCode.Return)
            {
                this.Pc = this.Stack.Pop().OpRnd;
            }
            else
                this.Pc++;

            return ret;
        }
    }

    public class YuUnKnownOpCodeException : Exception
    {
    }
    public class YuDataExecutionException : Exception
    {
    }
}
