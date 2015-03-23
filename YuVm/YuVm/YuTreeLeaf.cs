using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuVm
{
    public class YuTreeLeaf
    {
        private YuToken _Token;
        public YuToken Token
        {
            get { return this._Token; }
            private set { this._Token = value; }
        }

        public YuTreeLeaf(YuToken t)
        {
            this.Token = t;
        }
    }

    public class YuTreeNode : YuTreeLeaf
    {
        private List<YuTreeNode> _Children;
        public List<YuTreeNode> Children
        {
            get { return this._Children; }
            private set { this._Children = value; }
        }

        public YuTreeNode(YuToken t)
            : base(t)
        {
            this.Children = new List<YuTreeNode>();
        }
    }
}
