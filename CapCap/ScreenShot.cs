using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace CapCap
{
    internal class ScreenShot
    {
        public bool CaptureCursor { get; set; } = false;
        public string SaveFolder { get; set; } = string.Empty;
        public string NamePattern { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public bool Overwrite { get; set; } = false;

        private ImageFormat _Format = ImageFormat.Jpeg;
        public ImageFormat Format
        {
            get { return _Format; }
            set
            {
                _Format = value;
                MIMEType = dictMIMEType[value];
            }
        }

        #region WinAPI
        [DllImport("user32.dll")]
        private static extern bool GetCursorInfo(out CursorInfo pci);
        private struct CursorInfo
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public Point ptScreenPos;
        }
        #endregion

        private string MIMEType = "image/png";
        private Dictionary<ImageFormat, string> dictMIMEType = new Dictionary<ImageFormat, string>()
        {
            {ImageFormat.Bmp, "image/bmp" },
            {ImageFormat.Gif, "image/gif" },
            {ImageFormat.Jpeg, "image/jpeg" },
            {ImageFormat.Png, "image/png" },
            {ImageFormat.Tiff, "image/tiff" },
        };

        public ScreenShot(bool cursor, string folder, string pattern, string number, ImageFormat format, bool overwrite)
        {
            CaptureCursor = cursor;
            SaveFolder = folder;
            NamePattern = pattern;
            Number = number;
            Format = format;
            Overwrite = overwrite;
        }

        public ScreenShotInfo CaptureAndSave()
        {
            Image img = captureScreen(CaptureCursor);
            var fullname = saveImage(img);
            bool success = fullname[0] == '[' ? false : true;
            return new ScreenShotInfo(success, fullname, Auxiliary.GetFileName(fullname), success ? fullname : "");
        }

        private Image captureScreen(bool capture_cursor)
        {
            // Capture screen.
            Image img = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphic = Graphics.FromImage(img);
            graphic.CopyFromScreen(new Point(0, 0), new Point(0, 0), Screen.AllScreens[0].Bounds.Size);

            if (capture_cursor)
            {
                CursorInfo ci;
                ci.cbSize = Marshal.SizeOf(typeof(CursorInfo));
                GetCursorInfo(out ci);
                Cursor cursor = new Cursor(ci.hCursor);
                cursor.Draw(graphic, new Rectangle(ci.ptScreenPos.X, ci.ptScreenPos.Y, cursor.Size.Width, cursor.Size.Height));
                cursor.Dispose();
            }
            graphic.Dispose();
            return img;
        }

        private string saveImage(Image img)
        {
            try
            {
                string filename = getFullname();

                if (System.IO.File.Exists(filename) && !Overwrite)
                    return saveRenamedImage(img, filename, 2);

                var EPs = new EncoderParameters(1);
                var EP = new EncoderParameter(Encoder.Quality, 96L);
                EPs.Param[0] = EP;

                img.Save(filename, GetEncoderInfo(), EPs);
                return filename;
            }
            catch (Exception exp)
            {
                return $"[{exp.Message}]";
            }
            finally
            {
                img.Dispose();
            }
        }

        private string getFullname()
        {
            var nPattern = new NamePattern(NamePattern, int.Parse(Number));
            return string.Format(@"{0}\{1}.{2}", SaveFolder, nPattern.Convert(), Format.ToString().ToUpper());
        }

        private string saveRenamedImage(Image img, string fullname, int number)
        {
            try
            {
                string filename = fullname.Substring(0, fullname.Length - Format.ToString().Length - 1);
                filename += $" ({number.ToString()}).{Format.ToString().ToUpper()}";

                if (System.IO.File.Exists(filename))
                    return saveRenamedImage(img, fullname, number + 1);
                else
                    img.Save(filename, Format);

                return filename;
            }
            catch (Exception exp)
            {
                return $"[Exception:{exp.Message}]";
            }
            finally
            {
                img.Dispose();
            }
        }

        private ImageCodecInfo GetEncoderInfo()
        {
            // from MSDN
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (var encoder in encoders)
            {
                if (encoder.MimeType == MIMEType)
                    return encoder;
            }
            return null;
        }
    }

    internal class ScreenShotInfo
    {
        public bool Success { get; }
        public string FullName { get; }
        public string FileName { get; }
        public string Message { get; }

        public ScreenShotInfo(bool success, string fullname, string filename, string message)
        {
            Success = success;
            FullName = fullname;
            FileName = filename;
            Message = message;
        }
    }
}
