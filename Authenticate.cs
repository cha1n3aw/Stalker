﻿using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using MetroFramework.Forms;

namespace Stalker
{
    public partial class MetroAuthForm : MetroForm
    {
        private string RedirectUri = "https://oauth.vk.com/blank.html";
        private string AuthUri = "https://oauth.vk.com/authorize?";
        internal int APPid = 2685278; //7482854 fox 2685278 kate mobile2274003
        private string APIversion = "5.126";
        internal bool ClearCookies = false;
        public string Token { get; private set; }
        public MetroAuthForm()
        {
            InitializeComponent();
            ChrBrowser.Show();
            CefSettings settings = new CefSettings { CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF" };
            Cef.Initialize(settings);
            try
            {
                ChrBrowser.Load($"{AuthUri}client_id={APPid}&scope=‭notify,friends,photos,audio,video,stories,pages,"
                            + $"status,notes,wall,ads,offline,docs,groups,notifications,stats,email,market"
                            + $"&redirect_uri={RedirectUri}&revoke=1&display=mobile&response_type=token&v={APIversion}");
            }
            catch (Exception) { }
        }
        private void ChrBrowser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            string uri = e.Address.ToString();
            if (!uri.StartsWith(RedirectUri)) return;
            try
            {
                ChrBrowser.Invoke((MethodInvoker)delegate { ChrBrowser.Hide(); });
                var parameters = (from param in uri.Split('#')[1].Split('&') let parts = param.Split('=') select new { Name = parts[0], Value = parts[1] }).ToDictionary(v => v.Name, v => v.Value);
                Token = parameters["access_token"];
                DialogResult = DialogResult.Yes;
                if (ClearCookies) Cef.GetGlobalCookieManager().DeleteCookies("", "");
            }
            catch (Exception) { DialogResult = DialogResult.No; }
        }

        private void ChrBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (ChrBrowser.IsBrowserInitialized) ChrBrowser.ExecuteScriptAsyncWhenPageLoaded(" if (document.querySelector('input.textfield') === null) { document.querySelector('input.button').click(); }");
        }
    }
}
