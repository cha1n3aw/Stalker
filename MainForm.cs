using MetroFramework.Forms;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Stalker
{
    public partial class Stalker : MetroForm
    {
        private const string ApiRequestLink = "https://api.vk.com/method/";
        private static string AuthToken = String.Empty;
        private static string server_response = String.Empty;
        private List<string[]> FriendsList = new List<string[]>();
        int user_id = 193640924;
        Thread polling;
        private void Get(string uri) { server_response = new WebClient { Encoding = Encoding.UTF8 }.DownloadString(uri); }
        private void FetchFriends()
        {
            Get($"{ApiRequestLink}friends.get?order=name&count=5000&fields=domain&access_token={AuthToken}&v=5.126");
            dynamic friendslist = JObject.Parse(server_response);
            var friendsl = friendslist.response;
            for (int i = 0; i < Convert.ToInt32(friendsl.count); i++)
            {
                FriendsList[i][0] = friendsl.items[i].id;
                FriendsList[i][1] = friendsl.items[i].first_name;
                FriendsList[i][2] = friendsl.items[i].last_name;
            }
            foreach (string[] str in FriendsList) friends.Items.Add($"{str[1]} {str[2]}");
        }
        private void LogInVK() 
        {
            MetroAuthForm AuthForm = new MetroAuthForm();
            if (AuthForm.ShowDialog() == DialogResult.Yes)
            {
                AuthToken = AuthForm.Token;
                FetchFriends();
            }
            else if (Application.MessageLoop) Application.Exit();
            else Environment.Exit(1);
            AuthForm.Dispose();
            GC.Collect();
        }
        private void Poll()
        {
            Counters counters = new Counters();
            bool continuous = false;
            while (true)
            {
                try
                {
                    string onlineappid = string.Empty;
                    string newplatform = string.Empty;
                    Get($"{ApiRequestLink}users.get?user_ids={user_id}&fields=online,counters,last_seen,activity&access_token={AuthToken}&v=5.126");
                    dynamic decodedresponse = JObject.Parse(server_response);
                    var decoded = decodedresponse.response[0];
                    DateTime pDate = new DateTime(1970, 1, 1, 5, 0, 0, 0).AddSeconds((double)decoded.last_seen.time);
                    if (!(bool)decoded.is_closed)
                    {
                        if (decoded.online_app != null)
                        {
                            switch ((int)decoded.online_app)
                            {
                                case 2274003:
                                    { onlineappid = "Android VK"; }
                                    break;
                                case 3140623:
                                    { onlineappid = "iPhone VK"; }
                                    break;
                                case 3682744:
                                    { onlineappid = "iPad VK"; }
                                    break;
                                case 3697615:
                                    { onlineappid = "Windows Desktop VK"; }
                                    break;
                                case 3502557:
                                    { onlineappid = "Windows Phone VK"; }
                                    break;
                                case 2685278:
                                    { onlineappid = "Kate Mobile"; }
                                    break;
                                default:
                                    { onlineappid = ((int)decoded.online_app).ToString(); }
                                    break;
                            }
                            if (counters.onlineappid != onlineappid)
                            {
                                if (continuous) WriteToFile($"User switched to another app: from { counters.onlineappid }  to { onlineappid }{ Environment.NewLine }");
                                counters.onlineappid = onlineappid;
                            }
                        }
                        switch ((int)decoded.last_seen.platform)
                        {
                            case 1:
                                { newplatform = "Mobile WEB"; }
                                break;
                            case 2:
                                { newplatform = "iPhone"; }
                                break;
                            case 3:
                                { newplatform = "iPad"; }
                                break;
                            case 4:
                                { newplatform = "Android"; }
                                break;
                            case 5:
                                { newplatform = "WinPhone"; }
                                break;
                            case 6:
                                { newplatform = "Windows 8"; }
                                break;
                            case 7:
                                { newplatform = "WEB"; }
                                break;
                            case 8:
                                { newplatform = "VK Mobile"; }
                                break;
                        }
                        if (counters.platform != newplatform)
                        {
                            if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"User switched from { counters.platform }  to { newplatform }{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"User switched from { counters.platform }  to { newplatform }{ Environment.NewLine }");
                            });
                            counters.platform = newplatform;
                        }
                        if (counters.albums != (int)decoded.counters.albums)
                        {
                            if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"Update in albums: { counters.albums }  to { (int)decoded.counters.albums }{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"Update in albums: { counters.albums }  to { (int)decoded.counters.albums }{ Environment.NewLine }");
                            });
                            counters.albums = (int)decoded.counters.albums;
                        }
                        if (counters.audios != (int)decoded.counters.audios)
                        {
                            if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"Update in audios: { counters.audios }  to { (int)decoded.counters.audios }{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"Update in audios: { counters.audios }  to { (int)decoded.counters.audios }{ Environment.NewLine }");
                            });
                            counters.audios = (int)decoded.counters.audios;
                        }
                        if (counters.followers != (int)decoded.counters.followers)
                        {
                            if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"Update in followers: { counters.followers }  to { (int)decoded.counters.followers }{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"Update in followers: { counters.followers }  to { (int)decoded.counters.followers }{ Environment.NewLine }");
                            });
                            counters.followers = (int)decoded.counters.followers;
                        }
                        if (counters.friends != (int)decoded.counters.friends)
                        {
                            if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"Update in friends: { counters.friends }  to { (int)decoded.counters.friends }{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"Update in friends: { counters.friends }  to { (int)decoded.counters.friends }{ Environment.NewLine }");
                            });
                            counters.friends = (int)decoded.counters.friends;
                        }
                        if (counters.gifts != (int)decoded.counters.gifts)
                        {
                            if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"Update in gifts: { counters.gifts }  to { (int)decoded.counters.gifts }{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"Update in gifts: { counters.gifts }  to { (int)decoded.counters.gifts }{ Environment.NewLine }");
                            });
                            counters.gifts = (int)decoded.counters.gifts;
                        }
                        if (counters.pages != (int)decoded.counters.pages)
                        {
                            /*if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"Update in pages: { counters.pages }  to { (int)decoded.counters.pages }{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"Update in pages: { counters.pages }  to { (int)decoded.counters.pages }{ Environment.NewLine }");
                            });*/
                            counters.pages = (int)decoded.counters.pages;
                        }
                        if (counters.photos != (int)decoded.counters.photos)
                        {
                            if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"Update in photos: { counters.photos }  to { (int)decoded.counters.photos }{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"Update in photos: { counters.photos }  to { (int)decoded.counters.photos }{ Environment.NewLine }");
                            });
                            counters.photos = (int)decoded.counters.photos;
                        }
                        if (counters.subscriptions != (int)decoded.counters.subscriptions)
                        {
                            if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"Update in subscpritions: { counters.subscriptions }  to { (int)decoded.counters.subscriptions }{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"Update in subscpritions: { counters.subscriptions }  to { (int)decoded.counters.subscriptions }{ Environment.NewLine }");
                            });
                            counters.subscriptions = (int)decoded.counters.subscriptions;
                        }
                        if (counters.videos != (int)decoded.counters.videos)
                        {
                            if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"Update in videos: { counters.videos }  to { (int)decoded.counters.videos }{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"Update in videos: { counters.videos }  to { (int)decoded.counters.videos }{ Environment.NewLine }");
                            });
                            counters.videos = (int)decoded.counters.videos;
                        }
                        if (counters.clips_followers != (int)decoded.counters.clips_followers)
                        {
                            if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"Update in clips followers: { counters.clips_followers }  to { (int)decoded.counters.clips_followers }{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"Update in clips followers: { counters.clips_followers }  to { (int)decoded.counters.clips_followers }{ Environment.NewLine }");
                            });
                            counters.clips_followers = (int)decoded.counters.clips_followers;
                        }
                        if (counters.activity != (string)decoded.activity)
                        {
                            if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                            {
                                if (EventLogs.Text[EventLogs.Text.Length - 1] == 'M') WriteToFile($"   —  { pDate }{ Environment.NewLine }"
                                + $"Update in status: old '{ counters.clips_followers }', new '{ (int)decoded.counters.clips_followers }'{ Environment.NewLine }Online { newplatform }: { pDate }");
                                else WriteToFile($"Update in status: old '{ counters.clips_followers }', new '{ (int)decoded.counters.clips_followers }'{ Environment.NewLine }");
                            });
                            counters.activity = (string)decoded.activity;
                        }
                    }
                    else if (!continuous) WriteToFile("User has private profile, only online sessions logging");
                    if (counters.online != (bool)decoded.online)
                    {
                        if (!counters.online && (bool)decoded.online) 
                            if (!(bool)decoded.is_closed) WriteToFile($"Online {newplatform}, {onlineappid}: { pDate }");
                            else WriteToFile($"Online: { pDate }");
                        else WriteToFile($"   —  { pDate }{ Environment.NewLine }");
                        counters.online = (bool)decoded.online;
                    }
                    continuous = true;
                }
                catch (Exception) { }
                Thread.Sleep(500);
            }
        }
        private void WriteToFile(string text)
        {
            string[] str = File.ReadAllLines(Application.StartupPath + $"\\{user_id}.txt");
            EventLogs.Invoke((MethodInvoker)delegate { EventLogs.AppendText(text); });
            if (Logs.Checked) File.AppendAllText(Application.StartupPath + $"\\{user_id}.txt", text);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (polling != null) Stop();
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
        }
        private void CustomUserIdSelected(object sender, EventArgs e) { if (user_id != Convert.ToInt32(customuserid.Text)) { EventLogs.Text = ""; user_id = Convert.ToInt32(customuserid.Text); } }
        private void FriendsIdSelected(object sender, EventArgs e) { if (user_id != Convert.ToInt32(FriendsList[friends.SelectedIndex][0])) { EventLogs.Text = ""; user_id = Convert.ToInt32(FriendsList[friends.SelectedIndex][0]); } }
        private void StartStop_CheckedChanged(object sender, EventArgs e) { if (StartStop.Checked) Start(); else Stop(); }
        private void Start()
        {
            if (Logs.Checked)
                if (File.Exists(Application.StartupPath + $"\\{user_id}.txt")) EventLogs.Invoke((MethodInvoker)delegate { EventLogs.Text = File.ReadAllText(Application.StartupPath + $"\\{user_id}.txt"); });
                else File.WriteAllText(Application.StartupPath + $"\\{user_id}.txt", $"Log file for user id{user_id}");
            polling = new Thread(Poll) { IsBackground = true, Priority = ThreadPriority.Highest };
            polling.Start();
            friends.Enabled = customuserid.Enabled = Logs.Enabled = false;
            WriteToFile($"{ Environment.NewLine }——————————————————————————————————————————{ Environment.NewLine }");
        }
        private void Stop()
        {
            polling.Abort();
            while (!(polling.ThreadState == System.Threading.ThreadState.Aborted)) ;
            friends.Enabled = customuserid.Enabled = Logs.Enabled = true;
        }
        private void StalkerTrayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) { Show(); WindowState = FormWindowState.Normal; }
            else if (WindowState == FormWindowState.Normal) WindowState = FormWindowState.Minimized;
        }
        public Stalker()
        {
            InitializeComponent();
            LogInVK();
        }
    }
    public class Counters
    {
        public int albums = 0;
        public int audios = 0;
        public int followers = 0;
        public int friends = 0;
        public int gifts = 0;
        public int pages = 0;
        public int photos = 0;
        public int subscriptions = 0;
        public int videos = 0;
        public int clips_followers = 0;
        public bool online = false;
        public string activity = string.Empty;
        public string platform = "Undefined";
        public string onlineappid = "Undefined";
    }
}

