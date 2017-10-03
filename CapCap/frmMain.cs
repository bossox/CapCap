using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using HotKeys;

namespace CapCap
{
    public partial class frmMain : Form
    {
        // Debug: Show (test) menu item.
        private bool _isDebugMode = false;
        public bool isDebugMode
        {
            get
            {
                return _isDebugMode;
            }
            set
            {
                _isDebugMode = value;
                MenuItemTest.Visible = value;
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nPattern.Number = int.Parse(tstbNumber.Text);
            MessageBox.Show(nPattern.Convert(tstbNamePattern.Text));
        }
        ////////////////////////////////////////////////////////////////////////////////

        private delegate void ScreenShotEventHandler(ScreenShotInfo info);
        private event ScreenShotEventHandler OnScreenShotCaptured;

        private delegate bool LegalTest(string value);

        HotKey hotkey = new HotKey(Modifier.Alt, Keys.W);

        private bool isFolderSelected = false;
        private bool isExiting = false;
        private int vTotalNumber = 1;   // total number of history items.
        private int vSameTimeNumber = 1;
        private string vLastNow = string.Empty;
        private int vNextNumber = 1;    // next number.

        private System.Media.SoundPlayer soundplayer = new System.Media.SoundPlayer();

        private NamePattern nPattern = new NamePattern();

        private List<Task> vTasks = new List<Task>();

        private bool isWelcomeScreen = true;

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
            tstbNamePattern.Text = "CapImg_<year>-<month>-<day>_<#>";
            tstbNumber.Text = nPattern.Number.ToString();
            loadSettings();

            tstbNamePattern.Visible = false;
            tstbNumber.Visible = false;
            tslImageFormat.Visible = false;

            tsSeparatorOfNumber.Visible = false;
            tslNumber.Visible = false;

            // panelMain
            panelMain.Dock = DockStyle.Fill;
            panelMain.BringToFront();

            // NOTI
            NOTI.Visible = true;

            // Register hotkey, Alt + W by default.
            hotkey.Register();
            hotkey.OnPressed += hotkey_OnPressed;

            // About
            panelAbout.Dock = DockStyle.Fill;

            label1.Left = (panelInnerAbout.Width - label1.Width) / 2;
            label2.Left = (panelInnerAbout.Width - label2.Width) / 2;
            label3.Left = (panelInnerAbout.Width - label3.Width) / 2;
            lnkWeibo.Left = (panelInnerAbout.Width - lnkWeibo.Width) / 2;
            btnCloseAboutPanel.Left = (panelInnerAbout.Width - btnCloseAboutPanel.Width) / 2;

            lnkRUC80.Left = label2.Right + 5;

            centralizePanelInnerAbout();

            // Help
            panelHelp.Dock = DockStyle.Fill;


            // RUC80
            panelRUC80.Dock = DockStyle.Fill;
            picRUC80.Left = (panelRUC80.Width - picRUC80.Width) / 2;
            picRUC80.Top = (panelRUC80.Height - picRUC80.Height) / 2;

            // Parallel
            OnScreenShotCaptured += ScreenShot_OnScreenShotCaptured;

            // RUC 80 Auto show.
            showRUC80();
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
            if (vTasks.Count == 0 && isNextNumberLegal(tstbNumber.Text))
                vNextNumber = int.Parse(tstbNumber.Text);
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
            centralizePanelRUC80();
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
                tsbtnFolder.Text = getFolderName(FBD.SelectedPath) + "\\";
                isFolderSelected = true;

                if (!Directory.Exists(FBD.SelectedPath))
                    tsbtnFolder.ForeColor = Color.Red;
                else
                    tsbtnFolder.ForeColor = Color.Black;

                tstbNamePattern.Visible = true;
                tstbNumber.Visible = true;
                tslImageFormat.Visible = true;
                tsSeparatorOfNumber.Visible = true;
                tslNumber.Visible = true;
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
            panelMain.BringToFront();
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

        private void lnkWeibo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://weibo.com/BOSoftwareService");
        }

        #region Custom Method: Auxiliary
        private void showRUC80()
        {
            if (DateTime.Now.Year == 2017 && DateTime.Now.Month == 10 && DateTime.Now.Day <= 15)
                panelRUC80.BringToFront();
        }

        private void loadSettings()
        {
            settings_Cursor.Checked = Properties.Settings.Default.capture_cursor;
            settings_Notification.Checked = Properties.Settings.Default.notify;
            settings_Sound.Checked = Properties.Settings.Default.play_sound;
            convertCode2ImageFormat(Properties.Settings.Default.image_format);
            tslImageFormat.Text = $".{convertImageFormat2Code()}";
        }

        private void saveSettings()
        {
            Properties.Settings.Default.capture_cursor = settings_Cursor.Checked;
            Properties.Settings.Default.notify = settings_Notification.Checked;
            Properties.Settings.Default.play_sound = settings_Sound.Checked;
            Properties.Settings.Default.image_format = convertImageFormat2Code();
            Properties.Settings.Default.Save();
        }

        private void convertCode2ImageFormat(string code)
        {
            switch (code)
            {
                case "BMP":
                    settingsBMP.Checked = true;
                    break;
                case "GIF":
                    settingsGIF.Checked = true;
                    break;
                case "JPEG":
                    settingsJPEG.Checked = true;
                    break;
                case "PNG":
                    settingsPNG.Checked = true;
                    break;
                case "TIFF":
                    settingsTIFF.Checked = true;
                    break;
            }
        }

