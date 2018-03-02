using System;
using System.Collections.Generic;
using System.Text;

namespace OS
{
    class Message
    {
        public bool suc;
        public string msg;

        public Message(bool flag, string msg)
        {
            this.suc = flag;
            this.msg = msg;
        }
        
        public string Msg
        {
            get {
                return msg;
            }
            set {
                this.msg = value;
            }
        }

        public bool Suc
        {
            get
            {
                return suc;
            }
            set
            {
                this.suc = value;
            }
        }
    }
}
