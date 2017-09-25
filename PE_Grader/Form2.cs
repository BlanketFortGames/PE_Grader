using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SevenZipExtractor;

namespace PE_Grader
{
    public partial class Form2 : Form
    {
        private const string cscCmd1 =
            "\"C:\\Users\\bkalb\\Documents\\IGME 106 Online\\PE_Grader\\packages\\Microsoft.Net.Compilers.2.3.2\\tools\\csc.exe\" ";
        private const string cscCmd2 =
            @"C:\Users\bkalb\Documents\IGME 106 Online\PE_Grader\packages\Microsoft.Net.Compilers.2.3.2\tools\csc.exe";

        private const string msbuild =
            @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe";

        private List<StudentWork> _studentWork;

        private List<Form3> _codeWindows;

        private Process _runningApp;

        private StudentWork SelectedStudent => _studentWork[listBox1.SelectedIndex];

        public Form2()
        {
            InitializeComponent();
            _studentWork = new List<StudentWork>();
            _codeWindows = new List<Form3>();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            if (SelectedStudent.CodeFileNames != null)
            {
                listBox2.Items.AddRange(SelectedStudent.CodeFileNames.ToArray());
            }
            if (File.Exists(SelectedStudent.ExeFileName))
            {
                richTextBox1.Text = SelectedStudent.ExeOutput;
            }
            else
            {
                richTextBox1.Text = SelectedStudent.CompilationOutput;
            }
            foreach (var codeWindow in _codeWindows)
            {
                codeWindow.Close();
            }
            _codeWindows.Clear();
            if (_runningApp != null && !_runningApp.HasExited)
            {
                _runningApp.Kill();
            }
            _runningApp = null;
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            string file = listBox2.SelectedItem as string;
            string code = File.ReadAllText(SelectedStudent.SourceDirectory + "\\" + file);
            var codeWindow = new Form3(code, SelectedStudent.StudentName + " - " + file);
            _codeWindows.Add(codeWindow);
            codeWindow.Show();
        }

        private async void button3_Click(object sender, EventArgs e) // Process
        {
            button3.Enabled = false;
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                return;
            }
            _studentWork = new List<StudentWork>();
            listBox1.Items.Clear();
            // set directory
            DirectoryInfo currentDir = new DirectoryInfo(textBox1.Text);

            RenameFiles("*.zip");
            RenameFiles("*.rar");

            _studentWork = _studentWork.OrderBy(sw => sw.StudentName).ToList();

            foreach (var studentWork in _studentWork)
            {
                listBox1.Items.Add(studentWork.StudentName);
            }

            foreach (var studentWork in _studentWork)
            {
                string studentDirectory = "";
                if (Directory.GetDirectories(currentDir.FullName).Any(dir => dir.Contains(studentWork.ZipFileName)))
                {
                    studentWork.Directory = Directory.GetDirectories(currentDir.FullName)
                        .First(dir => dir.Contains(studentWork.ZipFileName));
                }
                else
                {
                    studentWork.Directory = ExtractFile(studentWork.ZipFileName);
                }
            }

            foreach (var studentWork in _studentWork)
            {
                studentWork.SourceDirectory = FindSourceCode(studentWork.Directory, "*.cs");
                studentWork.SolutionFileName =
                    Directory.GetFiles(FindSourceCode(studentWork.Directory, "*.sln"), "*.sln")[0];
            }

            foreach (var studentWork in _studentWork)
            {
                if (studentWork.SourceDirectory != "No Code")
                {
                    studentWork.CodeFileNames = Directory.GetFiles(studentWork.SourceDirectory, "*.cs")
                        .Select(Path.GetFileName).ToList();

                    string debugDir = studentWork.SourceDirectory + "\\bin\\Debug\\";

                    //studentWork.ExeFileName = Directory.GetFiles(debugDir, "*.exe")[0];

                    studentWork.ExeFileName = studentWork.SourceDirectory + "\\bin\\Debug\\Test.exe";
                }
            }

            var compileTasks = _studentWork.Where(sw => sw.SourceDirectory != "No Code")
                .Select(CompileProgram);
            await Task.WhenAll(compileTasks);

            var executeTasks = _studentWork.Where(sw => sw.SourceDirectory != "No Code")
                .Select(ExecuteProgram);
            await Task.WhenAll(executeTasks);

            /*foreach (var studentWork in _studentWork)
            {
                if (studentWork.SourceDirectory != "No Code")
                {
                    string cmd = cscCmd1 + "/out:\"" + studentWork.SourceDirectory + "\\Test.exe\" " +
                                 "\"" + studentWork.SourceDirectory + @"\*.cs" + "\"";

                    //studentWork.CompilationOutput = ExecuteCommandSync(cmd);
                }
            }//*/

            button3.Enabled = true;
        }

