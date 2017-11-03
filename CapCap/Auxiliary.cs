using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapCap
{
    internal static class Auxiliary
    {
        private static HashSet<Keys> hashAllowedKeys = new HashSet<Keys>()
        {
            // Control Key
            Keys.ControlKey,
            Keys.ShiftKey,
            Keys.Menu,
            Keys.Control,
            Keys.Shift,
            Keys.Alt,

            // Function Key
            Keys.F1,
            Keys.F2,
            Keys.F3,
            Keys.F4,
            Keys.F5,
            Keys.F6,
            Keys.F7,
            Keys.F8,
            Keys.F9,
            Keys.F10,
            Keys.F11,
            Keys.F12,

            // Number Key
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.D5,
            Keys.D6,
            Keys.D7,
            Keys.D8,
            Keys.D9,
            Keys.D0,

            // Letter Key
            Keys.A,
            Keys.B,
            Keys.C,
            Keys.D,
            Keys.E,
            Keys.F,
            Keys.G,
            Keys.H,
            Keys.I,
            Keys.J,
            Keys.K,
            Keys.L,
            Keys.M,
            Keys.N,
            Keys.O,
            Keys.P,
            Keys.Q,
            Keys.R,
            Keys.S,
            Keys.T,
            Keys.U,
            Keys.V,
            Keys.W,
            Keys.X,
            Keys.Y,
            Keys.Z,

            // Symbol Key
            //Keys.Oemtilde,          // `
            //Keys.OemMinus,          // -
            //Keys.Oemplus,           // +
            //Keys.OemOpenBrackets,   // [
            //Keys.Oem6,              // ]
            //Keys.Oem1,              // ;
            //Keys.Oem7,              // '
            //Keys.Oemcomma,          // ,
            //Keys.OemPeriod,         // .
            //Keys.Oem5,              // \
            //Keys.OemQuestion,       // /
        };

        // Refer to AllowedKeys dictionary.
        private static Dictionary<string, Keys> dictStringKey = new Dictionary<string, Keys>()
        {
            // Letter
            {"A", Keys.A},
            {"B", Keys.B},
            {"C", Keys.C},
            {"D", Keys.D},
            {"E", Keys.E},
            {"F", Keys.F},
            {"G", Keys.G},
            {"H", Keys.H},
            {"I", Keys.I},
            {"J", Keys.J},
            {"K", Keys.K},
            {"L", Keys.L},
            {"M", Keys.M},
            {"N", Keys.N},
            {"O", Keys.O},
            {"P", Keys.P},
            {"Q", Keys.Q},
            {"R", Keys.R},
            {"S", Keys.S},
            {"T", Keys.T},
            {"U", Keys.U},
            {"V", Keys.V},
            {"W", Keys.W},
            {"X", Keys.X},
            {"Y", Keys.Y},
            {"Z", Keys.Z},

            // Number
            {"D0", Keys.D0},
            {"D1", Keys.D1},
            {"D2", Keys.D2},
            {"D3", Keys.D3},
            {"D4", Keys.D4},
            {"D5", Keys.D5},
            {"D6", Keys.D6},
            {"D7", Keys.D7},
            {"D8", Keys.D8},
            {"D9", Keys.D9},

            {"0", Keys.D0},
            {"1", Keys.D1},
            {"2", Keys.D2},
            {"3", Keys.D3},
            {"4", Keys.D4},
            {"5", Keys.D5},
            {"6", Keys.D6},
            {"7", Keys.D7},
            {"8", Keys.D8},
            {"9", Keys.D9},

            // Function Key
            {"F1", Keys.F1},
            {"F2", Keys.F2},
            {"F3", Keys.F3},
            {"F4", Keys.F4},
            {"F5", Keys.F5},
            {"F6", Keys.F6},
            {"F7", Keys.F7},
            {"F8", Keys.F8},
            {"F9", Keys.F9},
            {"F10", Keys.F10},
            {"F11", Keys.F11},
            {"F12", Keys.F12},

            // Control Key
            {"Ctrl", Keys.Control},
            {"Shift", Keys.Shift},
            {"Alt", Keys.Alt},
            {"Control", Keys.Control},
        };

        public static string GetFileName(string fullname)
        {
            // Input:   C:\Boss Ox\CapCap\Image.Jpg
            // Output:  Image.Jpg
            return fullname.Substring(fullname.LastIndexOf('\\') + 1);
        }

        public static string GetFolderName(string fullpath)
        {
            // Input:   C:\Boss Ox\CapCap\Image.Jpg
            // Output:  CapCap
            if (System.IO.Directory.GetDirectoryRoot(fullpath) != fullpath)
                fullpath = fullpath.Substring(fullpath.LastIndexOf('\\') + 1);
            return fullpath;
        }

        public static string GetFolder(string fullname)
        {
            // Input:   C:\Boss Ox\CapCap\Image.Jpg
            // Output:  C:\Boss Ox\CapCap
            if (System.IO.File.Exists(fullname))
                return fullname.Substring(0, fullname.Length - GetFileName(fullname).Length);
            return fullname;
        }

        public static Keys GetKeyFromKeyCodeString(string code)
        {
            // Input:   A; D1; F1
            // Output:  Keys.A; Keys.D1; Keys.F1
            var key = Keys.None;
            if (dictStringKey.ContainsKey(code))
                key = dictStringKey[code];
            return key;
        }

        public static bool AllowKey(string keyCode)
        {
            return hashAllowedKeys.Contains(GetKeyFromKeyCodeString(keyCode));
        }

        public static bool AllowKey(Keys key) { return hashAllowedKeys.Contains(key); }

        public static string KeyDataToString(Keys keyData, bool trim = true)
        {
            string result = keyData.ToString();
            return trim ? result.Replace(" ", "") : result;
        }

        public static string FilterKeyDataToString(Keys keyData)
        {
            string result = KeyDataToString(keyData);
            result = result.Replace("Menu,", "").Replace("ControlKey,", "").Replace("ShiftKey,", "");
            return result;
        }

        public static List<string> FilterKeyDataToList(Keys keyData)
        {
            string result = KeyDataToString(keyData);
            result = result.Replace("Menu,", "").Replace("ControlKey,", "").Replace("ShiftKey,", "");
            return result.Split(',').ToList();
        }

        public static string GetCombinationFromKeyData(Keys keyData)
        {
            string result = "";
            var keys = FilterKeyDataToList(keyData);

            if (keys.Contains("Control"))
            {
                result += "Ctrl + ";
                keys.Remove("Control");
            }

            if (keys.Contains("Alt"))
            {
                result += "Alt + ";
                keys.Remove("Alt");
            }

            if (keys.Contains("Shift"))
            {
                result += "Shift + ";
                keys.Remove("Shift");
            }

            if (keys.Count > 0)
                result += keys[0];

            return result;
        }

        private static bool Ctrl(this Keys keyData) { return keyData.HasFlag(Keys.Control); }

        private static bool Shift(this Keys keyData) { return keyData.HasFlag(Keys.Shift); }

        private static bool Alt(this Keys keyData) { return keyData.HasFlag(Keys.Alt); }

        private static Keys GetKeyFromString(string keydata)
        {
            // Input:   K; Control: F1
            // Output:  Keys.K; Keys.Control; Keys.F1
            var dict = new Dictionary<string, Keys>();
            foreach (var value in Enum.GetValues(typeof(Keys)))
                if (!dict.ContainsKey(Enum.GetName(typeof(Keys), value)))
                    dict.Add(Enum.GetName(typeof(Keys), value), (Keys)value);

            if (keydata == "Ctrl")
                keydata = "Control";

            if (dict.ContainsKey(keydata))
                return dict[keydata];
            else
                return Keys.None;
        }

        public static Keys GetKeysFromString(string keydata, char delimiter = '+')
        {
            // Input:   "Ctrl + Shift + Alt + K"
            // Output:  Keys.Control | Keys.Shift | Keys.Alt | Keys.K
            var components = keydata.Replace(" ", "").Split(delimiter).ToList();
            var result = Keys.None;

            foreach (var component in components)
                result |= GetKeyFromString(component);

            return result;
        }
    }
}
