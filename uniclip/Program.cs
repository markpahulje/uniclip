using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Windows;
using System.Diagnostics;
using System.Threading;

namespace UniClip
{
    public static class Program
    {

        //Original Code - Copyright © 2019 Aaron Meyers (https://github.com/bluemarsh/utf8clip)
        //Revised         COPYRIGHT © 2024 Mark Pahulje (https://github.com/markpahulje/uniclip) Win 7+, UTF-8 conversion proper, word-wrap, launch into Notepad, pro edtn Notepad++
        
        //Comment: Tried to change this a little as possible but fell short of what I was expecting, so I went to town on it M.Pahulje
        //Compiled with Visual Studio 2010 with .Net 4.0 Client Profile to work on Win7 for sure & always upgradable, LOL.
     
        //Uniclip Revised Mission - mainly to convert console input into UTF-8 'Unicode' give Win7 cmd default code page is 1251, and on Win10 its already utf-8. 
        
        //https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/5.0/utf-7-code-paths-obsolete after .dotnet 5 no UTF-7
        //https://ss64.com/nt/syntax-redirection.html understanding piping

        [STAThread] //STAThreadAttribute indicates that the COM threading model for the application is single-threaded apartment. This attribute must be present on the entry point of any application that uses Windows Forms;
        public static void Main(string[] args)
        {
            Encoding gEncoding = Encoding.UTF8; //global encoding
            
            Console.OutputEncoding = gEncoding;
            //Console.InputEncoding = gEncoding; //this we determine on the fly, Windows 7 is CP1251 and Windows 10+ is UTF-8 (so nothing to do).
            
            //USE CASE "This is how you say hello in Japanese: こんにちは" > japan.txt
            //USE CASE "COPYRIGHT © 2024 Mark Pahulje (https://github.com/markpahulje/uniclip)" > copy.txt
            
            //THIS PIPING DOES NOT WORK
            //file.txt > uniclip    This opens default text editor, usually notepad
            //file.txt > null       This will open default text editor, usually notepad

            string cmdname = HelpHelper.asm.GetName().Name.ToLower(); //uniclip
                        
            try
            {   
                if (ConsoleHelper.IsInputRedirected && args.Length == 1 && args[0] == "/notepad") //else if (Console.IsInputRedirected) //.NET 4.5+
                {
                    try
                    {
                        DoConsoleInputandConverttoUTF8();   
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not get input redirection stream. " + ex.Message);
                    }
                    
                }
                else if (ConsoleHelper.IsInputRedirected && args.Length == 1 && args[0] == "/notepad++") //else if (Console.IsInputRedirected) //.NET 4.5+
                {
                    try
                    {
                        DoConsoleInputandConverttoUTF8();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not get input redirection stream. " + ex.Message);
                    }

                }
                else if (ConsoleHelper.IsInputRedirected && args.Length == 1 && args[0] == "/fit") //else if (Console.IsInputRedirected) //.NET 4.5+
                {
                    try
                    {
                        DoConsoleInputandConverttoUTF8();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not get input redirection stream. " + ex.Message);
                    }
                }
                else if (args.Length == 1 && args[0]=="/notepad") 
                {
                    SendMessageWindow.SendTexttoNotepad();
                }
                //else if (args.Length == 1 && args[0] == "/notepad++")
                //{
                //    //Sat 15-Jun-24 1:47pm EDT MDC - actually used
                //    SendMessageWindow.SendTexttoNotepadPlusPlus();
                //}
                else if (args.Length == 1 && args[0] == "/fit")
                {
                    
                    try
                    {
                        string clip = Clipboard.GetText(TextDataFormat.UnicodeText);
                        Console.Write(HelpHelper.FormattedforCurrentConsole(clip)); 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not get clibpoard text to format. " + ex.Message); 
                    }
                    
                }
                else if (args.Length == 1 && (args[0] == "/help" || args[0] == "/h" || args[0] == "/?"))
                {
                    Console.WriteLine(HelpHelper.OutputFormattedHelp());
                }
                else if (ConsoleHelper.IsInputRedirected) //else if (Console.IsInputRedirected) //.NET 4.5+
                {
                    try
                    {
                        DoConsoleInputandConverttoUTF8();   
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not get input redirection stream. " + ex.Message);
                    }
                     
                }
                else if (ConsoleHelper.IsOutputRedirected) //else if (Console.IsOutputRedirected) //.NET 4.5+
                {
                    try
                    {
                        //'file.txt > uniclip' does not work, but commonly used 
                        //normally, this write file.txt into file named uniclip!

                        Console.WriteLine("We are in Output");


                        Stream output = Console.OpenStandardOutput();

                        // StreamWriter will use UTF-8 encoding without byte order mark by default.
                        // When output is redirected we don't modify the console encoding to avoid issues
                        // if the consuming program modifies the console encoding.
                        using (var w = new StreamWriter(output))
                            WriteToClipboard(w);
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not get output redirection stream. " + ex.Message); 
                    }

                }
                else if (ConsoleHelper.IsErrorRedirected) 
                {
                    try
                    {
                        Stream error = Console.OpenStandardError();

                        foreach (var line in ReadLines(error))
                            Console.WriteLine(line);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not get standard error redirection stream. " + ex.Message);
                    }
                
                }
                else
                {
                    // When output is not redirected, we need to set OutputEncoding so console
                    // will display output correctly.
                    using (new UnicodeEncodingOverride())
                        WriteToClipboard(Console.Out);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                try
                {
                    bool clearedPrevError = ConsoleHelper.clearMarshalledError;
                }
                catch 
                {
                    //throw;
                }
                Console.WriteLine('\a');
            }

        }
        
        private static void ReadToClipboard(TextReader r)
        {

            using (var w = new StringWriter())
            {
                CopyContent(r, w);

                try
                {
                    Clipboard.SetText(w.ToString(),TextDataFormat.UnicodeText);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void WriteToClipboard(TextWriter w)
        {
            string strOut = string.Empty; 
            try
            {
                strOut = Clipboard.GetText(TextDataFormat.UnicodeText);  

                using (var r = new StringReader(strOut))
                {
                    CopyContent(r, w);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Read lines from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// Mon 22-Apr-24 1:30pm EDT MDC - 
        private static IEnumerable<string> ReadLines(Stream stream)
        {
            StreamReader sr = null; 
            StringBuilder sb = new StringBuilder();

            try
            {
                sr = new StreamReader(stream);  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            } 
            
            int symbol = sr.Peek();
            while (symbol != -1)
            {
                symbol = sr.Read();
                if (symbol == 13 && sr.Peek() == 10)
                {
                    sr.Read();

                    string line = sb.ToString();
                    sb.Clear();

                    yield return line;
                }
                else
                    sb.Append((char)symbol);
            }

            yield return sb.ToString();
        }

        private static void CopyContent(TextReader r, TextWriter w)
        {
            bool first = true;
            string s;

            try
            {
                while ((s = r.ReadLine()) != null)
                {
                    if (first)
                        first = false;
                    else
                        w.WriteLine();

                    w.Write(s);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                
            }
            
        }

        //https://stackoverflow.com/questions/38533903/set-c-sharp-console-application-to-unicode-output
        private sealed class UnicodeEncodingOverride : IDisposable
        {
            private readonly Encoding originalEncoding = null; 

            public UnicodeEncodingOverride()
            {
                try
                {
                    if (!(Console.OutputEncoding is UTF8Encoding))
                    {

                        //ORG https://github.com/bluemarsh/utf8clip
                        Console.OutputEncoding = new UTF8Encoding(
                            encoderShouldEmitUTF8Identifier: false,
                            throwOnInvalidBytes: false);

                        //UNTST "This is how you say hello in Japanese: こんにちは"
                        //Console.OutputEncoding = Encoding.UTF8;
                        //Console.OutputEncoding = System.Text.Encoding.GetEncoding(1200);// not available in Win 7, for Win10+
                        
                        //Console.WriteLine("chcp 65001"); //UTF-8 issues not full coverage for Win7, still does not display 

                    }
                }
                catch (Exception ex)
                {    
                    Console.WriteLine(ex.Message); 
                }
            }

            public void Dispose()
            {
                if (this.originalEncoding != null) 
                {
                    try
                    {
                        Console.OutputEncoding = this.originalEncoding;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                    
            }

        }

        /// <summary>
        /// Convert console line in using Moz UCD algo into UTF-8 'Unicode for win' 
        /// </summary>
        //Sat 15-Jun-24 9:08pm EDT MDC - proper string conversion
        private static void DoConsoleInputandConverttoUTF8()
        {
        
            Encoding UDEEncoding = null; //Mozilla sniffer
                            
            string strConsoleIn = string.Empty;
            string strConsoleInConverted = string.Empty;

            MemoryStream memoryStream = new MemoryStream();
                            
            using (var sr = new StreamReader(Console.OpenStandardInput()))
            {
                sr.BaseStream.CopyTo(memoryStream);
            }


            //copy entire contents of the stream, this did not work
            //sr.CopyTo(memoryStream);
                        
            using (var srb = new BufferedStream(memoryStream))
            {
                srb.Position = 0;
                UDEEncoding = FileEncoding.DetectStreamEncoding(srb, null); //use MUCD (UDE lib)

                string msg = "Mozilla Universal Charset Detector algorithm probabilistically inferred encoding " + UDEEncoding.ToString() + " and input converted to UTF-8.";
                Console.WriteLine(HelpHelper.FormattedforCurrentConsole(msg)); 
                            
            }

            strConsoleIn = UDEEncoding.GetString(memoryStream.ToArray());

            //byte[] bytes = UDEEncoding.GetBytes(strConsoleIn);
            //strConsoleInConverted = Encoding.UTF8.GetString(bytes);

            if (UDEEncoding != Encoding.UTF8)
            {   
                //This works and it converts string strConsoleIn inline! (instead of memoryStream.ToArrat())
                Encoding.Convert(UDEEncoding, Encoding.UTF8, UDEEncoding.GetBytes(strConsoleIn));
            }

            try
            {
                Clipboard.SetText(strConsoleIn, TextDataFormat.UnicodeText);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Cannot set clipboard with empty string.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            memoryStream.Close();
            memoryStream.Dispose(); 

        }

    }
}
