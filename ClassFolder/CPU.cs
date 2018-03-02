using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
namespace OS.ClassFolder
{
    public enum Interrupt
    {
        IO,
        End,
        No
    }
    public enum ProcessState
    {
        Block,
        Ready,
        Execute,
        White
    }
    public enum DeviceType
	{
	    a,
        b,
        c,
        no
	}         
    public struct PCB
    {
        public int ProcessID;                           //进程块的编号（0-9）
        public string ProcessName;                      //使用该进程块的进程名
        public int PageAdress;                          //页表的首地址
        public int Sum;                                 //页表的长度    
        public int PC;                                  //各个寄存器的状态
        public string IR;
        public int DR;
        public Interrupt PSW;
        public int Pri;                                 //优先级
        public int WaitTime;                            //要使用设备多长时间
        public int GetDeviceTime;                       //获得设备的时间
        public int ExecuteTime;                         //开始执行的时间
        public DeviceType NeedDevice;                   //申请失败的设备类型
        public DeviceType HaveDevice;                   //正在使用的设备类型
        public int DN;                                  //使用的是哪个设备
        public string BlockReason;                      //阻塞的原因
        public int Next;
    }
    class CPU
    {
        public int PC;
        public int DR;
        public  string IR;
        public Interrupt PSW;
        public Interrupt PSW1;
        public PCB[] PCBArray=new PCB[10];
        public DateTime XTTime;
        public int XDTime;
        public int White;
        public int Ready;
        public int Block;
        public int Execute;
        private DeviceType type;
        private int time;
        public OS.ClassFolder.MainRam ram = new MainRam();
        public OS.ClassFolder.Device Dev = new Device();
        private void Init()
        {
            //
            //初始化PCB块
            //
            White = 0;
            Ready =Block=Execute=10;
            for (int i = 0; i < 10; i++)
            {
                PCBArray[i].ProcessID = i;
                PCBArray[i].Next = i + 1;
            }
            //
            //初始化寄存器
            //
            PC = 0;
            PSW = Interrupt.No;
            PSW1 = Interrupt.No;
            IR = "";
            //
            //初始化时间
            //
            XTTime = Convert.ToDateTime("00:00:00");
            XDTime = 0;
        }
        //
        //构造函数
        //
        public CPU()
        {
            Init();
        }
        #region cpu类的事件和委托
        //
        //申请设备的事件和委托
        //
        public class DeviceStateChangeEventArgs : EventArgs
        {
            private int _Atime;
            private DeviceType _type;
            private int _DN;
            private string _processname;
            private int _needtime;
            public int Atime
            {
                get { return _Atime; }
                set { _Atime = value; }
            }
            public DeviceType type
            {
                get { return _type; }
                set { _type = value;}
            }
            public int DN
            {
                get { return _DN; }
                set { _DN = value;}
            }
            public string processname
            {
                get { return _processname; }
                set { _processname = value; }
            }
            public int needtime
            {
                get { return _needtime; }
                set { _needtime = value;}
            }
        }
        public delegate void DeviceStateChangeEventHander(object sender, DeviceStateChangeEventArgs e);
        public event DeviceStateChangeEventHander DeviceStateChange;
        public delegate void ErrorIREventHander(object sender, EventArgs e);
        public event ErrorIREventHander ErrorIR;
        public delegate void QueueChangeHander(object sender, EventArgs e);
        public event QueueChangeHander QueueChange;
        public delegate void FinishIRHander(object sender,EventArgs e);
        public event FinishIRHander FinishIR;
        public delegate void ExecuteIsWhiteHander(object sender, EventArgs e);
        public event ExecuteIsWhiteHander ExecuteIsWhite;
        #endregion
        //
        //空闲PCB链表的操作
        //
        public int GetOneFromWhite()
        {
            int a = White;
            if (a<10)
            {
                White = PCBArray[a].Next;              
            }
            return a;
        }
        public void InsertOneToWhite(int a)
        {
            PCBArray[a].Next = White;
            White = a;
        }
        //
        //就绪PCB链表的操作
        //
        public void InsertOneToReady(int a)
        {
            PCBArray[a].Next = Ready;
            Ready = a;
        }
        public void GetOneFromReady(int a)
        {
            int b = Ready;
            if (a == b)
            {
                Ready = PCBArray[a].Next;
            }
            else
            {
                while (b < 10)
                {
                    if (PCBArray[b].Next == a)
                    {
                        PCBArray[b].Next = PCBArray[PCBArray[b].Next].Next;
                    }
                    b = PCBArray[b].Next;
                }
            }
        }
        //
        //阻塞PCB链表的操作
        //
        public void InsertOneToBlock(int a)
        {
            PCBArray[a].Next = Block;
            Block = a;
        }
        //
        //Creat函数，创建进程
        //
        public void Creat(string Name,string str)
        {
            //
            //申请PCB，a>10，则申请失败
            //
            int a = GetOneFromWhite();
            int b;
            if (str.Length > 0)
            {
                int sum = (str.Length + 15) / 16;
                if (a < 10)
                {
                    if (ram.Judge(sum) == true)
                    {
                        //
                        //分配内存并加载到内存
                        //
                        b = ram.Allocate(sum);
                        ram.LoadContent(str, b);
                        //
                        //初始化PCB
                        //
                        PCBArray[a].ProcessName = Name;
                        PCBArray[a].PageAdress = b;
                        PCBArray[a].Sum = sum;
                        PCBArray[a].PC = 0;
                        PCBArray[a].IR = "";
                        PCBArray[a].DR = 0;
                        PCBArray[a].PSW = Interrupt.No;
                        PCBArray[a].WaitTime = -10;
                        PCBArray[a].Pri = 1024 / str.Length;
                        PCBArray[a].ExecuteTime = 0;
                        PCBArray[a].NeedDevice = DeviceType.no;
                        PCBArray[a].HaveDevice = DeviceType.no;
                        PCBArray[a].GetDeviceTime = 0;
                        PCBArray[a].DN = -1;
                        PCBArray[a].BlockReason = "";
                        InsertOneToReady(a);
                        //
                        //是否转向进程调度
                        //
                        int c = JudgeAttemper();
                        if (c < 10)
                        {
                            Attemper(c);
                        }
                 
                    }
                    else
                    {
                        InsertOneToWhite(a);
                        MessageBox.Show("内存不足或文件太长，创建进程失败", "消息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("PCB块不足，创建进程失败", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("文件为空,不能创建进程","错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        //
        //Destroy函数，撤销程序
        //
        public void Destory(int a)
        {
            //
            //回收内存
            //
            int p = PCBArray[a].PageAdress;
            int sum = PCBArray[a].Sum;
            ram.DeAllocate(p,sum);
            //
            //回收PCB块
            //
            InsertOneToWhite(a);
            Execute = 10;
            //
            //显示结果
            //
            //
            //
            //
          

        }
        //
        //BlockProcess函数，阻塞进程
        //
        public void BlockProcess(int a,DeviceType b,int time)
        {
            //
            //保护CPU现场
            //
            PCBArray[a].PC = PC;
            PCBArray[a].IR = IR;
            PCBArray[a].DR = DR;
            //
            //判断申请设备是否成功，根据不同情况填写BlockReason项
            //
            bool d = Dev.JudgeDevice(b);
            if (d == false)
            {
                PCBArray[a].NeedDevice = b;
                PCBArray[a].HaveDevice = DeviceType.no;
                PCBArray[a].BlockReason = "申请" + b + "设备失败";
                PCBArray[a].PC = PCBArray[a].PC - 4;
            }
            else
            {
                PCBArray[a].DN = Dev.Allocate(b);
                PCBArray[a].HaveDevice = b;
                PCBArray[a].NeedDevice = DeviceType.no;
                PCBArray[a].GetDeviceTime = XDTime;
                PCBArray[a].WaitTime = time;
                PCBArray[a].BlockReason = "等待IO输入输出";
                if (DeviceStateChange != null)
                {
                    DeviceStateChangeEventArgs e=new DeviceStateChangeEventArgs();
                    e.DN = PCBArray[a].DN;
                    e.type = b;
                    e.processname = PCBArray[a].ProcessName;
                    e.needtime = time;
                    e.Atime = XDTime;
                    DeviceStateChange(null,e);
                }            
            }
            //
            //修改进程状态
            //
            InsertOneToBlock(a);
            Execute = 10;
            //
            //转向进程调度
            //
            int c = JudgeAttemper();
            if (c < 10)
            {
                Attemper(c);
            }
 
        }
        //
        //WakeUp函数，唤醒进程
        //
        public void WakeUp(int ID,DeviceType device)
        {
            //
            //唤醒自己
            //
            int d = Block;
            if (Block == ID)
            {
                Block = PCBArray[ID].Next;
                InsertOneToReady(ID);
            }
            else
            {
                while (PCBArray[d].Next < 10)
                {
                    if (PCBArray[d].Next == ID)
                    {
                        PCBArray[d].Next = PCBArray[ID].Next;
                        InsertOneToReady(ID);
                        break;
                    }
                    d = PCBArray[d].Next;
                }
            }
            //
            //检查第一个节点
            //
                while(Block<10&&PCBArray[Block].NeedDevice == device)
                {
                    int h = Block;
                    Block = PCBArray[h].Next;
                    InsertOneToReady(h);
                }
                //
                //检查其他节点
                //
            if(Block<10)
            {
                int a = Block;
                while (PCBArray[a].Next < 10)
                {
                    if (PCBArray[PCBArray[a].Next].NeedDevice == device)
                    {
                        int h = PCBArray[a].Next;
                        PCBArray[a].Next = PCBArray[PCBArray[a].Next].Next;
                        InsertOneToReady(h);
                    }
                    else
                    {
                        a = PCBArray[a].Next;
                    }
                }
            }
            int c = JudgeAttemper();
            if (c < 10)
            {
                Attemper(c);
            }
        }
        //
        //判断是否需要进行进程的调度，若需要则返回进程块号（0-9），不需要则返回10
        //
        public int JudgeAttemper()
        {
            //
            //选出就绪链表中优先级最高
            //
            int k;
            if (Ready < 10)
            {
                int p = Ready;
                int a = PCBArray[Ready].Pri;
                k = Ready;                                      //优先级最高的块号
                while (PCBArray[p].Next < 10)
                {
                    if (PCBArray[PCBArray[p].Next].Pri > a)
                    {
                        a = PCBArray[PCBArray[p].Next].Pri;
                        k = PCBArray[p].Next;
                    }
                    p = PCBArray[p].Next;
                }
            }
            else
            {
                return 10;
            }
            //
            //跟执行链表内的PCB块进行比较
            //
            if (Execute < 10)
            {
                if (PCBArray[k].Pri > PCBArray[Execute].Pri)
                {
                    return k;
                }
                else
                {
                    return 10;
                }
            }
            else
            {
                return k;
            }
        }
        //
        //进程调度函数
        //
        public void Attemper(int a)
        {
            //
            //保护CPU现场
            //
            if (Execute < 10)
            {
                PCBArray[Execute].PC = PC;
                PCBArray[Execute].IR = IR;
                PCBArray[Execute].DR = DR;
                InsertOneToReady(Execute);
            }
            //
            //选择一个进程，初始化CPU中的寄存器
            //

            GetOneFromReady(a);
            Execute = a;
            PC = PCBArray[a].PC;
            IR = PCBArray[a].IR;
            DR = PCBArray[a].DR;
       
        }
        //
        //主函数cpu
        //
        public void cpu()
        {
          
            if (true)
            {
                switch (PSW)
                {
                    case Interrupt.End:
                        {
                            //
                            //写入out文件
                            //
                            //            
                            //撤销进程，进行进程调度
                            //
                            Destory(Execute);
                            int a = JudgeAttemper();
                            if (a < 10)
                            {
                                Attemper(a);
                            }
                            PSW = Interrupt.No;
                            break;
                        }
                    case Interrupt.IO:
                        {
                            BlockProcess(Execute,type,time);
                            PSW = Interrupt.No;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                switch (PSW1)
                {
                    case Interrupt.IO:
                        {
                            int b = Block;
                            while (b < 10)
                            {
                                if (XDTime - PCBArray[b].GetDeviceTime - 1 == PCBArray[b].WaitTime)
                                {
                                    Dev.DeAllocate(PCBArray[b].HaveDevice, PCBArray[b].DN);                                  
                                    WakeUp(b, PCBArray[b].HaveDevice);
                                    PCBArray[b].DN = -1;
                                    PCBArray[b].GetDeviceTime = 0;
                                    PCBArray[b].HaveDevice = DeviceType.no;
                                    PCBArray[b].WaitTime = -10;
                                }
                                b = PCBArray[b].Next;
                            }
                            PSW1 = Interrupt.No;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                if (Execute < 10)
                {
                    IR = ram.SendToIR(PCBArray[Execute].PageAdress, PC);
                    PC = PC + 4;
                    bool str = ram.JudgeIR(IR);
                    if (str == true)
                    {
                        
                        switch (IR[1])
                        {
                            case '=':
                                {
                                    DR = Convert.ToInt32(IR[2].ToString());
                                    break;
                                }
                            case '+':
                                {
                                    DR++;
                                    break;
                                }
                            case '-':
                                {
                                    DR--;
                                    break;
                                }
                            case 'a':
                            case 'A':
                                {
                                    type=DeviceType.a;
                                    time=Convert.ToInt32(IR[2].ToString());                                   
                                    PSW = Interrupt.IO;
                                    break;
                                }
                            case 'b':
                            case 'B':
                                {
                                    type = DeviceType.b;
                                    time = Convert.ToInt32(IR[2].ToString());
                                    PSW = Interrupt.IO;                                     
                                    break;
                                }
                            case 'c':
                            case 'C':
                                {
                                    type = DeviceType.c;
                                    time = Convert.ToInt32(IR[2].ToString());
                                    PSW = Interrupt.IO;
                                    break;
                                }
                            case 'n':
                                {
                                    PSW = Interrupt.End;
                                    break;
                                }
                        }
                        PCBArray[Execute].ExecuteTime++;
                        if (FinishIR != null)
                        {
                            EventArgs e = new EventArgs();
                            FinishIR(null, e);
                        }
                    }
                    else
                    {
                        Destory(Execute);
                        int b = JudgeAttemper();
                        if (b < 10)
                        {
                            Attemper(b);
                        }
                        if (ErrorIR != null)
                        {
                            EventArgs e = new EventArgs();
                            ErrorIR(null,e);
                        }
                    }
                }
                else
                {
                    //
                    //do nothing
                    //
                    if (this.ExecuteIsWhite != null)
                    {
                        EventArgs e = new EventArgs();
                        ExecuteIsWhite(null,e);
                    }
                }
                int k = Block;
                while (k < 10)
                {
                    if (XDTime - PCBArray[k].GetDeviceTime == PCBArray[k].WaitTime)
                    {
                        PSW1 = Interrupt.IO;
                    }
                    k = PCBArray[k].Next;
                }
                if (QueueChange != null)
                {
                    EventArgs e = new EventArgs();
                    QueueChange(null, e);
                }
                XDTime++;
                IR = "";
            }           
        }
    }
}
