﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscUtils.Iso9660;

namespace ShoddyLauncher
{
    using ArchiveDescriptionTable = Dictionary<string, ArchiveContentDescriptor>;
    using System.Diagnostics;
    using System.IO;

    public partial class Player : Form
    {
        public Player(ArchiveDescriptionTable adt)
        {
            InitializeComponent();

            //listView1.Columns.Add("-", -2, HorizontalAlignment.Center);
            //listView1.Columns.Add("Contents", -2);
            listView1.Columns.Add("Archive Name", -1);
            listView1.Columns.Add("Archive Path", 0);

            ImageList il = new ImageList();

            il.Images.Add("invalidarchive", Properties.Resources.icon_invalidbox);
            il.Images.Add("noexes", Properties.Resources.icon_openbox);
            il.Images.Add("win", Properties.Resources.icon_win);
            il.Images.Add("dos", Properties.Resources.icon_dos);
            il.Images.Add("doswin", Properties.Resources.icon_doswin);

            listView1.SmallImageList = il;

            foreach (string archivePath in adt.Keys)
            {
                string icon = "";
                switch (adt[archivePath]) {
                    case ArchiveContentDescriptor.InvalidArchive:
                        icon = "invalidarchive";
                        break;
                    case ArchiveContentDescriptor.NoExecutables:
                        icon = "noexes";
                        break;
                    case ArchiveContentDescriptor.WindowsBinaries:
                        icon = "win";
                        break;
                    case ArchiveContentDescriptor.DOSBinaries:
                        icon = "dos";
                        break;
                    case ArchiveContentDescriptor.MixedDOSWindowsBinaries:
                        icon = "doswin";
                        break;
                }
                ListViewItem itm = new ListViewItem(new string[] {
                    System.IO.Path.GetFileName(archivePath),
                    archivePath
                }, icon);
                listView1.Items.Add(itm);
            }
        }

