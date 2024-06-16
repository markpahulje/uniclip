UNICLIP v1.1.0615.611 Help

DESCRIPTION 
Copies and converts command line input text into UTF-8 'Unicode' and pastes it to the Windows clipboard.  

Conversion using Mozilla Universal Charset Detector to convert input to UTF-8.

LIMITATIONS
There is still VERY limited support for Unicode in the CMD shell, piping, redirection and most 
commands are still ANSI only! 
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
uniclip 

Writes the current Unicode text contents of the Windows clipboard to the console. 

NOTES:  

The default code page for Win7 is Windows-1251, and this utility will convert to UTF-8 and 
paste onto clipboard.  

Default CMD.exe may not be set to display Unicode characters.
To to display Unicode, change console code page using 'chcp 1200' Unicode for Win10+. Try 
'chcp 65001' UTF-8 for Win 7+. 

See 
https://learn.microsoft.com/en-us/dotnet/api/system.text.encodinginfo.getencoding?view=netframe 
work-4.0       

Also you can run cmd.exe /U  Causes the output of internal commands to a pipe or file to be 
Unicode.  

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

Writes the current Unicode text contents of the Windows clipboard to the console word wrapped 
to current width of executing console. 

UNICLIP /notepad

Sends current clipboard (Unicode text) into a new instance of Notepad.

UNICLIP /notepad++ (Pro Edtn)

Sends current clipboard (Unicode text) into most recently used Notepad++ tab. Pastes at last 
known cursor location in the current tab. 

BONUS
No file used; CTRL-V message sent to Notepad instance directly. Title change in Notepad to 
reflect paste success. 

COMMON ERRORS
dir > uniclip       Redirects dir command output to a file named uniclip. 
file.txt > uniclip  Redirects file.txt output to a file named uniclip. 

COPYRIGHT © 2024 Mark Pahulje (https://github.com/markpahulje/uniclip) Win 7+, launch into 
Notepad, reflow 
Copyright © 2019 Aaron Meyers (https://github.com/bluemarsh/utf8clip) non-Win 7 compliant, not 
working properly   

LICENSE
Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the 'Software'), to deal in the Software without 
restriction, including without limitation the rights to use, copy, modify, merge, publish, 
distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions: 

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software. 

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 