        private async Task ExecuteProgram(StudentWork studentWork)
        {
            if (checkBox1.Checked)
            {
                studentWork.ExeOutput = "MonoGame Project.\nClick \"Run .exe\" to run";
                return;
            }

            string filename = studentWork.ExeFileName;
            string result = "";
            if (!File.Exists(filename))
            {
                studentWork.ExeOutput = result;
                return;
            }
            Directory.SetCurrentDirectory(Path.GetDirectoryName(filename));
            ProcessStartInfo info = new ProcessStartInfo(filename);
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            Process proc = new Process();
            proc.StartInfo = info;
            proc.Start();
            for (int i = 0; i < 10; i++)
            {
                proc.StandardInput.WriteLine();
            }
            var task = proc.StandardOutput.ReadToEndAsync();
            if (await Task.WhenAny(task, Task.Delay(1500)) == task)
            {
                result = task.Result;
            }
            else
            {
                result = ".exe Timeout";
            }
            if (!proc.HasExited)
            {
                proc.Kill();
            }
            studentWork.ExeOutput = result;
        }

        private async Task CompileProgram(StudentWork studentWork)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(studentWork.SourceDirectory));
            ProcessStartInfo info = new ProcessStartInfo(msbuild);
            info.RedirectStandardOutput = true;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            //info.Arguments = "/out:\"" + studentWork.SourceDirectory + "\\bin\\Debug\\Test.exe\" " + "\"" + studentWork.SourceDirectory + @"\*.cs" + "\"";
            Process proc = new Process();
            proc.StartInfo = info;
            proc.Start();
            for (int i = 0; i < 10; i++)
            {
                proc.StandardInput.WriteLine();
            }
            var task = proc.StandardOutput.ReadToEndAsync();
            var result = await task;
            /*if (await Task.WhenAny(task, Task.Delay(5000)) == task)
            {
                result = task.Result;
            }
            else
            {
                result = "Compile Timeout";
            }//*/
            if (!proc.HasExited)
            {
                proc.Kill();
            }
            studentWork.CompilationOutput = result;
            string[] lines = result.Split('\n');
            string exeline = lines.LastOrDefault(ln => ln.Trim().EndsWith(".exe"));
            if (exeline != null)
            {
                studentWork.ExeFileName = exeline.Substring(exeline.IndexOf("->") + 2).Trim();
            }
        }

        public string ExecuteCommandSync(string command, bool mashEnter = false)
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

            if (mashEnter)
            {
                procStartInfo.RedirectStandardInput = true;
            }

            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;

            // Now we create a process, assign its ProcessStartInfo and start it
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            if (mashEnter)
            {
                for (int i = 0; i < 10; i++)
                {
                    proc.StandardInput.WriteLine();
                }
            }

            // Get the output into a string
            string result = proc.StandardOutput.ReadToEnd();

            if (mashEnter && !proc.HasExited)
            {
                proc.Kill();
            }

            return result;
        }


        private string FindSourceCode(string dir, string mask)
        {
            // locate a directory with the source code
            // test current directory by getting a list of files with a .cs extension
            DirectoryInfo currentDir = new DirectoryInfo(dir);
            FileInfo[] csfiles = currentDir.GetFiles(mask);

            if (csfiles.Length > 0) return dir;

            // test all subdirectories
            DirectoryInfo[] subdirs = currentDir.GetDirectories();
            foreach (DirectoryInfo subdir in subdirs)
            {
                string result = FindSourceCode(subdir.FullName, mask);
                if (result != "No Code") return result;
            }
            return "No Code";
        }

        private string ExtractFile(string fName)
        {
            // get full file name
            string filename = textBox1.Text + @"\" + fName;

            // create the directory name
            string dirname = filename.Substring(0, filename.Length - 4);

            // unarchive
            using (ArchiveFile archiveFile = new ArchiveFile(filename))
            {
                archiveFile.Extract(dirname); // extract all
            }
            return dirname;
        }

        private void RenameFiles(string fileType)
        {
            // set directory
            DirectoryInfo currentDir = new DirectoryInfo(textBox1.Text);

            // get files with a .zip extension
            FileInfo[] zipfiles = currentDir.GetFiles(fileType);

            string name = "";

            // rename the files by removing the student ID from
            // each filename
            foreach (FileInfo fiObj in zipfiles)
            {
                // get the file name
                name = fiObj.Name;

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

                string studentName = name.Substring(0, name.IndexOf("-")).Trim();

                // rename the file if name was changed
                if (i > 0)
                {
                    fiObj.MoveTo(textBox1.Text + @"\" + name);
                }

                _studentWork.Add(new StudentWork
                {
                    StudentName = studentName,
                    ZipFileName = name,
                });
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(SelectedStudent.SolutionFileName);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(SelectedStudent.ExeFileName))
            {
                _runningApp = Process.Start(SelectedStudent.ExeFileName);
            }
            else
            {
                MessageBox.Show("No .exe file!");
            }
        }
    }
}
