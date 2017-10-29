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
using System.Linq;
using System.Resources;
using System.Diagnostics;

namespace CapCap
{
    public partial class frmMain : Form
    {
        #region Debug
        // Debug: Show (Debug: test) menu item.
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
        #endregion

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

        private bool isHotKeySet = false;
        private bool isSettingHotKey = false;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private bool isHotKeyConflict = false;

        internal Dictionary<string, string> LangPack = Localization.EN_US.LanguagePack;
        private string vLanguage = "EN_US";
        private string vStatusCode = "Ready";
        /*
         * None
         * Ready
         * FileMissing
         */

        private delegate void WelcomeCloseedEventHandler();
        private event WelcomeCloseedEventHandler OnWelcomeClosed;

        public frmMain()
        {
            InitializeComponent();

            ///////////////////////////////

            initAutosave();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Initiation
            OnWelcomeClosed += FrmMain_OnWelcomeClosed;

            // Load Settings
            loadSettings();
            loadIcons();
            loadApplicationInfo();

            // LV
            LV.Columns.Add("No.", 35);
            LV.Columns.Add("Filename", 348);
            LV.Columns.Add("Time", 126);
            LV.Columns.Add("Status", 55);

            LV.FullRowSelect = true;
            LV.Dock = DockStyle.Fill;

            // tstb
            tstbNamePattern.Text = "CapImg_<year>-<month>-<day>_<hour>-<minute>-<second>_<3#>";
            tstbNumber.Text = nPattern.Number.ToString();

            tstbNamePattern.Visible = false;
            tstbNumber.Visible = false;
            tslImageFormat.Visible = false;

            tsSeparatorOfNumber.Visible = false;
            tslNumber.Visible = false;

            tsbtnInsertVariable.Visible = false;

            // tsbtnInsertVariable
            loadVariableToMenu();

            // panelMain
            panelMain.Dock = DockStyle.Fill;
            panelMain.BringToFront();

            // NOTI
            NOTI.Visible = true;

            // About
            panelAbout.Dock = DockStyle.Fill;

            label1.Left = (panelInnerAbout.Width - label1.Width) / 2;
            label_Author.Left = (panelInnerAbout.Width - label_Author.Width) / 2;
            label3.Left = (panelInnerAbout.Width - label3.Width) / 2;
            lnkWeibo.Left = (panelInnerAbout.Width - lnkWeibo.Width) / 2;
            btnCloseAboutPanel.Left = (panelInnerAbout.Width - btnCloseAboutPanel.Width) / 2;

            lnkRUC80.Left = label_Author.Right + 5;

            centralizePanelInnerAbout();

            // Help
            panelHelp.Dock = DockStyle.Fill;


            // RUC80
            panelRUC80.Dock = DockStyle.Fill;
            picRUC80.Left = (panelRUC80.Width - picRUC80.Width) / 2;
            picRUC80.Top = (panelRUC80.Height - picRUC80.Height) / 2;

            // Panel of new HotKey
            panelHotKey.Dock = DockStyle.Fill;
            centralizePanelHotKey();

            // Parallel
            OnScreenShotCaptured += ScreenShot_OnScreenShotCaptured;

            // Language
            switchLanguage(vLanguage);

            // RUC 80 Auto show.
            if (Properties.Settings.Default.ads)
                if (DateTime.Now.Year == 2017)
                    showRUC80();
        }

        private void FrmMain_OnWelcomeClosed()
        {
            if (isHotKeyConflict)
                tsbtnNewHotKey.PerformClick();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!isSettingHotKey)
                return base.ProcessCmdKey(ref msg, keyData);

            if (keyData.HasFlag(Keys.Escape) || keyData.HasFlag(Keys.Enter))
            {
                isSettingHotKey = false;
                try
                {
                    hotkey.Register();
                    panelMain.BringToFront();
                }
                catch (Exception ex)
                {
                    lab_NewHotKey.ForeColor = Color.Red;
                    isSettingHotKey = true;
                }
                return true;
            }

            if (!isAllowedKey(keyData))
                return base.ProcessCmdKey(ref msg, keyData);

