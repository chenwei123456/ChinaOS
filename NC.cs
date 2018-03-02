using System;
using System.Collections.Generic;
using System.Text;

namespace OS
{
    /// <summary>
    /// 内存类
    /// </summary>
    class NC
    {
        public static char[] nc = new char[500];//内存
        public static int cxStart = 0;//程序开始位置
        public static int csEnd =499;//程序终止位置
        public static int[] wst = new int[500];//位示图

        public NC()
        {
            for (int i = 0; i < 500; i++)//初始化位示图
                wst[i] = 0;
        }

        

        //申请内存
        public static bool askForNC(string data,PosInNC pos)
        {
           
            int count = 0;
            char[] c = data.ToCharArray();
            for (int i = 0; i <= 499; i++)
            {
                if (wst[i] == 0)
                    count++;
                else
                    count = 0;
                if (count == c.Length)//如果有合适的连续空间，就分配
                {
                    pos.start = i - count + 1;
                    pos.end = i;
                    int m = pos.start;
                    int j = 0;
                    for (int k = 0; k < c.Length; k++)//存到内存
                    {
                        nc[m] = c[k];
                        wst[m] = 1;
                        m++;
                    }
                    Form1.Instance.busyNC(pos.start, pos.end);
                    return true;
                }
            }
            return false;
        }

        //释放内存
        public static void freeNc(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                nc[i] = ' ';
                wst[i] = 0;
            }
            Form1.Instance.freeNC(start, end);
        }
    }
}
