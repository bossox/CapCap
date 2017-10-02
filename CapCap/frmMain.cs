using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using HotKeys;

namespace CapCap
{
    public partial class frmMain : Form
    {
        #region WinAPI
        [DllImport("user32.dll")]
        private static extern bool GetCursorInfo(out CursorInfo pci);
        private const Int32 CURSOR_SHOWING = 0x00000001;
        private struct CursorInfo
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public Point ptScreenPos;
        }
        #endregion

        private delegate bool LegalTest(string value);

        HotKey hotkey = new HotKey(Modifier.Alt, Keys.W);

        private bool isFolderSelected = false;
        private bool isExiting = false;
        private int vTotalNumber = 1;
        private int vSameTimeNumber = 1;
        private string vLastNow = string.Empty;

        public frmMain()
        {
            InitializeComponent();

            ///////////////////////////////

            initAutosave();
        }
        private void frmMain_Load(object sender, EventArgs e)
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
            hotkey.Register();
            hotkey.OnPressed += hotkey_OnPressed;

            // About
            label1.Left = (panelInnerAbout.Width - label1.Width) / 2;
            label2.Left = (panelInnerAbout.Width - label2.Width) / 2;
            label3.Left = (panelInnerAbout.Width - label3.Width) / 2;
            btnCloseAboutPanel.Left = (panelInnerAbout.Width - btnCloseAboutPanel.Width) / 2;

            centralizePanelInnerAbout();

            panelAbout.Dock = DockStyle.Fill;
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

        private void LV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LV.SelectedItems.Count != 0)
                tslabel_Status.Text = getFileName(LV.SelectedItems[0].SubItems[1].Text);
            else
                tslabel_Status.Text = "";
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showAbout();
        }

        private void btnCloseAboutPanel_Click(object sender, EventArgs e)
        {
            panelMain.BringToFront();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            centralizePanelInnerAbout();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            exit();
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

                if (!Directory.Exists(FBD.SelectedPath))
                    tsbtnFolder.ForeColor = Color.Red;
                else
                    tsbtnFolder.ForeColor = Color.Black;
            }
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            showAbout();
        }

        private void tsmiOpenFolder_Click(object sender, EventArgs e)
        {
            openFolder();
        }

        private void hotkey_OnPressed(object sender, HotKeyEventArgs e)
        {
            takeTheBloodyShot();
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

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isExiting)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                NOTI.Visible = false;
                hotkey.Unregister();
                saveSettings();
            }
        }

        private void cmsmiExit_Click(object sender, EventArgs e)
        {
            tsmiExit.PerformClick();
        }

        private void cmsmiOpenFolder_Click(object sender, EventArgs e)
        {
            openFolder();
        }

        #region Custom Method: Auxiliary
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

        private string getFolderName(string path)
        {
            if (System.IO.Directory.GetDirectoryRoot(path) != path)
                path = path.Substring(path.LastIndexOf('\\') + 1);
            return path;
        }

        private void centralizePanelInnerAbout()
        {
            panelInnerAbout.Left = (panelAbout.Width - panelInnerAbout.Width) / 2;
            panelInnerAbout.Top = (panelAbout.Height - panelInnerAbout.Height) / 2;
        }

        private void showAbout()
        {
            centralizePanelInnerAbout();
            panelAbout.BringToFront();
        }

        private void initAutosave()
        {
            if (!System.IO.Directory.Exists(Application.StartupPath + @"\Autosave"))
                System.IO.Directory.CreateDirectory(Application.StartupPath + @"\Autosave");

            tsbtnFolder.Text = "Autosave";
            FBD.SelectedPath = Application.StartupPath + @"\Autosave";
        }

        private bool isNextNumberLegal(string value)
        {
            // Check if is non-negative integer.
            return (int.TryParse(value, out int whatsoever) && int.Parse(value) >= 0) && (value != "");
        }

        private bool isPrefixLegal(string value)
        {
            // Check if is available filename.
            return (Regex.Matches(value, "[\\/:*?\"<>|]").Count == 0) && (value != "") ? true : false;
        }

        private void check_tstb_TextLegal(ToolStripTextBox textbox, LegalTest lt)
        {
            // Check whether content is legal, if not change color to red.
            bool success = lt(textbox.Text);
            if (success)
            {
                textbox.BackColor = Color.White;
                textbox.ForeColor = Color.Black;
            }
            else
            {
                textbox.BackColor = Color.Crimson;
                textbox.ForeColor = Color.White;
                textbox.SelectAll();
                textbox.Focus();
            }
        }

        private void openFolder()
        {
            if (System.IO.Directory.Exists(FBD.SelectedPath))
                System.Diagnostics.Process.Start(FBD.SelectedPath);
        }

        private void exit()
        {
            isExiting = true;
            Application.Exit();
        }

        private string getFileName(string path)
        {
            return path.Substring(path.LastIndexOf('\\') + 1);
        }
        #endregion

        #region Custom Method: Screen Capturing
        // Method name quoted from madam M in 007 movies, "Take the bloody shot!".
        private void takeTheBloodyShot()
        {
            if (!checkEverythingIsRight())
            {
                playSound(false);
                return;
            }

            // Take screen shot.
            Image img = captureScreen();
            var filename = saveImage(img);
            bool success = filename == "?" ? false : true;
            if (success)
                addRecord(filename, true);
            else
                addRecord(string.Empty, false);

            incrementNextNumber();

            // Notification.
            notify(filename, success);
        }

        private bool checkEverythingIsRight()
        {
            if (!isPrefixLegal(tstbPrefix.Text))
            {
                addRecord("[Error: Invalid prefix.]", false);
                return false;
            }

            if (!isNextNumberLegal(tstbNumber.Text))
            {
                addRecord("[Error: Invalid next number.]", false);
                return false;
            }

            if (!Directory.Exists(FBD.SelectedPath))
            {
                addRecord("[Error: Invalid folder.]", false);
                return false;
            }

            return true;
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
        }

        private void addRecord(string filename, bool success)
        {
            LV.Items.Add(vTotalNumber.ToString());
            LV.Items[LV.Items.Count - 1].SubItems.Add(filename);
            LV.Items[LV.Items.Count - 1].SubItems.Add(string.Format("{0}", DateTime.Now));
            LV.Items[LV.Items.Count - 1].SubItems.Add(success ? "Saved" : "Failed");
            vTotalNumber += 1;
        }

        private void incrementNextNumber()
        {
            tstbNumber.Text = (int.Parse(tstbNumber.Text) + 1).ToString();
        }

        private void notify(string filename, bool success)
        {
            if (tsbtnSound.Checked)
                playSound(success);

            if (tsbtnNotification.Checked)
            {
                filename = getFileName(filename); // C:\a\a.jpg >> a.jpg
                sendNotification(filename, success);
            }
        }

        private void playSound(bool success)
        {
            System.Media.SoundPlayer sp;
            sp = new System.Media.SoundPlayer(success ? Resource.yes : Resource.no);
            sp.Play();
        }

        private void sendNotification(string filename, bool success)
        {
            string info = string.Format("Screenshot {0} {1}", filename, success ? "saved." : "failed.");
            NOTI.ShowBalloonTip(2000, "CapCap", string.Format("{0}", info), success ? ToolTipIcon.Info : ToolTipIcon.Error);
        }
        #endregion
    }
}
