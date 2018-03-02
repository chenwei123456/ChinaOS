using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace OS.ClassFolder
{
    class MainRam
    {
        private char[,] RamUnit = new char[32, 16];        //用数组模拟内存
        //private int[] UseState=new int[32];              //内存的使用情况
        private int[,] Wst = new int[4, 8];                //位示图
        private int UnuseRam;
        #region 定义的各个委托和事件
        //
        //分配内存的委托和事件
        //
        public class StateChangeEventArgs:EventArgs
        {
            private int _num1;
            private int _num2;
            public StateChangeEventArgs():base(){}
            public int num1
            {
                get{return _num1;}
                set{_num1=value;}
            }
            public int num2
            {
                get{return _num2;}
                set{_num2=value;}
            }
            
        }
        public delegate void StateChangeEventHander(object sender,StateChangeEventArgs e);
        public event StateChangeEventHander StateChange;
        //
        //内存回收的委托和事件
        //
        public class StateChangeEventArgs1 : EventArgs
        {
            private int _num1;
            public StateChangeEventArgs1() : base() { }
            public int num1
            {
                get { return _num1; }
                set { _num1 = value; }
            }
        }
        public delegate void StateChangeEventHander1(object sender, StateChangeEventArgs1 e);
        public event StateChangeEventHander1 StateChange1;
        //
        //
        //
        public class StateChangeEventArgs2 : EventArgs
        {
            private int _num1;
            public int num1
            {
                get { return _num1; }
                set { _num1 = value; }
            }
        }
        public delegate void StateChangeEventHander2(object sender, StateChangeEventArgs2 e);
        public event StateChangeEventHander2 StateChange2;
        #endregion
        public MainRam()
        {
            Init();
        }
        private void Init()                                //初始化,主存的存储单元全为空闲(未占用).
        {
            UnuseRam = 32;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Wst[i, j] = 0;
                }
            }
        }
        private int GetUnuseRam()
        {
            int a=0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Wst[i, j] == 0)
                    {
                        a++;
                    }
                }
            }
            return a;
        }
        //
        //查看空闲块是否够用，够用返回true，否则返回false
        //
        public bool Judge(int sum)
        {
            bool b=false;
            int a;
            a = GetUnuseRam();
            if (sum > 16)
            {
                
                b=false;
            }
            else
            {
                if (a > sum + 1)
                {
                    b = true;
                }
 
            }
            return b;
        }
        //
        //进行内存的分配，返回页表的地址(内存的第几块)
        //
        public int Allocate(int sum)                //sum程序需要的内存块数（页表长度）
        {
            int k = 0;
            int z=0;
            int PageAdress=0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Wst[i, j] == 0)
                    {
                        if (k == 0)                 //页表的地址
                        {
                            PageAdress = i * 8 + j;
                            Wst[i, j] = 1;
                            UnuseRam--;
                            k++;
                        }
                        else
                        {
                            if (z < sum)                    //根据需要的块数添加页表
                            {
                                RamUnit[PageAdress, z] = (char)(i * 8 + j);
                                Wst[i, j] = 1;
                                UnuseRam--;
                                z++;
                            }
                            else
                            {
                                break;
                            }

                        }
                    }
                   
                    if (StateChange != null)
                    {
                        StateChangeEventArgs e = new StateChangeEventArgs();
                        e.num1 = i;
                        e.num2 = j;
                        StateChange(null,e);
                    }
                }
            }
            return PageAdress;
        }
        //
        //内存的回收,根据页表回收内存块，最后回收页表
        //
        public void DeAllocate(int PageAdress,int sum)
        {
            for (int i = 0; i < sum; i++)
            {
                int a = (int)RamUnit[PageAdress, i];
                int zh = a/8;
                int wh = a % 8;
                Wst[zh, wh] = 0;
                UnuseRam++;
                if (StateChange1 != null)
                {
                    StateChangeEventArgs1 e = new StateChangeEventArgs1();
                    e.num1 = a;
                    StateChange1(null, e);
                }
            }
            Wst[PageAdress / 8, PageAdress % 8] = 0;
            if (StateChange1 != null)
            {
                StateChangeEventArgs1 e = new StateChangeEventArgs1();
                e.num1 = PageAdress;
                StateChange1(null, e);
            }
            UnuseRam++;
        }
        //
        //加载文件内容到内存
        //
        public void LoadContent(string content,int PageAdress)
        {
            int a = 0;
            for (int k = 0; k < 16; k++)
            {
                int m=(int)RamUnit[PageAdress,k];
                if (m != 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if (a < content.Length)
                        {
                            RamUnit[m, i] = content[a];
                            a++;
                        }
                        else
                        {
                             
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }
        //
        //判定指令是否合法
        //
        public bool JudgeIR(string IR)
        {
            bool str = true;
            if (IR.Length != 4)
            {
                str = false;
            }
            else
            {
                switch (IR[0])
                {
                    case 'e':
                        {
                            if (IR[1] != 'n' || IR[2] != 'd' || IR[3] != ';')
                            {
                                //MessageBox.Show("非法指令，程序已终止", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                str = false;
                            }
                            break;
                        }
                    case '!':
                        {
                            if ((IR[1] != 'a' && IR[1] != 'b' && IR[1] != 'c' && IR[1] != 'A' && IR[1] != 'B' && IR[1] != 'C') || char.IsDigit(IR[2]) != true || IR[2] == '0' || IR[3] != ';')
                            {
                                str = false;
                            }
                            break;
                        }
                    case 'i':
                        {
                            switch (IR[1])
                            {
                                case '+':
                                    {
                                        if (IR[2] != '+' || IR[3] != ';')
                                        {
                                            str = false;
                                        }
                                        break;
                                    }
                                case '-':
                                    {
                                        if (IR[2] != '-' || IR[3] != ';')
                                        {
                                            str = false;
                                        }
                                        break;
                                    }
                                case '=':
                                    {
                                        if (char.IsDigit(IR[2]) != true || IR[3] != ';')
                                        {
                                            str = false;
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            str = false;
                            break;
                        }
                }
            }
            return str;
        }
        //
        //把内存的内容读到IR中
        //
        public string SendToIR(int PageAdress,int pc)
        {
            string IR = "";
            int a = pc / 16;
            int b = pc % 16;
            int c = Convert.ToInt32(RamUnit[PageAdress, a]);
            for (int i = 0; i < 4; i++)
            {
                IR += RamUnit[c, b+i];
            }
            if (StateChange2 != null)
            {
                StateChangeEventArgs2 e = new StateChangeEventArgs2();
                e.num1 = c;
                StateChange2(null,e);
            }
            return IR;
            
        }
    }
}
