using System;
using System.Collections.Generic;
using System.Text;

namespace OS
{
    class FileSystem
    {
        Disk disk = new Disk();

        public FileSystem()
        {
        }

        //格式化硬盘(c.vhd,d.vhd)
        public void init(string root)
        {
            lock (this)
            {
                disk.format(root);
            }
        }

        //创建文件
        public Message createFile(string path, string fileName, string fileTail, string fileData)
        {
            lock (this)
            {
                if (exist(path + "/" + fileName))
                {
                    //文件已经存在
                    return new Message(false, "同名文件已经存在！");
                }

                int index = 0;//目录的块号

                if (path.Equals("c") || path.Equals("d"))
                {
                    index = 2;
                }
                else
                {
                    byte[] FCB = disk.getFCBbyPath(path);
                    if (FCB == null)
                    {
                        return new Message(false, "目录不存在！");
                    }
                    if (!isDir(path))
                    {
                        //所选路径不是文件夹
                        return new Message(false, "选择的路径不是文件夹！");
                    }
                    index = (int)FCB[7];
                }

                string root = path.Substring(0, 1) + ".vhd";

                int id = disk.getFreeFCB(root, index);
                if (id < 0)
                {
                    //没有文件控制块了
                    return new Message(false, "目录文件数已达最大！");
                }


                if (!disk.createFile(root, fileName, fileTail, fileData, index, id))
                {
                    return new Message(false, "文件创建失败！");
                }
                return new Message(true, "添加文件完毕！");
            }
        }

        //创建文件夹
        public Message createDir(string path, string dirName)
        {
            lock (this)
            {
                if (exist(path + "/" + dirName))
                {
                    //文件夹已经存在
                    return new Message(false, "同名文件夹已经存在！");
                }

                int index = 0;//目录的块号

                if (path.Equals("c") || path.Equals("d"))
                {
                    index = 2;
                }
                else
                {
                    byte[] FCB = disk.getFCBbyPath(path);
                    if (FCB == null)
                    {
                        return new Message(false, "目录不存在！");
                    }
                    index = (int)FCB[7];
                }
                string root = path.Substring(0, 1) + ".vhd";

                //id是一个块中的第几个文件控制块
                int id = disk.getFreeFCB(root, index);
                if (id < 0)
                {
                    //没有文件控制块了
                    return new Message(false, "目录文件数已达最大！");
                }

                if (!disk.createDir(root, dirName, index, id))
                {
                    return new Message(false, "文件创建失败(磁盘原因)！");
                }
                return new Message(true, "添加文件夹完毕！");
            }
        }

        //读取文件
        public Message readFile(string filePath)
        {
            lock (this)
            {
                if (isRoot(filePath))
                {
                    return new Message(false, "不可对磁盘进行读操作！");
                }
                byte[] FCB = disk.getFCBbyPath(filePath);
                if (FCB == null)
                {
                    //文件不存在
                    return new Message(false, "文件不存在！");
                }
                if (FCB[3] == Convert.ToByte('1'))
                {
                    //是文件夹
                    return new Message(false, "试图读取的是文件夹！");
                }
                string root = filePath.Substring(0, 1) + ".vhd";
                string fileData = disk.readFile(root, FCB);
                return new Message(true, fileData);
            }
        }

        //删除文件
        public Message delFile(string filePath)
        {
            lock (this)
            {
                byte[] FCB = disk.getFCBbyPath(filePath);
                if (FCB == null)
                {
                    //文件不存在
                    return new Message(false, "文件不存在！");
                }

                int start = disk.getStartOfFCB(filePath);

                byte index = (byte)(start / 64);
                byte id = (byte)((start - 64 * index) / 8);
                string root = filePath.Substring(0, 1) + ".vhd";
                disk.delFile(root, index, id);

                return new Message(true, "删除完毕！");
            }
        }