            // Normal keys
            if (msg.Msg == WM_KEYDOWN || msg.Msg == WM_SYSKEYDOWN)
            {
                if (isHotKeySet)
                {
                    return true;
                }
                else
                {
                    processKeyDown(keyData);
                }
            }
            else if (msg.Msg == WM_KEYUP || msg.Msg == WM_SYSKEYUP)
            {
                // Seems like, code here is useless.
                // Don't know why, the Msg is always 256 (WM_KEYDOWN, WM_KEYFIRST)
                if (isHotKeySet)
                {
                    if (isSettingHotKey)
                        processKeyUp(keyData);
                    return true;
                }
                else
                {
                    processKeyUp(keyData);
                    return base.ProcessCmdKey(ref msg, keyData);
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
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
            {
                var fullname = LV.SelectedItems[0].SubItems[1].Text;
                if (File.Exists(fullname))
                    tslab_Status.Text = Auxiliary.GetFileName(fullname);
                else
                {
                    tslab_Status.Text = LangPack["tslab_Status_FileMissing"];
                    vStatusCode = "FileMissing";
                }
            }
            else
            {
                tslab_Status.Text = LangPack["tslab_Status_Ready"];
                vStatusCode = "Ready";
            }
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
            centralizePanelHotKey();
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
                tsbtnFolder.Text = Auxiliary.GetFolderName(FBD.SelectedPath) + "\\";
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
                tsbtnInsertVariable.Visible = true;
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
            if (isSettingHotKey)
                return;

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

                if (!isSettingHotKey)
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

        private void tsmiVariable_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var text = item.Text;

            insertVariable(text);
        }
        #region Custom Method: Auxiliary
        private void showRUC80()
        {
            panelRUC80.BringToFront();
        }

        private void loadSettings()
        {
            tsmi_Cursor.Checked = Properties.Settings.Default.capture_cursor;
            tsmi_Notification.Checked = Properties.Settings.Default.notify;
            tsmi_Sound.Checked = Properties.Settings.Default.play_sound;
            convertCode2ImageFormat(Properties.Settings.Default.image_format);
            tslImageFormat.Text = $".{convertImageFormat2Code()}";
            convertStringToHotKey(Properties.Settings.Default.hotkey);
            tsmi_ShowAds.Checked = Properties.Settings.Default.ads;

            // Overwrite existing file
            tsmi_OverWrite.Checked = Properties.Settings.Default.overwrite_existing_file;
            tsmi_ReName.Checked = !tsmi_OverWrite.Checked;

            // Language
            vLanguage = Properties.Settings.Default.language;
        }

        private void saveSettings()
        {
            Properties.Settings.Default.capture_cursor = tsmi_Cursor.Checked;
            Properties.Settings.Default.notify = tsmi_Notification.Checked;
            Properties.Settings.Default.play_sound = tsmi_Sound.Checked;
            Properties.Settings.Default.image_format = convertImageFormat2Code();
            Properties.Settings.Default.hotkey = tsbtnNewHotKey.Text;
            Properties.Settings.Default.ads = tsmi_ShowAds.Checked;

            // Overwrite existing file
            Properties.Settings.Default.overwrite_existing_file = tsmi_OverWrite.Checked;

            // Language
            Properties.Settings.Default.language = vLanguage;

            // Save settings
            Properties.Settings.Default.Save();
        }

        private void convertCode2ImageFormat(string code)
        {
            switch (code)
            {
                case "BMP":
                    tsmi_BMP.Checked = true;
                    break;
                case "GIF":
                    tsmi_GIF.Checked = true;
                    break;
                case "JPEG":
                    tsmi_JPEG.Checked = true;
                    break;
                case "PNG":
                    tsmi_PNG.Checked = true;
                    break;
                case "TIFF":
                    tsmi_TIFF.Checked = true;
                    break;
            }
        }

        private string convertImageFormat2Code()
        {
            if (tsmi_BMP.Checked)
                return "BMP";

            if (tsmi_GIF.Checked)
                return "GIF";

            if (tsmi_JPEG.Checked)
                return "JPEG";

            if (tsmi_PNG.Checked)
                return "PNG";

            if (tsmi_TIFF.Checked)
                return "TIFF";

            return "JPEG";
        }

        private void convertStringToHotKey(string value)
        {
            tsbtnNewHotKey.Text = value;
            lab_NewHotKey.Text = value;

            var keys = Auxiliary.GetKeysFromString(value);

            hotkey.Ctrl |= keys.HasFlag(Keys.Control);
            hotkey.Shift |= keys.HasFlag(Keys.Shift);
            hotkey.Alt |= keys.HasFlag(Keys.Alt);
            hotkey.Key = keys & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;

            hotkey.OnPressed += hotkey_OnPressed;

            try
            {
                hotkey.Register();
            }
            catch (Exception ex)
            {
                isHotKeyConflict = true;
            }
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

        private void centralizePanelHotKey()
        {
            lab_NewHotKey.Left = (panelHotKey.Width - lab_NewHotKey.Width) / 2;
            lab_NewHotKey.Top = (panelHotKey.Height - lab_NewHotKey.Height) / 2;
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

        private void switchImageFormat(string code)
        {
            tsmi_BMP.Checked = tsmi_BMP.Text == code;
            tsmi_GIF.Checked = tsmi_GIF.Text == code;
            tsmi_JPEG.Checked = tsmi_JPEG.Text == code;
            tsmi_PNG.Checked = tsmi_PNG.Text == code;
            tsmi_TIFF.Checked = tsmi_TIFF.Text == code;
            tslImageFormat.Text = $".{code}";
        }

        private void processKeyDown(Keys keyData)
        {
            lab_NewHotKey.ForeColor = Color.Black;
            lab_NewHotKey.Text = Auxiliary.GetCombinationFromKeyData(keyData);
            centralizePanelHotKey();

            if (pressedNormalKey(keyData))
                trySetNewHotKey(keyData);
        }

        private void processKeyUp(Keys keyData)
        {
            // Due to Windows Message issue, code here will never run.
            if (keyData == Keys.None)
            {
                panelMain.BringToFront();
                isSettingHotKey = false;
                isHotKeySet = false;
            }
        }

        private bool pressedNormalKey(Keys keyData)
        {
            var result = keyData.ToString().Replace(" ", "");
            result = result.Replace("ShiftKey,", "").Replace("Menu,", "").Replace("ControlKey,", "")
                .Replace("Shift", "").Replace("Control", "").Replace("Alt", "").Replace(",", "");
            return result.Length > 0 ? true : false;
        }

        private void trySetNewHotKey(Keys keyData)
        {
            isHotKeySet = true;

            hotkey.Ctrl = keyData.HasFlag(Keys.Control);
            hotkey.Alt = keyData.HasFlag(Keys.Alt);
            hotkey.Shift = keyData.HasFlag(Keys.Shift);

            hotkey.Key = keyData & ~Keys.Shift & ~Keys.Alt & ~Keys.Control;

            try
            {
                hotkey.Register();

                tsbtnNewHotKey.Text = lab_NewHotKey.Text;
                panelMain.BringToFront();
                isSettingHotKey = false;
            }
            catch (Exception ex)
            {
                lab_NewHotKey.ForeColor = Color.Red;
                isHotKeySet = false;
            }
            finally
            {
                isHotKeySet = false;
            }
        }

        private void loadVariableToMenu()
        {
            foreach (string text in nPattern.GetVariables())
            {
                var item = new ToolStripMenuItem(text);
                item.Click += tsmiVariable_Click;
                tsbtnInsertVariable.DropDownItems.Add(item);
            }
        }

        private void insertVariable(string variable)
        {
            var tb = tstbNamePattern;
            var insertpoint = tb.SelectionStart;
            tb.Select(0, insertpoint);
            var prefix = tb.SelectedText;
            tb.Select(insertpoint, tb.TextLength - insertpoint);
            var postfix = tb.SelectedText;
            tb.Text = $"{prefix}{variable}{postfix}";
            tb.SelectionStart = insertpoint + variable.Length;
        }

        private bool isAllowedKey(Keys keyData)
        {
            var keyCodes = Auxiliary.FilterKeyDataToList(keyData);

            foreach (var keyCode in keyCodes)
                if (!Auxiliary.AllowKey(keyCode))
                    return false;

            return true;
        }

        private void loadApplicationInfo()
        {
            label_Version.Text = Application.ProductVersion;
            Text = $"CapCap {Application.ProductVersion.Remove(3)}";

            label_Author.Text = $"Boss Ox / {Resource.ReleaseDate} / Beijing";

            cmslbl_CapCap.Text = Text;
            cmslbl_Author.Text = label_Author.Text;
        }

        private void updateOverWrite(bool overwrite)
        {
            if (overwrite)
            {
                tsmi_OverWrite.Checked = true;
                tsmi_ReName.Checked = false;
            }
            else
            {
                tsmi_OverWrite.Checked = false;
                tsmi_ReName.Checked = true;
            }
            updateSettingsIcon();
        }

        private void switchLanguage(string lang)
        {
            vLanguage = lang;
            switch (lang)
            {
                case "EN_US":
                    LangPack = Localization.EN_US.LanguagePack;
                    break;
                case "ZH_CN":
                    LangPack = Localization.ZH_CN.LanguagePack;
                    break;
            }
            updateUILanguage();
            updateLanguageMenuItemCheckState();
        }

        private void updateUILanguage()
        {
            tsbtnFolder.Text = LangPack["tsbtn_Folder"];

            tsddbMainMenu.Text = LangPack["tsddb_MainMenu"];
            tsmiOpenFolder.Text = LangPack["tsmi_OpenFolder"];
            tsmi_ClearHistory.Text = LangPack["tsmi_ClearHistory"];
            tsmiHelp.Text = LangPack["tsmi_Help"];
            tsmiAbout.Text = LangPack["tsmi_About"];
            tsmiExit.Text = LangPack["tsmi_Exit"];

            tsddbSettings.Text = LangPack["tsddb_Settings"];
            tsmi_ShowAds.Text = LangPack["tsmi_ShowAds"];
            tsmi_Language.Text = LangPack["tsmi_Language"];
            tsmi_Cursor.Text = LangPack["tsmi_Cursor"];
            tsmi_Notification.Text = LangPack["tsmi_Notification"];
            tsmi_Sound.Text = LangPack["tsmi_Sound"];
            tsmi_ReName.Text = LangPack["tsmi_Rename"];
            tsmi_OverWrite.Text = LangPack["tsmi_Overwrite"];

            LV.Columns[1].Text = LangPack["LV_Filename"];
            LV.Columns[2].Text = LangPack["LV_Time"];
            LV.Columns[3].Text = LangPack["LV_Status"];

            cmsmiExit.Text = LangPack["CMS_Exit"];
            cmsmiOpenFolder.Text = LangPack["CMS_OpenFolder"];
            cmsmiAbout.Text = LangPack["CMS_About"];

            // Update tslab_Status
            switch (vStatusCode)
            {
                case "None":
                    break;
                case "Ready":
                    tslab_Status.Text = LangPack["tslab_Status_Ready"];
                    break;
                case "FileMissing":
                    tslab_Status.Text = LangPack["tslab_Status_FileMissing"];
                    break;
            }
        }

        private void updateLanguageMenuItemCheckState()
        {
            foreach (var item in tsmi_Language.DropDownItems)
            {
                var mi = item as ToolStripMenuItem;
                mi.Checked = false;
            }

            switch (vLanguage)
            {
                case "EN_US":
                    tsmi_Lang_EN_US.Checked = true;
                    break;
                case "ZH_CN":
                    tsmi_Lang_ZH_CN.Checked = true;
                    break;
            }
        }

        private void loadIcons()
        {
            tsbtnFolder.Image = Resource.save.ToBitmap();
            tsddbMainMenu.Image = Resource.menu.ToBitmap();

            tsddbSettings.Image = Resource.setting.ToBitmap();
            tsbtnNewHotKey.Image = Resource.hotkey.ToBitmap();

            tsmiExit.Image = Resource.exit.ToBitmap();
            tsmiOpenFolder.Image = Resource.folder.ToBitmap();
            tsmi_ClearHistory.Image = Resource.clear.ToBitmap();
            tsmi_Language.Image = Resource.language.ToBitmap();

            cmsmiOpenFolder.Image = tsmiOpenFolder.Image;
            cmsmiExit.Image = tsmiExit.Image;

            updateSettingsIcon();
        }

        private void updateSettingsIcon()
        {
            tsmi_ShowAds.Image = tsmi_ShowAds.Checked ? null : Resource.RUC80.ToBitmap();
            tsmi_Cursor.Image = tsmi_Cursor.Checked ? null : Resource.cursor.ToBitmap();
            tsmi_Notification.Image = tsmi_Notification.Checked ? null : Resource.notification.ToBitmap();
            tsmi_Sound.Image = tsmi_Sound.Checked ? null : Resource.sound.ToBitmap();
            tsmi_ReName.Image = tsmi_ReName.Checked ? null : Resource.rename.ToBitmap();
            tsmi_OverWrite.Image = tsmi_OverWrite.Checked ? null : Resource.overwrite.ToBitmap();
        }
        #endregion

        #region Custom Method: Screen Capturing
        // Method name quoted from madam M in 007 movies, "Take the bloody shot!".
        private void takeTheBloodyShot()
        {
            if (!isSettingsCorrect())
            {
                MessageBox.Show(LangPack["MsgBox_InvalidSettings"], "CapCap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ImageFormat
            ImageFormat format = getImageFormat();

            // Take screen shot.
            var task = new Task(() => captureScreen(tsmi_Cursor.Checked, FBD.SelectedPath, tstbNamePattern.Text, vNextNumber++.ToString(), format));
            task.Start();

            vTasks.Add(task);
        }

        private bool isSettingsCorrect()
        {
            if (!isPrefixLegal(tstbNamePattern.Text))
            {
                addRecord("LV_Filename_InvalidNamePattern", false);
                return false;
            }

            if (!isNextNumberLegal(tstbNumber.Text))
            {
                addRecord(LangPack["LV_Filename_InvalidNumber"], false);
                return false;
            }

            if (!Directory.Exists(FBD.SelectedPath))
            {
                addRecord(LangPack["LV_Filename_InvalidFolder"], false);
                return false;
            }

            return true;
        }

        private ImageFormat getImageFormat()
        {
            if (tsmi_BMP.Checked)
                return ImageFormat.Bmp;

            if (tsmi_GIF.Checked)
                return ImageFormat.Gif;

            if (tsmi_JPEG.Checked)
                return ImageFormat.Jpeg;

            if (tsmi_PNG.Checked)
                return ImageFormat.Png;

            if (tsmi_TIFF.Checked)
                return ImageFormat.Tiff;

            return ImageFormat.Jpeg;
        }

        private void captureScreen(bool cursor, string folder, string pattern, string number, ImageFormat format)
        {
            var screenshot = new ScreenShot(cursor, folder, pattern, number, format, tsmi_OverWrite.Checked);
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

            tslab_Status.Text = string.Format(LangPack["tslab_Status_ScreenCaptureResult"],
                info.Success ? LangPack["word_Successed"] : LangPack["word_Failed"]);

            if (vTasks.Count > 0)
                vTasks.Remove(vTasks[0]);
        }

        private void addRecord(string filename, bool success)
        {
            LV.Items.Add(vTotalNumber.ToString());

            // Check if exception occured
            if (!success)
            {
                filename = string.Format(LangPack["LV_Filename_OtherException"], filename.Substring(1, filename.Length - 2));
            }

            LV.Items[LV.Items.Count - 1].SubItems.Add(filename);
            LV.Items[LV.Items.Count - 1].SubItems.Add(string.Format("{0}", DateTime.Now));
            LV.Items[LV.Items.Count - 1].SubItems.Add(success ? LangPack["LV_Status_Success"] : LangPack["LV_Status_Failed"]);
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
            if (tsmi_Sound.Checked)
                playSound(success);

            if (tsmi_Notification.Checked)
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
            string info = string.Format("{0} {1} {2}", LangPack["Screenshot"], filename,
                success ? LangPack["LV_Status_Success"] : LangPack["LV_Status_Failed"]);
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

            OnWelcomeClosed();
            isHotKeyConflict = false;
        }

        private void tsmiHelp_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\Readme.html"))
            {
                MessageBox.Show(LangPack["tsmi_Help_ReadmeMissing"], "CapCap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            webHelp.Navigate(Application.StartupPath + "\\Readme.html");
            panelHelp.BringToFront();
        }

        private void tsReturnToMainPanel_Click(object sender, EventArgs e)
        {
            panelMain.BringToFront();
        }

        private void tsbtnNewHotKey_Click(object sender, EventArgs e)
        {
            centralizePanelHotKey();
            isSettingHotKey = true;
            panelHotKey.BringToFront();
            if (hotkey.Registered)
                hotkey.Unregister();
        }

        private void frmMain_Activated(object sender, EventArgs e)
        {
            if (isSettingHotKey)
            {
                lab_NewHotKey.Text = LangPack["lab_NewHotKey_PressToSet"];
                centralizePanelHotKey();
            }
        }

        private void panelHotKey_Click(object sender, EventArgs e)
        {
            lab_NewHotKey.Text = LangPack["lab_NewHotKey_PressToSet"];
            centralizePanelHotKey();
        }

        private void tsmiOverWrite_Click(object sender, EventArgs e)
        {
            updateOverWrite(true);
        }

        private void tsmiReName_Click(object sender, EventArgs e)
        {
            updateOverWrite(false);
        }

        private void tsmiLanguagePack_Click(object sender, EventArgs e)
        {
            var menu_item = sender as ToolStripMenuItem;
            switch (menu_item.Text)
            {
                case "English":
                    switchLanguage("EN_US");
                    break;
                case "简体中文":
                    switchLanguage("ZH_CN");
                    break;
            }
        }

        private void tsmi_ClearHistory_Click(object sender, EventArgs e)
        {
            LV.Items.Clear();
            vTotalNumber = 1;
        }

        private void tsmi_Cursor_Click(object sender, EventArgs e)
        {
            updateSettingsIcon();
        }

        private void tsmi_Notification_Click(object sender, EventArgs e)
        {
            updateSettingsIcon();
        }

        private void tsmi_Sound_Click(object sender, EventArgs e)
        {
            updateSettingsIcon();
        }

        private void tsmi_ShowAds_Click(object sender, EventArgs e)
        {
            updateSettingsIcon();
        }

        private void cmsmiAbout_Click(object sender, EventArgs e)
        {
            tsmiAbout.PerformClick();
            this.Show();
            this.Activate();
        }
    }
}
