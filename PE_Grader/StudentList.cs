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
    public partial class StudentList : Form
    {
        //NOTE: You may have to change this line.
        private const string msbuild =
            @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe";

        private List<StudentWork> _studentWork;

        private List<CodeWindow> _codeWindows;

        private Process _runningApp;

        private bool _monoGameProject;

        private StudentWork SelectedStudent => _studentWork[listBox1.SelectedIndex];

        public StudentList()
        {
            InitializeComponent();
            _studentWork = new List<StudentWork>();
            _codeWindows = new List<CodeWindow>();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) // Students
        {
            if (listBox1.SelectedIndex < 0)
            {
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }

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

        private void listBox2_DoubleClick(object sender, EventArgs e) // Code Files
        {
            string file = listBox2.SelectedItem as string;
            string code = File.ReadAllText(SelectedStudent.SourceDirectory + "\\" + file);
            var codeWindow = new CodeWindow(code, SelectedStudent.StudentName + " - " + file);
            _codeWindows.Add(codeWindow);
            codeWindow.Show();
        }

        private async void button3_Click(object sender, EventArgs e) // Process
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                return;
            }
            button3.Enabled = false;

            _monoGameProject = MessageBox.Show("Is this a MonoGame Project?", "MonoGame", MessageBoxButtons.YesNo) ==
                               DialogResult.Yes;

            _studentWork = new List<StudentWork>();

            listBox1.Items.Clear();

            RenameFiles("*.zip");
            RenameFiles("*.rar");

            _studentWork = _studentWork.OrderBy(sw => sw.StudentName).ToList();

            foreach (var studentWork in _studentWork)
            {
                listBox1.Items.Add(studentWork.StudentName);
            }

            var searchTasks = _studentWork.Select(ProcessStudent);
            await Task.WhenAll(searchTasks);

            button3.Enabled = true;
        }

        private async Task ProcessStudent(StudentWork studentWork)
        {
            SearchDirectory(studentWork);

            if (studentWork.SourceDirectory != "No Code")
            {
                await CompileProgram(studentWork);
                await ExecuteProgram(studentWork);
            }
        }

        private void SearchDirectory(StudentWork studentWork)
        {
            DirectoryInfo currentDir = new DirectoryInfo(textBox1.Text);
            if (Directory.GetDirectories(currentDir.FullName).Any(dir => dir.Contains(studentWork.ZipFileName)))
            {
                studentWork.Directory = Directory.GetDirectories(currentDir.FullName)
                    .First(dir => dir.Contains(studentWork.ZipFileName));
            }
            else
            {
                studentWork.Directory = ExtractFile(studentWork.ZipFileName);
            }
            studentWork.SourceDirectory = FindSourceCode(studentWork.Directory, "*.cs");
            var solutionDirectory = FindSourceCode(studentWork.Directory, "*.sln");
            if (solutionDirectory != "No Code")
            {
                studentWork.SolutionFileName =
                    Directory.GetFiles(solutionDirectory, "*.sln")[0];
            }
            if (studentWork.SourceDirectory != "No Code")
            {
                studentWork.CodeFileNames = Directory.GetFiles(studentWork.SourceDirectory, "*.cs")
                    .Select(Path.GetFileName).ToList();
            }
        }




        private async Task ExecuteProgram(StudentWork studentWork)
        {
            if (_monoGameProject)
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
            foreach (string t in textBox2.Lines)
            {
                proc.StandardInput.WriteLine(t);
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
            Process proc = new Process();
            proc.StartInfo = info;
            proc.Start();
            for (int i = 0; i < 10; i++)
            {
                proc.StandardInput.WriteLine();
            }
            var task = proc.StandardOutput.ReadToEndAsync();
            var result = await task;
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
