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
            NOTI.Icon = Resource.Icon_SysTrayCapture;
            System.Threading.Thread.Sleep(250);
            NOTI.Icon = Resource.Icon_Systray;

            GC.Collect();
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////

        // Windows API
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        private const int vShift = 10;

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

        // for cmsFile right click menu
        private string vSelectedFileName = "";

        // for preview resize form back
        private int vFormWidth = 0, vFormHeight = 0;
        private int vFormLeft = 0, vFormTop = 0;

        // for pb_Preview drag and move window
        private int vOldX = 0, vOldY = 0;
        private bool preventClickEvent = false;

        // for blocking hotkey press from preview.
        private bool isPreviewing = false;

        // Software Mode, for hotkey blocking.
        private enum SoftwareMode { Default, Main, Preview, About, RUC80, Hotkey, Manage }
        private SoftwareMode Mode = SoftwareMode.Main;

        private enum StatusStyle { Normal, Good, Bad }

        public frmMain()
        {
            InitializeComponent();

            ///////////////////////////////

            initAutosave();

            // Resize form.
            Size = new Size(600, 400);

            // Load Logo and Icon
            this.Icon = Resource.Icon_Logo;
            pbLogo.Image = Resource.Image_Logo;
            NOTI.Icon = Resource.Icon_Systray;
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
            panelAbout.BackColor = Color.White;

            ResetPanelInnerAbout();
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

            // Panel of Preview
            panelPreview.Dock = DockStyle.Fill;
            pb_Preview.Dock = DockStyle.Fill;
            pb_Preview.MouseWheel += pb_Preview_MouseWheel;

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
            if (isPreviewing)
            {
                pb_Preview_KeyEvent(msg, keyData);
                return base.ProcessCmdKey(ref msg, keyData);
            }

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
                cmsSysTray.Show(Control.MousePosition);
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
                    SetStatus(Auxiliary.GetFileName(fullname));
                else
                {
                    SetStatus(LangPack["tslab_Status_FileMissing"], StatusStyle.Bad);
                    vStatusCode = "FileMissing";
                }
                vSelectedFileName = fullname;
            }
            else
            {
                SetStatus(LangPack["tslab_Status_Ready"]);
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

            if (isPreviewing)
            {
                cmsPreview_miReturn.PerformClick();
            }

            doHotKeyColorFeedback();
            ToggleSysTrayIcon();

            panelMain.BringToFront();
            takeTheBloodyShot();
        }

        private void LV_DoubleClick(object sender, EventArgs e)
        {
            if (LV.SelectedItems.Count == 0) return;
            if (File.Exists(LV.SelectedItems[0].SubItems[1].Text))
            {
                var shift = GetAsyncKeyState((int)Keys.ShiftKey);
                var ctrl = GetAsyncKeyState((int)Keys.ControlKey);
                if (shift == 0 && ctrl == 0)
                {
                    pb_Preview.Image = Image.FromFile(LV.SelectedItems[0].SubItems[1].Text);
                    panelPreview.BringToFront();
                    ResizeFormFitImage();
                    isPreviewing = true;
                }
                else if (shift != 0 && ctrl == 0)
                {
                    cmsFile_miOpen.PerformClick();
                }
                else if (shift == 0 && ctrl != 0)
                {
                    cmsFile_miOpenFolder.PerformClick();
                }
            }
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
            lblVersion.Text = Application.ProductVersion;
            Text = $"CapCap {Application.ProductVersion.Remove(3)}";

            lblAuthor.Text = $"Boss Ox / {Resource.ReleaseDate} / Beijing";

            cmslbl_CapCap.Text = Text;
            cmslbl_Author.Text = lblAuthor.Text;

            cmsPreview_lblCapCap.Text = Text;
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

            cmsmiExit.Text = LangPack["cmsSysTray_Exit"];
            cmsmiOpenFolder.Text = LangPack["cmsSysTray_OpenFolder"];
            cmsmiAbout.Text = LangPack["cmsSysTray_About"];

            btnCloseAboutPanel.Text = LangPack["btnCloseAboutPanel"];
            lblDescription.Text = LangPack["lbl_AboutDescription"];

            ResetPanelInnerAbout();

            // File context menu.
            cmsFile_miPreview.Text = LangPack["cmsFile_miPreview"];
            cmsFile_miPreview.ShortcutKeyDisplayString = LangPack["word_DblClick"];
            cmsFile_miOpen.Text = LangPack["cmsFile_miOpen"];
            cmsFile_miOpen.ShortcutKeyDisplayString = $"Shift+{LangPack["word_DblClick"]}";
            cmsFile_miOpenFolder.Text = LangPack["cmsFile_miOpenFolder"];
            cmsFile_miOpenFolder.ShortcutKeyDisplayString = $"Ctrl+{LangPack["word_DblClick"]}";
            cmsFile_miClearHistory.Text = LangPack["cmsFile_miClearHistory"];
            cmsFile_miCopyImage.Text = LangPack["cmsFile_miCopyImage"];
            cmsFile_miCopyFile.Text = LangPack["cmsFile_miCopyFile"];
            cmsFile_miMore.Text = LangPack["cmsFile_miMore"];

            // Preview context menu
            cmsPreview_miCopyImage.Text = LangPack["cmsPreview_miCopyImage"];
            cmsPreview_miCopyFile.Text = LangPack["cmsPreview_miCopyFile"];
            cmsPreview_miZoomIn.Text = LangPack["cmsPreview_miZoomIn"];
            cmsPreview_miZoomOut.Text = LangPack["cmsPreview_miZoomOut"];
            cmsPreview_miPrevious.Text = LangPack["cmsPreview_miPrevious"];
            cmsPreview_miNext.Text = LangPack["cmsPreview_miNext"];
            cmsPreview_miLockTop.Text = LangPack["cmsPreview_miLockTop"];
            cmsPreview_miMaximize.Text = LangPack["cmsPreview_miMaximize"];
            cmsPreview_miReturn.Text = LangPack["cmsPreview_miReturn"];

            // Update tslab_Status
            switch (vStatusCode)
            {
                case "None":
                    break;
                case "Ready":
                    SetStatus(LangPack["tslab_Status_Ready"], StatusStyle.Normal);
                    break;
                case "FileMissing":
                    SetStatus(LangPack["tslab_Status_FileMissing"], StatusStyle.Bad);
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
            // Icon in main ToolBar.
            tsbtnFolder.Image = Resource.Icon_Save.ToBitmap();
            tsddbMainMenu.Image = Resource.Icon_MainMenu.ToBitmap();

            // Icon in main StatusBar.
            tsddbSettings.Image = Resource.Icon_Settings.ToBitmap();
            tsbtnNewHotKey.Image = Resource.Icon_Hotkey.ToBitmap();

            // Icon in SysTray context menu.
            cmsmiOpenFolder.Image = Resource.Icon_Folder.ToBitmap();
            cmsmiExit.Image = Resource.Icon_Exit.ToBitmap();

            // Icon in Main menu
            tsmiOpenFolder.Image = Resource.Icon_Folder.ToBitmap();
            tsmi_ClearHistory.Image = Resource.Icon_Clear.ToBitmap();
            tsmiExit.Image = Resource.Icon_Exit.ToBitmap();

            // Icon in Settings menu.
            tsmi_ShowAds.Image = Resource.Icon_RUC80.ToBitmap();
            tsmi_Language.Image = Resource.Icon_Language.ToBitmap();
            tsmi_Cursor.Image = Resource.Icon_Cursor.ToBitmap();
            tsmi_Notification.Image = Resource.Icon_Notification.ToBitmap();
            tsmi_Sound.Image = Resource.Icon_Sound.ToBitmap();
            tsmi_ReName.Image = Resource.Icon_Rename.ToBitmap();
            tsmi_OverWrite.Image = Resource.Icon_Overwrite.ToBitmap();

            // Icon in file context menu.
            cmsFile_miPreview.Image = Resource.Icon_Preview.ToBitmap();
            cmsFile_miOpen.Image = Resource.Icon_OpenWith.ToBitmap();
            cmsFile_miOpenFolder.Image = Resource.Icon_Folder.ToBitmap();
            cmsFile_miClearHistory.Image = Resource.Icon_Clear.ToBitmap();
            cmsFile_miCopyImage.Image = Resource.Icon_Image.ToBitmap();
            cmsFile_miCopyFile.Image = Resource.Icon_ImageFile.ToBitmap();
            cmsFile_miMore.Image = Resource.Icon_Forward.ToBitmap();

            // Icon in preview context menu.
            cmsPreview_miCopyImage.Image = Resource.Icon_Image.ToBitmap();
            cmsPreview_miCopyFile.Image = Resource.Icon_ImageFile.ToBitmap();
            cmsPreview_miLockTop.Image = Resource.Icon_TopMost.ToBitmap();
            cmsPreview_miMaximize.Image = Resource.Icon_Maximize.ToBitmap();
            cmsPreview_miOpacity100.Image = Resource.Icon_Opacity.ToBitmap();
            cmsPreview_miReturn.Image = Resource.Icon_Return.ToBitmap();
        }

        private async void doHotKeyColorFeedback()
        {
            tsbtnNewHotKey.BackColor = Color.YellowGreen;

            await Task.Delay(300);

            tsbtnNewHotKey.BackColor = SystemColors.Control;
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

            var status = string.Format(LangPack["tslab_Status_ScreenCaptureResult"],
                info.Success ? LangPack["word_Successed"] : LangPack["word_Failed"]);
            SetStatus(status, StatusStyle.Normal);

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
            if (WindowState != FormWindowState.Minimized)
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

        private async void playSound(bool success)
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
            SetStatus(LangPack["tslab_Status_HistoryCleared"], StatusStyle.Good);
        }

        private void cmsmiAbout_Click(object sender, EventArgs e)
        {
            tsmiAbout.PerformClick();
            this.Show();
            this.Activate();
        }

        private void LV_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                if (LV.SelectedItems.Count > 0)
                    ShowCMSFile(LV.SelectedItems[0].SubItems[1].Text, e.X, e.Y);
        }

        private void ShowCMSFile(string filename, int X, int Y)
        {
            // Switch menu content depends on file existence.
            var fileExists = File.Exists(filename);
            cmsFile_miPreview.Visible = fileExists;
            cmsFile_miOpen.Visible = fileExists;
            cmsFile_miOpenFolder.Visible = fileExists;
            cmsFile_sep2.Visible = fileExists;
            cmsFile_sep3.Visible = fileExists;
            cmsFile_miCopyImage.Visible = fileExists;
            cmsFile_miCopyFile.Visible = fileExists;

            // Display file name.
            cmsFile_lblFilename.Text = Auxiliary.GetFileName(filename);
            cmsFile.PerformLayout();

            // Show menu.
            cmsFile.Show(LV, X, Y);
        }

        private void cmsFile_miOpen_Click(object sender, EventArgs e)
        {
            OpenFile(vSelectedFileName);
        }

        private void OpenFile(string filename)
        {
            System.Diagnostics.Process.Start(filename);
        }

        private void cmsFile_miOpenFolder_Click(object sender, EventArgs e)
        {
            OpenFile(Auxiliary.GetFolder(vSelectedFileName));
        }

        private void cmsFile_miClearHistory_Click(object sender, EventArgs e)
        {
            tsmi_ClearHistory.PerformClick();
        }

        private void cmsFile_miCopyImage_Click(object sender, EventArgs e)
        {
            var image = Bitmap.FromFile(vSelectedFileName);
            Clipboard.SetImage(image);
            SetStatus(LangPack["tslab_Status_ImageCopied"], StatusStyle.Good);
        }

        private void cmsFile_miCopyFile_Click(object sender, EventArgs e)
        {
            Clipboard.SetFileDropList(new System.Collections.Specialized.StringCollection() { vSelectedFileName });
            SetStatus(LangPack["tslab_Status_FileCopied"], StatusStyle.Good);
        }

        private void pb_Preview_Click(object sender, EventArgs e)
        {
            if (preventClickEvent || cmsPreview_miLockTop.Checked || WindowState == FormWindowState.Maximized)
                return;

            cmsPreview_miReturn.PerformClick();
        }

        private void SetStatus(string content, StatusStyle style = StatusStyle.Normal)
        {
            tslab_Status.Text = content;

            switch (style)
            {
                case StatusStyle.Good: tslab_Status.ForeColor = Color.Green; break;
                case StatusStyle.Normal: tslab_Status.ForeColor = Color.Black; break;
                case StatusStyle.Bad: tslab_Status.ForeColor = Color.Red; break;
            }
        }

        private void ResizeFormFitImage()
        {
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var ratioScreen = (float)screenWidth / screenHeight;

            var pbHeight = pb_Preview.Height;
            var pbWidth = pb_Preview.Width;
            var ratioPB = (float)pbWidth / pbHeight;

            var ratio = ratioScreen / ratioPB;

            var newFormHeight = 0;
            var newFormWidth = 0;

            if (ratio == 1) // Perfect match.
            {
                newFormHeight = this.Height;
                newFormWidth = this.Width;
            }
            else // Otherwise.
            {
                newFormHeight = (int)Math.Round(this.Width / ratioScreen);
                newFormWidth = this.Width;
            }

            vFormHeight = this.Height;
            vFormWidth = this.Width;
            vFormLeft = this.Left;
            vFormTop = this.Top;

            // Resize form.
            this.MinimumSize = new Size(0, 0);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Height = newFormHeight;
            this.Width = newFormWidth;

            this.BackColor = Color.FromArgb(255, 1, 0, 0);
            this.TransparencyKey = this.BackColor;
            this.AllowTransparency = true;
        }

        private void pb_Preview_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && WindowState == FormWindowState.Normal)
            {
                this.Left += e.X - vOldX;
                this.Top += e.Y - vOldY;
                vFormLeft += e.X - vOldX;
                vFormTop += e.Y - vOldY;
                preventClickEvent = true;
            }
        }

        private void pb_Preview_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                preventClickEvent = WindowState == FormWindowState.Normal ? false : true;
            else if (e.Button == MouseButtons.Middle)
            {
                WindowState = WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal;
                preventClickEvent = WindowState == FormWindowState.Normal ? false : true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                cmsPreview_lblInfo.Text = string.Format(LangPack["cmsPreview_lblInfo"], LV.SelectedItems[0].Index + 1, LV.Items.Count);
                cmsPreview_miPrevious.Enabled = LV.SelectedItems[0].Index == 0 ? false : true;
                cmsPreview_miNext.Enabled = LV.SelectedItems[0].Index == LV.Items.Count - 1 ? false : true;
                cmsPreview_miZoomIn.Enabled = WindowState == FormWindowState.Normal ? true : false;
                cmsPreview_miZoomOut.Enabled = WindowState == FormWindowState.Normal ? true : false;
                cmsPreview_lblFilename.Text = Auxiliary.GetFileName(vSelectedFileName);
                cmsPreview_miMaximize.Checked = WindowState == FormWindowState.Maximized;
                cmsPreview.PerformLayout();
                cmsPreview.Show(pb_Preview, e.X, e.Y);
                // Set preventClickEvent to false after context menu is closed.
            }
        }

        private void cmsPreview_miCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(pb_Preview.Image);
        }

        private void cmsPreview_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            preventClickEvent = false;
        }

        private void pb_Preview_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                vOldX = e.X;
                vOldY = e.Y;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                preventClickEvent = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                preventClickEvent = true;
            }
        }

        private void ResizeFormBack()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Left = Left + (Width - vFormWidth) / 2;
            this.Top = Top + (Height - vFormHeight) / 2;
            this.Height = vFormHeight;
            this.Width = vFormWidth;
            this.Left = vFormLeft;
            this.Top = vFormTop;
            this.MinimumSize = new Size(600, 400);

            Left = Left < 0 ? 0 : Left;
            Top = Top < 0 ? 0 : Top;

            Left = Right > Screen.PrimaryScreen.Bounds.Width ? Screen.PrimaryScreen.Bounds.Width - Width : Left;
            Top = Bottom > Screen.PrimaryScreen.Bounds.Height ? Screen.PrimaryScreen.WorkingArea.Height - Height : Top;

            this.BackColor = SystemColors.Control;
            this.AllowTransparency = false;
        }

        private void cmsPreview_miLockTop_Click(object sender, EventArgs e)
        {
            TopMost = cmsPreview_miLockTop.Checked;
        }

        private void cmsPreview_miReturn_Click(object sender, EventArgs e)
        {
            TopMost = false;
            cmsPreview_miLockTop.Checked = false;
            cmsPreview_miOpacity100.PerformClick();
            preventClickEvent = false;

            // Return to main UI and resize the form back.
            ResizeFormBack();
            panelMain.BringToFront();
            pb_Preview.Image.Dispose();

            isPreviewing = false;
        }

        private void pb_Preview_DoubleClick(object sender, EventArgs e)
        {
            WindowState = WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal;
        }

        private void cmsPreview_miMaximize_Click(object sender, EventArgs e)
        {
            WindowState = cmsPreview_miMaximize.Checked ? FormWindowState.Maximized : FormWindowState.Normal;
        }

        private void cmsPreview_miOpacity_Click(object sender, EventArgs e)
        {
            cmsPreview_miOpacity100.Checked = cmsPreview_miOpacity100 == sender;
            cmsPreview_miOpacity75.Checked = cmsPreview_miOpacity75 == sender;
            cmsPreview_miOpacity50.Checked = cmsPreview_miOpacity50 == sender;
            cmsPreview_miOpacity25.Checked = cmsPreview_miOpacity25 == sender;
            Opacity = (float)int.Parse(((ToolStripMenuItem)sender).Tag.ToString()) / 100;
        }

        private void pb_Preview_MouseWheel(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                ResizeFormOnMouseWheel(e.Delta, e.X, e.Y);
        }

        private void cmsFile_miMore_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Functionality under development.\nComing soon.",
                Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cmsFile_miPreview_Click(object sender, EventArgs e)
        {
            LV_DoubleClick(this, null);
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            // Why it doesn't work here when pressing keys in pb_Preview ?
            //MessageBox.Show(e.KeyCode.ToString());
        }

        private void ResizeFormOnMouseWheel(int delta, int x, int y)
        {
            // Calculate parameters.
            int unit = 60;
            int step = delta / 120;

            int oldWidth = this.Width;
            int oldHeight = this.Height;

            float ratio = (float)this.Width / this.Height;

            int newWidth = Width + unit * step;
            int newHeight = (int)Math.Round(newWidth / ratio);

            // If too small, prevent zoom out (smaller).
            if (newWidth >= 300 || delta >= 0)
            {
                // Zoom: Height, Width.
                Size = new Size(newWidth, newHeight);
            }

            float left_proportion = (float)x / oldWidth;
            float top_proportion = (float)y / oldHeight;

            // Reposition: Left, Top.
            Location = new Point(Left - (int)Math.Round((Width - oldWidth) * left_proportion),
                    Top - (int)Math.Round((Height - oldHeight) * top_proportion));
        }

        private async void ToggleSysTrayIcon()
        {
            NOTI.Icon = Resource.Icon_SysTrayCapture;
            await Task.Delay(300);
            NOTI.Icon = Resource.Icon_Systray;
        }

        private void pb_Preview_KeyEvent(Message msg, Keys keyData)
        {
            if (msg.Msg == WM_KEYDOWN)
            {
                // Methods that can be invoked anytime.
                switch (keyData.ToString())
                {
                    case "Left": PreviewImageOffset(-1); break;
                    case "Right": PreviewImageOffset(1); break;
                    case "Escape": // Return to main panel with Escape.
                        if (WindowState == FormWindowState.Maximized)
                            WindowState = FormWindowState.Normal;
                        else if (WindowState == FormWindowState.Normal)
                            cmsPreview_miReturn.PerformClick();
                        break;
                    case "Space": // Toggle window state between normal and maximized with Space.
                        WindowState = WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal;
                        break;
                    case "C, Control": // Copy image with Ctrl + C.
                        cmsPreview_miCopyImage.PerformClick();
                        break;
                    case "C, Shift, Control": // Copy file with Ctrl + Shift + C.
                        cmsPreview_miCopyFile.PerformClick();
                        break;
                    case "Oemtilde": // TopMost with ` (Tilde).
                        cmsPreview_miLockTop.PerformClick();
                        break;
                    case "D1": // 100% opacity with 1.
                        cmsPreview_miOpacity100.PerformClick();
                        break;
                    case "D2": // 75% opacity with 2.
                        cmsPreview_miOpacity75.PerformClick();
                        break;
                    case "D3": // 50% opacity with 3.
                        cmsPreview_miOpacity50.PerformClick();
                        break;
                    case "D4": // 25% opacity with 4.
                        cmsPreview_miOpacity25.PerformClick();
                        break;
                    default:
                        //MessageBox.Show(keyData.ToString());
                        break;
                }

                // Following methods should not be invoked when Maximized.
                if (WindowState == FormWindowState.Maximized) return;
                switch (keyData.ToString())
                {
                    case "Up": Zoom(120); break;
                    case "Down": Zoom(-120); break;
                }
            }
        }

        private void PreviewImageOffset(int offset)
        {
            int currentIndex = LV.SelectedItems[0].Index;
            int newIndex = currentIndex + offset;

            if (newIndex >= 0 && newIndex <= LV.Items.Count - 1)
            {
                LV.Items[currentIndex].Selected = false;
                LV.Items[newIndex].Selected = true;

                string filename = LV.SelectedItems[0].SubItems[1].Text;
                if (File.Exists(filename))
                {
                    pb_Preview.Image.Dispose();
                    pb_Preview.Image = Image.FromFile(filename);
                }
                else
                {
                    PreviewImageOffset(offset);
                }
            }
        }
        private void Zoom(int Delta, int StepLength = 50)
        {
            Zoom(Delta, new Point(Width / 2, Height / 2), StepLength);
        }

        private void Zoom(int Delta, Point Center, int StepLength = 50)
        {
            float ratio = (float)Width / Height;
            float left_proportion = (float)Center.X / Width;
            float top_proportion = (float)Center.Y / Height;

            int step = Delta / 120;

            int oldHeight = Height;
            int oldWidth = Width;

            int newWidth = Width + StepLength * step;
            int newHeight = (int)Math.Round(newWidth / ratio);

            if (newWidth >= 300 || Delta >= 0)
            {
                Size = new Size(newWidth, newHeight);

                Location = new Point(Left - (int)Math.Round((Width - oldWidth) * left_proportion),
                    Top - (int)Math.Round((Height - oldHeight) * top_proportion));
            }
        }

        private void cmsPreview_miCopyFile_Click(object sender, EventArgs e)
        {
            if (File.Exists(vSelectedFileName))
                Clipboard.SetFileDropList(new System.Collections.Specialized.StringCollection() { vSelectedFileName });
        }

        private void lnkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"mailto://BO_SoftwareService@yeah.net");
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            if (isPreviewing)
            {
                SuspendLayout();
                AllowTransparency = false;
            }
            base.OnResizeBegin(e);
        }

        private void cmsPreview_miPrevious_Click(object sender, EventArgs e)
        {
            PreviewImageOffset(-1);
        }

        private void cmsPreview_miNext_Click(object sender, EventArgs e)
        {
            PreviewImageOffset(1);
        }

        private void cmsPreview_miZoomIn_Click(object sender, EventArgs e)
        {
            Zoom(120);
        }

        private void LV_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData.ToString())
            {
                case "C, Control":
                    cmsFile_miCopyImage.PerformClick();
                    break;
                case "C, Shift, Control":
                    cmsFile_miCopyFile.PerformClick();
                    break;
            }
        }

        private void cmsPreview_miZoomOut_Click(object sender, EventArgs e)
        {
            Zoom(-120);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            if (isPreviewing)
            {
                ResumeLayout();
                AllowTransparency = true;
            }
            base.OnResizeEnd(e);
        }

        private void ResetPanelInnerAbout()
        {
            pbLogo.Left = 10; pbLogo.Top = 10;

            lblCapCap.Left = pbLogo.Left + pbLogo.Width / 2 - lblCapCap.Width / 2;
            lblCapCap.Top = pbLogo.Bottom + 5;

            lblVersion.Left = pbLogo.Left + pbLogo.Width / 2 - lblVersion.Width / 2;
            lblVersion.Top = lblCapCap.Bottom + 5;

            lblDescription.Left = pbLogo.Right + 30;
            lblDescription.Top = 45;

            lblAuthor.Left = lblDescription.Left;
            lblAuthor.Top = lblDescription.Bottom + 14;

            lnkRUC80.Left = lblAuthor.Right + 10;
            lnkRUC80.Top = lblAuthor.Top + 2;

            lnkEmail.Left = lblDescription.Left + 2;
            lnkEmail.Top = lblDescription.Bottom + 63;

            lnkWeibo.Left = lblDescription.Left + 2;
            lnkWeibo.Top = lnkEmail.Bottom + 10;

            panelInnerAbout.Width = (lblDescription.Right > lnkRUC80.Right ?
                lblDescription.Right : lnkRUC80.Right) + 10;

            btnCloseAboutPanel.Left = (panelInnerAbout.Width - btnCloseAboutPanel.Width) / 2;
        }
    }
}
