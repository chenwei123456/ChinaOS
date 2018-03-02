using System;
using System.Collections.Generic;
using System.Text;

namespace OS
{
    /// <summary>
    /// 进程控制块类
    /// </summary>
    class PCB
    {
       
        /// <summary>
        /// /////////////////////////////////////////////////PCB内容
        /// </summary>
        public int pid;//进程pid
        public String discription;//进程外部标志符
        public int pc;//下一条要执行的指令
        public int cxStart;//程序起始地址
        public int cxEnd;//程序终止地址
        public int data;
        public bool hasData;

        public PCB(int startPoint, int endPoint,string str)
        {
            this.cxStart = startPoint;
            this.cxEnd = endPoint;
            this.pc = startPoint;
            pid = cxStart + 100;//pid为内存中地址+1
            discription = str;
            hasData = false;
        }
    }
}
