using System;
using System.Collections.Generic;
using System.Text;

namespace OS
{
    class Cmd
    {
        public Form1 form1;
        public string curPath = "c";
        public FileSystem fs = new FileSystem();
        public static Cmd instance;
       
        public Cmd(Form1 form1)
        {
            this.form1 = form1;
            form1.setCurPathLabel("当前命令行路径："+curPath);
            instance = this;
        }

        //判断指令类型，并选择操作
        public void manage(string cmd)
        {

            //转换为绝对路径的命令
            string[] str = getPath(cmd);
           
            if (cmd.StartsWith("read"))
            {
               ////////////////////////////////////////////////////////////////////////// //读文件
                form1.addResult("----------------------");
                form1.addResult("读文件");
                if (str.Length !=2)
                {
                    form1.addResult("命令格式不争确！");
                    return;
                }
                string path = str[1];
                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }

                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }

                if (path.Length == 1)
                {
                    form1.addResult("不能对根目录进行读操作！");
                    return;
                }
                if (!fs.exist(path))
                {
                    form1.addResult("文件不存在！");
                    return;
                }
                if (fs.isDir(path))
                {
                    form1.addResult("不能对目录进行读操作！");
                    return;
                }

                string msg = fs.readFile(path).msg;

                form1.addResult("读取的结果是：");
                form1.addResult(msg);
                form1.addEdit(msg);
            }
            else if (cmd.StartsWith("start"))
            {
                ////////////////////////////////////////////////////////////////////////////////////执行文件
                form1.addResult("----------------------");
                form1.addResult("执行文件");
                if (str.Length != 2)
                {
                    form1.addResult("命令格式不争确！");
                    return;
                }
                string path = str[1];

                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }
                if (fs.isRoot(path))
                {
                    form1.addResult("不能执行硬盘！");
                    return;
                }
                if (!fs.exist(path))
                {
                    form1.addResult("文件不存在！");
                    return;
                }
                if (!fs.isEx(path))
                {
                    form1.addResult("选择的不是可执行文件！");
                    return;
                }

