using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;

namespace Automata
{

    

    public partial class mainform : Form
    {
        public mainform()
        {
            InitializeComponent();
        }
        public const string version = "v0.1";
        public string tempdir = System.IO.Path.GetTempPath();
        string system32Directory = Path.Combine(Environment.ExpandEnvironmentVariables("%windir%"), "system32");


        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }


        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        private bool Is64Bit()
        {
            if (IntPtr.Size == 8 || (IntPtr.Size == 4 && Is32BitProcessOn64BitProcessor()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Is32BitProcessOn64BitProcessor()
        {
            bool retVal;

            IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);

            return retVal;
        }


        private void mainform_Load(object sender, EventArgs e)
        {
        
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop\\WindowMetrics", false);
            comboBox1.Text = (string)key.GetValue("Shell Icon Size");
            comboBox2.Text = (string)key.GetValue("Shell Icon BPP");
            key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion",false);
            this.Text = "Automata " + version + " running on: " + key.GetValue("ProductName") + " " + key.GetValue("BuildLab");
            key.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string myTempFile = Path.Combine(System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "deltemp.cmd");
            using (StreamWriter sw = new StreamWriter(myTempFile))
            {
                sw.WriteLine("@echo off\n"+
                "echo Deleting user and windows temp files...\n"+
                "rem - delete user temp files\n"+
                "%SystemDrive%\n"+
                "cd %temp%\n" +
                "del /q /f /s *.*\n" +
                "set CAT=%tmp%\n" +
                "dir \"%%CAT%%\"/s/b/a | sort /r >> %TEMP%\\files2del.txt\n" +
                "for /f \"delims=;\" %%D in (%TEMP%\\files2del.txt) do (del /q \"%%D\" & rd \"%%D\")\n" +
                "del /q %TEMP%\\files2del.txt\n" +
                "rem - delete windows temp files\n" +
                "cd %systemroot%\\temp\n" +
                "del /q /f /s *.*\n" +
                "set CAT=%systemroot%\\temp\n" +
                "dir \"%%CAT%%\"/s/b/a | sort /r >> %TEMP%\\files2del.txt\n" +
                "for /f \"delims=;\" %%D in (%TEMP%\\files2del.txt) do (del /q \"%%D\" & rd \"%%D\")\n" +
                "del /q %TEMP%\\files2del.txt\n" +
                "rd /S /Q \"%systemroot%\\temp\"\n" +
                "mkdir \"%systemroot%\\temp\"\n" +
                "rem - delete user's internet temporary files\n" +
                "echo Deleting Temporary Internet Files...\n" +
                "rd /S /Q \"%USERPROFILE%\\Local Settings\\Temporary Internet Files\\\"\n" +
                "mkdir \"%USERPROFILE%\\Local Settings\\Temporary Internet Files\\\"\n" +
                "rd /S /Q \"%USERPROFILE%\\Local Settings\\Microsoft\\Windows\\Temporary Internet Files\\\"\n" +
                "mkdir \"%USERPROFILE%\\Local Settings\\Microsoft\\Windows\\Temporary Internet Files\\\"\npause\n");

                sw.Close();
            }
            System.Diagnostics.Process.Start(Path.Combine(System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "deltemp.cmd"));
            //File.Delete(Path.Combine(System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "deltemp.cmd"));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string myTempFile = Path.Combine(Path.GetTempPath(), "cleanupdate.cmd");

            using (StreamWriter sw = new StreamWriter(myTempFile))
            {
                sw.WriteLine("@echo off\n"+
                    "net stop wuauserv\n"+
                    "del %systemroot%\\SoftwareDistribution\\download /q /s\n"+
                    "net start wuauserv\n"+
                    "dism /online /cleanup-image /spsuperseded\npause\n");
                sw.Close();

            }
            System.Diagnostics.Process.Start(Path.Combine(Path.GetTempPath(), "cleanupdate.cmd"));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
    {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey("Directory\\shell",true);
            key.CreateSubKey("deleteinvalid");
            key = key.OpenSubKey("deleteinvalid", true);
            key.SetValue("", "Delete invalid files");
            key.SetValue("HasLUAShield", "");
            key.SetValue("NoWorkingDirectory", "");
            key.CreateSubKey("command");
            key = key.OpenSubKey("command", true);
            key.SetValue("", "cmd.exe /c RD /S \"%1\"");
            key.SetValue("IsolatedCommand", "cmd.exe /c RD /S \"%1\"");
            key.Close();
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
    catch (Exception)
    {
        throw;
    }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey("Directory\\shell", true);
                key.DeleteSubKeyTree("deleteinvalid");
                key.Close();
                MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey("Directory\\shell", true);
                key.CreateSubKey("deletelongpath");
                key = key.OpenSubKey("deletelongpath", true);
                key.SetValue("", "Delete longpath");
                key.SetValue("HasLUAShield", "");
                key.SetValue("NoWorkingDirectory", "");
                key.CreateSubKey("command");
                key = key.OpenSubKey("command", true);
                key.SetValue("", "cmd.exe /c del_long_path.cmd \"%1\"");
                key.SetValue("IsolatedCommand", "cmd.exe /c del_long_path.cmd \"%1\"");
                key.Close();
                string myTempFile = Path.Combine(System.IO.Path.GetDirectoryName(Environment.SystemDirectory), "del_long_path.cmd");
                using (StreamWriter sw = new StreamWriter(myTempFile))
                {
                    sw.WriteLine("@echo off\n"+
                "if _%1_==__ (\n"+
                "echo.\n"+
                "echo Usage: %0 folderpath\n"+
                "echo Deletes specified folder and all subfolders, even if the path depth\n"+
                "echo is larger than what CMD/Explorer will manage.\n"+
                "echo   folderpath - the folder to delete\n"+
                "echo.\n"+
                "goto end\n"+
                ")\n\n"+
                "setlocal\n"+
                "rem ** The full path of the folder to delete\n"+
                "set target_folder=\"%~f1\"\n"+
                "set target_drive=%~d1\n"+
                "set tmp=%3\n"+
                "set del_temp_folder=nope\n"+
                "if {%tmp%}=={} set tmp=%target_drive%\\temp_%RANDOM%\n"+
                "if not exist %tmp% (\n"+
                "md %tmp%\n"+
                "echo Will delete folder: %target_folder%\n"+
                "echo Will use temp folder: %tmp%\n"+
                "set del_temp_folder=yepp\n"+
                "pause\n)\n\n"+
                "rem ** Create a prefix to use on moved folders to avoid name clashes\n"+
                "set i=%2\n"+
                "if {%i%}=={} set i=0\n"+
                "set /A i=%i% + 1\n"+
                "echo Level %i%, folder %target_folder%\n"+
                "rem ** Delete all files (non-directories) from the folder\n"+
                "del /Q /F %target_folder%\\*\n"+
                "rem ** Move all subfolders to the tempfolder, and delete the now empty folder\n"+
                "for /D %%d in (%target_folder%\\*) do (\n"+
                "move \"%%d\" \"%tmp%\\%i%_%%~nd\"\n"+
                ")\n"+
                "rd /Q %target_folder%\n"+
                "rem ** Recursively repeat the above for all the moved subfolders\n"+
                "for /D %%d in (%tmp%\\%i%_*) do (\n"+
                "call %0 \"%%d\" %i% %tmp%\n)\n"+
                "rem ** Clean upp\n"+
                "if {%del_temp_folder%}=={yepp} (\n"+
                "rd /Q %tmp%\n)\n"+
                "endlocal\n"+
                ":end\n"+
                "pause\n");
            sw.Close();
                }
                MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey("Directory\\shell", true);
                key.DeleteSubKeyTree("deletelongpath");
                key.Close();
                File.Delete(Path.Combine(System.IO.Path.GetDirectoryName(Environment.SystemDirectory), "del_long_path.cmd"));
                MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("ipconfig","/flushdns");
                MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("cmd", "/k net share");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("net", "user guest /active:yes");
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("net", "user guest /active:no");
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey("Directory\\shell", true);
                key.CreateSubKey("instantshare");
                key = key.OpenSubKey("instantshare", true);
                key.SetValue("", "Instant Share");
                key.SetValue("HasLUAShield", "");
                key.SetValue("NoWorkingDirectory", "");
                key.CreateSubKey("command");
                key = key.OpenSubKey("command", true);
                key.SetValue("", "cmd.exe /c instant_share.cmd \"%1\"");
                key.SetValue("IsolatedCommand", "cmd.exe /c instant_share.cmd \"%1\"");
                key.Close();
                key = Registry.ClassesRoot.OpenSubKey("Directory\\shell", true);
                key.CreateSubKey("instantunshare");
                key = key.OpenSubKey("instantunshare", true);
                key.SetValue("", "Instant UnShare");
                key.SetValue("HasLUAShield", "");
                key.SetValue("NoWorkingDirectory", "");
                key.CreateSubKey("command");
                key = key.OpenSubKey("command", true);
                key.SetValue("", "cmd.exe /c net share \"%1\" /DELETE");
                key.SetValue("IsolatedCommand", "cmd.exe /c net share \"%1\" /DELETE");
                key.Close();
      
                string myTempFile = Path.Combine(System.IO.Path.GetDirectoryName(Environment.SystemDirectory), "instant_share.cmd");
                using (StreamWriter sw = new StreamWriter(myTempFile))
                {
                    sw.WriteLine("@echo off\n"+
                    "rem (c) 2015 e1z0 && EofNET Networks.\n"+
                    "if _%1_==__ (\n"+
                    "echo.\n"+
                    "echo Usage: %0 folderpath\n"+
                    "echo Shares specified folder and all subfolders, with read permissions for everyone\n"+
                    "echo   folderpath - the folder to share\n"+
                    "echo.\n"+
                    "goto end\n)\n"+
                    "setlocal\n"+
                    "rem ** The full path of the folder to delete\n"+
                    "set target_folder=\"%~f1\"\n"+
                    "set target_drive=%~d1\n"+
                    "for /F %%i in (\"%1\") do @set FN=%%~nxi\n"+
                    "net share %FN%=\"%1\"\n"+
                    "icacls \"%1\" /grant Everyone:(OI)(CI)R\n"+
                    "endlocal\n"+
                    ":end\npause\n");
            sw.Close();
            }
                MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                throw;
            }

        }
        

        private void button12_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey("Directory\\shell", true);
            key.DeleteSubKeyTree("instantshare");
            key.DeleteSubKeyTree("instantunshare");
            key.Close();
            File.Delete(Path.Combine(System.IO.Path.GetDirectoryName(Environment.SystemDirectory), "instant_share.cmd"));
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("cmd", "/k sfc /ScanNow");
        }

        private void button14_Click(object sender, EventArgs e)
        {
           
                System.Diagnostics.Process.Start("cmd", "/k Dism /Online /Cleanup-Image /ScanHealth");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("cmd", "/k Dism /Online /Cleanup-Image /CheckHealth");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
            key.SetValue("EnableLUA", 0);
           
            key.Close();
            MessageBox.Show ("You need to restart your PC to completely turn off the UAC!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
            key.SetValue("EnableLUA", 1);
            key.Close();
            MessageBox.Show("You need to restart your PC to turn on the UAC!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("cmd", "/k w32tm /resync");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("control", "USERPASSWORDS2");
        }

        private void button21_Click(object sender, EventArgs e)
        {
            comboBox1.Text = "32";
            comboBox2.Text = "24";
        }

        private void button20_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop\\WindowMetrics", true);
            key.SetValue("Shell Icon Size", comboBox1.Text);
            key.SetValue("Shell Icon BPP", comboBox2.Text);
            key.Close();
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("cmd", "/k systeminfo | find /i \"install date\"");
            
        }

        private void button23_Click(object sender, EventArgs e)
        {
          
        }

        private void button23_Click_1(object sender, EventArgs e)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey("*\\shell", true);
            key.CreateSubKey("takeownership");
            key = key.OpenSubKey("takeownership", true);
            key.SetValue("", "Take Ownership");
            key.SetValue("HasLUAShield", "");
            key.SetValue("NoWorkingDirectory", "");
            key.CreateSubKey("command");
            key = key.OpenSubKey("command", true);
            key.SetValue("", "cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F");
            key.SetValue("IsolatedCommand", "cmd.exe /c takeown /f \"%1\" && icacls \"%1\" /grant administrators:F");
            key = Registry.ClassesRoot.OpenSubKey("Directory\\shell", true);
            key.CreateSubKey("takeownership");
            key = key.OpenSubKey("takeownership", true);
            key.SetValue("", "Take Ownership");
            key.SetValue("HasLUAShield", "");
            key.SetValue("NoWorkingDirectory", "");
            key.CreateSubKey("command");
            key = key.OpenSubKey("command", true);
            key.SetValue("", "cmd.exe /c takeown /f \"%1\" /r /d y && icacls \"%1\" /grant administrators:F /t");
            key.SetValue("IsolatedCommand", "cmd.exe /c takeown /f \"%1\" /r /d y && icacls \"%1\" /grant administrators:F /t");
            key.Close();
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey("Directory\\shell", true);
            key.DeleteSubKeyTree("takeownership");
            key = Registry.ClassesRoot.OpenSubKey("*\\shell", true);
            key.DeleteSubKeyTree("takeownership");
            key.Close();
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            string hosts = system32Directory+"\\drivers\\etc\\hosts";
            if (File.Exists(hosts))
            {
                FileAttributes attributes = File.GetAttributes(hosts);

                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    // Make the file RW
                    attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                    File.SetAttributes(hosts, attributes);
                }


                if (textBox2.Text != "")
                {
                    using (StreamWriter sw = File.AppendText(hosts))
                    {
                        sw.WriteLine("127.0.0.1 "+this.textBox2.Text);
                        sw.Close();

                    }
                    MessageBox.Show("Done!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else MessageBox.Show("Please speficy the hostname to be faked", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            string hosts = system32Directory + "\\drivers\\etc\\hosts";

            System.Diagnostics.Process.Start("notepad", hosts);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            System.Diagnostics.Process.Start("http://chat.bsdnet.lt/");
        }

        private void button27_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows", true);
            key.CreateSubKey("WindowsUpdate");
            key = key.OpenSubKey("WindowsUpdate", true);
            key.CreateSubKey("AU");
            key = key.OpenSubKey("AU", true);
            key.SetValue("NoAutoRebootWithLoggedOnUsers", 1);
            key.SetValue("RebootRelaunchTimeout", 0x5a0,RegistryValueKind.DWord);
            key.SetValue("RebootRelaunchTimeoutEnabled", 1);
            key.Close();
            System.Diagnostics.Process.Start("sc","stop wuauserv");
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows", true);
            key.CreateSubKey("WindowsUpdate");
            key = key.OpenSubKey("WindowsUpdate", true);
            key.CreateSubKey("AU");
            key = key.OpenSubKey("AU", true);
            key.SetValue("NoAutoRebootWithLoggedOnUsers", 0);
            key.Close();
            System.Diagnostics.Process.Start("sc", "start wuauserv");
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            if (Is64Bit()) MessageBox.Show("bibela!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button30_Click(object sender, EventArgs e)
        {
      //some kind of sufleravimas

       // http://programmingtricks9.blogspot.lt/2014/02/how-to-create-own-mac-address-changer.html
            // http://stackoverflow.com/questions/8753043/how-to-change-mac-address-with-batch-file-on-windows-7
        }

        private void button31_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.Users.OpenSubKey(".DEFAULT\\Control Panel\\Keyboard", true);
            key.SetValue("InitialKeyboardIndicators", "2147483650");
            key.Close();
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.Users.OpenSubKey(".DEFAULT\\Control Panel\\Keyboard", true);
            key.SetValue("InitialKeyboardIndicators", "2147483648");
            key.Close();
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Defender\\MpEngine", true);
            key.SetValue("MpEnablePus", 1, RegistryValueKind.DWord);
            key.Close();
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Defender\\MpEngine", true);
            key.DeleteValue("MpEnablePus");
            key.Close();
            MessageBox.Show("Operation OK!", "Automata", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    
    }
}