        private string convertImageFormat2Code()
        {
            if (settingsBMP.Checked)
                return "BMP";

            if (settingsGIF.Checked)
                return "GIF";

            if (settingsJPEG.Checked)
                return "JPEG";

            if (settingsPNG.Checked)
                return "PNG";

            if (settingsTIFF.Checked)
                return "TIFF";

            return "JPEG";
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

        private void centralizePanelRUC80()
        {
            picRUC80.Left = (panelRUC80.Width - picRUC80.Width) / 2;
            picRUC80.Top = (panelRUC80.Height - picRUC80.Height) / 2;
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

            tsbtnFolder.Text = "Save to (Autosave)...";
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
            return nPattern.Verify(value);
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

        private void switchImageFormat(string code)
        {
            settingsBMP.Checked = settingsBMP.Text == code;
            settingsGIF.Checked = settingsGIF.Text == code;
            settingsJPEG.Checked = settingsJPEG.Text == code;
            settingsPNG.Checked = settingsPNG.Text == code;
            settingsTIFF.Checked = settingsTIFF.Text == code;
            tslImageFormat.Text = $".{code}";
        }
        #endregion

        #region Custom Method: Screen Capturing
        // Method name quoted from madam M in 007 movies, "Take the bloody shot!".
        private void takeTheBloodyShot()
        {
            if (!isSettingsCorrect())
            {
                MessageBox.Show("There's something wrong in settings.", "CapCap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ImageFormat
            ImageFormat format = getImageFormat();

            // Take screen shot.
            var task = new Task(() => captureScreen(settings_Cursor.Checked, FBD.SelectedPath, tstbNamePattern.Text, vNextNumber++.ToString(), format));
            task.Start();

            vTasks.Add(task);
        }

        private bool isSettingsCorrect()
        {
            if (!isPrefixLegal(tstbNamePattern.Text))
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

        private ImageFormat getImageFormat()
        {
            if (settingsBMP.Checked)
                return ImageFormat.Bmp;

            if (settingsGIF.Checked)
                return ImageFormat.Gif;

            if (settingsJPEG.Checked)
                return ImageFormat.Jpeg;

            if (settingsPNG.Checked)
                return ImageFormat.Png;

            if (settingsTIFF.Checked)
                return ImageFormat.Tiff;

            return ImageFormat.Jpeg;
        }

        private void captureScreen(bool cursor, string folder, string pattern, string number, ImageFormat format)
        {
            var screenshot = new ScreenShot(cursor, folder, pattern, number, format);
            if (InvokeRequired)
                Invoke(OnScreenShotCaptured, screenshot.CaptureAndSave());
            else
                OnScreenShotCaptured(screenshot.CaptureAndSave());
        }

        private void ScreenShot_OnScreenShotCaptured(ScreenShotInfo info)
        {
            addRecord(info.FullName, info.Success);
            incrementNextNumber();
            notify(info.FileName, info.Success);

            if (vTasks.Count > 0)
                vTasks.Remove(vTasks[0]);
        }

        private void addRecord(string filename, bool success)
        {
            LV.Items.Add(vTotalNumber.ToString());
            LV.Items[LV.Items.Count - 1].SubItems.Add(filename);
            LV.Items[LV.Items.Count - 1].SubItems.Add(string.Format("{0}", DateTime.Now));
            LV.Items[LV.Items.Count - 1].SubItems.Add(success ? "Saved" : "Failed");
            vTotalNumber += 1;
            LV.TopItem = LV.Items[LV.Items.Count - 1];
        }

        private void incrementNextNumber()
        {
            tstbNumber.Text = (int.Parse(tstbNumber.Text) + 1).ToString();
            nPattern.IncrementNumber();
        }

        private void notify(string filename, bool success)
        {
            if (settings_Sound.Checked)
                playSound(success);

            if (settings_Notification.Checked)
            {
                sendNotification(filename, success);
            }
        }

        private void playSound(bool success)
        {
            soundplayer.Stream = success ? Resource.yes : Resource.no;
            soundplayer.Play();
        }

        private void sendNotification(string filename, bool success)
        {
            string info = string.Format("Screenshot {0} {1}", filename, success ? "saved." : "failed.");
            NOTI.ShowBalloonTip(2000, "CapCap", string.Format("{0}", info), success ? ToolTipIcon.Info : ToolTipIcon.Error);
        }
        #endregion

        private void settingsImageFormat_Clicked(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem)sender;
            switchImageFormat(mi.Text);
        }

        private void lnkRUC80_Click(object sender, EventArgs e)
        {
            panelRUC80.BringToFront();
        }

        private void RUC80_Click(object sender, EventArgs e)
        {
            if (isWelcomeScreen)
            {
                panelMain.BringToFront();
                isWelcomeScreen = false;
            }
            else
                panelAbout.BringToFront();
        }

        private void tsmiHelp_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\Readme.html"))
            {
                MessageBox.Show("Readme.html file is missing.", "CapCap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            webHelp.Navigate(Application.StartupPath + "\\Readme.html");
            panelHelp.BringToFront();
        }

        private void tsReturnToMainPanel_Click(object sender, EventArgs e)
        {
            panelMain.BringToFront();
        }
    }
}
