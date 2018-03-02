using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;

namespace OS
{
    /// <summary>
    /// 总体控制类
    /// </summary>
    class Control
    {
        public static PCB[] readayPCBs = new PCB[10];//就绪列表
        public static PCB[] zcPCBs = new PCB[10];//阻塞列表
        public static string[] doneList = new String[1000];//已完成的进程
        public static int totalTime = 5;//时间片长度
        public static float curTime = 5;//当前时间片剩余
        public int count = 0;//当前pcb总数，包括阻塞的pcb
        public int countOfZcPCBs = 0;//阻塞进程的个数
        public int countOfDone = 0;//已完成的进程数
        public int countOfReaday = 0;//就绪进程数
        public SheBei SHEBEI;//生成设备类
        public static Form1 form1;//界面类
        public bool started = false;//表示cpu函数是否运行
        public bool zcFlag = false;//阻塞标志
        public bool doneFlag = false;//执行完毕标志
        public Thread threadOfTimer;//时间片线程
        public static int sleepTime = 400;//执行一个指令后休息时间

        private int reg;

        public Control(Form1 form)
        {
            form1 = form;
            SHEBEI = new SheBei(this);
            //监视是否有就绪进程
            Thread monitorThread = new Thread(new ThreadStart(monitor));
            monitorThread.IsBackground = true;
            monitorThread.Start();
        }

        //根据在内存中的位置和进程外部描述符创建进程,pid为内存中地址+1（当分配到内存后，开始执行本函数)
        public bool create(int startPoint, int endPoint, string str)
        {
            lock (this)
            {
                if (count >= 10)
                    return false;
                PCB newPCB = new PCB(startPoint, endPoint, str);
                readayPCBs[countOfReaday] = newPCB;
                countOfReaday++;
                count++;
                form1.setReadayList(getReadayList());
                return true;
            }
        }

        //把因为申请设备阻塞的进程唤醒，调入就如队列
        public bool wakeup(PCB pcb)
        {
            lock (this)
            {
                int i;
                for (i = 0; i < countOfZcPCBs; i++)
                {
                    if (zcPCBs[i].pid == pcb.pid)
                        break;
                }
                for (int j = i; j < countOfZcPCBs - 1; j++)
                {
                    zcPCBs[j] = zcPCBs[j + 1];
                }
                countOfZcPCBs--;
                readayPCBs[countOfReaday] = pcb;
                countOfReaday++;
                //更新界面
                form1.setReadayList(getReadayList());
                form1.setZcList(getZcList());
                form1.setDoneList(getDoneList());
                return true;
            }
        }

        //监视是否有就绪进程
        public void monitor()
        {
            while (countOfReaday == 0)
            {
                Thread.Sleep(100);
            }
            //有了就绪进程，调用cpu函数
            cpu();
        }

