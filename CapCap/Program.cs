using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapCap
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var frm = new frmMain();
            if (args.Select(x => x.ToLower() == "-debug").Count() > 0)
                frm.isDebugMode = true;

            Application.Run(frm);
        }
    }
}
