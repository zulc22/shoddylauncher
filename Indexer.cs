using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace ShoddyLauncher
{
    public partial class Indexer : Form
    {
        List<string> ArchiveFiles;
        bool Cancelled = false;
        Task SearchTask;

        public Indexer()
        {
            InitializeComponent();
            progressBar1.Maximum = 1000;
            ArchiveFiles = new List<string>();

            MessageBox.Show(
                "Please select a folder to scan for archive files.\n"+
                "Archives will be extracted when they are needed.",
            "ShoddyLauncher", MessageBoxButtons.OK, MessageBoxIcon.Question);

            if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel) {
                Cancelled = true;
                return;
            }

            string[] files = System.IO.Directory.GetFiles(folderBrowserDialog.SelectedPath);
            ArchiveFiles.AddRange(files);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (Cancelled) Close();
            Activate();

            SearchTask = new Task(SearchArchives);
            SearchTask.Start();
        }

        private void ProgressTxt(string t) {
            if (progressBar1.InvokeRequired)
            {
                Action a = delegate { ProgressTxt(t); };
                progressBar1.Invoke(a);
            }
            else label1.Text = t;
        }
        private void ProgressMax(int max) {
            if (progressBar1.InvokeRequired) {
                Action a = delegate { ProgressMax(max); };
                progressBar1.Invoke(a);
            } else progressBar1.Maximum = max;
        }
        private void ProgressVal(int val) {
            if (progressBar1.InvokeRequired)
            {
                Action a = delegate { ProgressVal(val); };
                progressBar1.Invoke(a);
            } else progressBar1.Value = val;
        }

        private void SearchArchives() {
            ProgressTxt("Extracting 7z...");

            Directory.SetCurrentDirectory(Path.GetTempPath());
            string sevenza_path = Directory.GetCurrentDirectory() + @"\7z.exe";
            File.WriteAllBytes(sevenza_path, ShoddyLauncher.Properties.Resources._7z_exe);

            int count = ArchiveFiles.Count;
            int index = 0;
            ProgressMax(count - 1);
            foreach (string filePath in ArchiveFiles)
            {
                ProgressVal(index++);
                ProgressTxt("Scanning '" + Path.GetFileName(filePath) + "'... (" + (count-index).ToString() + " remain)");
                Process _7z = new Process();
                _7z.StartInfo.FileName = sevenza_path;
                _7z.StartInfo.Arguments = "l -bd \"" + filePath + "\"";
                _7z.StartInfo.UseShellExecute = false;
                _7z.StartInfo.RedirectStandardOutput = true;
                _7z.StartInfo.CreateNoWindow = true;
                _7z.Start();
                string list_output = _7z.StandardOutput.ReadToEnd();
                string[] lines = list_output.Split(new string[]{"\r\n"}, StringSplitOptions.None);
                if (lines[3].EndsWith("Can not open file as archive"))
                {
                    MessageBox.Show("Failed to open: " + filePath);
                    continue;
                }
                int linesUntil = -1;
                List<string> filesInArchive = new List<string>();
                foreach (string line in lines)
                {
                    if (linesUntil > 0) linesUntil--;
                    if (line.StartsWith("   Date"))
                    {
                        linesUntil = 2;
                    }
                    if (linesUntil != 0) continue;
                    // The file list table has ended
                    if (line == "-----------------------")
                    {
                        break;
                    }
                    // File list table has started
                    string filename = line.Substring(54);
                    filesInArchive.Add(filename);
                }
                continue;
            }
            Invoke(new Action(delegate { Close(); }));
        }
    }
}
