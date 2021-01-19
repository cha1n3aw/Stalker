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
            this.ListOfIDs = new MetroFramework.Controls.MetroComboBox();
            this.CustomIdInput = new MetroFramework.Controls.MetroTextBox();
            this.EventLogs = new MetroFramework.Controls.MetroTextBox();
            this.StartStop = new MetroFramework.Controls.MetroToggle();
            this.Logs = new MetroFramework.Controls.MetroToggle();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.StalkerTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.PreventSleep = new MetroFramework.Controls.MetroToggle();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.StartStopLabel = new MetroFramework.Controls.MetroLabel();
            this.OpenSettings = new MetroFramework.Controls.MetroToggle();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.CheckAlbums = new MetroFramework.Controls.MetroToggle();
            this.CheckAudios = new MetroFramework.Controls.MetroToggle();
            this.CheckFollowers = new MetroFramework.Controls.MetroToggle();
            this.CheckFriends = new MetroFramework.Controls.MetroToggle();
            this.CheckGifts = new MetroFramework.Controls.MetroToggle();
            this.CheckPages = new MetroFramework.Controls.MetroToggle();
            this.CheckPhotos = new MetroFramework.Controls.MetroToggle();
            this.CheckSubscriptions = new MetroFramework.Controls.MetroToggle();
            this.CheckVideos = new MetroFramework.Controls.MetroToggle();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel8 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel9 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel10 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel11 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel12 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel13 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel14 = new MetroFramework.Controls.MetroLabel();
            this.CheckClips = new MetroFramework.Controls.MetroToggle();
            this.metroLabel15 = new MetroFramework.Controls.MetroLabel();
            this.CheckStatus = new MetroFramework.Controls.MetroToggle();
            this.metroLabel16 = new MetroFramework.Controls.MetroLabel();
            this.CheckOnlineDevice = new MetroFramework.Controls.MetroToggle();
            this.metroLabel17 = new MetroFramework.Controls.MetroLabel();
            this.SaveToken = new MetroFramework.Controls.MetroToggle();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.ClearCookies = new MetroFramework.Controls.MetroToggle();
            this.metroLabel18 = new MetroFramework.Controls.MetroLabel();
            this.Autostart = new MetroFramework.Controls.MetroToggle();
            this.metroLabel19 = new MetroFramework.Controls.MetroLabel();
            this.CheckPosts = new MetroFramework.Controls.MetroToggle();
            this.LastOnlineLabel = new MetroFramework.Controls.MetroLabel();
            this.CustomAppIdToggle = new MetroFramework.Controls.MetroToggle();
            this.CustomAppId = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel20 = new MetroFramework.Controls.MetroLabel();
            this.CheckStories = new MetroFramework.Controls.MetroToggle();
            this.SuspendLayout();
            // 
            // ListOfIDs
            // 
            this.ListOfIDs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ListOfIDs.FontSize = MetroFramework.MetroComboBoxSize.Tall;
            this.ListOfIDs.FormattingEnabled = true;
            this.ListOfIDs.ItemHeight = 29;
            this.ListOfIDs.Location = new System.Drawing.Point(23, 63);
            this.ListOfIDs.Name = "ListOfIDs";
            this.ListOfIDs.PromptText = "Select User";
            this.ListOfIDs.Size = new System.Drawing.Size(267, 35);
            this.ListOfIDs.Style = MetroFramework.MetroColorStyle.Black;
            this.ListOfIDs.TabIndex = 0;
            this.ListOfIDs.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ListOfIDs.UseSelectable = true;
            this.ListOfIDs.SelectedIndexChanged += new System.EventHandler(this.FriendsIdSelected);
            // 
            // CustomIdInput
            // 
            // 
            // 
            // 
            this.CustomIdInput.CustomButton.Image = null;
            this.CustomIdInput.CustomButton.Location = new System.Drawing.Point(70, 2);
            this.CustomIdInput.CustomButton.Name = "";
            this.CustomIdInput.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.CustomIdInput.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.CustomIdInput.CustomButton.TabIndex = 1;
            this.CustomIdInput.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.CustomIdInput.CustomButton.UseSelectable = true;
            this.CustomIdInput.CustomButton.Visible = false;
            this.CustomIdInput.Lines = new string[0];
            this.CustomIdInput.Location = new System.Drawing.Point(296, 78);
            this.CustomIdInput.MaxLength = 32767;
            this.CustomIdInput.Name = "CustomIdInput";
            this.CustomIdInput.PasswordChar = '\0';
            this.CustomIdInput.PromptText = "Custom ID";
            this.CustomIdInput.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.CustomIdInput.SelectedText = "";
            this.CustomIdInput.SelectionLength = 0;
            this.CustomIdInput.SelectionStart = 0;
            this.CustomIdInput.ShortcutsEnabled = true;
            this.CustomIdInput.Size = new System.Drawing.Size(88, 20);
            this.CustomIdInput.Style = MetroFramework.MetroColorStyle.Black;
            this.CustomIdInput.TabIndex = 1;
            this.CustomIdInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.CustomIdInput.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CustomIdInput.UseSelectable = true;
            this.CustomIdInput.WaterMark = "Custom ID";
            this.CustomIdInput.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.CustomIdInput.WaterMarkFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CustomIdInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CustomUserIdEntered);
            // 
            // EventLogs
            // 
            // 
            // 
            // 
            this.EventLogs.CustomButton.Image = null;
            this.EventLogs.CustomButton.Location = new System.Drawing.Point(63, 1);
            this.EventLogs.CustomButton.Name = "";
            this.EventLogs.CustomButton.Size = new System.Drawing.Size(399, 399);
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
            this.EventLogs.ReadOnly = true;
            this.EventLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.EventLogs.SelectedText = "";
            this.EventLogs.SelectionLength = 0;
            this.EventLogs.SelectionStart = 0;
            this.EventLogs.ShortcutsEnabled = true;
            this.EventLogs.Size = new System.Drawing.Size(463, 401);
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
            this.StartStop.DisplayStatus = false;
            this.StartStop.Location = new System.Drawing.Point(390, 78);
            this.StartStop.Name = "StartStop";
            this.StartStop.Size = new System.Drawing.Size(45, 20);
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
            this.Logs.Location = new System.Drawing.Point(595, 126);
            this.Logs.Name = "Logs";
            this.Logs.Size = new System.Drawing.Size(25, 15);
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
            this.metroLabel1.Location = new System.Drawing.Point(509, 126);
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
            // PreventSleep
            // 
            this.PreventSleep.Checked = true;
            this.PreventSleep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PreventSleep.DisplayStatus = false;
            this.PreventSleep.Location = new System.Drawing.Point(595, 63);
            this.PreventSleep.Name = "PreventSleep";
            this.PreventSleep.Size = new System.Drawing.Size(25, 15);
            this.PreventSleep.Style = MetroFramework.MetroColorStyle.Black;
            this.PreventSleep.TabIndex = 9;
            this.PreventSleep.Text = "On";
            this.PreventSleep.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.PreventSleep.UseSelectable = true;
            this.PreventSleep.CheckedChanged += new System.EventHandler(this.PreventSleep_CheckedChanged);
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel2.Location = new System.Drawing.Point(509, 63);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(59, 15);
            this.metroLabel2.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel2.TabIndex = 10;
            this.metroLabel2.Text = "Anti-Sleep";
            this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // StartStopLabel
            // 
            this.StartStopLabel.AutoSize = true;
            this.StartStopLabel.FontSize = MetroFramework.MetroLabelSize.Small;
            this.StartStopLabel.Location = new System.Drawing.Point(397, 60);
            this.StartStopLabel.Name = "StartStopLabel";
            this.StartStopLabel.Size = new System.Drawing.Size(32, 15);
            this.StartStopLabel.Style = MetroFramework.MetroColorStyle.Black;
            this.StartStopLabel.TabIndex = 12;
            this.StartStopLabel.Text = "Start";
            this.StartStopLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // OpenSettings
            // 
            this.OpenSettings.DisplayStatus = false;
            this.OpenSettings.Location = new System.Drawing.Point(441, 78);
            this.OpenSettings.Name = "OpenSettings";
            this.OpenSettings.Size = new System.Drawing.Size(45, 20);
            this.OpenSettings.Style = MetroFramework.MetroColorStyle.Black;
            this.OpenSettings.TabIndex = 13;
            this.OpenSettings.Text = "Off";
            this.OpenSettings.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.OpenSettings.UseSelectable = true;
            this.OpenSettings.CheckedChanged += new System.EventHandler(this.OpenSettings_CheckedChanged);
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel4.Location = new System.Drawing.Point(441, 60);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(47, 15);
            this.metroLabel4.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel4.TabIndex = 14;
            this.metroLabel4.Text = "Settings";
            this.metroLabel4.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // CheckAlbums
            // 
            this.CheckAlbums.Checked = true;
            this.CheckAlbums.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckAlbums.DisplayStatus = false;
            this.CheckAlbums.Location = new System.Drawing.Point(595, 189);
            this.CheckAlbums.Name = "CheckAlbums";
            this.CheckAlbums.Size = new System.Drawing.Size(25, 15);
            this.CheckAlbums.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckAlbums.TabIndex = 15;
            this.CheckAlbums.Text = "On";
            this.CheckAlbums.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckAlbums.UseSelectable = true;
            // 
            // CheckAudios
            // 
            this.CheckAudios.Checked = true;
            this.CheckAudios.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckAudios.DisplayStatus = false;
            this.CheckAudios.Location = new System.Drawing.Point(595, 210);
            this.CheckAudios.Name = "CheckAudios";
            this.CheckAudios.Size = new System.Drawing.Size(25, 15);
            this.CheckAudios.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckAudios.TabIndex = 16;
            this.CheckAudios.Text = "On";
            this.CheckAudios.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckAudios.UseSelectable = true;
            // 
            // CheckFollowers
            // 
            this.CheckFollowers.Checked = true;
            this.CheckFollowers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckFollowers.DisplayStatus = false;
            this.CheckFollowers.Location = new System.Drawing.Point(595, 273);
            this.CheckFollowers.Name = "CheckFollowers";
            this.CheckFollowers.Size = new System.Drawing.Size(25, 15);
            this.CheckFollowers.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckFollowers.TabIndex = 17;
            this.CheckFollowers.Text = "On";
            this.CheckFollowers.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckFollowers.UseSelectable = true;
            // 
            // CheckFriends
            // 
            this.CheckFriends.Checked = true;
            this.CheckFriends.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckFriends.DisplayStatus = false;
            this.CheckFriends.Location = new System.Drawing.Point(595, 294);
            this.CheckFriends.Name = "CheckFriends";
            this.CheckFriends.Size = new System.Drawing.Size(25, 15);
            this.CheckFriends.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckFriends.TabIndex = 18;
            this.CheckFriends.Text = "On";
            this.CheckFriends.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckFriends.UseSelectable = true;
            // 
            // CheckGifts
            // 
            this.CheckGifts.Checked = true;
            this.CheckGifts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckGifts.DisplayStatus = false;
            this.CheckGifts.Location = new System.Drawing.Point(595, 315);
            this.CheckGifts.Name = "CheckGifts";
            this.CheckGifts.Size = new System.Drawing.Size(25, 15);
            this.CheckGifts.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckGifts.TabIndex = 19;
            this.CheckGifts.Text = "On";
            this.CheckGifts.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckGifts.UseSelectable = true;
            // 
            // CheckPages
            // 
            this.CheckPages.Checked = true;
            this.CheckPages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckPages.DisplayStatus = false;
            this.CheckPages.Location = new System.Drawing.Point(595, 336);
            this.CheckPages.Name = "CheckPages";
            this.CheckPages.Size = new System.Drawing.Size(25, 15);
            this.CheckPages.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckPages.TabIndex = 20;
            this.CheckPages.Text = "On";
            this.CheckPages.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckPages.UseSelectable = true;
            // 
            // CheckPhotos
            // 
            this.CheckPhotos.Checked = true;
            this.CheckPhotos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckPhotos.DisplayStatus = false;
            this.CheckPhotos.Location = new System.Drawing.Point(595, 357);
            this.CheckPhotos.Name = "CheckPhotos";
            this.CheckPhotos.Size = new System.Drawing.Size(25, 15);
            this.CheckPhotos.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckPhotos.TabIndex = 21;
            this.CheckPhotos.Text = "On";
            this.CheckPhotos.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckPhotos.UseSelectable = true;
            // 
            // CheckSubscriptions
            // 
            this.CheckSubscriptions.Checked = true;
            this.CheckSubscriptions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckSubscriptions.DisplayStatus = false;
            this.CheckSubscriptions.Location = new System.Drawing.Point(595, 420);
            this.CheckSubscriptions.Name = "CheckSubscriptions";
            this.CheckSubscriptions.Size = new System.Drawing.Size(25, 15);
            this.CheckSubscriptions.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckSubscriptions.TabIndex = 22;
            this.CheckSubscriptions.Text = "On";
            this.CheckSubscriptions.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckSubscriptions.UseSelectable = true;
            // 
            // CheckVideos
            // 
            this.CheckVideos.Checked = true;
            this.CheckVideos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckVideos.DisplayStatus = false;
            this.CheckVideos.Location = new System.Drawing.Point(595, 441);
            this.CheckVideos.Name = "CheckVideos";
            this.CheckVideos.Size = new System.Drawing.Size(25, 15);
            this.CheckVideos.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckVideos.TabIndex = 23;
            this.CheckVideos.Text = "On";
            this.CheckVideos.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckVideos.UseSelectable = true;
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel5.Location = new System.Drawing.Point(509, 189);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(45, 15);
            this.metroLabel5.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel5.TabIndex = 24;
            this.metroLabel5.Text = "Albums";
            this.metroLabel5.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel6.Location = new System.Drawing.Point(509, 210);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(42, 15);
            this.metroLabel6.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel6.TabIndex = 25;
            this.metroLabel6.Text = "Audios";
            this.metroLabel6.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel7
            // 
            this.metroLabel7.AutoSize = true;
            this.metroLabel7.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel7.Location = new System.Drawing.Point(509, 273);
            this.metroLabel7.Name = "metroLabel7";
            this.metroLabel7.Size = new System.Drawing.Size(54, 15);
            this.metroLabel7.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel7.TabIndex = 26;
            this.metroLabel7.Text = "Followers";
            this.metroLabel7.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel8
            // 
            this.metroLabel8.AutoSize = true;
            this.metroLabel8.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel8.Location = new System.Drawing.Point(509, 294);
            this.metroLabel8.Name = "metroLabel8";
            this.metroLabel8.Size = new System.Drawing.Size(43, 15);
            this.metroLabel8.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel8.TabIndex = 27;
            this.metroLabel8.Text = "Friends";
            this.metroLabel8.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel9
            // 
            this.metroLabel9.AutoSize = true;
            this.metroLabel9.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel9.Location = new System.Drawing.Point(509, 315);
            this.metroLabel9.Name = "metroLabel9";
            this.metroLabel9.Size = new System.Drawing.Size(29, 15);
            this.metroLabel9.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel9.TabIndex = 28;
            this.metroLabel9.Text = "Gifts";
            this.metroLabel9.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel10
            // 
            this.metroLabel10.AutoSize = true;
            this.metroLabel10.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel10.Location = new System.Drawing.Point(509, 336);
            this.metroLabel10.Name = "metroLabel10";
            this.metroLabel10.Size = new System.Drawing.Size(37, 15);
            this.metroLabel10.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel10.TabIndex = 29;
            this.metroLabel10.Text = "Pages";
            this.metroLabel10.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel11
            // 
            this.metroLabel11.AutoSize = true;
            this.metroLabel11.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel11.Location = new System.Drawing.Point(509, 357);
            this.metroLabel11.Name = "metroLabel11";
            this.metroLabel11.Size = new System.Drawing.Size(43, 15);
            this.metroLabel11.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel11.TabIndex = 30;
            this.metroLabel11.Text = "Photos";
            this.metroLabel11.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel12
            // 
            this.metroLabel12.AutoSize = true;
            this.metroLabel12.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel12.Location = new System.Drawing.Point(509, 420);
            this.metroLabel12.Name = "metroLabel12";
            this.metroLabel12.Size = new System.Drawing.Size(73, 15);
            this.metroLabel12.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel12.TabIndex = 31;
            this.metroLabel12.Text = "Subscriptions";
            this.metroLabel12.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel13
            // 
            this.metroLabel13.AutoSize = true;
            this.metroLabel13.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel13.Location = new System.Drawing.Point(509, 441);
            this.metroLabel13.Name = "metroLabel13";
            this.metroLabel13.Size = new System.Drawing.Size(41, 15);
            this.metroLabel13.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel13.TabIndex = 32;
            this.metroLabel13.Text = "Videos";
            this.metroLabel13.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // metroLabel14
            // 
            this.metroLabel14.AutoSize = true;
            this.metroLabel14.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel14.Location = new System.Drawing.Point(509, 231);
            this.metroLabel14.Name = "metroLabel14";
            this.metroLabel14.Size = new System.Drawing.Size(80, 15);
            this.metroLabel14.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel14.TabIndex = 34;
            this.metroLabel14.Text = "Clips Followers";
            this.metroLabel14.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // CheckClips
            // 
            this.CheckClips.Checked = true;
            this.CheckClips.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckClips.DisplayStatus = false;
            this.CheckClips.Location = new System.Drawing.Point(595, 231);
            this.CheckClips.Name = "CheckClips";
            this.CheckClips.Size = new System.Drawing.Size(25, 15);
            this.CheckClips.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckClips.TabIndex = 33;
            this.CheckClips.Text = "On";
            this.CheckClips.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckClips.UseSelectable = true;
            // 
            // metroLabel15
            // 
            this.metroLabel15.AutoSize = true;
            this.metroLabel15.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel15.Location = new System.Drawing.Point(509, 378);
            this.metroLabel15.Name = "metroLabel15";
            this.metroLabel15.Size = new System.Drawing.Size(38, 15);
            this.metroLabel15.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel15.TabIndex = 36;
            this.metroLabel15.Text = "Status";
            this.metroLabel15.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // CheckStatus
            // 
            this.CheckStatus.Checked = true;
            this.CheckStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckStatus.DisplayStatus = false;
            this.CheckStatus.Location = new System.Drawing.Point(595, 378);
            this.CheckStatus.Name = "CheckStatus";
            this.CheckStatus.Size = new System.Drawing.Size(25, 15);
            this.CheckStatus.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckStatus.TabIndex = 35;
            this.CheckStatus.Text = "On";
            this.CheckStatus.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckStatus.UseSelectable = true;
            // 
            // metroLabel16
            // 
            this.metroLabel16.AutoSize = true;
            this.metroLabel16.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel16.Location = new System.Drawing.Point(509, 252);
            this.metroLabel16.Name = "metroLabel16";
            this.metroLabel16.Size = new System.Drawing.Size(39, 15);
            this.metroLabel16.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel16.TabIndex = 38;
            this.metroLabel16.Text = "Device";
            this.metroLabel16.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // CheckOnlineDevice
            // 
            this.CheckOnlineDevice.Checked = true;
            this.CheckOnlineDevice.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckOnlineDevice.DisplayStatus = false;
            this.CheckOnlineDevice.Location = new System.Drawing.Point(595, 252);
            this.CheckOnlineDevice.Name = "CheckOnlineDevice";
            this.CheckOnlineDevice.Size = new System.Drawing.Size(25, 15);
            this.CheckOnlineDevice.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckOnlineDevice.TabIndex = 37;
            this.CheckOnlineDevice.Text = "On";
            this.CheckOnlineDevice.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckOnlineDevice.UseSelectable = true;
            // 
            // metroLabel17
            // 
            this.metroLabel17.AutoSize = true;
            this.metroLabel17.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel17.Location = new System.Drawing.Point(509, 147);
            this.metroLabel17.Name = "metroLabel17";
            this.metroLabel17.Size = new System.Drawing.Size(61, 15);
            this.metroLabel17.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel17.TabIndex = 41;
            this.metroLabel17.Text = "Save Token";
            this.metroLabel17.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // SaveToken
            // 
            this.SaveToken.DisplayStatus = false;
            this.SaveToken.Location = new System.Drawing.Point(595, 147);
            this.SaveToken.Name = "SaveToken";
            this.SaveToken.Size = new System.Drawing.Size(25, 15);
            this.SaveToken.Style = MetroFramework.MetroColorStyle.Black;
            this.SaveToken.TabIndex = 40;
            this.SaveToken.Text = "Off";
            this.SaveToken.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.SaveToken.UseSelectable = true;
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel3.Location = new System.Drawing.Point(509, 105);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(74, 15);
            this.metroLabel3.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel3.TabIndex = 43;
            this.metroLabel3.Text = "Clear Cookies";
            this.metroLabel3.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // ClearCookies
            // 
            this.ClearCookies.DisplayStatus = false;
            this.ClearCookies.Location = new System.Drawing.Point(595, 105);
            this.ClearCookies.Name = "ClearCookies";
            this.ClearCookies.Size = new System.Drawing.Size(25, 15);
            this.ClearCookies.Style = MetroFramework.MetroColorStyle.Black;
            this.ClearCookies.TabIndex = 42;
            this.ClearCookies.Text = "Off";
            this.ClearCookies.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.ClearCookies.UseSelectable = true;
            // 
            // metroLabel18
            // 
            this.metroLabel18.AutoSize = true;
            this.metroLabel18.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel18.Location = new System.Drawing.Point(509, 84);
            this.metroLabel18.Name = "metroLabel18";
            this.metroLabel18.Size = new System.Drawing.Size(56, 15);
            this.metroLabel18.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel18.TabIndex = 45;
            this.metroLabel18.Text = "Autostart";
            this.metroLabel18.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // Autostart
            // 
            this.Autostart.DisplayStatus = false;
            this.Autostart.Location = new System.Drawing.Point(595, 84);
            this.Autostart.Name = "Autostart";
            this.Autostart.Size = new System.Drawing.Size(25, 15);
            this.Autostart.Style = MetroFramework.MetroColorStyle.Black;
            this.Autostart.TabIndex = 44;
            this.Autostart.Text = "Off";
            this.Autostart.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Autostart.UseSelectable = true;
            // 
            // metroLabel19
            // 
            this.metroLabel19.AutoSize = true;
            this.metroLabel19.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel19.Location = new System.Drawing.Point(509, 462);
            this.metroLabel19.Name = "metroLabel19";
            this.metroLabel19.Size = new System.Drawing.Size(57, 15);
            this.metroLabel19.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel19.TabIndex = 47;
            this.metroLabel19.Text = "Wall Posts";
            this.metroLabel19.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // CheckPosts
            // 
            this.CheckPosts.DisplayStatus = false;
            this.CheckPosts.Location = new System.Drawing.Point(595, 462);
            this.CheckPosts.Name = "CheckPosts";
            this.CheckPosts.Size = new System.Drawing.Size(25, 15);
            this.CheckPosts.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckPosts.TabIndex = 46;
            this.CheckPosts.Text = "Off";
            this.CheckPosts.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckPosts.UseSelectable = true;
            // 
            // LastOnlineLabel
            // 
            this.LastOnlineLabel.AutoSize = true;
            this.LastOnlineLabel.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.LastOnlineLabel.Location = new System.Drawing.Point(509, 480);
            this.LastOnlineLabel.Name = "LastOnlineLabel";
            this.LastOnlineLabel.Size = new System.Drawing.Size(97, 25);
            this.LastOnlineLabel.Style = MetroFramework.MetroColorStyle.Black;
            this.LastOnlineLabel.TabIndex = 48;
            this.LastOnlineLabel.Text = "Last Online";
            this.LastOnlineLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // CustomAppIdToggle
            // 
            this.CustomAppIdToggle.DisplayStatus = false;
            this.CustomAppIdToggle.Location = new System.Drawing.Point(595, 168);
            this.CustomAppIdToggle.Name = "CustomAppIdToggle";
            this.CustomAppIdToggle.Size = new System.Drawing.Size(25, 15);
            this.CustomAppIdToggle.Style = MetroFramework.MetroColorStyle.Black;
            this.CustomAppIdToggle.TabIndex = 49;
            this.CustomAppIdToggle.Text = "Off";
            this.CustomAppIdToggle.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CustomAppIdToggle.UseSelectable = true;
            this.CustomAppIdToggle.CheckedChanged += new System.EventHandler(this.CustomAppIdToggled);
            // 
            // CustomAppId
            // 
            // 
            // 
            // 
            this.CustomAppId.CustomButton.Image = null;
            this.CustomAppId.CustomButton.Location = new System.Drawing.Point(62, 2);
            this.CustomAppId.CustomButton.Name = "";
            this.CustomAppId.CustomButton.Size = new System.Drawing.Size(15, 15);
            this.CustomAppId.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.CustomAppId.CustomButton.TabIndex = 1;
            this.CustomAppId.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.CustomAppId.CustomButton.UseSelectable = true;
            this.CustomAppId.CustomButton.Visible = false;
            this.CustomAppId.Enabled = false;
            this.CustomAppId.Lines = new string[0];
            this.CustomAppId.Location = new System.Drawing.Point(509, 165);
            this.CustomAppId.MaxLength = 32767;
            this.CustomAppId.Name = "CustomAppId";
            this.CustomAppId.PasswordChar = '\0';
            this.CustomAppId.PromptText = "App ID";
            this.CustomAppId.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.CustomAppId.SelectedText = "";
            this.CustomAppId.SelectionLength = 0;
            this.CustomAppId.SelectionStart = 0;
            this.CustomAppId.ShortcutsEnabled = true;
            this.CustomAppId.Size = new System.Drawing.Size(80, 20);
            this.CustomAppId.Style = MetroFramework.MetroColorStyle.Black;
            this.CustomAppId.TabIndex = 50;
            this.CustomAppId.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CustomAppId.UseSelectable = true;
            this.CustomAppId.UseStyleColors = true;
            this.CustomAppId.WaterMark = "App ID";
            this.CustomAppId.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.CustomAppId.WaterMarkFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            // 
            // metroLabel20
            // 
            this.metroLabel20.AutoSize = true;
            this.metroLabel20.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel20.Location = new System.Drawing.Point(509, 399);
            this.metroLabel20.Name = "metroLabel20";
            this.metroLabel20.Size = new System.Drawing.Size(41, 15);
            this.metroLabel20.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel20.TabIndex = 52;
            this.metroLabel20.Text = "Stories";
            this.metroLabel20.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // CheckStories
            // 
            this.CheckStories.DisplayStatus = false;
            this.CheckStories.Location = new System.Drawing.Point(595, 399);
            this.CheckStories.Name = "CheckStories";
            this.CheckStories.Size = new System.Drawing.Size(25, 15);
            this.CheckStories.Style = MetroFramework.MetroColorStyle.Black;
            this.CheckStories.TabIndex = 51;
            this.CheckStories.Text = "Off";
            this.CheckStories.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.CheckStories.UseSelectable = true;
            // 
            // Stalker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 528);
            this.Controls.Add(this.metroLabel20);
            this.Controls.Add(this.CheckStories);
            this.Controls.Add(this.CustomAppId);
            this.Controls.Add(this.CustomAppIdToggle);
            this.Controls.Add(this.LastOnlineLabel);
            this.Controls.Add(this.metroLabel19);
            this.Controls.Add(this.CheckPosts);
            this.Controls.Add(this.metroLabel18);
            this.Controls.Add(this.Autostart);
            this.Controls.Add(this.metroLabel3);
            this.Controls.Add(this.ClearCookies);
            this.Controls.Add(this.metroLabel17);
            this.Controls.Add(this.SaveToken);
            this.Controls.Add(this.metroLabel16);
            this.Controls.Add(this.CheckOnlineDevice);
            this.Controls.Add(this.metroLabel15);
            this.Controls.Add(this.CheckStatus);
            this.Controls.Add(this.metroLabel14);
            this.Controls.Add(this.CheckClips);
            this.Controls.Add(this.metroLabel13);
            this.Controls.Add(this.metroLabel12);
            this.Controls.Add(this.metroLabel11);
            this.Controls.Add(this.metroLabel10);
            this.Controls.Add(this.metroLabel9);
            this.Controls.Add(this.metroLabel8);
            this.Controls.Add(this.metroLabel7);
            this.Controls.Add(this.metroLabel6);
            this.Controls.Add(this.metroLabel5);
            this.Controls.Add(this.CheckVideos);
            this.Controls.Add(this.CheckSubscriptions);
            this.Controls.Add(this.CheckPhotos);
            this.Controls.Add(this.CheckPages);
            this.Controls.Add(this.CheckGifts);
            this.Controls.Add(this.CheckFriends);
            this.Controls.Add(this.CheckFollowers);
            this.Controls.Add(this.CheckAudios);
            this.Controls.Add(this.CheckAlbums);
            this.Controls.Add(this.metroLabel4);
            this.Controls.Add(this.OpenSettings);
            this.Controls.Add(this.StartStopLabel);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.PreventSleep);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.Logs);
            this.Controls.Add(this.StartStop);
            this.Controls.Add(this.EventLogs);
            this.Controls.Add(this.CustomIdInput);
            this.Controls.Add(this.ListOfIDs);
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

        private MetroFramework.Controls.MetroComboBox ListOfIDs;
        private MetroFramework.Controls.MetroTextBox CustomIdInput;
        private MetroFramework.Controls.MetroTextBox EventLogs;
        private MetroFramework.Controls.MetroToggle StartStop;
        private MetroFramework.Controls.MetroToggle Logs;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private System.Windows.Forms.NotifyIcon StalkerTrayIcon;
        private MetroFramework.Controls.MetroToggle PreventSleep;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel StartStopLabel;
        private MetroFramework.Controls.MetroToggle OpenSettings;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroToggle CheckAlbums;
        private MetroFramework.Controls.MetroToggle CheckAudios;
        private MetroFramework.Controls.MetroToggle CheckFollowers;
        private MetroFramework.Controls.MetroToggle CheckFriends;
        private MetroFramework.Controls.MetroToggle CheckGifts;
        private MetroFramework.Controls.MetroToggle CheckPages;
        private MetroFramework.Controls.MetroToggle CheckPhotos;
        private MetroFramework.Controls.MetroToggle CheckSubscriptions;
        private MetroFramework.Controls.MetroToggle CheckVideos;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroLabel metroLabel6;
        private MetroFramework.Controls.MetroLabel metroLabel7;
        private MetroFramework.Controls.MetroLabel metroLabel8;
        private MetroFramework.Controls.MetroLabel metroLabel9;
        private MetroFramework.Controls.MetroLabel metroLabel10;
        private MetroFramework.Controls.MetroLabel metroLabel11;
        private MetroFramework.Controls.MetroLabel metroLabel12;
        private MetroFramework.Controls.MetroLabel metroLabel13;
        private MetroFramework.Controls.MetroLabel metroLabel14;
        private MetroFramework.Controls.MetroToggle CheckClips;
        private MetroFramework.Controls.MetroLabel metroLabel15;
        private MetroFramework.Controls.MetroToggle CheckStatus;
        private MetroFramework.Controls.MetroLabel metroLabel16;
        private MetroFramework.Controls.MetroToggle CheckOnlineDevice;
        private MetroFramework.Controls.MetroLabel metroLabel17;
        private MetroFramework.Controls.MetroToggle SaveToken;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroToggle ClearCookies;
        private MetroFramework.Controls.MetroLabel metroLabel18;
        private MetroFramework.Controls.MetroToggle Autostart;
        private MetroFramework.Controls.MetroLabel metroLabel19;
        private MetroFramework.Controls.MetroToggle CheckPosts;
        private MetroFramework.Controls.MetroLabel LastOnlineLabel;
        private MetroFramework.Controls.MetroToggle CustomAppIdToggle;
        private MetroFramework.Controls.MetroTextBox CustomAppId;
        private MetroFramework.Controls.MetroLabel metroLabel20;
        private MetroFramework.Controls.MetroToggle CheckStories;
    }
}