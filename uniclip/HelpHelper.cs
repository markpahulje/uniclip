using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace UniClip
{
    static class HelpHelper
    {
        public static string OutputFormattedHelp() 
        {

            int consoleWidth = 45; //min size is 15, choose a little larger 

            try
            {
                consoleWidth = Console.WindowWidth - 5;
            }
            catch { }
            
            return FullHelp.FormatToWidth(consoleWidth); 

        }

        public static string FormattedforCurrentConsole(string s)
        {
            if (string.IsNullOrEmpty(s)) return s; 

            int consoleWidth = 45;
            try
            {
                consoleWidth = Console.WindowWidth - 5;
            }
            catch { }
            
            if (s.Length < consoleWidth)
                return s; 
            else 
                return s.FormatToWidth(consoleWidth);
            
        }

        /// <summary>
        /// Gets Assembly Attribute Values smart helper function  
        /// </summary>  
        /// https://blogs.msdn.microsoft.com/ivo_manolov/2008/12/17/introduction-to-testapi-part-2-command-line-parsing-apis/
        private static string GetAssemblyAttributeValue<T>(Assembly assembly, string propertyName) where T : Attribute
        {
            if (assembly == null || string.IsNullOrEmpty(propertyName)) return string.Empty;

            object[] attributes = assembly.GetCustomAttributes(typeof(T), false);
            if (attributes.Length == 0) return string.Empty;

            var attribute = attributes[0] as T;
            if (attribute == null) return string.Empty;

            var propertyInfo = attribute.GetType().GetProperty(propertyName);
            if (propertyInfo == null) return string.Empty;

            var value = propertyInfo.GetValue(attribute, null);
            return value.ToString();
        }

        /// <summary>
        /// Gets Assembly File Version
        /// </summary>
        public static string FileVersion
        {
            get { return GetAssemblyAttributeValue<AssemblyFileVersionAttribute>(asm, "Version"); }
            //FileVersionInfo.GetVersionInfo(this.asm.Location).FileVersion;  
        }

        public static Assembly asm = Assembly.GetExecutingAssembly();
        /// <summary>
        /// Gets Full Extened Help for Command Line
        /// </summary>
        public static string FullHelp
        {
            get
            {
                return @"
" + asm.GetName().Name.ToUpper() + " v" + FileVersion + @" Help

DESCRIPTION 
Copies and converts command line input text into UTF-8 'Unicode' and pastes it to the Windows clipboard. 
Conversion using Mozilla Universal Charset Detector to convert input to UTF-8.

LIMITATIONS
There is still VERY limited support for Unicode in the CMD shell, piping, redirection and most commands are still ANSI only!
Run cmd.exe /U  Causes the output of internal commands to a pipe or file to be in Unicode.

REQUIREMENTS
32-bit app which requires .NET Framework 4 Client Profile, so this can run on Win 7+.

SYNTAX
UNICLIP             Dump contents
UNICLIP /h or /?    Help
UNICLIP /notepad    Open in Notepad
UNICLIP /fit        Word wraps contents to fit current console window width

command | UNICLIP   Copies contents of command to Windows clipboard via uniclip.
UNICLIP > command   Copies contents of Windows clipboard into command via uniclip.

USAGE
" + asm.GetName().Name + @" 

Writes the current Unicode text contents of the Windows clipboard to the console. 

NOTES:  

The default code page for Win7 is Windows-1251, and this utility will convert to UTF-8 and paste onto clipboard. 

Default CMD.exe may not be set to display Unicode characters.
To to display Unicode, change console code page using 'chcp 1200' Unicode for Win10+. Try 'chcp 65001' UTF-8 for Win 7+.

See https://learn.microsoft.com/en-us/dotnet/api/system.text.encodinginfo.getencoding?view=netframework-4.0      

Also you can run cmd.exe /U  Causes the output of internal commands to a pipe or file to be Unicode. 

ARGUMENTS
No arguments recognized. All argument results in Help.
                                       
FLAGS
/h|/help    Help
/notepad    Opens Notepad with current clipboard contents
/fit        Word wraps output to fit current console width

EXAMPLES

DIR | UNICLIP

Sends a Unicode directory listing to the clipboard.

UNICLIP < file.txt

Sends Unicode text file.txt onto the clipboard.

UNICLIP /fit 

Writes the current Unicode text contents of the Windows clipboard to the console word wrapped to current width of executing console.

UNICLIP /notepad

Sends current clipboard (Unicode text) into a new instance of Notepad.

UNICLIP /notepad++ (Pro Edtn)

Sends current clipboard (Unicode text) into most recently used Notepad++ tab. Pastes at last known cursor location in the current tab.

BONUS
No file used; CTRL-V message sent to Notepad instance directly. Title change in Notepad to reflect paste success.

COMMON ERRORS
dir > uniclip       Redirects dir command output to a file named uniclip. 
file.txt > uniclip  Redirects file.txt output to a file named uniclip. 

COPYRIGHT © " + DateTime.Now.Year.ToString() + @" Mark Pahulje (https://github.com/markpahulje/uniclip) Win 7+, launch into Notepad, reflow
Copyright © 2019 Aaron Meyers (https://github.com/bluemarsh/utf8clip) non-Win 7 compliant, not working properly  

LICENSE
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the 'Software'), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

";

            }
        }

        /// <summary>
        /// Reformat string to a specific column width, respecting paragraphs. 
        /// Try not using fake columns using tabs, instead use new lines for best reflowing.
        /// Author : https://metadataconsulting.blogspot.com/
        /// </summary>
        public static string FormatToWidth(this string input, int colWidth)
        {

            StringBuilder line = new StringBuilder();
            StringBuilder result = new StringBuilder();
            string formatted = string.Empty; 
            char singleSpace = ' ';
            
            string[] lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (string paragraph in lines)
            {

                if (paragraph.Length > colWidth)
                {

                    Stack<string> stack = new Stack<string>(ReverseString(paragraph).Split(singleSpace));

                    while (stack.Count > 0)
                    {
                        var word = stack.Pop();
                        if (word.Length > colWidth)
                        {
                            string head = word.Substring(0, colWidth);
                            string tail = word.Substring(colWidth);

                            word = head;
                            stack.Push(tail);
                        }

                        if (line.Length + word.Length >= colWidth)
                        {
                            result.AppendLine(line.ToString());
                            line.Clear();

                        }

                        line.Append(word + singleSpace);
                    }

                    result.Append(line);
                    formatted += result.ToString() + Environment.NewLine;
                    result.Clear();
                    line.Clear();
                }
                else
                {
                    formatted += paragraph + Environment.NewLine;
                }

            }

            return formatted;
        }

        /// <summary>
        /// Helper function to Extension Method FormatToWidth
        /// </summary>
        private static String ReverseString(String str)
        {
            int word_length = 0;
            String result = string.Empty;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == ' ')
                {
                    result = " " + result;
                    word_length = 0;
                }
                else
                {
                    result = result.Insert(word_length, str[i].ToString());
                    word_length++;
                }
            }
            return result;
        }

    }
}
