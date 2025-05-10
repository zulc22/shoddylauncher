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
    using ArchiveDescriptionTable = Dictionary<string, ArchiveContentDescriptor>;

    public enum ArchiveContentDescriptor {
        InvalidArchive,
        NoExecutables,
        WindowsBinaries,
        DOSBinaries,
        MixedDOSWindowsBinaries
    }

    public partial class Indexer : Form
    {
        List<string> ArchiveFiles;
        public ArchiveDescriptionTable archiveContents;
        bool Cancelled = false;
        Task SearchTask;
        string sevenz_path = "";

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
            sevenz_path = Directory.GetCurrentDirectory() + @"\7z.exe";
            File.WriteAllBytes(sevenz_path, ShoddyLauncher.Properties.Resources.exe_7z);

            int count = ArchiveFiles.Count;
            int index = 0;
            ProgressMax(count - 1);
            archiveContents = new ArchiveDescriptionTable();
            foreach (string filePath in ArchiveFiles)
            {
                ArchiveContentDescriptor archiveContentType = ArchiveContentDescriptor.NoExecutables;

                ProgressVal(index++);
                ProgressTxt("Scanning '" + Path.GetFileName(filePath) + "'... (" + (count-index).ToString() + " remain)");
                Process _7z = new Process();
                _7z.StartInfo.FileName = sevenz_path;
                _7z.StartInfo.Arguments = "l -bd \"" + filePath + "\"";
                _7z.StartInfo.UseShellExecute = false;
                _7z.StartInfo.RedirectStandardOutput = true;
                _7z.StartInfo.CreateNoWindow = true;
                _7z.Start();
                string list_output = _7z.StandardOutput.ReadToEnd();
                string[] lines = list_output.Split(new string[]{"\r\n"}, StringSplitOptions.None);
                if (lines[3].EndsWith("Can not open file as archive"))
                {
                    archiveContentType = ArchiveContentDescriptor.InvalidArchive;
                    archiveContents.Add(filePath, archiveContentType);
                    continue;
                }
                // Collect file list from archive
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
                    if (line.StartsWith("-------------------"))
                    {
                        break;
                    }
                    // File list table has started
                    string filename = line.Substring(53);
                    filesInArchive.Add(filename);
                }
                // Filter for files in archive that have executable file extensions
                int dosExecutables = 0;
                int windowsExecutables = 0;

                List<string> executablesToScanInArchive = new List<string>();
                foreach (string filename in filesInArchive)
                {
                    string fn = filename.ToLower();
                    if (fn.EndsWith(".com"))
                    {
                        dosExecutables++;
                    }
                    if (fn.EndsWith(".exe"))
                    {
                        executablesToScanInArchive.Add(filename);
                    }
                }
                // Detect what the .exe files are
                foreach (string filename in executablesToScanInArchive)
                {
                    ArchiveContentDescriptor d = ScanExecutable(filePath, filename);
                    if (d == ArchiveContentDescriptor.DOSBinaries) dosExecutables++;
                    if (d == ArchiveContentDescriptor.WindowsBinaries) windowsExecutables++;
                }
                if (dosExecutables > 0) archiveContentType = ArchiveContentDescriptor.DOSBinaries;
                if (windowsExecutables > 0) archiveContentType = ArchiveContentDescriptor.WindowsBinaries;
                if (dosExecutables > 0 && windowsExecutables > 0)
                    archiveContentType = ArchiveContentDescriptor.MixedDOSWindowsBinaries;

                archiveContents.Add(filePath, archiveContentType);
            }
            File.Delete(sevenz_path);
            Invoke(new Action(delegate { Close(); }));
        }

        private ArchiveContentDescriptor ScanExecutable(string archive, string path)
        {
            Process _7z = new Process();
            _7z.StartInfo.FileName = sevenz_path;
            _7z.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            // we drop the file into a special dir because we can't be sure what the filename will be
            // (-so redirection just... doesn't work. this is the only way to get binary data out)
            string launcherTestDir = "ShoddyLauncherTest";
            if (!Directory.Exists(launcherTestDir)) Directory.CreateDirectory(launcherTestDir);
            else
            {
                if (Directory.GetFiles(launcherTestDir).Length != 0)
                {
                    foreach (string p in Directory.GetFiles(launcherTestDir))
                    {
                        File.Delete(p);
                    }
                }
            }
            _7z.StartInfo.Arguments = "e -oShoddyLauncherTest \"" + archive + "\" \"" + path + "\"";
            _7z.StartInfo.UseShellExecute = false;
            _7z.StartInfo.CreateNoWindow = true;
            _7z.Start();
            _7z.WaitForExit();
            // find the first file in that dir
            if (Directory.GetFiles(launcherTestDir).Length == 0) return ArchiveContentDescriptor.InvalidArchive;
            string extractedfile = Directory.GetFiles(launcherTestDir)[0];

            byte[] exeBuffer = File.ReadAllBytes(extractedfile);
            File.Delete(extractedfile);

            // "ZM" is valid for MZ DOS-format files, apparently? PE files don't have these.
            if (exeBuffer[0] == 'Z' && exeBuffer[1] == 'M') return ArchiveContentDescriptor.DOSBinaries;
            if (exeBuffer[0] != 'M' && exeBuffer[1] != 'Z')
            {
                return ArchiveContentDescriptor.InvalidArchive;
            }
            ushort relocTable = BitConverter.ToUInt16(exeBuffer, 0x18);
            if (relocTable > 0x40)
            {
                return ArchiveContentDescriptor.WindowsBinaries;
            }
            else return ArchiveContentDescriptor.DOSBinaries;
        }
    }
}
