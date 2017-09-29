using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CapCap
{
    public partial class fMain : Form
    {
        private bool isFolderSelected = false;
        private bool isExiting = false;
        private int vCurrentNumber = 0;
        private int vTotalNumber = 1;

        private int vSameTimeNumber = 1;
        private string vLastNow = string.Empty;

        HotKeys hotkey = new HotKeys();

        private delegate bool BooleanTest(string value);

        private struct CursorInfo
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public Point ptScreenPos;
        }

        private const Int32 CURSOR_SHOWING = 0x00000001;

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CursorInfo pci);

        public fMain()
        {
            InitializeComponent();

            ///////////////////////////////

            initAutosave();
        }

        private void loadSettings()
        {
            tsbtnCursor.Checked = Properties.Settings.Default.capture_cursor;
            tsbtnNotification.Checked = Properties.Settings.Default.notify;
            tsbtnSound.Checked = Properties.Settings.Default.play_sound;
        }

        private void saveSettings()
        {
            Properties.Settings.Default.capture_cursor = tsbtnCursor.Checked;
            Properties.Settings.Default.notify = tsbtnNotification.Checked;
            Properties.Settings.Default.play_sound = tsbtnSound.Checked;
            Properties.Settings.Default.Save();
        }

        private void takeTheBloodyShot()
        {
            // Method name quoted from madam M in 007 movies, "Take the bloody shot!".

            // Take screen shot.
            Image img = captureScreen();
            var filename = saveImage(img);
            bool success = filename == "?" ? false : true;
            if (success)
                addRecord(filename, true);
            else
                addRecord(string.Empty, false);


            // Play sound.
            if (tsbtnSound.Checked)
                notify();

            // Notification.
            if (tsbtnNotification.Checked)
            {
                string info = string.Format("Screenshot {0} {1}", vCurrentNumber.ToString(), success ? "saved." : "failed.");
                NOTI.ShowBalloonTip(2000, "CapCap", string.Format("{0}", info), success ? ToolTipIcon.Info : ToolTipIcon.Error);
            }
        }

        private void addRecord(string filename, bool success)
        {
            tstbNumber.Text = (int.Parse(tstbNumber.Text) + 1).ToString();
            LV.Items.Add(vTotalNumber.ToString());
            LV.Items[LV.Items.Count - 1].SubItems.Add(filename);
            LV.Items[LV.Items.Count - 1].SubItems.Add(string.Format("{0}", DateTime.Now));
            LV.Items[LV.Items.Count - 1].SubItems.Add(success ? "Saved" : "Failed");
            vTotalNumber += 1;
        }

        private Image captureScreen()
        {
            // Capture screen.
            Image img = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphic = Graphics.FromImage(img);
            graphic.CopyFromScreen(new Point(0, 0), new Point(0, 0), Screen.AllScreens[0].Bounds.Size);

            if (tsbtnCursor.Checked)
            {
                CursorInfo ci;
                ci.cbSize = Marshal.SizeOf(typeof(CursorInfo));
                GetCursorInfo(out ci);
                Cursor cursor = new Cursor(ci.hCursor);
                cursor.Draw(graphic, new Rectangle(ci.ptScreenPos.X, ci.ptScreenPos.Y, cursor.Size.Width, cursor.Size.Height));
            }
            graphic.Dispose();
            return img;
        }

        private string saveImage(Image img)
        {
            try
            {
                string filename = getNextFullname();
                img.Save(filename, ImageFormat.Png);
                img.Dispose();
                return filename;
            }
            catch (Exception exp)
            {
                return "?";
            }
        }

        private string getNextFullname()
        {
            if (isFolderSelected)
                return string.Format(@"{0}\{1}{2}.jpg", FBD.SelectedPath, tstbPrefix.Text, tstbNumber.Text);
            else
            {
                string postfix = string.Empty;
                string now = DateTime.Now.ToString();

                if (now == vLastNow)
                {
                    vSameTimeNumber += 1;
                    postfix = $"({vSameTimeNumber.ToString()})";
                }
                else
                {
                    vSameTimeNumber = 1;
                    vLastNow = now;
                }

                return string.Format(@"{0}\{1}{2}.jpg", FBD.SelectedPath, DateTime.Now.ToString()
                    .Replace(':', '-'), postfix);
            }
            // what if empty tbName ???
            // well well, let's ignore it for now.
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            // Initiation
            // LV
            LV.Columns.Add("No.", 35);
            LV.Columns.Add("Filename", 354);
            LV.Columns.Add("Time", 120);
            LV.Columns.Add("Status", 55);

            LV.FullRowSelect = true;
            LV.Dock = DockStyle.Fill;

            // tstb
            tstbPrefix.Text = "CapCap_Image_";
            tstbNumber.Text = "1";
            loadSettings();

            // panelMain
            panelMain.Dock = DockStyle.Fill;
            panelMain.BringToFront();

            // NOTI
            NOTI.Visible = true;

            // Register hotkey, Alt + W by default.
            hotkey.Regist(this.Handle, (int)HotKeys.HotkeyModifiers.Alt, Keys.W, CallBack);

            // About
            label1.Left = (panelInnerAbout.Width - label1.Width) / 2;
            label2.Left = (panelInnerAbout.Width - label2.Width) / 2;
            label3.Left = (panelInnerAbout.Width - label3.Width) / 2;
            btnCloseAboutPanel.Left = (panelInnerAbout.Width - btnCloseAboutPanel.Width) / 2;

            centralizePanelInnerAbout();

            panelAbout.Dock = DockStyle.Fill;
        }

        private void centralizePanelInnerAbout()
        {
            panelInnerAbout.Left = (panelAbout.Width - panelInnerAbout.Width) / 2;
            panelInnerAbout.Top = (panelAbout.Height - panelInnerAbout.Height) / 2;
        }

        protected override void WndProc(ref Message m)
        {
            // Deal with Windows message.
            hotkey.ProcessHotKey(m);
            base.WndProc(ref m);
        }

        public void CallBack()
        {
            takeTheBloodyShot();
        }

        private string getFolderName(string path)
        {
            if (System.IO.Directory.GetDirectoryRoot(path) != path)
                path = path.Substring(path.LastIndexOf('\\') + 1);
            return path;
        }

        private void LV_DoubleClick(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(LV.SelectedItems[0].SubItems[1].Text))
                System.Diagnostics.Process.Start(LV.SelectedItems[0].SubItems[1].Text);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            isExiting = true;
            Application.Exit();
        }

        private void fMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isExiting)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                NOTI.Visible = false;
                hotkey.UnRegist(this.Handle, CallBack);
                saveSettings();
            }
        }

        private void showAbout()
        {
            centralizePanelInnerAbout();
            panelAbout.BringToFront();
        }

        private void notify()
        {
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Properties.Resources.notify);
            sp.Play();
        }

        private void initAutosave()
        {
            if (!System.IO.Directory.Exists(Application.StartupPath + @"\Autosave"))
                System.IO.Directory.CreateDirectory(Application.StartupPath + @"\Autosave");

            tsbtnFolder.Text = "Autosave";
            FBD.SelectedPath = Application.StartupPath + @"\Autosave";
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsmiExit.PerformClick();
        }

        private void 打开文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFolder();
        }

        private void NOTI_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible)
                {
                    this.Hide();
                }
                else
                {
                    this.Show();
                    this.Activate();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                CMS.Show(Control.MousePosition);
            }
        }

        private void tstbNumber_TextChanged(object sender, EventArgs e)
        {
            check_tstb_TextLegal((ToolStripTextBox)sender, isNextNumberLegal);
        }

        private void check_tstb_TextLegal(ToolStripTextBox textbox, BooleanTest bt)
        {
            // Check whether content is legal, if not change color to red.
            bool success = bt(textbox.Text);
            if (success)
            {
                textbox.BackColor = Color.White;
            }
            else
            {
                textbox.BackColor = Color.Tomato;
                textbox.SelectAll();
                textbox.Focus();
            }
        }

        private bool isNextNumberLegal(string value)
        {
            // Check if is non-negative integer.
            return int.TryParse(value, out int whatsoever) && int.Parse(value) >= 0;
        }

        private bool isPrefixLegal(string value)
        {
            // Check if is available filename.
            return Regex.Matches(value, "[\\/:*?\"<>|]").Count == 0 ? true : false;
        }

        private void tstbPrefix_TextChanged(object sender, EventArgs e)
        {
            check_tstb_TextLegal((ToolStripTextBox)sender, isPrefixLegal);
        }

        private void tsbtnFolder_Click(object sender, EventArgs e)
        {
            var result = FBD.ShowDialog();
            if (result == DialogResult.OK)
            {
                tsbtnFolder.Text = getFolderName(FBD.SelectedPath);
                isFolderSelected = true;
            }
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            isExiting = true;
            Application.Exit();
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            showAbout();
        }

        private void tsmiOpenFolder_Click(object sender, EventArgs e)
        {
            openFolder();
        }

        private void openFolder()
        {
            if (System.IO.Directory.Exists(FBD.SelectedPath))
                System.Diagnostics.Process.Start(FBD.SelectedPath);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showAbout();
        }

        private void btnCloseAboutPanel_Click(object sender, EventArgs e)
        {
            panelMain.BringToFront();
        }

        private void fMain_Resize(object sender, EventArgs e)
        {
            centralizePanelInnerAbout();
        }

        private void tsmiAbout_Click_1(object sender, EventArgs e)
        {
            showAbout();
        }

        private void tsmiExit_Click_1(object sender, EventArgs e)
        {
            exit();
        }

        private void exit()
        {
            isExiting = true;
            Application.Exit();
        }

        private void LV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LV.SelectedItems.Count != 0)
                tslabel_Status.Text = getFileName(LV.SelectedItems[0].SubItems[1].Text);
            else
                tslabel_Status.Text = "";
        }

        private string getFileName(string path)
        {
            return path.Substring(path.LastIndexOf('\\') + 1);
        }
    }

    public class HotKeys
    {
        //引入系统API
        [DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, Keys vk);
        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        int keyid = 10;     //区分不同的快捷键
        Dictionary<int, HotKeyCallBackHanlder> keymap = new Dictionary<int, HotKeyCallBackHanlder>();   //每一个key对于一个处理函数
        public delegate void HotKeyCallBackHanlder();

        //组合控制键
        public enum HotkeyModifiers
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

        //注册快捷键
        public void Regist(IntPtr hWnd, int modifiers, Keys vk, HotKeyCallBackHanlder callBack)
        {
            int id = keyid++;
            if (!RegisterHotKey(hWnd, id, modifiers, vk))
                throw new Exception("注册失败！");
            keymap[id] = callBack;
        }

        // 注销快捷键
        public void UnRegist(IntPtr hWnd, HotKeyCallBackHanlder callBack)
        {
            foreach (KeyValuePair<int, HotKeyCallBackHanlder> var in keymap)
            {
                if (var.Value == callBack)
                {
                    UnregisterHotKey(hWnd, var.Key);
                    return;
                }
            }
        }

        // 快捷键消息处理
        public void ProcessHotKey(Message m)
        {
            if (m.Msg == 0x312)
            {
                int id = m.WParam.ToInt32();
                HotKeyCallBackHanlder callback;
                if (keymap.TryGetValue(id, out callback))
                    callback();
            }
        }
    }
}