        //删除文件夹
        public Message delDir(string filePath)
        {
            lock (this)
            {
                byte[] FCB = disk.getFCBbyPath(filePath);
                if (FCB == null)
                {
                    //文件不存在
                    return new Message(false, "文件夹不存在！");
                }

                int start = disk.getStartOfFCB(filePath);

                byte index = (byte)(start / 64);
                byte id = (byte)((start - 64 * index) / 8);
                string root = filePath.Substring(0, 1) + ".vhd";
                disk.delDir(root, index, id);

                return new Message(true, "删除完毕！");
            }
        }

        //剪切文件
        public Message cutFile(string sFilePath, string dFilePath)
        {
            lock (this)
            {
                if (sFilePath.StartsWith("d") && !dFilePath.StartsWith("d") || sFilePath.StartsWith("c") && !dFilePath.StartsWith("c"))
                {
                    if (isRoot(sFilePath))
                    {
                        return new Message(false, "不能剪切根目录！");
                    }
                    if (!exist(sFilePath))
                    {
                        return new Message(false, "剪切源文件不存在！");
                    }
                    if (isDir(sFilePath))
                    {
                        return new Message(false, "不能向另一个磁盘剪贴文件夹！");
                    }
                    if (!isRoot(dFilePath))
                    {

                        if (!exist(dFilePath))
                        {
                            return new Message(false, "目的目录不存在！");
                        }
                        if (!isDir(dFilePath))
                        {
                            return new Message(false, "剪切目的路径必须是文件夹！");
                        }
                    }
                    Message msg = copyFile(sFilePath, dFilePath);
                    if (!msg.suc)
                    {
                        return new Message(false, msg.msg);
                    }
                    msg = delFile(sFilePath);
                    if (!msg.suc)
                    {
                        return new Message(false, msg.msg);
                    }
                    return new Message(true, "操作完毕！");
                }
                if (dFilePath.IndexOf(sFilePath) >= 0)
                {
                    return new Message(false, "不能向自己的子目录剪切文件！");
                }
                int indexD = 0;//目录的块号

                if (dFilePath.Equals("c") || dFilePath.Equals("d"))
                {
                    indexD = 2;
                }
                else
                {
                    byte[] FCB = disk.getFCBbyPath(dFilePath);
                    if (FCB == null)
                    {
                        return new Message(false, "目的目录不存在！");
                    }
                    indexD = (int)FCB[7];
                }

                string sFileName = sFilePath.Substring(sFilePath.Length - 3, 3);
                if (exist(dFilePath + "/" + sFileName))
                {
                    //目的文件夹下已经有了同名文件
                    return new Message(false, "目的文件夹下已经有了同名文件");
                }


                string root = dFilePath.Substring(0, 1) + ".vhd";

                //id是一个块中的第几个文件控制块
                int idD = disk.getFreeFCB(root, indexD);
                if (idD < 0)
                {
                    //没有文件控制块了
                    return new Message(false, "目的目录文件数已达最大！");
                }

                byte[] FCB2 = disk.getFCBbyPath(sFilePath);
                if (FCB2 == null)
                {
                    //文件不存在
                    return new Message(false, "源文件或文件夹不存在！");
                }
                int startOfFCB = disk.getStartOfFCB(sFilePath);
                string rootS = sFilePath.Substring(0, 1) + ".vhd";
                string rootD = dFilePath.Substring(0, 1) + ".vhd";


                disk.curFile(rootS, rootD, startOfFCB, indexD, idD);


                return new Message(true, "剪切完毕！");
            }
        }

