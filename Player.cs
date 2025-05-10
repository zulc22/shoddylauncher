using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShoddyLauncher
{
    using ArchiveDescriptionTable = Dictionary<string, ArchiveContentDescriptor>;

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
    }
}
