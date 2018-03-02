using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OS
{
    /// <summary>
    /// 文件、目录名：3
    /// 扩展名：2
    /// 目录、文件属性:1:只读，2：读写
    /// 文件长度%64:1
    /// 起始块号：1
    /// 
    /// 说明：char(0)表示目录或前三个块,char(1)表示没有被分配,char(20)表示文件结尾
    /// 
    /// 创建的文件或文件夹的首字母不能是“1”，否则不能创建
    /// 
    /// 文件路径用‘/’分割
    /// </summary>
    /// 
    class Disk
    {
        public Disk()
        {

        }

        //得到单个字符
        public byte getByte(string root, int start)
        {
            byte[] bytes = new byte[1];

            FileStream fs = new FileStream(root, FileMode.Open);
            fs.Seek(start, SeekOrigin.Begin);
            fs.Read(bytes, 0, 1);
            fs.Close();

            return bytes[0];
        }

        //用字节填充一个区域
        public void fillWithByte(string root, int start, byte data, int count)
        {
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                bytes[i] = data;
            }

            FileStream fs = new FileStream(root, FileMode.Open);
            fs.Seek(start, SeekOrigin.Begin);
            fs.Write(bytes, 0, count);
            fs.Close();
        }

        //得到一个字节区域
        public byte[] getBytes(string root, int start, int count)
        {
            byte[] bytes = new byte[count];
            FileStream fs = new FileStream(root, FileMode.Open);
            fs.Seek(start, SeekOrigin.Begin);
            fs.Read(bytes, 0, count);
            fs.Close();

            return bytes;
        }

        //设置字符串
        public void setString(string root, int start, string str)
        {
            char[] chars = str.ToCharArray();
            byte[] bytes = new byte[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                bytes[i] = (byte)chars[i];
            }

            FileStream fs = new FileStream(root, FileMode.Open);
            fs.Seek(start, SeekOrigin.Begin);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }

        //设置字符串
        public void setString(string root, int start, string str, int begin, int count)
        {
            char[] chars = str.ToCharArray();
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                bytes[i] = (byte)chars[begin + i];
            }

            FileStream fs = new FileStream(root, FileMode.Open);
            fs.Seek(start, SeekOrigin.Begin);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }

        //得到字符串
        public string getString(string root, int start, int count)
        {
            StringBuilder sb = new StringBuilder();
            byte[] bytes = new byte[count];

            FileStream fs = new FileStream(root, FileMode.Open);
            fs.Seek(start, SeekOrigin.Begin);
            fs.Read(bytes, 0, count);
            fs.Close();

            for (int i = 0; i < count; i++)
            {
                sb.Append((char)bytes[i]);
            }

            return sb.ToString();
        }


        //设置单个字符
        public void setByte(string root, int start, byte c)
        {
            byte[] bytes = new byte[1];
            bytes[0] = c;

            FileStream fs = new FileStream(root, FileMode.Open);
            fs.Seek(start, SeekOrigin.Begin);
            fs.Write(bytes, 0, 1);
            fs.Close();
        }

        //得到FCB
        public byte[] getFCB(string root, int start)
        {
            byte[] b_FCB = new byte[8];

            FileStream fs = new FileStream(root, FileMode.Open);
            fs.Seek(start, SeekOrigin.Begin);
            fs.Read(b_FCB, 0, 8);
            fs.Close();

            return b_FCB;
        }

        //写入文件控制块
        public void setFCB(string root, int start, byte[] FCB)
        {
            FileStream fs = new FileStream(root, FileMode.Open);
            fs.Seek(start, SeekOrigin.Begin);
            fs.Write(FCB, 0, 8);
            fs.Close();

        }

        //得到一物理块的内容
        public byte[] getBlcok(string root, int start)
        {
            byte[] b_FCB = new byte[64];

            FileStream fs = new FileStream(root, FileMode.Open);
            fs.Seek(start, SeekOrigin.Begin);
            fs.Read(b_FCB, 0, 64);
            fs.Close();

            return b_FCB;
        }

        //格式化
        public void format(string root)
        {
            FileStream fs = new FileStream(root, FileMode.Open);

            int max = 64 * 128;

            byte[] bytes = new byte[2];
            bytes[0] = 0;
            bytes[1] = 1;

            for (int i = 0; i < 3; i++)
            {
                fs.Write(bytes, 0, 1);
            }
            for (int i = 3; i < max; i++)
            {
                fs.Write(bytes, 1, 1);
            }
            fs.Close();
        }

        //根据路径，得到文件控制块
        public byte[] getFCBbyPath(string path)
        {
            string root = path.Substring(0, 1) + ".vhd";
            string[] fileNames = path.Split('/');

            int count = 1;
            int index = 2;

            byte[] FCB = new byte[8];

            while (count < fileNames.Length)
            {
                byte[] block = getBlcok(root, index * 64);
                int i = 0;
                for (; i < 8; i++)
                {
                    string tempFileName = ((char)block[i * 8]).ToString();
                    tempFileName += ((char)block[i * 8 + 1]).ToString();
                    tempFileName += ((char)block[i * 8 + 2]).ToString();
                    if (tempFileName.Equals(fileNames[count]))
                    {
                        //找到了同名的文件
                        for (int j = 0; j < 8; j++)
                        {
                            FCB[j] = block[8 * i + j];
                        }
                        index = (int)FCB[7];
                        count++;
                        if (count == fileNames.Length)
                        {
                            //找到了末尾
                            return FCB;
                        }
                        //跳出本次循环
                        break;
                    }
                }
                if (i >= 8)
                {
                    //没有找到目录
                    return null;
                }
            }
            return null;
        }

        //在指定的物理块中找到空闲的文件控制块块的id
        public int getFreeFCB(string root, int par)
        {
            byte[] block = getBlcok(root, 64 * par);

            for (int i = 0; i < 8; i++)
            {
                if (block[i * 8] == 1)
                {
                    //找到了，返回i
                    return i;
                }
            }
            //没有找到，返回-1
            return -1;
        }

        //创建文件
        public bool createFile(string root, string fileName, string fileTail, string fileData, int index, int id)
        {

            int count = fileData.Length / 64 + 1;
            int left = fileData.Length % 64;
            byte[] blocks = getFreeBlcok(root, count);
            if (blocks == null)
            {
                //没有找到足够的物理块
                return false;
            }
            string str = fileName + fileTail + "0" + ((char)(fileData.Length % 64)).ToString() + ((char)blocks[0]).ToString();
            //存储文件控制块
            setString(root, index * 64 + 8 * id, str);
            //把文件内容放入物理块
            for (int i = 0; i < blocks.Length; i++)
            {
                if (i == fileData.Length / 64)
                {
                    //最后一个的快
                    setByte(root, blocks[i], 2);
                    setString(root, 64 * blocks[count - 1], fileData.Substring(fileData.Length - left));
                    return true;
                }
                else
                {
                    setByte(root, blocks[i], blocks[i + 1]);
                    setString(root, 64 * blocks[i], fileData, 64 * i, 64);
                }
            }
            return false;
        }

        //创建文件夹
        public bool createDir(string root, string fileName, int index, int id)
        {
            byte[] blocks = getFreeBlcok(root, 1);
            if (blocks == null)
            {
                //没有找到足够的物理块  
                return false;
            }
            //设置标志位
            setByte(root, blocks[0], 0);

            //文件控制块内容
            string str = fileName + "11" + "0" + "0" + ((char)blocks[0]).ToString();
            //存储文件控制块
            setString(root, index * 64 + 8 * id, str);

            return true;
        }

        //找空闲的物理块
        public byte[] getFreeBlcok(string root, int count)
        {
            byte[] blocks = new byte[count];

            byte num = 0;

            for (byte i = 3; i < 128; i++)
            {
                if (getByte(root, i) == 1)
                {
                    blocks[num] = i;
                    num++;
                    if (num == count)
                    {
                        //找了足够的物理块
                        return blocks;
                    }
                }
            }
            return null;
        }

        //删除文件
        public void delFile(string root, int index, int id)
        {
            byte start = getByte(root, index * 64 + id * 8 + 7);
            //去除文件控制块
            fillWithByte(root, index * 64 + id * 8, 1, 8);

            //删除文件的内容
            byte flag = getByte(root, start);
            fillWithByte(root, 64 * start, 1, 64);
            setByte(root, start, 1);//更改物理块标志
            while (flag != 2)
            {

                start = flag;
                flag = getByte(root, start);
                fillWithByte(root, 64 * start, 1, 64);
                setByte(root, start, 1);//更改物理块标志
            }
        }

        //删除文件夹
        public void delDir(string root, int index, int id)
        {
            byte start = getByte(root, 64 * index + 8 * id + 7);
            //Form1.alert("root:" + root + ",index:" + index + ",id:" + id);
            byte[] block = getBlcok(root, start * 64);
            string str="";
            for (int i = 0; i < 64; i++)
            {
                str += (char)block[i] + "";
            }
            for (int i = 0; i < 8; i++)
            {
                if (block[8 * i] != 1)
                {
                    //如果有文件控制块，判断是文件还是文件夹
                    if (block[8 * i + 3] == (byte)'1')
                    {
                        //是文件夹
                        //递归调用删除文件夹函数
                        delDir(root, start, i);
                    }
                    else
                    {
                        //是文件
                        //调用删除文件函数
                        delFile(root, start, i);
                    }

                }
            }
            setByte(root, start, 1);
            fillWithByte(root, index * 64 + id * 8, 1, 8);
            fillWithByte(root, start * 64, 1, 64);
        }

        //读取文件
        public string readFile(string root, byte[] FCB)
        {
            StringBuilder str = new StringBuilder();

            byte index = FCB[7];
            byte flag = getByte(root, index);
            if (flag != 2)
            {
                while (flag != 2)
                {
                    str.Append(getString(root, 64 * index, 64));
                    index = flag;
                    flag = getByte(root, index);
                }
            }
            str.Append(getString(root, index * 64, (int)FCB[6]));

            return str.ToString();
        }

        //得到文件的长度
        public int getFileLength(string root, byte[] FCB)
        {
            int count = 0;

            byte index = FCB[7];
            byte flag = getByte(root, index);
            if (flag != 2)
            {
                while (flag != 2)
                {
                    count++;
                    index = flag;
                    flag = getByte(root, index);
                }
            }
            count++;

            return (count - 1) * 64 + (int)FCB[6];
        }

        //得到文件控制块的起始地址
        public int getStartOfFCB(string path)
        {
            string root = path.Substring(0, 1) + ".vhd";
            string[] fileNames = path.Split('/');

            int count = 1;
            int index = 2;

            byte[] FCB = new byte[8];

            while (count < fileNames.Length)
            {
                byte[] block = getBlcok(root, index * 64);
                int i = 0;
                for (; i < 8; i++)
                {
                    string tempFileName = ((char)block[i * 8]).ToString();
                    tempFileName += ((char)block[i * 8 + 1]).ToString();
                    tempFileName += ((char)block[i * 8 + 2]).ToString();
                    if (tempFileName.Equals(fileNames[count]))
                    {
                        //找到了同名的文件
                        count++;
                        if (count == fileNames.Length)
                        {
                            //找到了末尾
                            return 64 * index + 8 * i;
                        }

                        index = (int)block[8 * i + 7];

                        //跳出本次循环
                        break;
                    }
                }
            }
            return -1;
        }

        //剪切文件（源文件起始FCB地址，目的FCB块号，第几个文件控制块
        public void curFile(string rootS, string rootD, int startOfSource, int indexD, int idD)
        {
            //得到文件控制块
            byte[] FCB = getFCB(rootS, startOfSource);
            //把文件控制块复制到目的目录
            setFCB(rootD, indexD * 64 + 8 * idD, FCB);
            //删除源文件控制块
            fillWithByte(rootS, startOfSource, 1, 8);
        }

        //复制文件（源文件起始FCB地址，目的FCB块号，第几个文件控制块)---不能复制文件夹
        public bool copyFile(string rootS, string rootD, int startOfSouce, int indexD, int idD)
        {
            byte[] sFCB = getFCB(rootS, startOfSouce);

            string fileName = ((char)sFCB[0]).ToString() + ((char)sFCB[1]).ToString() + ((char)sFCB[2]).ToString();

            string fileTail = ((char)sFCB[3]).ToString() + ((char)sFCB[4]).ToString();

            string fileData = readFile(rootS, sFCB);

            return createFile(rootD, fileName, fileTail, fileData, indexD, idD);
        }

        //编辑文件
        public bool changeFile(string root, byte[] FCB, int startOfFCB, string fileName,string fileData)
        {
            int oldFileLength = getFileLength(root, FCB);

            int leftNums = 0;
            for (int i = 3; i < 128;i++ )
            {
                if (getByte(root, i) == 1)
                {
                    leftNums++;
                }
            }

            int oldBlcoks = oldFileLength / 64 + 1;
            int newBlocks = fileData.Length / 64 + 1;

            if (newBlocks > (oldBlcoks + leftNums))
            { 
                //磁盘空间不足
                return false;
            }

            
            //删除原来的文件的内容
            byte start = (byte)FCB[7];
            byte flag = getByte(root, start);
            fillWithByte(root, 64 * start, 1, 64);//填充物理块
            setByte(root, start, 1);//更改物理块标志
            while (flag != 2)
            {
                start = flag;
                flag = getByte(root, start);
                fillWithByte(root, 64 * start, 1, 64);
                setByte(root, start, 1);//更改物理块标志
            }

            //重新申请物理块
            byte[] blocks = getFreeBlcok(root, newBlocks);


            //更改文件控制块的文件
            string fileTail = ((char)FCB[3]).ToString() + ((char)FCB[4]).ToString();
            string str = fileName + fileTail + "0" + ((char)(fileData.Length % 64)).ToString() + ((char)blocks[0]).ToString();
            //存储文件控制块
            setString(root, startOfFCB, str);

            //存储新的文件内容
            //把文件内容放入物理块
            
            //开始存储
            for (int i = 0; i < blocks.Length; i++)
            {
                if (i == fileData.Length / 64)
                {
                    //最后一个的快
                    setByte(root, blocks[i], 2);
                    setString(root, 64 * blocks[i], fileData.Substring(fileData.Length - fileData.Length%64));
                    return true;
                }
                else
                {
                    setByte(root, blocks[i], blocks[i + 1]);
                    setString(root, 64 * blocks[i], fileData, 64 * i, 64);
                }
            }
            return true;
            //return false;
        }
    }
}
