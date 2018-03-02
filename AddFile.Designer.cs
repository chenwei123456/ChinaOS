namespace OS
{
    partial class AddFile
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.fileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioDir = new System.Windows.Forms.RadioButton();
            this.radioFileEx = new System.Windows.Forms.RadioButton();
            this.radioFileTx = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.fileData = new System.Windows.Forms.TextBox();
            this.filePath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // fileName
            // 
            this.fileName.Location = new System.Drawing.Point(138, 12);
            this.fileName.Name = "fileName";
            this.fileName.Size = new System.Drawing.Size(271, 21);
            this.fileName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "文件名(三个字符）：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "文件内容：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "文件路径：";
            // 
            // radioDir
            // 
            this.radioDir.AutoSize = true;
            this.radioDir.Location = new System.Drawing.Point(71, 128);
            this.radioDir.Name = "radioDir";
            this.radioDir.Size = new System.Drawing.Size(59, 16);
            this.radioDir.TabIndex = 4;
            this.radioDir.TabStop = true;
            this.radioDir.Text = "文件夹";
            this.radioDir.UseVisualStyleBackColor = true;
            // 
            // radioFileEx
            // 
            this.radioFileEx.AutoSize = true;
            this.radioFileEx.Location = new System.Drawing.Point(147, 127);
            this.radioFileEx.Name = "radioFileEx";
            this.radioFileEx.Size = new System.Drawing.Size(107, 16);
            this.radioFileEx.TabIndex = 5;
            this.radioFileEx.TabStop = true;
            this.radioFileEx.Text = "可执行（ｅｘ）";
            this.radioFileEx.UseVisualStyleBackColor = true;
            // 
            // radioFileTx
            // 
            this.radioFileTx.AutoSize = true;
            this.radioFileTx.Checked = true;
            this.radioFileTx.Location = new System.Drawing.Point(269, 128);
            this.radioFileTx.Name = "radioFileTx";
            this.radioFileTx.Size = new System.Drawing.Size(95, 16);
            this.radioFileTx.TabIndex = 6;
            this.radioFileTx.TabStop = true;
            this.radioFileTx.Text = "文本（ｔｘ）";
            this.radioFileTx.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(108, 190);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "添加";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(233, 190);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // fileData
            // 
            this.fileData.Location = new System.Drawing.Point(86, 38);
            this.fileData.Name = "fileData";
            this.fileData.Size = new System.Drawing.Size(323, 21);
            this.fileData.TabIndex = 9;
            // 
            // filePath
            // 
            this.filePath.Location = new System.Drawing.Point(88, 70);
            this.filePath.Name = "filePath";
            this.filePath.ReadOnly = true;
            this.filePath.Size = new System.Drawing.Size(321, 21);
            this.filePath.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(88, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "例如：c/abc/def";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(83, 231);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(173, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "警告：本程序不支持错误输入！";
            // 
            // AddFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 261);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.filePath);
            this.Controls.Add(this.fileData);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.radioFileTx);
            this.Controls.Add(this.radioFileEx);
            this.Controls.Add(this.radioDir);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AddFile";
            this.Text = "添加文件/文件夹";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddFile_FormClosing);
            this.Load += new System.EventHandler(this.AddFile_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox fileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioDir;
        private System.Windows.Forms.RadioButton radioFileEx;
        private System.Windows.Forms.RadioButton radioFileTx;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox fileData;
        private System.Windows.Forms.TextBox filePath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}