namespace Stalker
{
    partial class Stalker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Stalker));
            this.friends = new MetroFramework.Controls.MetroComboBox();
            this.customuserid = new MetroFramework.Controls.MetroTextBox();
            this.EventLogs = new MetroFramework.Controls.MetroTextBox();
            this.StartStop = new MetroFramework.Controls.MetroToggle();
            this.Logs = new MetroFramework.Controls.MetroToggle();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.StalkerTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // friends
            // 
            this.friends.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.friends.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.friends.FormattingEnabled = true;
            this.friends.ItemHeight = 29;
            this.friends.Location = new System.Drawing.Point(23, 63);
            this.friends.Name = "friends";
            this.friends.Size = new System.Drawing.Size(198, 35);
            this.friends.Style = MetroFramework.MetroColorStyle.Black;
            this.friends.TabIndex = 0;
            this.friends.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.friends.UseSelectable = true;
            this.friends.SelectedIndexChanged += new System.EventHandler(this.FriendsIdSelected);
            // 
            // customuserid
            // 
            // 
            // 
            // 
            this.customuserid.CustomButton.Image = null;
            this.customuserid.CustomButton.Location = new System.Drawing.Point(47, 2);
            this.customuserid.CustomButton.Name = "";
            this.customuserid.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.customuserid.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.customuserid.CustomButton.TabIndex = 1;
            this.customuserid.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.customuserid.CustomButton.UseSelectable = true;
            this.customuserid.CustomButton.Visible = false;
            this.customuserid.Lines = new string[] {
        "193640924"};
            this.customuserid.Location = new System.Drawing.Point(227, 78);
            this.customuserid.MaxLength = 32767;
            this.customuserid.Name = "customuserid";
            this.customuserid.PasswordChar = '\0';
            this.customuserid.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.customuserid.SelectedText = "";
            this.customuserid.SelectionLength = 0;
            this.customuserid.SelectionStart = 0;
            this.customuserid.ShortcutsEnabled = true;
            this.customuserid.Size = new System.Drawing.Size(65, 20);
            this.customuserid.Style = MetroFramework.MetroColorStyle.Black;
            this.customuserid.TabIndex = 1;
            this.customuserid.Text = "193640924";
            this.customuserid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.customuserid.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.customuserid.UseSelectable = true;
            this.customuserid.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.customuserid.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.customuserid.Validated += new System.EventHandler(this.CustomUserIdSelected);
            // 
            // EventLogs
            // 
            // 
            // 
            // 
            this.EventLogs.CustomButton.Image = null;
            this.EventLogs.CustomButton.Location = new System.Drawing.Point(146, 2);
            this.EventLogs.CustomButton.Name = "";
            this.EventLogs.CustomButton.Size = new System.Drawing.Size(395, 395);
            this.EventLogs.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.EventLogs.CustomButton.TabIndex = 1;
            this.EventLogs.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.EventLogs.CustomButton.UseSelectable = true;
            this.EventLogs.CustomButton.Visible = false;
            this.EventLogs.Lines = new string[0];
            this.EventLogs.Location = new System.Drawing.Point(23, 104);
            this.EventLogs.MaxLength = 32767;
            this.EventLogs.Multiline = true;
            this.EventLogs.Name = "EventLogs";
            this.EventLogs.PasswordChar = '\0';
            this.EventLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.EventLogs.SelectedText = "";
            this.EventLogs.SelectionLength = 0;
            this.EventLogs.SelectionStart = 0;
            this.EventLogs.ShortcutsEnabled = true;
            this.EventLogs.Size = new System.Drawing.Size(544, 400);
            this.EventLogs.Style = MetroFramework.MetroColorStyle.Black;
            this.EventLogs.TabIndex = 2;
            this.EventLogs.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.EventLogs.UseSelectable = true;
            this.EventLogs.UseStyleColors = true;
            this.EventLogs.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.EventLogs.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // StartStop
            // 
            this.StartStop.AutoSize = true;
            this.StartStop.DisplayStatus = false;
            this.StartStop.Location = new System.Drawing.Point(298, 81);
            this.StartStop.Name = "StartStop";
            this.StartStop.Size = new System.Drawing.Size(50, 17);
            this.StartStop.Style = MetroFramework.MetroColorStyle.Black;
            this.StartStop.TabIndex = 6;
            this.StartStop.Text = "Off";
            this.StartStop.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.StartStop.UseSelectable = true;
            this.StartStop.CheckedChanged += new System.EventHandler(this.StartStop_CheckedChanged);
            // 
            // Logs
            // 
            this.Logs.Checked = true;
            this.Logs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Logs.DisplayStatus = false;
            this.Logs.Location = new System.Drawing.Point(267, 63);
            this.Logs.Name = "Logs";
            this.Logs.Size = new System.Drawing.Size(25, 10);
            this.Logs.Style = MetroFramework.MetroColorStyle.Black;
            this.Logs.TabIndex = 7;
            this.Logs.Text = "On";
            this.Logs.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Logs.UseSelectable = true;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel1.Location = new System.Drawing.Point(227, 60);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(31, 15);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel1.TabIndex = 8;
            this.metroLabel1.Text = "Logs";
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // StalkerTrayIcon
            // 
            this.StalkerTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("StalkerTrayIcon.Icon")));
            this.StalkerTrayIcon.Text = "Stalker";
            this.StalkerTrayIcon.Visible = true;
            this.StalkerTrayIcon.DoubleClick += new System.EventHandler(this.StalkerTrayIcon_DoubleClick);
            // 
            // Stalker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 527);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.Logs);
            this.Controls.Add(this.StartStop);
            this.Controls.Add(this.EventLogs);
            this.Controls.Add(this.customuserid);
            this.Controls.Add(this.friends);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Stalker";
            this.Resizable = false;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Black;
            this.Text = "Stalker";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroComboBox friends;
        private MetroFramework.Controls.MetroTextBox customuserid;
        private MetroFramework.Controls.MetroTextBox EventLogs;
        private MetroFramework.Controls.MetroToggle StartStop;
        private MetroFramework.Controls.MetroToggle Logs;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private System.Windows.Forms.NotifyIcon StalkerTrayIcon;
    }
}