        //复制文件---不能复制文件夹
        public Message copyFile(string sFilePath, string dFilePath)
        {
            lock (this)
            {
                int indexD = 0;//目录的块号
                if (dFilePath.Equals("c") || dFilePath.Equals("d"))
                {
                    indexD = 2;
                }
                else
                {
                    byte[] FCB = disk.getFCBbyPath(dFilePath);
                    if (FCB == null)
                    {
                        return new Message(false, "目的目录不存在！");
                    }
                    indexD = (int)FCB[7];
                }

                string sFileName = sFilePath.Substring(sFilePath.Length - 3, 3);
                if (exist(dFilePath + "/" + sFileName))
                {
                    //目的文件夹下已经有了同名文件
                    return new Message(false, "目的文件夹下已经有了同名文件");
                }

                string root = dFilePath.Substring(0, 1) + ".vhd";

                //id是一个块中的第几个文件控制块
                int idD = disk.getFreeFCB(root, indexD);
                if (idD < 0)
                {
                    //没有文件控制块了
                    return new Message(false, "目的目录文件数已达最大！");
                }

                byte[] FCB2 = disk.getFCBbyPath(sFilePath);
                if (FCB2 == null)
                {
                    //文件不存在
                    return new Message(false, "源文件或文件夹不存在！");
                }
                int startOfFCB = disk.getStartOfFCB(sFilePath);
                string rootS = sFilePath.Substring(0, 1) + ".vhd";
                string rootD = dFilePath.Substring(0, 1) + ".vhd";


                bool suc = disk.copyFile(rootS, rootD, startOfFCB, indexD, idD);

                if (suc)
                {
                    return new Message(true, "复制完毕！");
                }
                else
                {
                    return new Message(false, "空间不足复制失败！");
                }
            }
        }

        //编辑文件
        public Message editFile(string filePath, string fileName,string fileData,bool fileNameChanged)
        {
            lock (this)
            {
                byte[] FCB = disk.getFCBbyPath(filePath);
                if (FCB == null)
                {
                    //文件不存在
                    return new Message(false, "文件不存在！");
                }
                if (FCB[3] == Convert.ToByte('1'))
                {
                    //是文件夹
                    return new Message(false, "试图操作的是文件夹！");
                }
                if (FCB[5] == (byte)'1')
                {
                    //只读文件
                    return new Message(false, "文件是只读文件，不允许编辑！");
                }

                if (fileNameChanged)
                {
                    //文件改名了
                    string tempFileName = filePath.Substring(0, filePath.Length - 3) + fileName;
                    if (exist(tempFileName))
                    {
                        //文件改名后的名字与该目录下的某个文件同名
                        return new Message(false, "该目录下已经存在同名文件或文件夹");
                    }
                }

                string root = filePath.Substring(0, 1) + ".vhd";
                int startOfFCB = disk.getStartOfFCB(filePath);


                bool suc = disk.changeFile(root, FCB, startOfFCB, fileName, fileData);

                if (suc)
                {
                    return new Message(true, "编辑完毕！");
                }
                else
                {
                    return new Message(false, "空间不足！");
                }
            }
        }

        //得到文件的属性
        public Message getFileProperty(string filePath)
        {
            lock (this)
            {
                byte[] FCB = disk.getFCBbyPath(filePath);
                if (FCB == null)
                {
                    //文件不存在
                    return new Message(false, "文件不存在！");
                }
                string readOnly = FCB[5] == (byte)'1' ? "是" : "否";

                return new Message(true, readOnly);
            }
        }

        //改变文件的只读属性
        public Message setFileProperty(string filePath, bool readOnly)
        {
            lock (this)
            {
                byte[] FCB = disk.getFCBbyPath(filePath);
                if (FCB == null)
                {
                    //文件不存在
                    return new Message(false, "文件不存在！");
                }
                byte tempByte;
                if (readOnly)
                {
                    tempByte = (byte)'1';
                }
                else
                {
                    tempByte = (byte)'0';
                }
                int startOfFCB = disk.getStartOfFCB(filePath);
                string root = filePath.Substring(0, 1) + ".vhd";


                disk.setByte(root, startOfFCB + 5, tempByte);

                return new Message(true, "更改属性完毕！");
            }
        }

