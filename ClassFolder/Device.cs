using System;
using System.Collections.Generic;
using System.Text;
 
namespace OS.ClassFolder
{
    public struct DeviceTable
    {
        public DeviceType deviceType;
        public int total;
        public int[] useState;
        public DeviceTable(DeviceType type, int total)
        {
            this.total = total;
            deviceType = type;
            useState = new int[total];
            for (int i = 0; i < total; i++)
            {
                useState[i] = 0;
            }
        }
    }
    class Device
    {
        private DeviceTable[] table=new DeviceTable[3];
        private void Init()
        {
            table[0]=new DeviceTable(DeviceType.a, 3);
            table[1]=new DeviceTable(DeviceType.b, 2);
            table[2]=new DeviceTable(DeviceType.c, 1);
        }
        public Device()
        {
            Init();
        }
        //
        //检查类型为type的设备是否可用
        //
        public bool JudgeDevice(DeviceType type)
        {
            bool str = false;
            switch (type)
            {
                case DeviceType.a:
                    {
                        if (table[0].total > 0)
                        {
                            str = true;
                        }
                        break;
                    }
                case DeviceType.b:
                    {
                        if (table[1].total > 0)
                        {
                            str = true;
                        }
                        break;
                    }
                case DeviceType.c:
                    {
                        if (table[2].total > 0)
                        {
                            str = true;
                        }
                        break;
                    }
            }
            return str;
        }
        //
        //分配设备,返回第几个设备被占用
        //
        public int Allocate(DeviceType type)
        {
            int k = 0;
            switch (type)
            {
                case DeviceType.a:
                    {
                        table[0].total--;
                        for (int i = 0; i < 3; i++)
                        {
                            if (table[0].useState[i] == 0)
                            {
                                table[0].useState[i] = 1;
                                k = i;
                                break;
                            }
                        }
                        break;
                    }
                case DeviceType.b:
                    {
                        table[1].total--;
                        for (int i = 0; i < 2; i++)
                        {
                            if (table[0].useState[i] == 0)
                            {
                                table[0].useState[i] = 1;
                                k = i;
                                break;
                            }
                        }
                        break;
                    }
                case DeviceType.c:
                    {
                        table[2].total--;
                        break;
                    }
            }
            return k;
        }
        //
        //回收设备
        //
        public void DeAllocate(DeviceType type, int a)
        {
            switch (type)
            {
                case DeviceType.a:
                    {
                        table[0].total++;
                        table[0].useState[a] = 0;                       
                        break;
                    }
                case DeviceType.b:
                    {
                        table[1].total++;
                        table[1].useState[a] = 0;
                        break;
                    }
                case DeviceType.c:
                    {
                        table[2].total++;
                        table[2].useState[a] = 0;
                        break;
                    }
            }
        }

    }
}
