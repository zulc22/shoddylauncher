using System;
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
            //cd.AddFile("shoddy.ico", );

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
            ProgressTxt("Peter. The Status is here.");
            UnlockUI();
        }
    }
}
