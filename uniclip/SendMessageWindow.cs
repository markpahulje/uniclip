using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace UniClip
{
    static class SendMessageWindow
    {
        #region Imports
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
                
        //this is a constant indicating the window that we want to send a text message
        const int WM_SETTEXT = 0X000C;
        const uint WM_PASTE = 0x302;

        #endregion

        //How to send text to Notepad using C# Windows Form Application
        //https://learn.microsoft.com/en-us/archive/msdn-technet-forums/17fcc596-98ed-4330-a7cc-cde59f2cfc89
        
        /// <summary>
        /// Performs a paste-v command into Notepad itself.
        /// </summary>
        public static void SendTexttoNotepad()
        {
            Process p = null; 
            try
            {
               
                List<IntPtr> notepads = new List<IntPtr>();

                //collect open Notepad instances 
                Process[] processes = Process.GetProcesses();
                foreach (Process proc in processes)
                {
                    if (ProcessIsNotepad(proc))
                    {
                        notepads.Add(proc.MainWindowHandle);
                    }
                }

                //open Notepad on a new thread
                p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = @"/C %windir%\notepad.exe";
                //p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                
                System.Threading.Thread.Sleep(100);

                processes = Process.GetProcesses();
                foreach (Process proc in processes)
                {
                    if (ProcessIsNotepad(proc) && !notepads.Contains(proc.MainWindowHandle))
                    {

                        try
                        {
                            SendMessage(proc.MainWindowHandle, WM_SETTEXT, 0, "Clipboard pasted - Notepad"); //NICE! 

                            IntPtr ptrNoteEditHandle = FindWindowEx(proc.MainWindowHandle, IntPtr.Zero, "Edit", null);

                            if (ptrNoteEditHandle != null && (int)ptrNoteEditHandle > 0)
                            {
                                object obj = new object();
                                HandleRef hr = new HandleRef(obj, ptrNoteEditHandle); //hold from GC!!!
                                
                                PostMessage(hr, WM_PASTE, IntPtr.Zero, IntPtr.Zero);

                                //SendMessage(ptrNoteEditHandle, WM_SETTEXT, 0, text);//WORKS

                                //SendMessage(ptrNoteEditHandle, 0x011, 0, text);//WM_COMMAND //did not work
                                //SendMessage(ptrNoteEditHandle, 0x0112, 0, text); // WM_SYSCOMMAND ////did not work
                                //https://foxlearn.com/windows-forms/send-text-to-notepad-in-csharp-361.html
                                //SendMessage(ptrNoteEditHandle, 0x00B1, 0, text); //did not work
                                //SendMessage(ptrNoteEditHandle, 0x00C2, 0, text); //did not work
                            }
                            else
                                Console.WriteLine("Could not get Notepad handle.");
                        }
                        catch
                        {
                            Console.WriteLine("Could not get Notepad handle.");
                        }

                    }
                }
  
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not send clipboard to Notepad. " + ex.Message);
            }
            finally
            {
                if (p!=null)
                    p.Close(); 
            }
            
        }

        public static bool ProcessIsNotepad(Process proc)
        {
            return proc.MainWindowTitle.EndsWith("Notepad", StringComparison.InvariantCultureIgnoreCase);
        }
                
    }
}
