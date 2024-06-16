using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
//using WindowsInput;
//using WindowsInput.Native;

namespace UniClip
{
    class NotepadHelper
    {

        public static void ReliableSetNotepadtoForeground()
        {

            Process[] processes = Process.GetProcesses();
            int milliseconds = 1000;
            Thread.Sleep(milliseconds);
            foreach (Process proc in processes)
            {
                if (ProcessIsNotepad(proc))
                {
                    ActivateWindow(proc.MainWindowHandle);

                    //Console.WriteLine("Launched Notepad");
                    //System.Windows.Forms.SendKeys.Send("^V");
                    //int milliseconds = 3000;
                    //Thread.Sleep(milliseconds);
                    System.Windows.Forms.SendKeys.SendWait("^V");
                    // CTRL-C (effectively a copy command in many situations)
                    //var x = new KeyboardSimulator();
                    Console.WriteLine("Sent CTRL-V");
                }
            }
        }

        public static bool ProcessIsNotepad(Process proc) 
        {
            return proc.MainWindowTitle.EndsWith("Notepad", StringComparison.InvariantCultureIgnoreCase);
        }
        
        private const int ALT = 0xA4;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;
        private const int SHOW_MAXIMIZED = 3;
        private const int WM_SHOWWINDOW = 0x0018; 
        private const int KEYEVENTF_KEYDOWN = 0x0000; // New definition
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        private const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        private const int VK_LCONTROL = 0xA2; //Left Control key code
        private const int A = 0x41; //A key code
        private const int C = 0x43; //C key code

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        public static void ActivateWindow(IntPtr mainWindowHandle)
        {
            // Guard: check if window already has focus.
            if (mainWindowHandle == GetForegroundWindow()) 
                return;

            // Show window maximized.
            ShowWindow(mainWindowHandle, SHOW_MAXIMIZED);

            // Hold Control down and press C
            keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(C, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(C, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);
            
            // Show window in forground.
            SetForegroundWindow(mainWindowHandle);

            //// Hold Control down and press C
            keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(C, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(C, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);

        }
    }
    
}
