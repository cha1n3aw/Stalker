using MetroFramework.Forms;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stalker
{
    public partial class Stalker : MetroForm
    {
        private const string ApiRequestLink = "https://api.vk.com/method/";
        private static string AuthToken = String.Empty;
        private static string server_response = String.Empty;
        private List<string> FriendsIdList = new List<string>();
        private List<string> CustomIdList = new List<string>();
        private int user_id = 0; //622562260;//135917804;//193640924; 152795057
        string DateTime = string.Empty;
        string TimeShort = string.Empty;
        Thread polling;
        
        private void Get(string uri) { server_response = new WebClient { Encoding = Encoding.UTF8 }.DownloadString(uri); }
        private void FetchFriends()
        {
            Get($"{ApiRequestLink}friends.get?order=name&count=5000&fields=domain&access_token={AuthToken}&v=5.126");
            dynamic friendslist = JObject.Parse(server_response);
            var friendsl = friendslist.response;
            for (int i = 0; i < Convert.ToInt32(friendsl.count); i++)
            {
                FriendsIdList.Add((string)friendsl.items[i].id);
                friends.Items.Add($"{friendsl.items[i].first_name} {friendsl.items[i].last_name}");
            }
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
            RestrictToggles(false);
            while (StartStop.Checked)
            {
                try
                {
                    string onlineappid = string.Empty;
                    string newplatform = string.Empty;
                    bool changed = false;
                    Get($"{ApiRequestLink}users.get?user_ids={user_id}&fields=online,counters,last_seen,activity&access_token={AuthToken}&v=5.126");
                    dynamic decodedresponse = JObject.Parse(server_response);
                    var decoded = decodedresponse.response[0];
                    //Check for private profile, if so - then restrict unavailaible toggles
                    if (counters.closed != (bool)decoded.is_closed)
                    {
                        if ((bool)decoded.is_closed)
                        {
                            RestrictToggles(true);
                            WriteToFile($"User has private profile, settings were partially restricted{ Environment.NewLine }");
                        }
                        else RestrictToggles(false);
                        counters.closed = (bool)decoded.is_closed;
                    }
                    //Get last seen time in string format
                    DateTime = new DateTime(1970, 1, 1, 5, 0, 0, 0).AddSeconds((double)decoded.last_seen.time).ToString("dd MMM yyyy, HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    TimeShort = new DateTime(1970, 1, 1, 5, 0, 0, 0).AddSeconds((double)decoded.last_seen.time).ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);

                    if (CheckOnlineDevice.Checked)
                    {
                        //Checks for last platform seen, possible values 1-8
                        if (decoded.last_seen.platform != null)
                        {
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
                                    if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                                    WriteToFile($"{ TimeShort }, User switched from { counters.platform }  to { newplatform }{ Environment.NewLine }");
                                    if (counters.online || (bool)decoded.online)
                                        changed = true;
                                });
                                counters.platform = newplatform;
                            }
                        }
                        //Checks for an online app
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
                                if (continuous && counters.onlineappid != "Unidentified") EventLogs.Invoke((MethodInvoker)delegate
                                {
                                    if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                                    WriteToFile($"{ TimeShort }, User switched to another app: from { counters.onlineappid }  to { onlineappid }{ Environment.NewLine }");
                                    if (counters.online || (bool)decoded.online)
                                        changed = true;
                                });
                                counters.onlineappid = onlineappid;
                            }
                        }
                    }
                    if (counters.online || (bool)decoded.online) //execute checks only if theres offline-online, online-online, online-offline transitions
                    {
                        if (!counters.closed) //if profile is closed then theres nothing to check! also it comes with "restrict toggles" so this 'if' statement isnt so necessary
                        {
                            if (CheckAlbums.Checked && decoded.counters.albums != null && counters.albums != (int)decoded.counters.albums)
                            {
                                if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                                {
                                    if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                                    WriteToFile($"{ TimeShort }, Update in albums: { counters.albums }  to { (int)decoded.counters.albums }{ Environment.NewLine }");
                                    if (counters.online || (bool)decoded.online)
                                        changed = true;
                                });
                                counters.albums = (int)decoded.counters.albums;
                            }
                            if (CheckAudios.Checked && decoded.counters.audios != null && counters.audios != (int)decoded.counters.audios)
                            {
                                if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                                {
                                    if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                                    WriteToFile($"{ TimeShort }, Update in audios: { counters.audios }  to { (int)decoded.counters.audios }{ Environment.NewLine }");
                                    if (counters.online || (bool)decoded.online)
                                        changed = true;
                                });
                                counters.audios = (int)decoded.counters.audios;
                            }
                            if (CheckGifts.Checked && decoded.counters.gifts != null && counters.gifts != (int)decoded.counters.gifts)
                            {
                                if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                                {
                                    if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                                    WriteToFile($"{ TimeShort }, Update in gifts: { counters.gifts }  to { (int)decoded.counters.gifts }{ Environment.NewLine }");
                                    if (counters.online || (bool)decoded.online)
                                        changed = true;
                                });
                                counters.gifts = (int)decoded.counters.gifts;
                            }
                            if (CheckPhotos.Checked && decoded.counters.photos != null && counters.photos != (int)decoded.counters.photos)
                            {
                                if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                                {
                                    if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                                    WriteToFile($"{ TimeShort }, Update in photos: { counters.photos }  to { (int)decoded.counters.photos }{ Environment.NewLine }");
                                    if (counters.online || (bool)decoded.online)
                                        changed = true;
                                });
                                counters.photos = (int)decoded.counters.photos;
                            }
                            if (CheckSubscriptions.Checked && decoded.counters.subscriptions != null && counters.subscriptions != (int)decoded.counters.subscriptions)
                            {
                                if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                                {
                                    if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                                    WriteToFile($"{ TimeShort }, Update in subscpritions: { counters.subscriptions }  to { (int)decoded.counters.subscriptions }{ Environment.NewLine }");
                                    if (counters.online || (bool)decoded.online)
                                        changed = true;
                                });
                                counters.subscriptions = (int)decoded.counters.subscriptions;
                            }
                            if (CheckVideos.Checked && decoded.counters.videos != null && counters.videos != (int)decoded.counters.videos)
                            {
                                if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                                {
                                    if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                                    WriteToFile($"{ TimeShort }, Update in videos: { counters.videos }  to { (int)decoded.counters.videos }{ Environment.NewLine }");
                                    if (counters.online || (bool)decoded.online)
                                        changed = true;
                                });
                                counters.videos = (int)decoded.counters.videos;
                            }
                            if (CheckClips.Checked && decoded.counters.clips_followers != null && counters.clips_followers != (int)decoded.counters.clips_followers)
                            {
                                if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                                {
                                    if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                                    WriteToFile($"{ TimeShort }, Update in clips followers: { counters.clips_followers }  to { (int)decoded.counters.clips_followers }{ Environment.NewLine }");
                                    if (counters.online || (bool)decoded.online)
                                        changed = true;
                                });
                                counters.clips_followers = (int)decoded.counters.clips_followers;
                            }
                        }
                    }
                    if (CheckPages.Checked && decoded.counters.pages != null && counters.pages != (int)decoded.counters.pages) //checks independently from private profile, but only in offline-online online-online online-offline modes
                    {
                        if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                        {
                            if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                            WriteToFile($"{ TimeShort }, Update in pages: { counters.pages }  to { (int)decoded.counters.pages }{ Environment.NewLine }");
                            if (counters.online || (bool)decoded.online)
                                changed = true;
                        });
                        counters.pages = (int)decoded.counters.pages;
                    }
                    if (CheckStatus.Checked && decoded.activity != null && counters.activity != (string)decoded.activity) //checks independently from private profile, but only in offline-online online-online online-offline modes
                    {
                        if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                        {
                            if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                            WriteToFile($"{ TimeShort }, Update in status: old '{ counters.activity }', new '{ (string)decoded.activity }'{ Environment.NewLine }");
                            if (counters.online || (bool)decoded.online)
                                changed = true;
                        });
                        counters.activity = (string)decoded.activity;
                    }
                    if (CheckFriends.Checked && decoded.counters.friends != null && counters.friends != (int)decoded.counters.friends) //checks in all modes including offline-offline, independetly from private profile
                    {
                        if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                        {
                            if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                            WriteToFile($"{ TimeShort }, Update in friends: { counters.friends }  to { (int)decoded.counters.friends }{ Environment.NewLine }");
                            if (counters.online || (bool)decoded.online) changed = true;
                        });
                        counters.friends = (int)decoded.counters.friends;
                    }
                    if (CheckFollowers.Checked && !counters.closed && decoded.counters.followers != null && counters.followers != (int)decoded.counters.followers) //checks in all modes including offline-offline, but depends on private profile
                    {
                        if (continuous) EventLogs.Invoke((MethodInvoker)delegate
                        {
                            if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') WriteToFile($"  { DateTime }{ Environment.NewLine }");
                            WriteToFile($"{ TimeShort }, Update in followers: { counters.followers }  to { (int)decoded.counters.followers }{ Environment.NewLine }");
                            if (counters.online || (bool)decoded.online) changed = true;
                        });
                        counters.followers = (int)decoded.counters.followers;
                    }
                    //Main ONLINE-OFFLINE checking part
                    if (changed || (counters.online != (bool)decoded.online)) //if changes were committed (but there are no 'online status' changes), or if 'online status' changed
                    {
                        if (changed || (!counters.online && (bool)decoded.online)) //if changes were committed or offline to online transition happened
                            if (CheckOnlineDevice.Checked) 
                                if (onlineappid != string.Empty && newplatform != string.Empty) WriteToFile($"Online {newplatform}, {onlineappid}: { DateTime }   —");
                                    else WriteToFile($"Online {newplatform}: { DateTime }   —");
                            else WriteToFile($"Online: { DateTime }   —");
                        else WriteToFile($"  { DateTime }{ Environment.NewLine }"); //if transition online to offline happened
                        counters.online = (bool)decoded.online;
                        changed = false;
                    }
                    if (!continuous && (bool)decoded.online) continuous = true; //if online status is set and 'while' is not on its first itteration

                }
                catch (Exception) { }
                Thread.Sleep(1000); //timeout on api requests
            }
        }
        private void WriteToFile(string text)
        {
            string[] str = File.ReadAllLines(Application.StartupPath + $"\\{user_id}.txt");
            EventLogs.Invoke((MethodInvoker)delegate 
            { 
                EventLogs.AppendText(text); 
            });
            if (Logs.Checked) File.AppendAllText(Application.StartupPath + $"\\{user_id}.txt", text);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            SettingsThread(SettingsList());
            if (polling != null) Stop();
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
        }
        private void RestrictToggles(bool state)
        {
            if (state)
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    CheckAlbums.Checked = CheckAlbums.Enabled =
                    CheckAudios.Checked = CheckAudios.Enabled =
                    CheckClips.Checked = CheckClips.Enabled =
                    CheckFollowers.Checked = CheckFollowers.Enabled =
                    CheckGifts.Checked = CheckGifts.Enabled =
                    CheckPhotos.Checked = CheckPhotos.Enabled =
                    CheckSubscriptions.Checked = CheckSubscriptions.Enabled =
                    CheckVideos.Checked = CheckVideos.Enabled = false;
                }));
            }
            else
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    CheckAlbums.Enabled = CheckAudios.Enabled = CheckClips.Enabled = CheckFollowers.Enabled =
                    CheckGifts.Enabled = CheckPhotos.Enabled = CheckSubscriptions.Enabled = CheckVideos.Enabled = true;
                }));
            }
        }
        private void FriendsIdSelected(object sender, EventArgs e) 
        {
            if (FriendsIdList.Count > 0 && friends.SelectedIndex < FriendsIdList.Count) user_id = Convert.ToInt32(FriendsIdList[friends.SelectedIndex]);
            else if (CustomIdList.Count > 0 && friends.SelectedIndex >= FriendsIdList.Count) user_id = Convert.ToInt32(CustomIdList[friends.SelectedIndex - FriendsIdList.Count]);
            if (Logs.Checked)
                if (File.Exists(Application.StartupPath + $"\\{user_id}.txt")) EventLogs.Invoke((MethodInvoker)delegate { EventLogs.Text = File.ReadAllText(Application.StartupPath + $"\\{user_id}.txt"); });
                else
                {
                    File.AppendAllText(Application.StartupPath + $"\\{user_id}.txt", $"Log file for user id{user_id}");
                    EventLogs.Text = File.ReadAllText(Application.StartupPath + $"\\{user_id}.txt");
                }
        }
        private async void AddCustomIdClicked(object sender, EventArgs e)
        {
            if (int.TryParse(customuserid.Text, out int id))
            {
                try
                {
                    Get($"{ApiRequestLink}users.get?user_ids={id}&access_token={AuthToken}&v=5.126");
                    dynamic decodedresponse = JObject.Parse(server_response);
                    var decoded = decodedresponse.response[0];
                    if (!CustomIdList.Contains(id.ToString()) && !FriendsIdList.Contains(id.ToString()))
                    {
                        CustomIdList.Add(id.ToString());
                        friends.Items.Add($"{decoded.first_name} {decoded.last_name}");
                        customuserid.Text = "";
                    }
                    AddCustomId.Enabled = false;
                    await Task.Delay(1000);
                    AddCustomId.Enabled = true;
                }
                catch (Exception) { }
            }
        }
        private void StartStop_CheckedChanged(object sender, EventArgs e)
        {
            if (StartStop.Checked) 
            { 
                StartStopLabel.Text = "Stop"; 
                Start(); 
            } 
            else 
            { 
                Stop(); 
                StartStopLabel.Text = "Start"; 
            } 
        }
        private void Start()
        {
            polling = new Thread(Poll) { IsBackground = true, Priority = ThreadPriority.Highest };
            polling.Start();
            friends.Enabled = customuserid.Enabled = Logs.Enabled = false;
            if (Logs.Checked)
            if (!(EventLogs.Text[EventLogs.Text.Length - 3] == '—') && EventLogs.Text[EventLogs.Text.Length - 1] == '\n') 
                WriteToFile($"————————————————————————————————————{ Environment.NewLine }");
            else if (!(EventLogs.Text[EventLogs.Text.Length - 3] == '—')) 
                WriteToFile($"{ Environment.NewLine }————————————————————————————————————{ Environment.NewLine }");
        }
        private void Stop()
        {
            if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') 
                WriteToFile($"  { DateTime }{ Environment.NewLine }");
            polling.Abort();
            while (!(polling.ThreadState == ThreadState.Aborted)) ;
            friends.Enabled = customuserid.Enabled = Logs.Enabled = true;
        }
        private void StalkerTrayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) { Show(); WindowState = FormWindowState.Normal; }
            else if (WindowState == FormWindowState.Normal) WindowState = FormWindowState.Minimized;
        }
        private void PreventSleep_CheckedChanged(object sender, EventArgs e)
        {
            if (PreventSleep.Checked) DLLIMPORTS.SetThreadExecutionState(DLLIMPORTS.EXECUTION_STATE.ES_CONTINUOUS | DLLIMPORTS.EXECUTION_STATE.ES_SYSTEM_REQUIRED);
            else DLLIMPORTS.SetThreadExecutionState(DLLIMPORTS.EXECUTION_STATE.ES_CONTINUOUS);
        }
        private void OpenSettings_CheckedChanged(object sender, EventArgs e)
        {
            if(OpenSettings.Checked) Size = new System.Drawing.Size(643, 374);
            else Size = new System.Drawing.Size(509, 374);
        }
        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend) { StartStop.Checked = false; }
            else if (e.Mode == PowerModes.Resume) { StartStop.Checked = true; }
        }
        private Thread SettingsThread(List<KeyValuePair<string, string>> settingslist)
        {
            Thread settingsthread = new Thread(() => SetSetting(settingslist)) { IsBackground = false };
            settingsthread.Start();
            return settingsthread;
        }
        private static void SetSetting(List<KeyValuePair<string, string>> settingslist)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            foreach (KeyValuePair<string, string> pair in settingslist) configuration.AppSettings.Settings[pair.Key].Value = pair.Value;
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }
        private List<KeyValuePair<string, string>> SettingsList()
        {
            string temp_ids = "";
            if (CustomIdList.Count > 0)
            {
                foreach (string id in CustomIdList) temp_ids = temp_ids + id + ",";
                temp_ids = temp_ids.TrimEnd(',');
            }
            var settingslist = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("CustomIDs", temp_ids),
                new KeyValuePair<string, string>("Logs", Logs.Checked.ToString()),
                new KeyValuePair<string, string>("NoSleep", PreventSleep.Checked.ToString()),
                new KeyValuePair<string, string>("OnlineDevice", CheckOnlineDevice.Checked.ToString()),
                new KeyValuePair<string, string>("Albums", CheckAlbums.Checked.ToString()),
                new KeyValuePair<string, string>("Audios", CheckAudios.Checked.ToString()),
                new KeyValuePair<string, string>("Followers", CheckFollowers.Checked.ToString()),
                new KeyValuePair<string, string>("Friends", CheckFriends.Checked.ToString()),
                new KeyValuePair<string, string>("Gifts", CheckGifts.Checked.ToString()),
                new KeyValuePair<string, string>("Pages", CheckPages.Checked.ToString()),
                new KeyValuePair<string, string>("Photos", CheckPhotos.Checked.ToString()),
                new KeyValuePair<string, string>("Status", CheckStatus.Checked.ToString()),
                new KeyValuePair<string, string>("Subscriptions", CheckSubscriptions.Checked.ToString()),
                new KeyValuePair<string, string>("Videos", CheckVideos.Checked.ToString()),
                new KeyValuePair<string, string>("ClipsFollowers", CheckClips.Checked.ToString()),
                new KeyValuePair<string, string>("OpenSettings", OpenSettings.Checked.ToString()),
            };
            return settingslist;
        }
        private void LoadSettings()
        {
            SystemEvents.PowerModeChanged += OnPowerChange;
            Get($"{ApiRequestLink}users.get?user_ids={ConfigurationManager.AppSettings["CustomIDs"]}&access_token={AuthToken}&v=5.126");
            dynamic decodedresponse = JObject.Parse(server_response);
            foreach (var decoded in decodedresponse.response)
            {
                FriendsIdList.Add((string)decoded.id);
                friends.Items.Add($"{decoded.first_name} {decoded.last_name}");
            }
            Logs.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Logs"]);
            PreventSleep.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["NoSleep"]);
            CheckOnlineDevice.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["OnlineDevice"]);
            CheckAlbums.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Albums"]);
            CheckAudios.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Audios"]);
            CheckFollowers.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Followers"]);
            CheckFriends.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Friends"]);
            CheckGifts.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Gifts"]);
            CheckPages.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Pages"]);
            CheckPhotos.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Photos"]);
            CheckStatus.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Status"]);
            CheckSubscriptions.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Subscriptions"]);
            CheckVideos.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Videos"]);
            CheckClips.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["ClipsFollowers"]);
            OpenSettings.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["OpenSettings"]);
        }
        public Stalker()
        {
            InitializeComponent();
            Size = new System.Drawing.Size(509, 374);
            LogInVK();
            LoadSettings();
        }
    }
    public class Counters
    {
        public bool closed = false;
        public bool online = false;
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
        public string activity = string.Empty;
        public string platform = "Unidentified";
        public string onlineappid = "Unidentified";
    }
}

