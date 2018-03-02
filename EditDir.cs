using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OS
{
    public partial class EditDir : Form
    {
        string path = "";

        public EditDir()
        {
            InitializeComponent();
        }

        public void setDirBox(string dirName)
        {
            dirBox.Text = dirName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dirBox.Text.Length != 3)
            {
                MessageBox.Show("文件夹名长度必须为3！", "提示");
                return;
            }
            if (dirBox.Text.IndexOf("/") >= 0)
            {
                MessageBox.Show("文件夹名不能包含“/”！", "提示");
                return;
            }

            Message msg =Cmd.instance.fs.editDir(path, dirBox.Text);
            if (msg.suc)
            {
                Form1.Instance.initNode();
                this.Dispose();
            }
            else
            {
                MessageBox.Show("编辑失败：" + msg.msg, "提示");
            }
        }

        public void setPath(string path)
        {
            this.path = path;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}