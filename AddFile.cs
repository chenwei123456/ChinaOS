using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OS
{
    public partial class AddFile : Form
    {
        FileSystem fs;
        public AddFile()
        {
            InitializeComponent();
            fs = Cmd.instance.fs;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fileName.Text.Equals(""))
            {
                MessageBox.Show("请输入文件名！", "添加文件（夹）提示");
                return;
            }
            if (fileName.Text.StartsWith(((char)1).ToString()))
            {
                MessageBox.Show("文件名不能以(byte)1开头！", "添加文件（夹）提示");
                return;
            }
            if (fileName.Text.IndexOf("/") > 0)
            {
                MessageBox.Show("文件名不能包含分割符号“/”！", "添加文件（夹）提示");
                return;
            }
            if (fileName.Text.Length != 3)
            {
                MessageBox.Show("文件名长度必须为3！", "添加文件（夹）提示");
                return;
            }
            if (fileData.Text.Equals("") && !radioDir.Checked)
            {
                MessageBox.Show("请输入文件内容！", "添加文件（夹）提示");
                return;
            }
            if (filePath.Text.Equals(""))
            {
                MessageBox.Show("请输入文件路径！", "添加文件（夹）提示");
                return;
            }

            if (radioDir.Checked)
            {
                //如果要创建文件夹
                Message msg = fs.createDir(filePath.Text, fileName.Text);
                if (!msg.suc)
                {
                    MessageBox.Show("创建失败：" + msg.msg, "添加文件（夹）提示");
                }
                else
                {
                    Form1.Instance.initNode();
                    this.Dispose();
                }
            }
            else
            {
                string fileTail;
                if (radioFileTx.Checked)
                {
                    fileTail = "tx";
                }
                else
                {
                    fileTail = "ex";
                }

                Message msg = fs.createFile(filePath.Text, fileName.Text, fileTail, fileData.Text);
                if (!msg.suc)
                {
                    MessageBox.Show("创建失败：" + msg.msg, "添加文件（夹）提示");
                }
                else
                {
                    Form1.Instance.initNode();
                    this.Dispose();
                }
            }
        
        }

        private void AddFile_Load(object sender, EventArgs e)
        {

        }

        public void setFilePath(string path)
        {
            this.filePath.Text = path;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void AddFile_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            
        }
    }
}