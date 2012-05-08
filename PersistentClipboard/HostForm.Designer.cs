namespace PersistentClipboard
{
    partial class HostForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HostForm));
            this.clippedListBox = new System.Windows.Forms.ListBox();
            this.searchText = new System.Windows.Forms.TextBox();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // clippedListBox
            // 
            this.clippedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clippedListBox.BackColor = System.Drawing.SystemColors.Window;
            this.clippedListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clippedListBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clippedListBox.FormattingEnabled = true;
            this.clippedListBox.ItemHeight = 17;
            this.clippedListBox.Location = new System.Drawing.Point(0, -2);
            this.clippedListBox.Name = "clippedListBox";
            this.clippedListBox.Size = new System.Drawing.Size(384, 527);
            this.clippedListBox.TabIndex = 0;
            this.clippedListBox.Click += new System.EventHandler(this.clippedListBox_Click);
            this.clippedListBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.clippedListBox_KeyUp);
            // 
            // searchText
            // 
            this.searchText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchText.Location = new System.Drawing.Point(0, 504);
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(384, 23);
            this.searchText.TabIndex = 1;
            this.searchText.Visible = false;
            this.searchText.WordWrap = false;
            this.searchText.TextChanged += new System.EventHandler(this.searchText_TextChanged);
            this.searchText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.searchText_KeyUp);
            // 
            // trayIcon
            // 
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "Persistent Clipboard";
            this.trayIcon.Visible = true;
            // 
            // HostForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(384, 526);
            this.Controls.Add(this.searchText);
            this.Controls.Add(this.clippedListBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HostForm";
            this.ShowInTaskbar = false;
            this.Text = "Persistent Clipboard";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox clippedListBox;
        private System.Windows.Forms.TextBox searchText;
        private System.Windows.Forms.NotifyIcon trayIcon;
    }
}

