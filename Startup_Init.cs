using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Stalker
{
    static class Stratup_Init
    {
        [STAThread]
        static void Main()
        {
            GuidAttribute attribute = (GuidAttribute)typeof(Stratup_Init).Assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            using (Mutex mutex = new Mutex(false, @"Global\" + attribute.Value))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MetroFramework.MetroMessageBox.Show(new MetroFramework.Forms.MetroForm { Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - 400) / 2, (Screen.PrimaryScreen.WorkingArea.Height - 100) / 2) }, "\n\nStalker is already running", "Unable to start", MessageBoxButtons.OK, MessageBoxIcon.None);
                    return;
                }
                GC.Collect();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Stalker MainFormInstance = new Stalker();
                Application.Run(MainFormInstance);
            }
        }
    }
}
