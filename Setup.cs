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

    public partial class Setup : Form
    {
        List<string> ArchiveFiles;
        public ArchiveDescriptionTable archiveContents;
        public string romFolder;
        bool Cancelled = false;
        Task SearchTask;

        public Setup()
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
            ProgressTxt("Copying out 7-Zip...");

            DuctTape.Extract7zBinaries();
            
            int count = ArchiveFiles.Count;
            int index = 0;
            ProgressMax(count - 1);
            archiveContents = new ArchiveDescriptionTable();
            foreach (string filePath in ArchiveFiles)
            {
                ArchiveContentDescriptor archiveContentType = ArchiveContentDescriptor.NoExecutables;

                ProgressVal(index++);
                ProgressTxt("Scanning '" + Path.GetFileName(filePath) + "'... (" + (count-index).ToString() + " remain)");
                
                List<string> filesInArchive = DuctTape.ListArchive(filePath);
                if (filesInArchive == null)
                {
                    archiveContentType = ArchiveContentDescriptor.InvalidArchive;
                    archiveContents.Add(filePath, archiveContentType);
                    continue;
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
            Invoke(new Action(delegate { Close(); }));
        }

        private ArchiveContentDescriptor ScanExecutable(string archive, string path)
        {
            Process _7z = new Process();
            _7z.StartInfo.FileName = DuctTape.SevenzPath();
            _7z.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            // we drop the file into a special dir because we can't be sure what the filename will be
            // (-so redirection just... doesn't work. this is the only way to get binary data out)
            string launcherTestDir = DuctTape.TempDir() + @"\test";
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
            _7z.StartInfo.Arguments = "e \"" + archive + "\" \"" + path + "\"";
            _7z.StartInfo.WorkingDirectory = launcherTestDir;
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
            ushort pe_header_pointer = BitConverter.ToUInt16(exeBuffer, 0x3c);
            if (pe_header_pointer + 4 > exeBuffer.Length)
            {
                return ArchiveContentDescriptor.DOSBinaries;
            }
            if (exeBuffer[pe_header_pointer + 0] == 'P' &&
                exeBuffer[pe_header_pointer + 1] == 'E' &&
                exeBuffer[pe_header_pointer + 2] == 0 &&
                exeBuffer[pe_header_pointer + 3] == 0)
            {
                return ArchiveContentDescriptor.WindowsBinaries;
            }
            else
            {
                return ArchiveContentDescriptor.DOSBinaries;
            }
        }
    }
}
