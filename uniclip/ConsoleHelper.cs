using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace UniClip
{       
    public static class ConsoleHelper
    {
        #region Imports
        //https://stackoverflow.com/questions/3328173/writing-to-console-and-stdout-in-vb-net
        //http://msdn.microsoft.com/en-us/library/system.console.isinputredirected.aspx
        // Get the last error and display it. https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.getlastwin32error?view=net-8.0&redirectedfrom=MSDN#System_Runtime_InteropServices_Marshal_GetLastWin32Error

        //https://www.codeproject.com/articles/36382/clearing-the-console-screen-using-api

        private enum FileType { Unknown, Disk, Char, Pipe };
        private enum StdHandle { Stdin = -10, Stdout = -11, Stderr = -12 };

        [DllImport("kernel32.dll")]
        private static extern FileType GetFileType(IntPtr hdl);

        
        //[DllImport("kernel32.dll")]
        //static extern IntPtr GetStdHandle(StdHandle std);

        /// <summary>
        /// Standard input device.
        /// </summary>
        public const int STD_INPUT_HANDLE = -10;
        /// <summary>
        /// Standard output device.
        /// </summary>
        public const int STD_OUTPUT_HANDLE = -11;
        /// <summary>
        /// Standard error device (usually the output device.)
        /// </summary>
        public const int STD_ERROR_HANDLE = -12;

        /// <summary>
        /// Retrieves a handle for the console standard input, output, or error device.
        /// </summary>
        /// <param name="nStdHandle">The standard device of which to retrieve handle for.</param>
        /// <returns>The handle for the standard device selected. Or an invalid handle if the function failed.</returns>
        [DllImport("Kernel32.dll", SetLastError = true)] ////Mon 10-Jun-24 10:41pm EDT MDC - added
        public static extern IntPtr GetStdHandle([param: MarshalAs(UnmanagedType.I4)] int nStdHandle);

        //https://stackoverflow.com/questions/52551703/is-there-a-way-to-clear-the-value-of-getlastwin32error
        [DllImport("kernel32.dll")]
        internal static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern void SetLastErrorEx(uint dwErrCode, uint dwType);

        #endregion
        
        //UTF-8 Console Read - https://stackoverflow.com/questions/55897200/how-to-use-standard-input-stream-instead-of-console-readkey

        //p/Invoke required for Win 7 and with .NET 4.0, available in .NET 4.5+
        public static bool IsOutputRedirected
        {
            get
            {
                int error = 0;
                IntPtr handle = GetStdHandle((int)StdHandle.Stdout);
                error = Marshal.GetLastWin32Error();

                if (error != 0)
                    throw new Win32Exception(error);

                if (handle != null)
                {
                    FileType ft = GetFileType(handle);
                    error = Marshal.GetLastWin32Error();
                    if (error != 0)
                    {
                        throw new Win32Exception(error);
                    }
                    else
                        return FileType.Char != ft;

                }
                else
                    return false;
            }
        }
        public static bool IsInputRedirected
        {
            get
            {
                int error = 0;
                var handle = GetStdHandle((int)StdHandle.Stdin);
                error = Marshal.GetLastWin32Error();

                if (error != 0)
                    throw new Win32Exception(error);

                if (handle != null)
                {
                    FileType ft = GetFileType(handle);
                    error = Marshal.GetLastWin32Error();
                    if (error != 0)
                    {
                        throw new Win32Exception(error);
                    }
                    else
                        return FileType.Char != ft;

                }
                else
                    return false;
            }
            
        }
        public static bool IsErrorRedirected
        {
            get 
            {
                int error = 0; 
                var handle = GetStdHandle((int)StdHandle.Stderr);
                error = Marshal.GetLastWin32Error();

                if (error != 0)
                    throw new Win32Exception(error);

                if (handle != null)
                {
                    FileType ft = GetFileType(handle);
                    error = Marshal.GetLastWin32Error();
                    if (error != 0)
                    {
                        throw new Win32Exception(error);
                    }
                    else
                        return FileType.Char != ft; 
                   
                }
                else
                    return false; 
            }
            
        }

        public static bool clearMarshalledError 
        {
            get
            {
                bool cleared = false; 
                //https://stackoverflow.com/questions/52551703/is-there-a-way-to-clear-the-value-of-getlastwin32error
                // clear any previous WIN32 error code, otherwise

                SetLastErrorEx(0, 0);

                IntPtr hWnd = ConsoleHelper.GetConsoleWindow();
                if (Marshal.GetLastWin32Error() > 0) { /* do something */ }
                if (hWnd == IntPtr.Zero)
                {
                    SetLastErrorEx(0, 0);
                    AllocConsole();
                    if (Marshal.GetLastWin32Error() > 0) 
                    { /* do something else */
                        cleared = false; 
                    }
                    else 
                        cleared = true; 
                }
                return true; //executed 
                
            }
            
        }


    }
}
