namespace Stalker
{
    partial class MetroAuthForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetroAuthForm));
            this.ChrBrowser = new CefSharp.WinForms.ChromiumWebBrowser();
            this.SuspendLayout();
            // 
            // ChrBrowser
            // 
            this.ChrBrowser.ActivateBrowserOnCreation = false;
            this.ChrBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChrBrowser.Location = new System.Drawing.Point(20, 60);
            this.ChrBrowser.Name = "ChrBrowser";
            this.ChrBrowser.Size = new System.Drawing.Size(617, 450);
            this.ChrBrowser.TabIndex = 0;
            this.ChrBrowser.FrameLoadEnd += new System.EventHandler<CefSharp.FrameLoadEndEventArgs>(this.ChrBrowser_FrameLoadEnd);
            this.ChrBrowser.AddressChanged += new System.EventHandler<CefSharp.AddressChangedEventArgs>(this.ChrBrowser_AddressChanged);
            // 
            // MetroAuthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 530);
            this.Controls.Add(this.ChrBrowser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MetroAuthForm";
            this.Resizable = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Style = MetroFramework.MetroColorStyle.Black;
            this.Text = "Authenticate";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private CefSharp.WinForms.ChromiumWebBrowser ChrBrowser;
    }
}