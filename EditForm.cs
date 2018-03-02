using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OS
{
    public partial class EditForm : Form
    {
        FileSystem fs;
        string tempFileName;

        public EditForm()
        {
            InitializeComponent();
            fs = Cmd.instance.fs;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fileName.Text.Equals(""))
            {
                MessageBox.Show("请输入文件名！","编辑文件");
                return;
            }
            if (fileName.Text.StartsWith(((char)1).ToString()))
            {
                MessageBox.Show("文件名不能以(byte)1开头！", "编辑文件");
                return;
            }
            if (fileName.Text.IndexOf("/") > 0)
            {
                MessageBox.Show("文件名不能包含分割符号“/”！", "编辑文件");
                return;
            }
            if (fileName.Text.Length != 3)
            {
                MessageBox.Show("文件名长度必须为3！", "编辑文件");
                return;
            }
            if (fileData.Text.Equals(""))
            {
                MessageBox.Show("请输入文件内容！", "编辑文件");
                return;
            }

            bool fileNameChanged = true;
            if (tempFileName.Equals(fileName.Text))
            {
                fileNameChanged = false;
            }

            Message msg = fs.editFile(FilePathBox.Text, fileName.Text, fileData.Text,fileNameChanged);            

            if (!msg.suc)
            { 
                //编辑失败
                MessageBox.Show("编辑失败：" + msg.msg, "编辑文件");
            }
            this.Dispose();
        }

        public void setFilePath(string filePath)
        {
            this.FilePathBox.Text = filePath;
        }

        public void setFileName(string fileName)
        {
            this.fileName.Text = fileName;
            tempFileName = fileName;
        }

        public void setFileData(string fileData)
        {
            this.fileData.Text = fileData;
        }

        private void EditForm_Load(object sender, EventArgs e)
        {

        }

        private void EditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!tempFileName.Equals(fileName.Text))
            {
                Form1.Instance.initNode();
            }
        }
    }
}