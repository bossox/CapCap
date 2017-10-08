// Boss Ox / 2017.10.02 / Beijing @RUC

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace HotKeys
{
    public enum Modifier
    {
        None = 0,
        Alt = 1,
        Ctrl = 2,
        Shift = 4,
        Win = 8
    }

    public sealed class HotKeyEventArgs : EventArgs
    {
        public bool Ctrl { get; } = false;
        public bool Alt { get; } = false;
        public bool Shift { get; } = false;
        public bool Win { get; } = false;
        public Keys Key { get; } = Keys.None;

        public HotKeyEventArgs(bool ctrl, bool alt, bool shift, bool win, Keys key)
        {
            Ctrl = ctrl;
            Alt = alt;
            Shift = shift;
            Win = win;
            Key = key;
        }
    }

    public delegate void HotKeyEventHandler(object sender, HotKeyEventArgs e);

    public sealed class HotKey : NativeWindow
    {
        // Import Windows API
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, Keys vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("kernel32.dll")]
        private static extern int AddAtom(String lpString);
        [DllImport("kernel32.dll")]
        private static extern int DeleteAtom(UInt32 nAtom);

        private string hotkeyGUID;
        private int hotkeyID;
        private const int WM_HOTKEY = 0x0312;

        // Event
        public event HotKeyEventHandler OnRegistered;
        public event HotKeyEventHandler OnUnregistered;
        public event HotKeyEventHandler OnPressed;

        #region Properties
        private bool _Registered { get; set; } = false;
        public bool Registered { get { return _Registered; } }
        public bool AllowModification { get { return !_Registered; } }

        private bool _Ctrl = false;
        public bool Ctrl
        {
            get
            {
                return _Ctrl;
            }
            set
            {
                if (AllowModification)
                {
                    _Ctrl = value;
                    if (value)
                        Modifiers |= Modifier.Ctrl;
                    else
                        Modifiers &= ~Modifier.Ctrl;
                }
                else
                    throw new Exception("Modification disallowed for the hotkey is already registered.");
            }
        }

        private bool _Alt = false;
        public bool Alt
        {
            get
            {
                return _Alt;
            }
            set
            {
                if (AllowModification)
                {
                    _Alt = value;
                    if (value)
                        Modifiers |= Modifier.Alt;
                    else
                        Modifiers &= ~Modifier.Alt;
                }
                else
                    throw new Exception("Modification disallowed for the hotkey is already registered.");
            }
        }

        private bool _Shift = false;
        public bool Shift
        {
            get
            {
                return _Shift;
            }
            set
            {
                if (AllowModification)
                {
                    _Shift = value;
                    if (value)
                        Modifiers |= Modifier.Shift;
                    else
                        Modifiers &= ~Modifier.Shift;
                }
                else
                    throw new Exception("Modification disallowed for the hotkey is already registered.");
            }
        }

        private bool _Win = false;
        public bool Win
        {
            get
            {
                return _Win;
            }
            set
            {
                if (AllowModification)
                {
                    _Win = value;
                    if (value)
                        Modifiers |= Modifier.Win;
                    else
                        Modifiers &= ~Modifier.Win;
                }
                else
                    throw new Exception("Modification disallowed for the hotkey is already registered.");
            }
        }

        private Keys _Key = Keys.None;
        public Keys Key
        {
            get
            {
                return _Key;
            }
            set
            {
                if (AllowModification)
                    _Key = value;
                else
                    throw new Exception("Modification disallowed for the hotkey is already registered.");
            }
        }

        private Modifier _Modifiers = Modifier.None;
        public Modifier Modifiers
        {
            get
            {
                return _Modifiers;
            }
            set
            {
                if (AllowModification)
                {
                    _Modifiers = value;
                    updateModifiers();
                }
                else
                    throw new Exception("Modification disallowed for the hotkey is already registered.");
            }
        }
        #endregion

        public HotKey() { Initiate(); }

        public HotKey(Modifier modifiers, Keys key)
        {
            Initiate();
            Modifiers = modifiers;
            Key = key;
        }

        ~HotKey() { if (Registered) Unregister(); DestroyHandle(); }

        // Create handle to receive Windows message.
        private void Initiate() { CreateHandle(new CreateParams()); }

        // Filter out HotKey message and raise event.
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && Registered)
            {
                OnPressed?.Invoke(this, new HotKeyEventArgs(Ctrl, Alt, Shift, Win, Key));
            }

            base.WndProc(ref m);
        }

        public void Register()
        {
            if (Registered)
                throw new Exception("HotKey is already registed to current process.");

            updateHotKeyID();
            if (!RegisterHotKey(Handle, hotkeyID, (int)Modifiers, Key))
                throw new Exception("HotKey registration failed.");

            _Registered = true;
            OnRegistered?.Invoke(this, new HotKeyEventArgs(Ctrl, Alt, Shift, Win, Key));
        }

        public void Unregister()
        {
            if (!Registered)
                throw new Exception("HotKey is not registed to current process yet.");

            if (!UnregisterHotKey(Handle, hotkeyID))
                throw new Exception("HotKey unregistration failed.");

            _Registered = false;
            OnUnregistered?.Invoke(this, new HotKeyEventArgs(Ctrl, Alt, Shift, Win, Key));
        }

        public override string ToString()
        {
            string result = "";

            if (Ctrl)
                result += "Ctrl + ";

            if (Alt)
                result += "Alt + ";

            if (Shift)
                result += "Shift + ";

            if (Win)
                result += "Win + ";

            result += $"{Key.ToString()}";

            return result;
        }

        private void updateHotKeyID()
        {
            // Generate new process-unique id.
            hotkeyGUID = Guid.NewGuid().ToString();
            hotkeyID = AddAtom(hotkeyGUID);
        }

        private void updateModifiers()
        {
            _Ctrl = (_Modifiers & Modifier.Ctrl) > 0;
            _Alt = (_Modifiers & Modifier.Alt) > 0;
            _Shift = (_Modifiers & Modifier.Shift) > 0;
            _Win = (_Modifiers & Modifier.Win) > 0;
        }


    }
}
