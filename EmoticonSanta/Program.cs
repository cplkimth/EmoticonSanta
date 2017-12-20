#region
using System;
using System.Diagnostics;
using System.Windows.Forms;
using EmoticonSanta.Controls;
using EmoticonSanta.Properties;
#endregion

namespace EmoticonSanta
{
    static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Utility.Instance.EnsureSaveDirectory();

            //            Application.Run(new MainForm());
            Application.Run(new BrowserForm());
            //            Application.Run(new ImageGridForm("https://e.kakao.com/t/korean-ver-extremely-chibi-rabbit-chibi-bear"));
        }
    }
}