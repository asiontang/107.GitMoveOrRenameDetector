namespace GitMoveOrRenameDetector
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnRename = new System.Windows.Forms.Button();
            this.rtbRename = new System.Windows.Forms.RichTextBox();
            this.btnCheck = new System.Windows.Forms.Button();
            this.btnSwitchHg = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnOpenGitCmd = new System.Windows.Forms.Button();
            this.btnOpenGitLog = new System.Windows.Forms.Button();
            this.btnOpenGitReflog = new System.Windows.Forms.Button();
            this.btnOpenCommit = new System.Windows.Forms.Button();
            this.btnOpenSync = new System.Windows.Forms.Button();
            this.btnOpenPush = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(0, 0);
            label1.Margin = new System.Windows.Forms.Padding(0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(107, 12);
            label1.TabIndex = 1;
            label1.Text = "被删除的文件列表:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(453, 0);
            label2.Margin = new System.Windows.Forms.Padding(0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(119, 12);
            label2.TabIndex = 5;
            label2.Text = "待重命名的文件列表:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBox1.Location = new System.Drawing.Point(2, 16);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(360, 420);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.WordWrap = false;
            // 
            // btnRename
            // 
            this.btnRename.Location = new System.Drawing.Point(365, 74);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(85, 23);
            this.btnRename.TabIndex = 3;
            this.btnRename.Text = "2.确认重命名";
            this.btnRename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // rtbRename
            // 
            this.rtbRename.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbRename.Location = new System.Drawing.Point(453, 16);
            this.rtbRename.Margin = new System.Windows.Forms.Padding(0);
            this.rtbRename.Name = "rtbRename";
            this.rtbRename.ReadOnly = true;
            this.rtbRename.Size = new System.Drawing.Size(360, 420);
            this.rtbRename.TabIndex = 4;
            this.rtbRename.Text = "";
            this.rtbRename.WordWrap = false;
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(365, 45);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(85, 23);
            this.btnCheck.TabIndex = 2;
            this.btnCheck.Text = "1.立即检测";
            this.btnCheck.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnChcek_Click);
            // 
            // btnSwitchHg
            // 
            this.btnSwitchHg.Location = new System.Drawing.Point(365, 413);
            this.btnSwitchHg.Name = "btnSwitchHg";
            this.btnSwitchHg.Size = new System.Drawing.Size(85, 23);
            this.btnSwitchHg.TabIndex = 6;
            this.btnSwitchHg.Text = "切换为Hg模式";
            this.btnSwitchHg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSwitchHg.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(365, 16);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(85, 23);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnOpenGitCmd
            // 
            this.btnOpenGitCmd.Location = new System.Drawing.Point(365, 103);
            this.btnOpenGitCmd.Name = "btnOpenGitCmd";
            this.btnOpenGitCmd.Size = new System.Drawing.Size(85, 23);
            this.btnOpenGitCmd.TabIndex = 8;
            this.btnOpenGitCmd.Text = "打开Git.CMD";
            this.btnOpenGitCmd.UseVisualStyleBackColor = true;
            this.btnOpenGitCmd.Click += new System.EventHandler(this.btnOpenGitCmd_Click);
            // 
            // btnOpenGitLog
            // 
            this.btnOpenGitLog.Location = new System.Drawing.Point(365, 132);
            this.btnOpenGitLog.Name = "btnOpenGitLog";
            this.btnOpenGitLog.Size = new System.Drawing.Size(85, 23);
            this.btnOpenGitLog.TabIndex = 9;
            this.btnOpenGitLog.Text = "打开Git.Log";
            this.btnOpenGitLog.UseVisualStyleBackColor = true;
            this.btnOpenGitLog.Click += new System.EventHandler(this.btnOpenGitLog_Click);
            // 
            // btnOpenGitReflog
            // 
            this.btnOpenGitReflog.Location = new System.Drawing.Point(365, 161);
            this.btnOpenGitReflog.Name = "btnOpenGitReflog";
            this.btnOpenGitReflog.Size = new System.Drawing.Size(85, 23);
            this.btnOpenGitReflog.TabIndex = 10;
            this.btnOpenGitReflog.Text = "打开refog";
            this.btnOpenGitReflog.UseVisualStyleBackColor = true;
            this.btnOpenGitReflog.Click += new System.EventHandler(this.btnOpenGitReflog_Click);
            // 
            // btnOpenCommit
            // 
            this.btnOpenCommit.Location = new System.Drawing.Point(365, 190);
            this.btnOpenCommit.Name = "btnOpenCommit";
            this.btnOpenCommit.Size = new System.Drawing.Size(85, 23);
            this.btnOpenCommit.TabIndex = 11;
            this.btnOpenCommit.Text = "打开提交页面";
            this.btnOpenCommit.UseVisualStyleBackColor = true;
            this.btnOpenCommit.Click += new System.EventHandler(this.btnOpenCommit_Click);
            // 
            // btnOpenSync
            // 
            this.btnOpenSync.Location = new System.Drawing.Point(365, 219);
            this.btnOpenSync.Name = "btnOpenSync";
            this.btnOpenSync.Size = new System.Drawing.Size(85, 23);
            this.btnOpenSync.TabIndex = 12;
            this.btnOpenSync.Text = "打开同步页面";
            this.btnOpenSync.UseVisualStyleBackColor = true;
            this.btnOpenSync.Click += new System.EventHandler(this.btnOpenSync_Click);
            // 
            // btnOpenPush
            // 
            this.btnOpenPush.Location = new System.Drawing.Point(365, 248);
            this.btnOpenPush.Name = "btnOpenPush";
            this.btnOpenPush.Size = new System.Drawing.Size(85, 23);
            this.btnOpenPush.TabIndex = 13;
            this.btnOpenPush.Text = "打开推送页面";
            this.btnOpenPush.UseVisualStyleBackColor = true;
            this.btnOpenPush.Click += new System.EventHandler(this.btnOpenPush_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(820, 439);
            this.Controls.Add(this.btnOpenPush);
            this.Controls.Add(this.btnOpenSync);
            this.Controls.Add(this.btnOpenCommit);
            this.Controls.Add(this.btnOpenGitReflog);
            this.Controls.Add(this.btnOpenGitLog);
            this.Controls.Add(this.btnOpenGitCmd);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnSwitchHg);
            this.Controls.Add(label2);
            this.Controls.Add(this.rtbRename);
            this.Controls.Add(this.btnRename);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(label1);
            this.Controls.Add(this.richTextBox1);
            this.Location = new System.Drawing.Point(15, 15);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnOpenPush;

        private System.Windows.Forms.Button btnOpenSync;

        private System.Windows.Forms.Button btnOpenCommit;

        private System.Windows.Forms.Button btnOpenGitReflog;

        private System.Windows.Forms.Button btnOpenGitLog;

        private System.Windows.Forms.Button btnOpenGitCmd;

        private System.Windows.Forms.Button btnRefresh;

        private System.Windows.Forms.Button btnSwitchHg;

        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.RichTextBox rtbRename;
        private System.Windows.Forms.Button btnCheck;

        private System.Windows.Forms.RichTextBox richTextBox1;

        #endregion
    }
}