namespace ShoddyLauncher
{
    partial class Player
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Player));
            this.listView1 = new System.Windows.Forms.ListView();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnVPC = new System.Windows.Forms.Button();
            this.btnDOSBox = new System.Windows.Forms.Button();
            this.btnNative = new System.Windows.Forms.Button();
            this.listView2 = new System.Windows.Forms.ListView();
            this.lbArchiveContents = new System.Windows.Forms.Label();
            this.lbContentDescriptor = new System.Windows.Forms.Label();
            this.btnBrowseROMs = new System.Windows.Forms.Button();
            this.cbChangeROMExts = new System.Windows.Forms.CheckBox();
            this.tbChangeExtFrom = new System.Windows.Forms.TextBox();
            this.tbChangeExtTo = new System.Windows.Forms.TextBox();
            this.lbExtTo = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(13, 13);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(195, 285);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Sorry for using the shitty tree one. I had no other choice.";
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // btnVPC
            // 
            this.btnVPC.Image = global::ShoddyLauncher.Properties.Resources.vpc16;
            this.btnVPC.Location = new System.Drawing.Point(221, 244);
            this.btnVPC.Name = "btnVPC";
            this.btnVPC.Size = new System.Drawing.Size(96, 30);
            this.btnVPC.TabIndex = 2;
            this.btnVPC.Text = "Virtual PC";
            this.btnVPC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVPC.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnVPC.UseVisualStyleBackColor = true;
            this.btnVPC.Click += new System.EventHandler(this.btnVPC_Click);
            // 
            // btnDOSBox
            // 
            this.btnDOSBox.Image = global::ShoddyLauncher.Properties.Resources.dosboxx16;
            this.btnDOSBox.Location = new System.Drawing.Point(413, 244);
            this.btnDOSBox.Name = "btnDOSBox";
            this.btnDOSBox.Size = new System.Drawing.Size(96, 30);
            this.btnDOSBox.TabIndex = 2;
            this.btnDOSBox.Text = "DOSBox-X";
            this.btnDOSBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDOSBox.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDOSBox.UseVisualStyleBackColor = true;
            // 
            // btnNative
            // 
            this.btnNative.Image = global::ShoddyLauncher.Properties.Resources.shell32_dll_14_3_1;
            this.btnNative.Location = new System.Drawing.Point(317, 244);
            this.btnNative.Name = "btnNative";
            this.btnNative.Size = new System.Drawing.Size(96, 30);
            this.btnNative.TabIndex = 2;
            this.btnNative.Text = "Native";
            this.btnNative.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNative.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNative.UseVisualStyleBackColor = true;
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name});
            this.listView2.Enabled = false;
            this.listView2.Location = new System.Drawing.Point(214, 29);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(303, 125);
            this.listView2.TabIndex = 3;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // lbArchiveContents
            // 
            this.lbArchiveContents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbArchiveContents.AutoSize = true;
            this.lbArchiveContents.Location = new System.Drawing.Point(215, 13);
            this.lbArchiveContents.Name = "lbArchiveContents";
            this.lbArchiveContents.Size = new System.Drawing.Size(91, 13);
            this.lbArchiveContents.TabIndex = 4;
            this.lbArchiveContents.Text = "Archive Contents:";
            // 
            // lbContentDescriptor
            // 
            this.lbContentDescriptor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbContentDescriptor.Location = new System.Drawing.Point(364, 13);
            this.lbContentDescriptor.Name = "lbContentDescriptor";
            this.lbContentDescriptor.Size = new System.Drawing.Size(153, 13);
            this.lbContentDescriptor.TabIndex = 5;
            this.lbContentDescriptor.Text = "No Executables Found";
            this.lbContentDescriptor.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnBrowseROMs
            // 
            this.btnBrowseROMs.Location = new System.Drawing.Point(312, 171);
            this.btnBrowseROMs.Name = "btnBrowseROMs";
            this.btnBrowseROMs.Size = new System.Drawing.Size(110, 23);
            this.btnBrowseROMs.TabIndex = 6;
            this.btnBrowseROMs.Text = "Select ROM Folder";
            this.btnBrowseROMs.UseVisualStyleBackColor = true;
            this.btnBrowseROMs.Click += new System.EventHandler(this.btnBrowseROMs_Click);
            // 
            // cbChangeROMExts
            // 
            this.cbChangeROMExts.AutoSize = true;
            this.cbChangeROMExts.Enabled = false;
            this.cbChangeROMExts.Location = new System.Drawing.Point(237, 205);
            this.cbChangeROMExts.Name = "cbChangeROMExts";
            this.cbChangeROMExts.Size = new System.Drawing.Size(168, 17);
            this.cbChangeROMExts.TabIndex = 7;
            this.cbChangeROMExts.Text = "Change ROM Extensions from";
            this.cbChangeROMExts.UseVisualStyleBackColor = true;
            this.cbChangeROMExts.CheckedChanged += new System.EventHandler(this.cbChangeROMExts_CheckedChanged);
            // 
            // tbChangeExtFrom
            // 
            this.tbChangeExtFrom.Location = new System.Drawing.Point(401, 203);
            this.tbChangeExtFrom.Name = "tbChangeExtFrom";
            this.tbChangeExtFrom.Size = new System.Drawing.Size(38, 20);
            this.tbChangeExtFrom.TabIndex = 8;
            // 
            // tbChangeExtTo
            // 
            this.tbChangeExtTo.Location = new System.Drawing.Point(457, 203);
            this.tbChangeExtTo.Name = "tbChangeExtTo";
            this.tbChangeExtTo.Size = new System.Drawing.Size(38, 20);
            this.tbChangeExtTo.TabIndex = 8;
            // 
            // lbExtTo
            // 
            this.lbExtTo.AutoSize = true;
            this.lbExtTo.Enabled = false;
            this.lbExtTo.Location = new System.Drawing.Point(441, 206);
            this.lbExtTo.Name = "lbExtTo";
            this.lbExtTo.Size = new System.Drawing.Size(16, 13);
            this.lbExtTo.TabIndex = 9;
            this.lbExtTo.Text = "to";
            // 
            // lbStatus
            // 
            this.lbStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbStatus.Location = new System.Drawing.Point(219, 281);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(298, 17);
            this.lbStatus.TabIndex = 10;
            this.lbStatus.Text = "Peter? The status is here.";
            this.lbStatus.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // name
            // 
            this.name.Text = "File Name";
            this.name.Width = 299;
            // 
            // Player
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 310);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.lbExtTo);
            this.Controls.Add(this.tbChangeExtTo);
            this.Controls.Add(this.tbChangeExtFrom);
            this.Controls.Add(this.cbChangeROMExts);
            this.Controls.Add(this.btnBrowseROMs);
            this.Controls.Add(this.lbContentDescriptor);
            this.Controls.Add(this.lbArchiveContents);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.btnNative);
            this.Controls.Add(this.btnDOSBox);
            this.Controls.Add(this.btnVPC);
            this.Controls.Add(this.listView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Player";
            this.Text = "ShoddyLauncher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button btnVPC;
        private System.Windows.Forms.Button btnDOSBox;
        private System.Windows.Forms.Button btnNative;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.Label lbArchiveContents;
        private System.Windows.Forms.Label lbContentDescriptor;
        private System.Windows.Forms.Button btnBrowseROMs;
        private System.Windows.Forms.CheckBox cbChangeROMExts;
        private System.Windows.Forms.TextBox tbChangeExtFrom;
        private System.Windows.Forms.TextBox tbChangeExtTo;
        private System.Windows.Forms.Label lbExtTo;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.ColumnHeader name;
    }
}