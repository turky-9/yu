using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLisp
{
    public class Error : Exception, Sexp
    {
        private string message;
        private string errinfo;
        private int errorno;

        private static string[] ErrorMessage ={
                                                  "Undefined Error",
                                                  "Unbound Variable",
                                                  "Undefined Function",
                                                  "Not Function",
                                                  "Not Symbol"
                                             };

        public Error(int e, string info)
        {
            this.message = this.GetMessage(e);
            this.errinfo = info;
            this.errorno = e;
        }

        public Error(int e)
            : this(e, "")
        {
        }
        

        public Error()
            : this(0, "")
        {
        }

        private string GetMessage(int e)
        {
            return Error.ErrorMessage[e];
        }

        public void print(TextWriter tw)
        {
            tw.Write(this.serialize());
        }

        public string serialize()
        {
            return "Error: " + this.message + " --- " + this.errinfo;
        }
    }
}
