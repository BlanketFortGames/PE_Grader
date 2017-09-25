using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using SevenZipExtractor;


namespace PE_Grader
{
    public partial class Form1 : Form
    {
        // location of the csc.exe program (C# compiler)
        private const string cscCmd1 = 
            @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe ";

        public Form1()
        {
            InitializeComponent();
        }

        // display the folder browser dialog
        private void folderBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult browseResult = folderBrowserDialog1.ShowDialog();

            // save path if OK selected
            if(browseResult == DialogResult.OK)
            {
                directoryTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        // clear all text boxes
        private void clearAllButton_Click(object sender, EventArgs e)
        {
            statusTextBox.Text = "";
            directoryTextBox.Text = "";
        }

        // clear only the status text box
        private void clearStatusButton_Click(object sender, EventArgs e)
        {
            statusTextBox.Text = "";
        }

        // unarchive each file in the selected directory
        private void unarchiveButton_Click(object sender, EventArgs e)
        {
            // handles only zip files
            // start by getting a list of all zip files in the directory
            try
            {
                // set directory
                DirectoryInfo currentDir = new DirectoryInfo(directoryTextBox.Text);

                RenameFiles("*.zip");
                RenameFiles("*.rar");
                RenameFiles("*.doc");
                RenameFiles("*.docx");
                RenameFiles("*.pdf");

                // loop through the files and unpack them
                var zipfiles = currentDir.GetFiles("*.zip"); // get renamed files
                foreach (FileInfo fiObj in zipfiles)
                {
                    ExtractFile(fiObj);
                }
                zipfiles = currentDir.GetFiles("*.rar"); // get renamed files
                foreach (FileInfo fiObj in zipfiles)
                {
                    ExtractFile(fiObj);
                }
            }
            catch (Exception ex)
            {
                statusTextBox.Text += ex.Message + "\r\n";
            }
        }

        private void ExtractFile(FileInfo fiObj)
        {
            // get full file name
            string filename = directoryTextBox.Text + @"\" + fiObj.Name;
            statusTextBox.Text += "Unarchiving " + filename + "\r\n";

            // create the directory name
            string dirname = filename.Substring(0, filename.Length - 4);

            // unarchive
            try
            {
                using (ArchiveFile archiveFile = new ArchiveFile(filename))
                {
                    archiveFile.Extract(dirname); // extract all
                }
            }
            catch (Exception ex)
            {
                statusTextBox.Text += ex.Message + "\r\n";
            }
        }

        // helper method
        private void RenameFiles(string fileType)
        {
            // set directory
            DirectoryInfo currentDir = new DirectoryInfo(directoryTextBox.Text);

            // get files with a .zip extension
            FileInfo[] zipfiles = currentDir.GetFiles(fileType);

            // check count
            if (zipfiles.Length <= 0)
            {
                statusTextBox.Text += "No files in the directory for file type " + fileType + " .\r\n";
            }
            else // files found
            {
                // rename the files by removing the student ID from
                // each filename
                foreach (FileInfo fiObj in zipfiles)
                {
                    // get the file name
                    string name = fiObj.Name;

                    // locate the first letter
                    int i;
                    for (i = 0; i < name.Length; i++)
                    {
                        if ((name[i] >= 'a' && name[i] <= 'z') ||
                           (name[i] >= 'A' && name[i] <= 'Z'))
                        {
                            name = name.Substring(i);
                            break;
                        }
                    }

                    // rename the file if name was changed
                    if (i > 0)
                    {
                        statusTextBox.Text += "Renaming " + fiObj.Name + " to " + name + "\r\n";
                        fiObj.MoveTo(directoryTextBox.Text + @"\" + name);
                    }
                }
            }
        }

        // compile each program
        private void compileButton_Click(object sender, EventArgs e)
        {
            // start by getting a list of all subdirectories in the directory
            try
            {
                // set directory
                DirectoryInfo currentDir = new DirectoryInfo(directoryTextBox.Text);

                // get subdirectories
                DirectoryInfo[] subdirs = currentDir.GetDirectories();

                // check count
                if (subdirs.Length <= 0)
                {
                    statusTextBox.Text += "No subdirectories found in the directory.\r\n";
                }
                else // files found
                {
                    // loop through the files and compile them
                    foreach (DirectoryInfo diObj in subdirs)
                    {
                        string sourceDir = FindSourceCode(diObj.FullName);
                        if (sourceDir != "No Code")
                        {
                            statusTextBox.Text += "\r\nSource code found in " + sourceDir + "\r\n";
                            // compile the code 
                            string cmd = cscCmd1 + "/out:\"" + sourceDir + "\\Test.exe\" " +
                                "\"" + sourceDir + @"\*.cs" +"\"";

                            statusTextBox.Text += "\r\nCommand: " + cmd + "\r\n"; // debug
                             
                            ExecuteCommandSync(cmd);
                            statusTextBox.Text += "***Compilation complete\r\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                statusTextBox.Text += ex.Message + "\r\n";
            }
        }

        // locate the subdirectory with the source code. Code came from
        // this article: http://www.codeproject.com/Articles/25983/How-to-Execute-a-Command-in-C
        // with some modifications to support a GUI
        private string FindSourceCode(string dir)
        {
            // locate a directory with the source code
            // test current directory by getting a list of files with a .cs extension
            DirectoryInfo currentDir = new DirectoryInfo(dir);
            FileInfo[] csfiles = currentDir.GetFiles("*.cs");

            if (csfiles.Length > 0) return dir;

            // test all subdirectories
            DirectoryInfo[] subdirs = currentDir.GetDirectories();
            foreach(DirectoryInfo subdir in subdirs)
            {
                string result = FindSourceCode(subdir.FullName);
                if (result != "No Code") return result;
            }
            return "No Code";
        }

        // this code executes a command that is passed in

        public void ExecuteCommandSync(string command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;

                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;

                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();

                // Display the command output.
                statusTextBox.Text += result + "\r\n";
            }
            catch (Exception ex)
            {
                // Log the exception
                statusTextBox.Text += ex.Message + "\r\n";
            }
        }
    }
}
