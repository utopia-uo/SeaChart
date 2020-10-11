using System;
using System.Windows.Forms;

namespace SeaChart {
    internal static class Program {
        /// <summary>
        /// SeaChart options (dots position and tags)
        /// </summary>
        internal static MainOptions Options;

        [STAThread]
        private static void Main () {
            //1 - Loads options, and dies on non recoverable error
            if (!LoadOptions()) return;
            
            //2 - Prepares UI and begins the standard application message loop
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormSeaChart());

            //3 - Saves the options
            try {
                Options.Save();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Preferences error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        /// <summary>
        /// Loads the options.
        /// </summary>
        /// <returns></returns>
        private static bool LoadOptions () {
            try {
                //Deserializes options file as MainOptions class into Option
                Options = MainOptions.Load();
                return true;
            } catch (Exception ex) {
                //Houston, we've a problem.
                if (DialogResult.No == MessageBox.Show("Error loading preferences file.\n" + ex.Message + "\n\nDo you want to create a new file ?\n\nIf yes, your previous dots will be lost.\nIf no, SeaChart will terminate and you can manually fix the options file.", "Preferences error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)) {
                    //The user wants to terminate SeaChart to manually fix the problem.
                    return false;
                }
            }

            //The users wants to create a new options file
            try {
                Options = MainOptions.CreateNewOptionsFile(MainOptions.DefaultOptionsFile);
                return true;
            } catch (Exception ex) {
                //Arg... a very bad day for the user.
                MessageBox.Show(ex.Message, "Preferences error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
        }
    }
}
