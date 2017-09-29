namespace CapCap
{
    partial class fMain
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fMain));
            this.FBD = new System.Windows.Forms.FolderBrowserDialog();
            this.NOTI = new System.Windows.Forms.NotifyIcon(this.components);
            this.CMS = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.打开文件夹ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelAbout = new System.Windows.Forms.Panel();
            this.panelInnerAbout = new System.Windows.Forms.Panel();
            this.btnCloseAboutPanel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tslabel_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.LV = new System.Windows.Forms.ListView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbtnCursor = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnNotification = new System.Windows.Forms.ToolStripButton();
            this.tsbtnSound = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tstbPrefix = new System.Windows.Forms.ToolStripTextBox();
            this.tstbNumber = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsbtnFolder = new System.Windows.Forms.ToolStripButton();
            this.tsbtnMainMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.CMS.SuspendLayout();
            this.panelAbout.SuspendLayout();
            this.panelInnerAbout.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // NOTI
            // 
            this.NOTI.Icon = ((System.Drawing.Icon)(resources.GetObject("NOTI.Icon")));
            this.NOTI.Text = "CapCap";
            this.NOTI.Visible = true;
            this.NOTI.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NOTI_MouseClick);
            // 
            // CMS
            // 
            this.CMS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开文件夹ToolStripMenuItem,
            this.toolStripSeparator4,
            this.退出ToolStripMenuItem});
            this.CMS.Name = "CMS";
            this.CMS.Size = new System.Drawing.Size(148, 54);
            // 
            // 打开文件夹ToolStripMenuItem
            // 
            this.打开文件夹ToolStripMenuItem.Name = "打开文件夹ToolStripMenuItem";
            this.打开文件夹ToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.打开文件夹ToolStripMenuItem.Text = "Open folder";
            this.打开文件夹ToolStripMenuItem.Click += new System.EventHandler(this.打开文件夹ToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(144, 6);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.退出ToolStripMenuItem.Text = "Exit";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // panelAbout
            // 
            this.panelAbout.Controls.Add(this.panelInnerAbout);
            this.panelAbout.Location = new System.Drawing.Point(167, 75);
            this.panelAbout.Name = "panelAbout";
            this.panelAbout.Size = new System.Drawing.Size(405, 274);
            this.panelAbout.TabIndex = 11;
            // 
            // panelInnerAbout
            // 
            this.panelInnerAbout.Controls.Add(this.btnCloseAboutPanel);
            this.panelInnerAbout.Controls.Add(this.label2);
            this.panelInnerAbout.Controls.Add(this.label1);
            this.panelInnerAbout.Controls.Add(this.label3);
            this.panelInnerAbout.Location = new System.Drawing.Point(12, 12);
            this.panelInnerAbout.Name = "panelInnerAbout";
            this.panelInnerAbout.Size = new System.Drawing.Size(377, 239);
            this.panelInnerAbout.TabIndex = 4;
            // 
            // btnCloseAboutPanel
            // 
            this.btnCloseAboutPanel.AutoSize = true;
            this.btnCloseAboutPanel.Location = new System.Drawing.Point(144, 188);
            this.btnCloseAboutPanel.Name = "btnCloseAboutPanel";
            this.btnCloseAboutPanel.Size = new System.Drawing.Size(75, 23);
            this.btnCloseAboutPanel.TabIndex = 7;
            this.btnCloseAboutPanel.Text = "okay";
            this.btnCloseAboutPanel.UseVisualStyleBackColor = true;
            this.btnCloseAboutPanel.Click += new System.EventHandler(this.btnCloseAboutPanel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(91, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Boss Ox / 2017.09.28 / Beijing";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(127, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 36);
            this.label1.TabIndex = 4;
            this.label1.Text = "CapCap";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(53, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(275, 42);
            this.label3.TabIndex = 6;
            this.label3.Text = "Screen capturing with auto saving.\r\nIncredibly easier than ever beffore.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.LV);
            this.panelMain.Controls.Add(this.statusStrip1);
            this.panelMain.Controls.Add(this.toolStrip1);
            this.panelMain.Location = new System.Drawing.Point(12, 12);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(526, 269);
            this.panelMain.TabIndex = 12;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslabel_Status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 247);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(526, 22);
            this.statusStrip1.TabIndex = 13;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tslabel_Status
            // 
            this.tslabel_Status.Name = "tslabel_Status";
            this.tslabel_Status.Size = new System.Drawing.Size(44, 17);
            this.tslabel_Status.Text = "Ready";
            // 
            // LV
            // 
            this.LV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LV.Location = new System.Drawing.Point(14, 43);
            this.LV.MultiSelect = false;
            this.LV.Name = "LV";
            this.LV.Size = new System.Drawing.Size(109, 125);
            this.LV.TabIndex = 12;
            this.LV.UseCompatibleStateImageBehavior = false;
            this.LV.View = System.Windows.Forms.View.Details;
            this.LV.SelectedIndexChanged += new System.EventHandler(this.LV_SelectedIndexChanged);
            this.LV.DoubleClick += new System.EventHandler(this.LV_DoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnCursor,
            this.toolStripSeparator1,
            this.tsbtnNotification,
            this.tsbtnSound,
            this.toolStripSeparator2,
            this.tstbPrefix,
            this.tstbNumber,
            this.toolStripLabel1,
            this.tsbtnFolder,
            this.tsbtnMainMenu});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(526, 25);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbtnCursor
            // 
            this.tsbtnCursor.CheckOnClick = true;
            this.tsbtnCursor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnCursor.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnCursor.Image")));
            this.tsbtnCursor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnCursor.Name = "tsbtnCursor";
            this.tsbtnCursor.Size = new System.Drawing.Size(51, 22);
            this.tsbtnCursor.Text = "Cursor";
            this.tsbtnCursor.ToolTipText = "Capture cursor";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbtnNotification
            // 
            this.tsbtnNotification.CheckOnClick = true;
            this.tsbtnNotification.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnNotification.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnNotification.Image")));
            this.tsbtnNotification.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnNotification.Name = "tsbtnNotification";
            this.tsbtnNotification.Size = new System.Drawing.Size(79, 22);
            this.tsbtnNotification.Text = "Notification";
            this.tsbtnNotification.ToolTipText = "Show notification after capture";
            // 
            // tsbtnSound
            // 
            this.tsbtnSound.Checked = true;
            this.tsbtnSound.CheckOnClick = true;
            this.tsbtnSound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbtnSound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnSound.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnSound.Image")));
            this.tsbtnSound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnSound.Name = "tsbtnSound";
            this.tsbtnSound.Size = new System.Drawing.Size(49, 22);
            this.tsbtnSound.Text = "Sound";
            this.tsbtnSound.ToolTipText = "Play sound after capture";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tstbPrefix
            // 
            this.tstbPrefix.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tstbPrefix.Name = "tstbPrefix";
            this.tstbPrefix.Size = new System.Drawing.Size(100, 25);
            this.tstbPrefix.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tstbPrefix.ToolTipText = "File name prefix";
            // 
            // tstbNumber
            // 
            this.tstbNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tstbNumber.Name = "tstbNumber";
            this.tstbNumber.Size = new System.Drawing.Size(30, 25);
            this.tstbNumber.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tstbNumber.ToolTipText = "File name next number";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(48, 22);
            this.toolStripLabel1.Text = ".JPG @";
            // 
            // tsbtnFolder
            // 
            this.tsbtnFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnFolder.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnFolder.Image")));
            this.tsbtnFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnFolder.Name = "tsbtnFolder";
            this.tsbtnFolder.Size = new System.Drawing.Size(49, 22);
            this.tsbtnFolder.Text = "Folder";
            this.tsbtnFolder.ToolTipText = "Save images to this folder";
            this.tsbtnFolder.Click += new System.EventHandler(this.tsbtnFolder_Click);
            // 
            // tsbtnMainMenu
            // 
            this.tsbtnMainMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbtnMainMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnMainMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpenFolder,
            this.toolStripSeparator3,
            this.tsmiAbout,
            this.tsmiExit});
            this.tsbtnMainMenu.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnMainMenu.Image")));
            this.tsbtnMainMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnMainMenu.Name = "tsbtnMainMenu";
            this.tsbtnMainMenu.Size = new System.Drawing.Size(54, 22);
            this.tsbtnMainMenu.Text = "Menu";
            // 
            // tsmiOpenFolder
            // 
            this.tsmiOpenFolder.Name = "tsmiOpenFolder";
            this.tsmiOpenFolder.Size = new System.Drawing.Size(147, 22);
            this.tsmiOpenFolder.Text = "Open folder";
            this.tsmiOpenFolder.Click += new System.EventHandler(this.tsmiOpenFolder_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(144, 6);
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Size = new System.Drawing.Size(147, 22);
            this.tsmiAbout.Text = "About";
            this.tsmiAbout.Click += new System.EventHandler(this.tsmiAbout_Click_1);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(147, 22);
            this.tsmiExit.Text = "Exit";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click_1);
            // 
            // fMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.panelAbout);
            this.Controls.Add(this.panelMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "fMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CapCap";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fMain_FormClosing);
            this.Load += new System.EventHandler(this.fMain_Load);
            this.Resize += new System.EventHandler(this.fMain_Resize);
            this.CMS.ResumeLayout(false);
            this.panelAbout.ResumeLayout(false);
            this.panelInnerAbout.ResumeLayout(false);
            this.panelInnerAbout.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog FBD;
        private System.Windows.Forms.NotifyIcon NOTI;
        private System.Windows.Forms.ContextMenuStrip CMS;
        private System.Windows.Forms.ToolStripMenuItem 打开文件夹ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Panel panelAbout;
        private System.Windows.Forms.Panel panelInnerAbout;
        private System.Windows.Forms.Button btnCloseAboutPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.ListView LV;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbtnCursor;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbtnNotification;
        private System.Windows.Forms.ToolStripButton tsbtnSound;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripTextBox tstbPrefix;
        private System.Windows.Forms.ToolStripTextBox tstbNumber;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton tsbtnFolder;
        private System.Windows.Forms.ToolStripDropDownButton tsbtnMainMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tslabel_Status;
    }
}

