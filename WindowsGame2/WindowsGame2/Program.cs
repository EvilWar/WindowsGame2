using System;
using StalTrans;

namespace WindowsGame2
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Settings set = new Settings();
            //set.Show();
            //System.IO.StreamWriter file1 = new System.IO.StreamWriter("c:\\test.txt", true);
            //file1.WriteLine("\n"+DateTime.Now.ToString() + " Start application");

            //file1.Close();
            if (args.Length > 0)
            {
                foreach (string r in args)
                {
                //System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\test.txt", true);
                //file.WriteLine(DateTime.Now.ToString() + " args = \n" + r);

                //file.Close();

                }
         

                string firstArgument = args[0].ToLower().Trim();
                string secondArgument = null;

                // Handle cases where arguments are separated by colon.
                // Examples: /c:1234567 or /P:1234567
                if (firstArgument.Length > 2)
                {
                    secondArgument = firstArgument.Substring(3).Trim();
                    firstArgument = firstArgument.Substring(0, 2);
                }
                else if (args.Length > 1)
                    secondArgument = args[1];

                if (firstArgument == "/c" || firstArgument == "/C")           // Configuration mode
                {
                    Settings SetF = new Settings();
                    SetF.ShowDialog();
                    //ShowScreenSaver();
                }
                else if (firstArgument == "/p" || firstArgument == "/P")      // Preview mode
                {
                    IntPtr previewWndHandle = new IntPtr(long.Parse(secondArgument));
                    ShowScreenSaver(previewWndHandle);
                }
                else if (firstArgument == "/s" || firstArgument == "/S")      // Full-screen mode
                {
                    ShowScreenSaver();
                   
                }
                
            }
            else    // No arguments - treat like /c
            {
                Settings SetF = new Settings();
                SetF.ShowDialog();
            }



        }

        static void ShowScreenSaver()
            {
                using (Game1 game = new Game1())
                {
                    game.Run();

                }
            }

        static void ShowScreenSaver(IntPtr parentHwnd)
        {
            using (Game1 game = new Game1(parentHwnd))
            {
                game.Run();

            }
        }

    }
#endif
}