        public void cpu()
        {
            while (countOfReaday > 0)
            {
                if (readayPCBs[0].hasData)
                {
                    reg = readayPCBs[0].data;
                }
                started = true;
                if (countOfReaday > 0)
                {
                    form1.setProc(0, totalTime * 100);//设置进度条的取值范围
                    curTime = totalTime;
                    threadOfTimer = new Thread(new ThreadStart(timer));
                    threadOfTimer.IsBackground = true;
                    //时间片开始计时
                    threadOfTimer.Start();
                    zcFlag = false;
                    doneFlag = false;
                    int j = 0;
                    ////////////////////////////////////////////一个进程的执行开始
                    while (true)
                    {
                        lock (this)
                        {
                            if (curTime <= 0)
                            {
                                //如果时间片用完，结束本次进程执行
                                break;
                            }
                            String temp = "";
                            int prePc = readayPCBs[0].pc;


                            while (NC.nc[readayPCBs[0].pc] != ';')//取出一条指令
                            {
                                if (NC.nc[readayPCBs[0].pc+1] == ';')
                                {
                                    form1.currNC(readayPCBs[0].pc-2);
                                    form1.currNC(readayPCBs[0].pc-1);
                                    form1.currNC(readayPCBs[0].pc);
                                    form1.currNC(readayPCBs[0].pc+1);
                                }
                                temp += NC.nc[readayPCBs[0].pc];
                                if (readayPCBs[0].pc <= readayPCBs[0].cxEnd)
                                {
                                    //form1.currNC(readayPCBs[0].pc);
                                }
                                readayPCBs[0].pc++;
                                if (readayPCBs[0].pc > readayPCBs[0].cxEnd)
                                {
                                    //错误指令，不进行操作
                                    destory();
                                    j = 1;
                                    break;

                                }
                            }
                            if (j == 1)
                                break;
                            form1.setCurIns(temp);//更新界面当前指令
                            readayPCBs[0].pc++;//指向下一条指令
                            if (NC.nc[readayPCBs[0].pc-1] == ';'&&readayPCBs[0].pc> readayPCBs[0].cxStart+4)
                            {
                                form1.busyNC(readayPCBs[0].pc - 8, readayPCBs[0].pc - 8);
                                form1.busyNC(readayPCBs[0].pc - 7, readayPCBs[0].pc - 7);
                                form1.busyNC(readayPCBs[0].pc - 6, readayPCBs[0].pc - 6);
                                form1.busyNC(readayPCBs[0].pc - 5, readayPCBs[0].pc - 5);
                            }
                            //判断指令类型
                            if (temp.Equals("end"))
                            {
                                //进程结束
                                doneFlag = true;
                                break;
                            }
                            else if (temp.EndsWith("++"))
                            {
                                //自增操作
                                reg++;
                            }
                            else if (temp.EndsWith("--"))
                            {
                                //自减操作
                                reg--;
                            }
                            else if (temp.StartsWith("!"))
                            {
                                //申请设备操作
                                char shebei = Convert.ToChar(temp.Substring(1, 1));
                                int time = Convert.ToInt32(temp.Substring(2));
                                if (SHEBEI.askForShebei(readayPCBs[0], shebei, time))//如果申请设备成功
                                {
                                    form1.setCurResult("申请设备：" + shebei + "(" + time + "秒)");
                                    zcFlag = true;
                                    zcPCBs[countOfZcPCBs] = readayPCBs[0];
                                    break;
                                }
                                else
                                {
                                    //下次循环继续执行申请设备指令
                                    readayPCBs[0].pc = prePc;
                                }
                            }
                            else if (temp.IndexOf("=") > 0)
                            {
                                //赋值操作
                                string str = temp.Substring(temp.IndexOf("=") + 1);
                                readayPCBs[0].hasData = true;
                                reg = Convert.ToInt32(str);
                            }
                            else
                            {
                                //错误指令，不进行操作
                                break;
                            }
                            if (readayPCBs[0].hasData)
                            {
                                //如果存在数据
                                form1.setCurResult(reg + "");//显示到屏幕
                            }
                        }
                        Thread.Sleep(sleepTime);//休息
                    }
                    ////////////////////////////////////////////一个进程的执行中断
                    threadOfTimer.Abort();
                    form1.setProcValue(0);

                    form1.setCurIns("");
                    form1.setCurResult("");
                    form1.setCurPCB("");
                    //判断进程中断执行的原因(阻塞、执行完毕、时间片用完)
                    lock (this)
                    {
                        if (zcFlag)
                        {
                            //进程阻塞
                            readayPCBs[0].data = reg;
                            zcPCBs[countOfZcPCBs] = readayPCBs[0];
                            for (int i = 0; i < countOfReaday - 1; i++)
                            {
                                readayPCBs[i] = readayPCBs[i + 1];
                            }
                            countOfZcPCBs++;
                            countOfReaday--;
                        }
                        else if (doneFlag)
                        {
                            NC.freeNc(readayPCBs[0].cxStart, readayPCBs[0].cxEnd);
                            //进程执行完毕
                            doneList[countOfDone] = readayPCBs[0].discription + "/" + readayPCBs[0].pid;
                            if (readayPCBs[0].hasData)
                            {
                                //如果有操作数

                                doneList[countOfDone] += "/" + reg;
                            }
                            else
                            {
                                doneList[countOfDone] += "/null";
                            }
                            for (int i = 0; i < countOfReaday - 1; i++)
                            {
                                readayPCBs[i] = readayPCBs[i + 1];
                            }
                            countOfReaday--;
                            count--;
                            countOfDone++;
                        }
                        else if (curTime <= 0)
                        {
                            //时间片用完了
                            readayPCBs[0].data = reg;
                            PCB tempPCB = readayPCBs[0];
                            for (int i = 0; i < countOfReaday - 1; i++)
                            {
                                readayPCBs[i] = readayPCBs[i + 1];
                            }
                            readayPCBs[countOfReaday - 1] = tempPCB;
                        }
                    }
                    //更新界面
                    form1.setReadayList(getReadayList());
                    form1.setZcList(getZcList());
                    form1.setDoneList(getDoneList());
                }
            }
            //cpu函数执行完毕
            started = false;
            //继续监视是否有就绪进程
            Thread monitorThread = new Thread(new ThreadStart(monitor));
            monitorThread.IsBackground = true;
            monitorThread.Start();
        }

        public void destory()
        {
            NC.freeNc(readayPCBs[0].cxStart, readayPCBs[0].cxEnd);
            //进程执行完毕
            doneList[countOfDone] = readayPCBs[0].discription + "/" + readayPCBs[0].pid;
            if (readayPCBs[0].hasData)
            {
                //如果有操作数

                doneList[countOfDone] += "/" + reg;
            }
            else
            {
                doneList[countOfDone] += "/null";
            }
            for (int i = 0; i < countOfReaday - 1; i++)
            {
                readayPCBs[i] = readayPCBs[i + 1];
            }
            countOfReaday--;
            count--;
            countOfDone++;
        }

        //得到就绪队列数组
        public string[] getReadayList()
        {
            string[] str = new String[countOfReaday];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = readayPCBs[i].discription + "/" + readayPCBs[i].pid;
            }
            return str;
        }

        //得到阻塞队列数组
        public string[] getZcList()
        {
            string[] str = new String[countOfZcPCBs];
            for (int i = 0; i < countOfZcPCBs; i++)
            {
                str[i] = zcPCBs[i].discription + "/" + zcPCBs[i].pid;
            }
            return str;
        }

        //得到执行结束的进程数组
        public string[] getDoneList()
        {
            string[] str = new String[countOfDone];
            for (int i = 0; i < countOfDone; i++)
            {
                str[i] = doneList[i];
            }
            return str;
        }

        //设置时间片长度
        public static void setTotalTime(int data)
        {
            totalTime = data;
        }

        //时间片计时
        public static void timer()
        {
            curTime = totalTime;
            while (curTime > 0)
            {
                Thread.Sleep(10);
                curTime -= 0.01f;
                if (curTime < 0)
                    curTime = 0;
                form1.setProcValue(curTime * 100);
            }
        }
    }


}