        private void cbChangeROMExts_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control c in new Control[] { tbChangeExtFrom, tbChangeExtTo })
                c.Enabled = cbChangeROMExts.Checked;
        }

        private void btnBrowseROMs_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                cbChangeROMExts.Enabled = true; lbExtTo.Enabled = true;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView2.Enabled = false;
            listView2.Items.Clear();
            string path = SelectedPath();
            if (path == null) return;
            
            foreach (string s in DuctTape.ListArchive(path))
            {
                listView2.Items.Add(new ListViewItem(s));
            }
            listView2.Enabled = true;
        }

        private Dictionary<Control, bool> enabledBackup = new Dictionary<Control, bool>();
        private void LockUI()
        {
            foreach (Control c in Controls)
            {
                if (c == lbStatus) continue;
                enabledBackup[c] = c.Enabled;
                c.Enabled = false;
            }
        }
        private void UnlockUI()
        {
            if (InvokeRequired)
            {
                Invoke((Action)delegate { UnlockUI(); });
                return;
            }

            foreach (Control c in Controls)
            {
                try
                {
                    c.Enabled = enabledBackup[c];
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
            }
        }

        private void ProgressTxt(string t)
        {
            if (lbStatus.InvokeRequired)
            {
                Action a = delegate { ProgressTxt(t); };
                lbStatus.Invoke(a);
            }
            else lbStatus.Text = t;
        }

        private void btnVPC_Click(object sender, EventArgs e)
        {
            LockUI();
            Task t = new Task(LaunchInVPC);
            t.Start();
        }

        // Get the file path thats currently selected on the listbox on the left
        private string SelectedPath(bool complain=false)
        {
            if (listView1.InvokeRequired)
            {
                return (string)listView1.Invoke(
                    new Func<String>(() => SelectedPath(complain))
                );
            }

            string path;
            try
            {
                path = listView1.SelectedItems[0].SubItems[1].Text;
            }
            catch (ArgumentOutOfRangeException)
            {
                if (complain)
                {
                    MessageBox.Show(
                        "You haven't selected an archive to test!",
                    "ShoddyLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return null;
            }
            return path;
        }

        private void LaunchInVPC()
        {
            string p = SelectedPath(true);
            if (p == null) {
                UnlockUI();
                return;
            }

            CDBuilder cd = new CDBuilder();
            cd.UseJoliet = true;
            cd.VolumeIdentifier = "SHODDY";
            cd.AddFile("autorun.inf", Properties.Resources.autorun_inf);
            cd.AddFile("copy.bat", Properties.Resources.autorun_copy_bat);
            cd.AddFile("copy.pif", Properties.Resources.autorun_copy_pif);
            cd.AddFile("shoddy.ico", Properties.Resources.ico_shoddysmc);

            ProgressTxt("Extracting Archive...");
            if (Directory.Exists(DuctTape.TempDir() + @"\EMULATOR")) {
                Directory.Delete(DuctTape.TempDir() + @"\EMULATOR", true);
            }
            Process _7z = DuctTape.NewSevenz();
            _7z.StartInfo.Arguments = "x -oEMULATOR \"" + p + "\"";
            _7z.StartInfo.WorkingDirectory = DuctTape.TempDir();
            _7z.Start();
            _7z.WaitForExit();
            cd.AddDirectory("EMULATOR");
            int preamulatorlength = DuctTape.TempDir().Length + 1;
            string amulator = Path.Combine(DuctTape.TempDir(), "EMULATOR");
            foreach (string file in Directory.EnumerateFiles(
                amulator,
            "*.*", SearchOption.AllDirectories)) {
                string filePathInIso = file.Substring(preamulatorlength);
                ProgressTxt(filePathInIso);
                cd.AddFile(filePathInIso, File.ReadAllBytes(file));
            }

            string romsPath = folderBrowserDialog.SelectedPath;
            if (romsPath != "" && Directory.Exists(romsPath)) {

                string extFrom = tbChangeExtFrom.Text;
                string extTo = tbChangeExtTo.Text;
                if (extFrom.Length >= 1)
                    if (extFrom[0] == '.') extFrom = extFrom.Substring(1);
                if (extTo.Length >= 1)
                    if (extTo[0] == '.') extTo = extTo.Substring(1);

                foreach (string file in Directory.EnumerateFiles(
                    romsPath,
                "*.*", SearchOption.AllDirectories))
                {
                    string filePathInIso = "ROMS\\" + file.Substring(romsPath.Length+1);
                    if (cbChangeROMExts.Checked)
                    {
                        int extensionIndex = filePathInIso.LastIndexOf('.');
                        string extension = filePathInIso.Substring(extensionIndex + 1);
                        if (extension == extFrom)
                            filePathInIso = filePathInIso.Substring(0, extensionIndex) + "." + extTo;
                    }
                    ProgressTxt(filePathInIso);
                    cd.AddFile(filePathInIso, File.ReadAllBytes(file));
                }
            }

            string isoPath = Path.Combine(DuctTape.TempDir(), "vpc.iso");

            try
            {
                File.Delete(isoPath);
            }
            catch (IOException)
            {
                ProgressTxt("Unmounting ISO...");
                DuctTape.ExecuteAhk(Properties.Resources.ahk_vpcunmountiso);
            }

            ProgressTxt("Building ISO...");
            try
            {
                cd.Build(isoPath);
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "The ISO file ShoddyLauncher tried to build to is in use.\n"+
                    "Please make sure the VM doesn't have 'vpc.iso' inserted right now.",
                "ShoddyLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ProgressTxt("Mounting ISO...");
            DuctTape.ExecuteAhk(Properties.Resources.ahk_vpcmountiso, "\"" + isoPath + "\"");
            ProgressTxt("Peter? The status is here.");
            UnlockUI();
        }

        private void btnNative_Click(object sender, EventArgs e)
        {
            LockUI();
            Task t = new Task(LaunchNative);
            t.Start();
        }

        private void LaunchNative()
        {
            string p = SelectedPath(true);
            if (p == null)
            {
                UnlockUI();
                return;
            }

            string fromShoddyLauncher = Path.Combine(DuctTape.DesktopPath(), "From Shoddy Launcher");
            while (true) // looped in case of errors
            {
                try
                {
                    if (Directory.Exists(fromShoddyLauncher)) {
                        ProgressTxt("Deleting existing 'From ShoddyLauncher' directory...");
                        Directory.Delete(fromShoddyLauncher, true);
                    }
                    break;
                }
                catch (Exception ex) {
                    if (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Can't delete the 'From ShoddyLauncher' directory\n" +
                            "on the desktop. Close anything that might be\nusing it, and press OK to retry.",
                            "ShoddyLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error
                        );
                    }
                    else throw;
                }
            }
            Directory.CreateDirectory(fromShoddyLauncher);
            ProgressTxt("Extracting Emulator...");

            Process _7z = DuctTape.NewSevenz();
            _7z.StartInfo.Arguments = "x -oEmulator \"" + p + "\"";
            _7z.StartInfo.WorkingDirectory = fromShoddyLauncher;
            _7z.Start();
            _7z.WaitForExit();

            string romTargetDir = Path.Combine(fromShoddyLauncher, "ROMs");

            string romsPath = folderBrowserDialog.SelectedPath;
            if (romsPath != "" && Directory.Exists(romsPath))
            {
                Directory.CreateDirectory(romTargetDir);

                string extFrom = tbChangeExtFrom.Text;
                string extTo = tbChangeExtTo.Text;
                if (extFrom.Length >= 1)
                    if (extFrom[0] == '.') extFrom = extFrom.Substring(1);
                if (extTo.Length >= 1)
                    if (extTo[0] == '.') extTo = extTo.Substring(1);

                foreach (string file in Directory.EnumerateFiles(
                    romsPath,
                "*.*", SearchOption.AllDirectories))
                {
                    string targetFilePath = Path.Combine(romTargetDir, file.Substring(romsPath.Length + 1));
                    if (cbChangeROMExts.Checked)
                    {
                        int extensionIndex = targetFilePath.LastIndexOf('.');
                        string extension = targetFilePath.Substring(extensionIndex + 1);
                        if (extension == extFrom)
                            targetFilePath = targetFilePath.Substring(0, extensionIndex) + "." + extTo;
                    }
                    ProgressTxt(targetFilePath);
                    File.Copy(file, targetFilePath);
                }
            }

            ProgressTxt("Launching Explorer");
            Process.Start(fromShoddyLauncher);

            ProgressTxt("Peter? The status is here.");
            UnlockUI();
        }

        private void btnDOSBox_Click(object sender, EventArgs e)
        {
            LockUI();
            Task t = new Task(LaunchDOSBox);
            t.Start();
        }

        private void LaunchDOSBox()
        {
            string p = SelectedPath(true);
            if (p == null)
            {
                UnlockUI();
                return;
            }

            string fromShoddyLauncher = Path.Combine(DuctTape.SystemDrive(), "SHODLNCH");
            while (true) // looped in case of errors
            {
                try
                {
                    if (Directory.Exists(fromShoddyLauncher))
                    {
                        ProgressTxt("Deleting existing 'From ShoddyLauncher' directory...");
                        Directory.Delete(fromShoddyLauncher, true);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        MessageBox.Show("Can't delete the 'From ShoddyLauncher' directory\n" +
                            "on the desktop. Close anything that might be\nusing it, and press OK to retry.",
                            "ShoddyLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error
                        );
                    }
                    else throw;
                }
            }
            Directory.CreateDirectory(fromShoddyLauncher);
            ProgressTxt("Extracting Emulator...");

            Process _7z = DuctTape.NewSevenz();
            _7z.StartInfo.Arguments = "x -oEMU \"" + p + "\"";
            _7z.StartInfo.WorkingDirectory = fromShoddyLauncher;
            _7z.Start();
            _7z.WaitForExit();

            string romTargetDir = Path.Combine(fromShoddyLauncher, "ROMS");

            string romsPath = folderBrowserDialog.SelectedPath;
            if (romsPath != "" && Directory.Exists(romsPath))
            {
                Directory.CreateDirectory(romTargetDir);

                string extFrom = tbChangeExtFrom.Text;
                string extTo = tbChangeExtTo.Text;
                if (extFrom.Length >= 1)
                    if (extFrom[0] == '.') extFrom = extFrom.Substring(1);
                if (extTo.Length >= 1)
                    if (extTo[0] == '.') extTo = extTo.Substring(1);

                foreach (string file in Directory.EnumerateFiles(
                    romsPath,
                "*.*", SearchOption.AllDirectories))
                {
                    string targetFilePath = Path.Combine(romTargetDir, file.Substring(romsPath.Length + 1));
                    if (cbChangeROMExts.Checked)
                    {
                        int extensionIndex = targetFilePath.LastIndexOf('.');
                        string extension = targetFilePath.Substring(extensionIndex + 1);
                        if (extension == extFrom)
                            targetFilePath = targetFilePath.Substring(0, extensionIndex) + "." + extTo;
                    }
                    ProgressTxt(targetFilePath);
                    while (true) try
                        {
                            File.Copy(file, targetFilePath);
                        }
                        catch (Exception)
                        {
                            DialogResult r = MessageBox.Show("Couldn't copy file: \n" + file + "\nPress OK to retry, or Cancel to continue.", "ShoddyLauncher", MessageBoxButtons.OKCancel);
                            if (r == DialogResult.Cancel) break;
                        }
                }
            }

            ProgressTxt("Launching DOSBox-X");
            Process.Start(DuctTape.DOSBoxXExecutable(), "\"" + fromShoddyLauncher + "\"");

            ProgressTxt("Peter? The status is here.");
            UnlockUI();
        }
    }
}