        //显示目录的文件（在文件夹存在的情况下)
        public string[] getFileList(string filePath)
        {
            lock (this)
            {
                string[] files = new string[8];
                int count = 0;

                int index = 0;
                if (filePath.Equals("c") || filePath.Equals("d") || filePath.Equals("e"))
                {
                    index = 2;
                }
                else
                {
                    byte[] FCB = disk.getFCBbyPath(filePath);
                    index = (int)FCB[7];
                }

                string root = filePath.Substring(0, 1) + ".vhd";
                byte[] block = disk.getBlcok(root, index * 64);
                for (int i = 0; i < 8; i++)
                {
                    if (block[8 * i] != 1)
                    {
                        //有文件控制块
                        //读取文件名
                        string fileName = ((char)block[8 * i]).ToString() + ((char)block[8 * i + 1]).ToString() + ((char)block[8 * i + 2]).ToString();
                        //判断文件类型
                        if (block[8 * i + 3] == (byte)'t')
                        {
                            //文本文件   
                            fileName += "." + "tx";
                        }
                        else if (block[8 * i + 3] == (byte)'e')
                        {
                            //可执行文件
                            fileName += "." + "ex";
                        }
                        else
                        {

                        }
                        files[count] = fileName;
                        count++;
                    }
                }
                return files;
            }
        }

        //判断文件是否存在
        public bool exist(string filePath)
        {
            lock (this)
            {
                byte[] FCB = disk.getFCBbyPath(filePath);
                if (FCB == null)
                {
                    //文件不存在
                    return false;
                }
                return true;
            }
        }

        //判断是否是文件夹
        public bool isDir(string filePath)
        {
            lock (this)
            {
                byte[] FCB = disk.getFCBbyPath(filePath);
                if (FCB[3] == Convert.ToByte('1'))
                {
                    //是文件夹
                    return true;
                }
                return false;
            }
        }

        //判断是否是文本文件
        public bool isTx(string filePath)
        {
            lock (this)
            {
                byte[] FCB = disk.getFCBbyPath(filePath);
                if (FCB[3] == Convert.ToByte('t'))
                {
                    //是文件夹
                    return true;
                }
                return false;
            }
        }

        //判断是否是可执行文件
        public bool isEx(string filePath)
        {
            lock (this)
            {
                byte[] FCB = disk.getFCBbyPath(filePath);
                if (FCB[3] == Convert.ToByte('e'))
                {
                    //是文件夹
                    return true;
                }
                return false;
            }
        }

        //判断路径是否是根目录
        public bool isRoot(string path)
        {
            lock (this)
            {
                if (path.Length == 1)
                {
                    return true;
                }
                return false;
            }
        }

        //得到文件名
        public string getFileName(string filePath)
        {
            lock (this)
            {
                byte[] FCB = disk.getFCBbyPath(filePath);

                string fileName = ((char)FCB[0]).ToString() + ((char)FCB[1]).ToString() + ((char)FCB[2]).ToString();

                return fileName;
            }
        }

        //编辑目录（给目录改名）
        public Message editDir(string path, string dirName)
        {
            lock (this)
            {
                if (!(dirName.Length == 3))
                {
                    return new Message(false, "文件夹名长度必须为3！");
                }
                if (dirName.IndexOf("/") >= 0)
                {
                    return new Message(false, "文件夹名不能包含“/”！");
                }

                if (isRoot(path))
                {
                    return new Message(false, "不能给磁盘改名！");
                }
                if (!exist(path))
                {
                    return new Message(false, "文件夹不存在！");
                }
                string tempPath = path.Substring(0, path.Length - 3) + dirName;
                if (exist(tempPath))
                {
                    return new Message(false, "该目录下已经存在同名的文件了！");
                }
                string root = path.Substring(0, 1) + ".vhd";
                int index = disk.getStartOfFCB(path);
                if (disk.getByte(root, index + 5) == (byte)'1')
                {
                    //文件夹只读
                    return new Message(false, "文件夹只读，不能改名！");
                }

                disk.setString(root, index, dirName);

                return new Message(true, "更改完毕！");
            }
        }
    }
}
