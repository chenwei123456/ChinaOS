using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace OS
{
    public partial class Form1 : Form
    {
        Control control;
        Cmd cmd;
        FileSystem fs;

        Label[,] ncLabels=new Label[20,25];
        Label[,] diskCLabels = new Label[16, 8];
        Label[,] diskDLabels = new Label[16, 8];

        public static Form1 Instance;

        public static int NONE = -1;
        public static int COPY = 0;
        public static int CUT = 1;

        public string OperateFilePath = null;

        public int CurState = NONE;

        public Form1()
        {
            InitializeComponent();
            this.groupBox1.Enabled = false;
            this.groupBox2.Enabled = false;
            this.groupBoxRam.Enabled = false;
            this.diskFrame.Enabled = false;
            for (int i = 0; i < 500; i++)
            {
                panel[i] = new Panel();
                this.groupBoxRam.Controls.Add(panel[i]);
                panel[i].BackColor = Color.Silver;
                panel[i].BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                panel[i].Size = new Size(10, 10);
                panel[i].Name = "panel" + Convert.ToString(i + 4);
                panel[i].Show();
            }
            int m = 0;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    panel[m].Location = new System.Drawing.Point(20+10 * j, 60 + 11 * i);
                    m++;
                }
            }
            CheckForIllegalCrossThreadCalls = false;
            Instance = this;
            cmd = new Cmd(this);
            fs = cmd.fs;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label7.Text = "时间片长度："+totalTime.Value.ToString()+"秒";
            setTotalTime(Convert.ToInt32(totalTime.Value));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            control = new Control(this);
            int x;
            int y =10;
            //添加内存块
            for (int i = 0; i < 20; i++)
            {
                x =10;
                for (int j = 0; j < 25; j++)
                {
                    Label label = new Label();
                    label.BorderStyle = BorderStyle.FixedSingle;
                    label.Width = 8;
                    label.Height = 8;
                    label.Left = x;
                    label.Top = y;
                    label.BackColor = Color.Silver;
                    //ncFrame.Controls.Add(label);
                    ncLabels[i, j] = label;
                    x += 7;
                }
                y += 7;
            }
            //SheBeiA.Image = imageList1.Images[0];
            //SheBeiB.Image = imageList1.Images[1];
            //SheBeiC.Image = imageList1.Images[2];
            
            //添加硬盘快
            y = 15;
            for (int i = 0; i < 16; i++)
            {
                x = 10;
                for (int j = 0; j < 8; j++)
                {
                    Label label = new Label();
                    label.BorderStyle = BorderStyle.FixedSingle;
                    label.Width = 8;
                    label.Height =8;
                    label.Left = x;
                    label.Top = y;
                    //diskFrame.Controls.Add(label);
                    //diskCLabels[i, j] = label;

                    label = new Label();
                    label.BorderStyle = BorderStyle.FixedSingle;
                    label.Width = 8;
                    label.Height = 8;
                    label.Left = x+70;
                    label.Top = y;
                    //diskFrame.Controls.Add(label);
                    //diskDLabels[i, j] = label;

                    x += 7;
                }
                y += 7;
            }

            initNode();
        }

        //初始化文件树根结点
        public void initNode()
        {
            //initYP();

            fileTree.Nodes.Clear();

            TreeNode C = new TreeNode("c");
            C.ImageIndex = 0;
            fileTree.Nodes.Add(C);
            addChildNodes("c", C);

            TreeNode D = new TreeNode("d");
            D.ImageIndex = 0;
            fileTree.Nodes.Add(D);
            addChildNodes("d", D);
        }

        //初始化硬盘块
        public void initYP()
        {
            byte[] c = new Disk().getBytes("c.vhd", 0, 128);
            byte[] d = new Disk().getBytes("d.vhd", 0, 128);

            int x = 0, y = 0;
            for (int i = 0; i < 128; i++)
            {
                x = i / 8;
                y = i % 8;

                if (c[i] != 1)
                {
                    diskCLabels[x, y].BackColor = Color.Red;
                }
                else
                {
                    diskCLabels[x, y].BackColor = Color.Silver;
                }
                if (d[i] != 1)
                {
                    diskDLabels[x, y].BackColor = Color.Red;
                }
                else
                {
                    diskDLabels[x, y].BackColor = Color.Silver;
                }

            }
        }

        //添加文件树的结点
        public void addChildNodes(string path, TreeNode parNode)
        {
            string[] fileList = fs.getFileList(path);
            for (int i = 0; i < 8; i++)
            {
                if (fileList[i] != null)
                {
                    TreeNode tempNode = new TreeNode();
                    string fileName=fileList[i].Split('.')[0];
                    tempNode.Text = fileName;
                    parNode.Nodes.Add(tempNode);

                    if (fs.isEx(path + "/"+fileName))
                    {
                        tempNode.ImageIndex = 2;
                    }
                    else if (fs.isTx(path + "/" + fileName))
                    {
                        tempNode.ImageIndex = 3;
                    }
                    else if (fs.isDir(path + "/" + fileName))
                    {
                        tempNode.ImageIndex = 1;
                        addChildNodes(path + "/" + fileName, tempNode);
                    }
                }
            }
        }



        //设置当前就绪队列
        public void setReadayList(string[] list)
        {
            readayList.Items.Clear();
            for (int i = 1; i < list.Length; i++)
            {
                readayList.Items.Add(list[i]);
            }
            if(list.Length>0)
                setCurPCB(list[0]);
        }

        //设置阻塞队列
        public void setZcList(string[] list)
        {
            zsList.Items.Clear();
            for (int i = 0; i < list.Length; i++)
            {
                zsList.Items.Add(list[i]);
            }
        }

        //设置执行完毕队列
        public void setDoneList(string[] list)
        {
            doneList.Items.Clear();
            for (int i = 0; i < list.Length; i++)
            {
                doneList.Items.Add(list[i]);
            }
        }

        //设置当前进程
        public void setCurPCB(string str)
        {
            curPCB.Text = str;
        }

        //设置当前的指令
        public void setCurIns(string str)
        {
            curIns.Text = str;
        }

        //设置当前的结果
        public void setCurResult(string str)
        {
            curResult.Text = str;
        }

        

        //设置control的totaltime
        public void setTotalTime(int data)
        {
            Control.setTotalTime(data);
        }

        //设置进度条的范围
        public void setProc(int min, int max)
        {
            if (min < 0)
                min = 0;
            this.curTime.Minimum = min;
            this.curTime.Maximum = max;
        }

        //设置进度条的值
        public void setProcValue(float value)
        {
            if(value<0)
                value=0;
            this.curTime.Value = (int)value;
        }
        private bool tool = true;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tool == true)
            {
                if (MessageBox.Show("确定要关机吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    tool = false;
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        public static void alert(String msg)
        {
            MessageBox.Show(msg);
        }

        public void busyNC(int start,int end)
        {
            for (int i = start; i <= end; i++)
            {
                panel[i].BackColor = Color.Red;
                //ncLabels[i / 25, i % 25].BackColor = Color.Red;
            }
        }
        public void currNC(int i)
        {
             panel[i].BackColor = Color.Yellow;
             //ncLabels[i / 25, i % 25].BackColor = Color.Red;
        }

        public void freeNC(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                panel[i].BackColor = Color.Silver;
                //ncLabels[i / 25, i % 25].BackColor = Color.Silver;
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        //设置进程忙碌
        public void setShebeiBusy(string shebei)
        {
            switch (shebei)
            {
                case "a":
                    label12.Text = "忙碌中....";
                    break;
                case "b":
                    label15.Text = "忙碌中....";
                    break;
                case "a2":
                    label14.Text = "忙碌中....";
                    break;
                case "b2":
                    label18.Text = "忙碌中....";
                    break;
                case "c":
                    label17.Text = "忙碌中....";
                    break;
            }
        }

        //设置进程空闲
        public void setShebeiIdle(string shebei)
        {
            switch (shebei)
            {
                case "a":
                    label12.Text = "";
                    break;
                case "b":
                    label15.Text = "";
                    break;
                case "a2":
                    label14.Text = "";
                    break;
                case "b2":
                    label18.Text = "";
                    break;
                case "c":
                    label17.Text = "";
                    break;
            }
        }

        //改变占用设备的进程名
        public void setShebeiName(string shebei, string name,int pid)
        {
            if (pid != 0)
            {
                switch (shebei)
                {
                    case "a":
                        sba1.Text = name + pid;
                        break;
                    case "a2":
                        sbb1.Text = name + pid;
                        break;
                    case "b":
                        sbc1.Text = name + pid;
                        break;
                    case "b2":
                        sbd1.Text = name + pid;
                        break;
                    case "c":
                        sbe1.Text = name + pid;
                        break;
                }
            }
            else
                switch (shebei)
                {
                    case "a":
                        sba1.Text = name;
                        break;
                    case "a2":
                        sbb1.Text = name;
                        break;
                    case "b":
                        sbc1.Text = name;
                        break;
                    case "b2":
                        sbd1.Text = name;
                        break;
                    case "c":
                        sbe1.Text = name;
                        break;
                }
        }

        //设置设备进度条的范围
        public void setShebeiPBoth(string shebei, int min, int max)
        {

            switch (shebei)
            {
                case "a":
                    sba2.Minimum = 0;
                    sba2.Maximum = max * 100;
                    break;
                case "b":
                    sbc2.Minimum = 0;
                    sbc2.Maximum = max * 100;
                    break;
                case "a2":
                    sbb2.Minimum = 0;
                    sbb2.Maximum = max * 100;
                    break;
                case "b2":
                    sbd2.Minimum = 0;
                    sbd2.Maximum = max * 100;
                    break;
                case "c":
                    sbe2.Minimum = 0;
                    sbe2.Maximum = max * 100;
                    break;
            }
        }

        //设置设备进度条值
        public void setShebeiPValue(string shebei, int value)
        {
            switch (shebei)
            {
                case "a":
                    sba2.Value = value;
                    break;
                case "b":
                    sbc2.Value = value;
                    break;
                case "a2":
                    sbb2.Value = value;
                    break;
                case "b2":
                    sbd2.Value = value;
                    break;
                case "c":
                    sbe2.Value = value;
                    break;
            }
        }

        private void fileTree_Click(object sender, EventArgs e)
        {
        }

        //保持选择的树的结点的imageIndex不变
        private void fileTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageIndex == 0)
            {
                fileTree.SelectedImageIndex = 0;
            }
            else if (e.Node.ImageIndex == 1)
            {
                fileTree.SelectedImageIndex = 1;
            }
            else if (e.Node.ImageIndex == 2)
            {
                fileTree.SelectedImageIndex = 2;
            }
            else if (e.Node.ImageIndex == 3)
            {
                fileTree.SelectedImageIndex = 3;
            }
        }

        //打开文件
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = getNodePath(fileTree.SelectedNode);
            if (path == null)
            {
                return;
            }
            string msg = fs.readFile(path).msg;

            MessageBox.Show("读取结果是："+msg,"文件查看");
        }

        private void 创建文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = getNodePath(fileTree.SelectedNode);

            if (!fs.isRoot(path))
            {
                if (!fs.isDir(path))
                {
                    MessageBox.Show("请现选择目录！", "提示");
                    return;
                }
            }

            AddFile addFile = new AddFile();

            addFile.setFilePath(path);

            addFile.ShowDialog();
        }

        //执行文件
        private void 执行文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = getNodePath(fileTree.SelectedNode);

            if (fs.isRoot(path))
            {
                MessageBox.Show("不能执行硬盘！","执行文件");
                return;
            }
            if (!fs.isEx(path))
            {
                MessageBox.Show("选择的不是可执行文件！","执行文件");
                return;
            }

            Message msg = fs.readFile(path);
            if (msg.suc)
            {
                PosInNC pos = new PosInNC();
                if (NC.askForNC(msg.msg, pos))
                {
                    if (!control.create(pos.start,pos.end,fs.getFileName(path)))
                    {
                        //添加失败！
                        NC.freeNc(pos.start, pos.end);
                        MessageBox.Show("进程数已达10个，不能再添加，请等待其他进程执行完毕！", "执行文件");
                    }
                }
                else
                {
                    MessageBox.Show("内存不足，不能建立进程！", "执行文件");
                }
            }
            else
            {
                MessageBox.Show("操作失败！", "执行文件");
                return;
            }
        }


        //查看文件属性
        private void 属性ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = getNodePath(fileTree.SelectedNode);
            if (fs.isRoot(path))
            {
                MessageBox.Show("磁盘属性是读写", "提示");
                return;
            }
            string p = fs.getFileProperty(path).msg;
            MessageBox.Show("只读：" + p,"属性查看");
        }

        //根据树的结点返回路径
        public string getNodePath(TreeNode node)
        {
            try
            {
                string path = node.Text;
                TreeNode tempNode = node;
                while (!tempNode.Text.Equals("c") && !tempNode.Text.Equals("d"))
                {
                    tempNode = tempNode.Parent;
                    path = tempNode.Text + "/" + path;
                }
                return path;
            }
            catch (Exception ex)
            {
                return "c";
            }
        }

        //删除文件
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            string path = getNodePath(fileTree.SelectedNode);
            if (fs.isRoot(path))
            {
                MessageBox.Show("不能删除磁盘！", "提示");
                return;
            }
            DialogResult result = MessageBox.Show("真的要删除？", "提示", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {

                if (fs.isDir(path))
                {
                    //是文件夹
                    fs.delDir(path);
                }
                else
                {
                    //是文件
                    fs.delFile(path);
                }
                initNode();
            }
        }

        //更改文件属性
        private void 只读读写ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = getNodePath(fileTree.SelectedNode);
            if (fs.isRoot(path))
            {
                MessageBox.Show("不能改变磁盘的读写属性！", "提示");
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
                MessageBox.Show("更改完毕！", "更改属性提示");
            }
            else
            {
                MessageBox.Show("更改失败：" + msg.msg, "更改属性提示");
            }
        }

        //更改文件属性函数
        public void changeFileProperty(string path)
        {
        }

        //使用硬盘
        public void busyDisk(string root,int index)
        {
            int i = index / 8;
            int j = index % 8;
            if (root.Equals("c"))
            {
                diskCLabels[i, j].BackColor = Color.Red;
            }
            else
            {
                diskDLabels[i, j].BackColor = Color.Red;
            }
        }

        //释放硬盘
        public void freeDisk(string root, int index)
        {
            int i = index / 8;
            int j = index % 8;
            if (root.Equals("c"))
            {
                diskCLabels[i, j].BackColor = Color.Silver;
            }
            else
            {
                diskDLabels[i, j].BackColor = Color.Silver;
            }
        }

        //打开命令帮助窗口
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new CmdHelp().ShowDialog();
        }

        //执行命令按钮
        private void button1_Click(object sender, EventArgs e)
        {
            cmd.manage(cmdBox.Text);
        }

        //改变curPathLabel的值
        public void setCurPathLabel(string str)
        {
            curPathLabel.Text = str;
        }

        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = getNodePath(fileTree.SelectedNode);
            if (fs.isRoot(path))
            {
                MessageBox.Show("不能对磁盘进行编辑！", "提示");
                return;
            }
            if (OperateFilePath!=null && path.Equals(OperateFilePath))
            {
                MessageBox.Show("不能对已经选择为复制或剪切的文件或文件夹编辑！", "提示");
                return;
            }
            if (fs.isDir(path))
            {
                //是文件夹
                EditDir editDir = new EditDir();
                editDir.setDirBox(fs.getFileName(path));
                editDir.setPath(path);
                editDir.ShowDialog();
                return;
            }
            
            EditForm editForm = new EditForm();
            editForm.setFilePath(path);
            editForm.setFileName(fs.getFileName(path));
            editForm.setFileData(fs.readFile(path).msg);
            editForm.ShowDialog();
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = getNodePath(fileTree.SelectedNode);
            if (fs.isRoot(path))
            { 
                //选择的是磁盘
                MessageBox.Show("不可对磁盘进行复制操作！", "复制提示");
                return;
            }
            if (fs.isDir(path))
            {
                //选择的是文件夹
                MessageBox.Show("不可对文件夹进行复制操作！", "复制提示");
                return;
            }
            CurState = COPY;
            OperateFilePath = path;
        }

        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = getNodePath(fileTree.SelectedNode);
            if (fs.isRoot(path))
            {
                //选择的是磁盘
                MessageBox.Show("不可对磁盘进行剪切操作！", "剪贴提示");
                return;
            }
            CurState = CUT;
            OperateFilePath = path;
        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = getNodePath(fileTree.SelectedNode);

            if (CurState == NONE)
            {
                //还没有选择复制或者粘贴
                MessageBox.Show("没有粘贴来源！", "粘贴提示");
                return;
            }

            if (!fs.isRoot(path))
            {

                if (!fs.isDir(path))
                {
                    MessageBox.Show("粘贴的目的方向必须是文件夹或磁盘根目录", "粘贴提示");
                }
            }

            if (CurState == COPY)
            {
                Message msg = fs.copyFile(OperateFilePath, path);
                if (msg.suc)
                {
                    initNode();
                }
                else
                {
                    MessageBox.Show("复制失败：" + msg.msg, "复制失败");
                }
            }
            else if (CurState == CUT)
            {
                Message msg = fs.cutFile(OperateFilePath, path);
                if (msg.suc)
                {
                    initNode();
                }
                else
                {
                    MessageBox.Show("剪切失败：" + msg.msg, "剪切失败");
                }
            }

            CurState = NONE;
            OperateFilePath = null;
        }

        private void 格式化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = getNodePath(fileTree.SelectedNode);

            if (path.Equals("c"))
            {
                Format("c");
            }
            else if (path.Equals("d"))
            {
                Format("d");
            }
            else
            {
                MessageBox.Show("请选择磁盘！", "提示");
            }
        }

        //格式化
        public void Format(string root)
        {
            DialogResult result = MessageBox.Show("真的要格式化" + root + "盘吗？", "提示", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                fs.init(root + ".vhd");
                initNode();
                MessageBox.Show("格式化完毕！", "提示");
            }
        }

        private void fileTree_DoubleClick(object sender, EventArgs e)
        {
            string path = getNodePath(fileTree.SelectedNode);

            string msg = fs.readFile(path).msg;

            MessageBox.Show("读取结果是：" + msg, "文件查看");
        }

        //向输出结果的listbox添加文字
        public void addResult(string result)
        {
            resultBox.Items.Add(result);
        }

        //添加进程，供命令行调用
        public string addPCB(int start,int end, string fileName)
        {
            if (!control.create(start,end, fileName))
            {
                //添加失败！
                NC.freeNc(start, end);
                return ("进程数已达10个，不能再添加，请等待其他进程执行完毕！");
            }
            else
            { 
                //添加成功
                return "添加进程完毕！";
            }
        }

        public void addEdit(string msg)
        {
            textBox2.Text = msg;
            if (cmdBox.Text.StartsWith("edit"))
            {
                button4.Enabled = true;
                button3.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            resultBox.Items.Clear();
        }

        private void doneList_DoubleClick(object sender, EventArgs e)
        {
            //doneList.Items.Clear();
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void groupBoxRam_Enter(object sender, EventArgs e)
        {

        }

        private void buttonbegin_Click(object sender, EventArgs e)
        {
            this.timer1.Start();
            this.groupBox1.Enabled = true;
            this.groupBox2.Enabled = true;
            this.groupBoxRam.Enabled = true;
            this.diskFrame.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label24.Text = DateTime.Now.ToString();
        }

        private void buttontrunoff_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cmd.changeFile(cmdBox.Text, textBox2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cmd.start(cmdBox.Text, textBox2.Text);
        }

        private void label23_Click(object sender, EventArgs e)
        {

        }
    }
}