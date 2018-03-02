using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OS
{
    /// <summary>
    /// 设备类
    /// </summary>
    class SheBei
    {
        public static Control control;//进程控制
        public static int aCount = 0;//申请a类设备进程数
        public static int bCount = 0;//申请b类设备进程数
        public static int cCount = 0;//申请b类设备进程数

        public static PCB[] listOfA = new PCB[10];//等待的进程列表
        public static PCB[] listOfB = new PCB[10];
        public static PCB[] listOfC = new PCB[10];

        public PCB A=null;//正在占用设备的进程
        public PCB B=null;
        public PCB A2=null;
        public PCB B2 = null;
        public PCB C = null;

        int ta = 0;//正在占用设备的，要使用的时间
        int ta2 = 0;
        int tb = 0;
        int tb2 = 0;
        int tc = 0;

        public static int[] timeListOfA = new int[10];//时间列表
        public static int[] timeListOfB = new int[10];
        public static int[] timeListOfC = new int[10];

        public bool startedOfA = false;//标志是否开始计时了
        public bool startedOfB = false;
        public bool startedOfA2 = false;
        public bool startedOfB2 = false;
        public bool startedOfC = false;

        public Thread thread2;

        public SheBei(Control c)
        {
            control = c;
            thread2 = new Thread(new ThreadStart(timer2));
            thread2.IsBackground = true;
            thread2.Start();
        }

        //申请设备
        public bool askForShebei(PCB pcb, char sb, int time)
        {
            lock (this)
            {

                if (aCount == 10 || bCount == 10)
                    return false;
                else
                {
                    switch (sb)
                    {
                        case 'a':
                            listOfA[aCount] = pcb;
                            timeListOfA[aCount] = time;
                            aCount++;
                            break;
                        case 'b':
                            listOfB[bCount] = pcb;
                            timeListOfB[bCount] = time;
                            bCount++;
                            break;
                        case 'c':
                            listOfC[cCount] = pcb;
                            timeListOfC[cCount] = time;
                            cCount++;
                            break;
                    }
                    return true;
                }
            }
        }

        //判断是否就被使用的设备
        public void timer2()
        {
            while (true)
            {
                lock (this)
                {
                    if (aCount > 0)
                    {
                        if (!startedOfA)
                        {
                            startedOfA = true;
                            A = listOfA[0];
                            ta = timeListOfA[0];
                            for (int i = 0; i < aCount - 1; i++)
                            {
                                listOfA[i] = listOfA[i + 1];
                                timeListOfA[i] = timeListOfA[i + 1];
                            }
                            aCount--;
                            Thread tempThread = new Thread(new ThreadStart(shebeiTimerA));
                            tempThread.IsBackground = true;
                            tempThread.Start();
                        }
                        else if (!startedOfA2)
                        {
                            startedOfA2 = true;
                            A2 = listOfA[0];
                            ta2 = timeListOfA[0];
                            for (int i = 0; i < aCount - 1; i++)
                            {
                                listOfA[i] = listOfA[i + 1];
                                timeListOfA[i] = timeListOfA[i + 1];
                            }
                            aCount--;
                            Thread tempThread = new Thread(new ThreadStart(shebeiTimerA2));
                            tempThread.IsBackground = true;
                            tempThread.Start();
                        }
                    }
                    if (bCount > 0)
                    {
                        if (!startedOfB)
                        {
                            startedOfB = true;
                            B = listOfB[0];
                            tb = timeListOfB[0];
                            for (int i = 0; i < bCount - 1; i++)
                            {
                                listOfB[i] = listOfB[i + 1];
                                timeListOfB[i] = timeListOfB[i + 1];
                            }
                            bCount--;
                            Thread tempThread = new Thread(new ThreadStart(shebeiTimerB));
                            tempThread.IsBackground = true;
                            tempThread.Start();
                        }
                        else if(!startedOfB2)
                        {
                            startedOfB2 = true;
                            B2 = listOfB[0];
                            tb2 = timeListOfB[0];
                            for (int i = 0; i < aCount - 1; i++)
                            {
                                listOfB[i] = listOfB[i + 1];
                                timeListOfB[i] = timeListOfB[i + 1];
                            }
                            bCount--;
                            Thread tempThread = new Thread(new ThreadStart(shebeiTimerB2));
                            tempThread.IsBackground = true;
                            tempThread.Start();
                        }
                    }
                    if (cCount > 0)
                    {
                        if (!startedOfC)
                        {
                            startedOfC = true;
                            C = listOfC[0];
                            tc = timeListOfC[0];
                            for (int i = 0; i < cCount - 1; i++)
                            {
                                listOfC[i] = listOfC[i + 1];
                                timeListOfC[i] = timeListOfC[i + 1];
                            }
                            cCount--;
                            Thread tempThread = new Thread(new ThreadStart(shebeiTimerC));
                            tempThread.IsBackground = true;
                            tempThread.Start();
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }

        //计时
        public void shebeiTimerA()
        {
            Form1.Instance.setShebeiBusy("a");
            Form1.Instance.setShebeiName("a",A.discription,A.pid);
            int value = ta * 100;
            Form1.Instance.setShebeiPBoth("a", 0, ta);
            while (value > 0)
            {
                Thread.Sleep(10);
                value -= 1;
                Form1.Instance.setShebeiPValue("a", value);
            }
            Form1.Instance.setShebeiIdle("a");
            Form1.Instance.setShebeiName("a", "", 0);
            lock (this)
            {
                control.wakeup(A);
                startedOfA = false;
            }
            
        }

        //计时
        public void shebeiTimerB()
        {
            Form1.Instance.setShebeiBusy("b");
            Form1.Instance.setShebeiName("b", B.discription,B.pid);
            int value = tb * 100;
            Form1.Instance.setShebeiPBoth("b", 0, tb);
            while (value > 0)
            {
                Thread.Sleep(10);
                value -= 1;
                Form1.Instance.setShebeiPValue("b", value);
            }
            Form1.Instance.setShebeiIdle("b");
            Form1.Instance.setShebeiName("b", "",0);
            lock (this)
            {
                control.wakeup(B);
                startedOfB = false;
            }
           
        }

        public void shebeiTimerC()
        {
            Form1.Instance.setShebeiBusy("c");
            Form1.Instance.setShebeiName("c", C.discription, C.pid);
            int value = tc * 100;
            Form1.Instance.setShebeiPBoth("c", 0, tc);
            while (value > 0)
            {
                Thread.Sleep(10);
                value -= 1;
                Form1.Instance.setShebeiPValue("c", value);
            }
            Form1.Instance.setShebeiIdle("c");
            Form1.Instance.setShebeiName("c", "",0);
            lock (this)
            {
                control.wakeup(C);
                startedOfC = false;
            }

        }

        //计时
        public void shebeiTimerA2()
        {
            Form1.Instance.setShebeiBusy("a2");
            Form1.Instance.setShebeiName("a2", A2.discription,A2.pid);
            int value = ta2* 100;
            Form1.Instance.setShebeiPBoth("a2", 0, ta2);
            
            while (value > 0)
            {
                Thread.Sleep(10);
                value -= 1;
                Form1.Instance.setShebeiPValue("a2", value);
            }
            Form1.Instance.setShebeiIdle("a2");
            Form1.Instance.setShebeiName("a2", "",0);
            lock (this)
            {
                control.wakeup(A2);
                startedOfA2 = false;
            }
            
        }

        //计时
        public void shebeiTimerB2()
        {
            Form1.Instance.setShebeiBusy("b2");
            Form1.Instance.setShebeiName("b2", B2.discription,B2.pid);
            int value = tb2 * 100;
            Form1.Instance.setShebeiPBoth("b2", 0, tb2);

            while (value > 0)
            {
                Thread.Sleep(10);
                value -= 1;
                Form1.Instance.setShebeiPValue("b2", value);
            }
            Form1.Instance.setShebeiIdle("b2");
            Form1.Instance.setShebeiName("b2", "",0);
            lock (this)
            {
                control.wakeup(B2);
                startedOfB2 = false;
            }

        }
    }
}