                string fileName = path.Substring(path.Length - 3, 3);
                Message msg = fs.readFile(path);
                if (msg.suc)
                {
                    PosInNC pos = new PosInNC();
                    if (NC.askForNC(msg.msg, pos))
                    {
                        string tempResult = form1.addPCB(pos.start, pos.end, fileName);
                        form1.addResult(tempResult);
                    }
                    else
                    {
                        form1.addResult("内存不足，不能建立进程！");
                    }

                }
            }
            else if (cmd.StartsWith("addFile"))
            {
                //////////////////////////////////////////////////////////////////////////////添加文件
                form1.addResult("----------------------");
                form1.addResult("添加文件");
                if (str.Length < 5)
                {
                    form1.addResult("输入指令格式不正确！");
                    return;
                }

                string path = str[1];
                string fileName = str[2];
                string fileTail = str[3];
                string fileData=str[4];
                for (int i = 5; i < str.Length; i++)
                {
                    fileData += " " + str[i];
                }

                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }
                if (fileName.Length != 3)
                {
                    form1.addResult("文件名必须为3!");
                    return;
                }
                if (fileName.IndexOf("/") > 0)
                {
                    form1.addResult("文件名不能包含“/”！");
                    return;
                }
                if (!fileTail.Equals("ex") && !fileTail.Equals("tx"))
                {
                    form1.addResult("文件名后缀名不正确！");
                }
                Message msg=fs.createFile(path, fileName, fileTail, fileData);
                if (msg.suc)
                {
                    form1.addResult("添加文件完毕！");
                    Form1.Instance.initNode();
                }
                else
                {
                    form1.addResult("添加文件失败：" + msg.msg);
                }
            }
            else if (cmd.StartsWith("addDir"))
            {
                //////////////////////////////////////////////////////////////////////////////添加文件夹
                form1.addResult("----------------------");
                form1.addResult("添加文件夹");
                if (str.Length !=3)
                {
                    form1.addResult("输入指令格式不正确！");
                    return;
                }

                string path = str[1];
                string fileName = str[2];

                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }
                if (fileName.Length != 3)
                {
                    form1.addResult("文件夹名必须为3!");
                    return;
                }
                if (fileName.IndexOf("/") > 0)
                {
                    form1.addResult("文件夹名不能包含“/”！");
                    return;
                }
                Message msg = fs.createDir(path, fileName);
                if (msg.suc)
                {
                    form1.addResult("添加文件夹完毕！");
                    Form1.Instance.initNode();
                }
                else
                {
                    form1.addResult("添加文件夹失败：" + msg.msg);
                }
            }
            else if (cmd.StartsWith("delFile"))
            {
                //////////////////////////////////////////////////////////////////////////////删除文件
                form1.addResult("----------------------");
                form1.addResult("删除文件/文件夹");
                if (str.Length != 2)
                {
                    form1.addResult("输入指令格式不正确！");
                    return;
                }

                string path = str[1];

                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }

                if (fs.isRoot(path))
                {
                    form1.addResult("不能删除根目录！");
                    return;
                }

                if (!fs.exist(path))
                {
                    form1.addResult("文件不存在！");
                    return;
                }

                Message msg=fs.delFile(path);
                if (msg.suc)
                {
                    form1.addResult("删除完毕！");
                    form1.initNode();
                }
                else
                {
                    form1.addResult("删除失败：" + msg.msg);
                }

            }
            else if (cmd.StartsWith("delDir"))
            {
                //////////////////////////////////////////////////////////////////////////////删除文件
                form1.addResult("----------------------");
                form1.addResult("删除文件/文件夹");
                if (str.Length != 2)
                {
                    form1.addResult("输入指令格式不正确！");
                    return;
                }

                string path = str[1];

                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }

                if (fs.isRoot(path))
                {
                    form1.addResult("不能删除根目录！");
                    return;
                }

                if (!fs.exist(path))
                {
                    form1.addResult("文件夹不存在！");
                    return;
                }

                Message msg = fs.delDir(path);
                if (msg.suc)
                {
                    form1.addResult("删除完毕！");
                    form1.initNode();
                }
                else
                {
                    form1.addResult("删除失败：" + msg.msg);
                }
            }
            else if (cmd.StartsWith("getProperty"))
            {
                //////////////////////////////////////////////////////////////////////////////得到文件属性
                form1.addResult("----------------------");
                form1.addResult("查看属性");
                if (str.Length != 2)
                {
                    form1.addResult("输入指令格式不正确！");
                    return;
                }
                string path = str[1];
                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }
                if (fs.isRoot(path))
                {
                    form1.addResult("你难道不知道磁盘的只读属性吗？");
                    return;
                }
                string p = fs.getFileProperty(path).msg;
                form1.addResult("只读：" + p);
            }
            else if (cmd.StartsWith("changeProperty"))
            {
                //////////////////////////////////////////////////////////////////////////// //改变文件属性
                form1.addResult("----------------------");
                form1.addResult("更改属性");
                if (str.Length != 2)
                {
                    form1.addResult("输入指令格式不正确！");
                    return;
                }
                string path = str[1];
                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }
                if (fs.isRoot(path))
                {
                    form1.addResult("不能改变磁盘的只读属性！");
                    return;
                }
                string p = fs.getFileProperty(path).msg;

                bool readOnly;

                if (p.Equals("是"))
                {
                    readOnly = false;
                }
                else
                {
                    readOnly = true;
                }

                Message msg = fs.setFileProperty(path, readOnly);
                if (msg.suc)
                {
                    form1.addResult("更改完毕！");
                }
                else
                {
                    form1.addResult("更改失败：" + msg.msg);
                }
            }
            else if (cmd.StartsWith("cd"))
            {
                //////////////////////////////////////////////////////////////////////////////进入路径
                form1.addResult("----------------------");
                form1.addResult("进入路径");
                if (str.Length != 2)
                {
                    form1.addResult("命令格式不正确！");
                    return;
                }
                string path = str[1];
                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }
                if (!(str[1].Length == 1))
                {
                    //如果不是根目录
                    if (!fs.exist(str[1]))
                    {
                        form1.addResult("文件夹不存在！");
                        return;
                    }
                    if (!fs.isDir(path))
                    {
                        form1.addResult("选择的不是文件夹！");
                        return;
                    }
                }

                curPath = path;
                form1.addResult("当前命令行路径已经更改！");
                form1.setCurPathLabel("当前命令行路径：" + curPath);
            }
            else if (cmd.StartsWith("listFiles"))
            {
                //////////////////////////////////////////////////////////////////////////////显示目录文件
                form1.addResult("----------------------");
                form1.addResult("显示目录下的文件");
                if (str.Length != 2)
                {
                    form1.addResult("命令格式不正确！");
                    return;
                }
                string path = str[1];
                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }
                if (!(str[1].Length == 1))
                {
                    //如果不是根目录
                    if (!fs.exist(str[1]))
                    {
                        form1.addResult("文件夹不存在！");
                        return;
                    }
                    if (!fs.isDir(path))
                    {
                        form1.addResult("选择的不是文件夹！");
                        return;
                    }
                }

                string[] files = fs.getFileList(str[1]);

                bool added = false;
                form1.addResult("显示目录下的文件：");
                for (int i = 0; i < 8; i++)
                {
                    if (files[i] != null)
                    {
                        form1.addResult(files[i]);
                        added = true;
                    }
                }
                if (!added)
                {
                    form1.addResult("此目录下没有文件！");
                }
            }
            else if (cmd.StartsWith("cut"))
            {
                //////////////////////////////////////////////////////////////////////////////剪切文件
                form1.addResult("----------------------");
                form1.addResult("剪切文件/文件夹");
                if (str.Length != 3)
                {
                    form1.addResult("命令格式不正确！");
                    return;
                }
                string pathS = str[1];
                string pathD = str[2];
                if (pathD.StartsWith("currentPath"))
                { 
                    //是相对路径
                    pathD = curPath + pathD.Substring("currentPath".Length);
                }
                if (!checkPath(pathS) || !checkPath(pathD))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }
                if (fs.isRoot(pathS))
                {
                    form1.addResult("不能剪切磁盘！");
                    return;
                }
                if (!fs.exist(pathS))
                {
                    form1.addResult("剪切来源路径不存在！");
                    return;
                }
                if (!fs.isRoot(pathD) && !fs.exist(pathD))
                {
                    form1.addResult("剪切目的路径不存在！");
                    return;
                }
                if (!fs.isRoot(pathD) && !fs.isDir(pathD))
                {
                    form1.addResult("剪切目的路径不是文件夹！");
                    return;
                }

                Message msg = fs.cutFile(pathS, pathD);
                if (msg.suc)
                {
                    form1.addResult("剪切完毕！");
                    form1.initNode();
                    return;
                }
                else
                {
                    form1.addResult("剪切失败：" + msg.msg);
                }
            }
            else if (cmd.StartsWith("copy"))
            {
                //////////////////////////////////////////////////////////////////////////////复制文件
                form1.addResult("----------------------");
                form1.addResult("复制文件");
                if (str.Length != 3)
                {
                    form1.addResult("命令格式不正确！");
                    return;
                }
                string pathS = str[1];
                string pathD = str[2];

                if (pathD.StartsWith("currentPath"))
                {
                    //是相对路径
                    pathD = curPath + pathD.Substring("currentPath".Length);
                }

                if (!checkPath(pathS) || !checkPath(pathD))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }
                if (fs.isRoot(pathS))
                {
                    form1.addResult("只能复制文件，不能复制磁盘！");
                    return;
                }
                if (!fs.exist(pathS))
                {
                    form1.addResult("复制来源路径不存在！");
                    return;
                }
                if (fs.isDir(pathS))
                {
                    form1.addResult("只能复制文件，不能复制文件夹！");
                    return;
                }
                if (!fs.isRoot(pathD) && !fs.exist(pathD))
                {
                    form1.addResult("复制目的路径不存在！");
                    return;
                }
                if (!fs.isRoot(pathD) && !fs.isDir(pathD))
                {
                    form1.addResult("复制目的路径不是文件夹！");
                    return;
                }
                Message msg = fs.copyFile(pathS, pathD);
                if (msg.suc)
                {
                    form1.addResult("复制完毕！");
                    form1.initNode();
                    return;
                }
                else
                {
                    form1.addResult("复制失败：" + msg.msg);
                }
            }
            else if (cmd.StartsWith("edit"))
            {
                //////////////////////////////////////////////////////////////////////////////编辑文件
                form1.addResult("----------------------");
                form1.addResult("编辑文件");
                if (str.Length != 2)
                {
                    form1.addResult("命令格式不正确！");
                    return;
                }
                string path = str[1];

                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }

                if (path.Length == 1)
                {
                    form1.addResult("不能编辑根目录！");
                    return;
                }
                if (!fs.exist(path))
                {
                    form1.addResult("文件不存在！");
                    return;
                }
                if (fs.isDir(path))
                {
                    form1.addResult("不能对目录进行编辑！");
                    return;
                }
                if (form1.OperateFilePath != null && path.Equals(form1.OperateFilePath))
                {
                    form1.addResult("不能对复制或粘接的来源进行编辑！");
                    return;
                }

                string msg = fs.readFile(path).msg;
                form1.addEdit(msg);

            }
            else if (cmd.StartsWith("editFile"))
            {
                //////////////////////////////////////////////////////////////////////////////编辑文件
                form1.addResult("----------------------");
                form1.addResult("编辑文件");
                if (str.Length != 4)
                {
                    form1.addResult("命令格式不正确！");
                    return;
                }
                string path = str[1];
                string newFileName = str[2];
                string newFileData = str[3];

                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }

                if (path.Length == 1)
                {
                    form1.addResult("不能编辑根目录！");
                    return;
                }
                if (!fs.exist(path))
                {
                    form1.addResult("文件不存在！");
                    return;
                }
                if (fs.isDir(path))
                {
                    form1.addResult("不能对目录进行编辑！");
                    return;
                }
                if (form1.OperateFilePath != null && path.Equals(form1.OperateFilePath))
                {
                    form1.addResult("不能对复制或粘接的来源进行编辑！");
                    return;
                }
                if (!(newFileName.Length == 3))
                {
                    form1.addResult("文件名长度必须为3！");
                    return;
                }
                if (newFileName.IndexOf("/") >= 0)
                {
                    form1.addResult("文件名不能包含“/”！");
                    return;
                }
                Message msg = fs.editFile(path, newFileName, newFileData, true);

                if (msg.suc)
                {
                    form1.addResult("编辑完毕！");
                    form1.initNode();
                }
                else
                {
                    form1.addResult("编辑失败：" + msg.msg);
                }
            }
            else if (cmd.StartsWith("editDir"))
            {
                //////////////////////////////////////////////////////////////////////////////编辑文件夹
                form1.addResult("----------------------");
                form1.addResult("编辑文件夹");
                if (str.Length != 3)
                {
                    form1.addResult("命令格式不争确！");
                    return;
                }
                string path = str[1];
                string newFileName = str[2];

                if (!checkPath(path))
                {
                    form1.addResult("地址格式不正确！");
                    return;
                }

                Message msg = fs.editDir(path, newFileName);

                if (msg.suc)
                {
                    form1.addResult("更改完毕！");
                    form1.initNode();
                }
                else
                {
                    form1.addResult("更改失败：" + msg.msg);
                }
            }
            else if (cmd.StartsWith("format"))
            {
                //////////////////////////////////////////////////////////////////////////////格式化
                form1.addResult("----------------------");
                form1.addResult("格式化");
                if (str.Length != 2)
                {
                    form1.addResult("命令格式不争确！");
                    return;
                }
                string path = str[1];
                if (path.Equals("c"))
                {
                    fs.init("c.vhd");
                    form1.addResult("格式化完毕！");
                    form1.initNode();
                }
                else if (path.Equals("d"))
                {
                    fs.init("d.vhd");
                    form1.addResult("格式化完毕！");
                    form1.initNode();
                }
                else
                {
                    form1.addResult("输入错误！");
                }
            }
            else
            {
                //////////////////////////////////////////////////////////////////////////////输入错误
                form1.addResult("输入指令错误！");
            }
        }

        public void changeFile(string cmd,string newFileData)
        {
            string[] str = getPath(cmd);
            Message msg = fs.editFile(str[1], str[1].Substring(str[1].Length - 3, 3), newFileData, false);
        }

        //把命令改变为绝对路径的命令
        public string[] getPath(string cmd)
        {
            string[] str = cmd.Split(' ');//以空格分割字符串
            if (str.Length > 1 && str[1].StartsWith("currentPath"))
            {
                if (str[1].Equals("currentPath"))
                {
                    str[1] = curPath;
                }
                else
                {
                    str[1] = curPath + str[1].Substring("currentPath".Length);
                }
            }
            return str;
        }

        public void start(string cmd,string msg1)
        {
            Message msg=new Message(true,msg1);
            string[] str = getPath(cmd);
            if (msg.suc)
            {
                PosInNC pos = new PosInNC();
                if (NC.askForNC(msg.msg, pos))
                {
                    string tempResult = form1.addPCB(pos.start, pos.end, str[1].Substring(str[1].Length - 3, 3));
                    form1.addResult(tempResult);
                }
                else
                {
                    form1.addResult("内存不足，不能建立进程！");
                }

            }
        }

        //粗略的检查输入的地址是否正确
        public bool checkPath(string path)
        {
            if (path.StartsWith("c") || path.StartsWith("d"))
            {
                return true;
            }
            return false;
        }

    }
}
