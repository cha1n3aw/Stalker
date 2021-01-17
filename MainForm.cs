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
using System.Windows.Forms;

namespace Stalker
{
    public partial class Stalker : MetroForm
    {
        private const string ApiRequestLink = "https://api.vk.com/method/";
        private const string ApiVersion = "5.126";
        private static string AuthToken = String.Empty;
        private static string server_response = String.Empty;
        private List<string> FriendsIdList = new List<string>();
        private List<string> CustomIdList = new List<string>();
        private int user_id = 0;
        private readonly int WindowHeight = 483;
        string DateTime = string.Empty;
        string TimeShort = string.Empty;
        Thread Stalking;
        System.Timers.Timer timer;

        private void Get(string uri) { server_response = new WebClient { Encoding = Encoding.UTF8 }.DownloadString(uri); }
        private void FetchFriends()
        {
            Get($"{ApiRequestLink}friends.get?order=name&count=5000&fields=domain&access_token={AuthToken}&v={ApiVersion}");
            dynamic friendslist = JObject.Parse(server_response);
            var friendsl = friendslist.response;
            if (Convert.ToInt32(friendsl.count) > 0)
            for (int i = 0; i < Convert.ToInt32(friendsl.count); i++)
            {
                FriendsIdList.Add((string)friendsl.items[i].id);
                ListOfIDs.Items.Add($"{friendsl.items[i].first_name} {friendsl.items[i].last_name}");
            }
            Get($"{ApiRequestLink}users.get?user_ids={ConfigurationManager.AppSettings["CustomIDs"]}&access_token={AuthToken}&v={ApiVersion}");
            dynamic customusers = JObject.Parse(server_response);
            foreach (var user in customusers.response)
            {
                CustomIdList.Add((string)user.id);
                ListOfIDs.Items.Add($"{user.first_name} {user.last_name}");
            }
        }
        private void LogInVK() 
        {
            MetroAuthForm AuthForm = new MetroAuthForm();
            if (ClearCookies.Checked) AuthForm.ClearCookies = true;
            if (AuthForm.ShowDialog() == DialogResult.Yes) AuthToken = AuthForm.Token; 
            else if (Application.MessageLoop) Application.Exit();
            else Environment.Exit(1);
            AuthForm.Dispose();
            GC.Collect();
        }
        private void Poll()
        {
            bool timeout = true;
            timer = new System.Timers.Timer
            {
                Interval = 17300,
                AutoReset = true
            };
            timer.Elapsed += (sender, data) =>
            {
                timeout = true;
                timer.Stop();
            }; 
            timer.Enabled = true;
            Counters counters = new Counters();
            bool continuous = false;
            RestrictToggles(false);
            while (StartStop.Checked)
            {
                string temp_text = string.Empty;
                bool changed = false;
                string onlineappid = string.Empty;
                string newplatform = string.Empty;
                try
                {
                    Get($"{ApiRequestLink}users.get?user_ids={user_id}&fields=online,counters,last_seen,activity&access_token={AuthToken}&v={ApiVersion}");
                    dynamic decodedresponse = JObject.Parse(server_response);
                    var decoded = decodedresponse.response[0];
                    if (counters.closed != (bool)decoded.is_closed) //Check for private profile, if so - then restrict unavailaible toggles
                    {
                        if ((bool)decoded.is_closed)
                        {
                            RestrictToggles(true);
                            WriteToFile($"User has private profile, settings were partially restricted{ Environment.NewLine }");
                        }
                        else RestrictToggles(false);
                        counters.closed = (bool)decoded.is_closed;
                    }
                    DateTime = new DateTime(1970, 1, 1, 5, 0, 0, 0).AddSeconds((double)decoded.last_seen.time).ToString("dd MMM yyyy, HH:mm:ss", DateTimeFormatInfo.InvariantInfo); //Get last seen time in string format
                    TimeShort = new DateTime(1970, 1, 1, 5, 0, 0, 0).AddSeconds((double)decoded.last_seen.time).ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    if (TimeShort != counters.LastOnline)
                        LastOnlineLabel.Invoke((MethodInvoker)delegate
                        {
                            LastOnlineLabel.Text = $"{TimeShort}"; 
                            counters.LastOnline = TimeShort;
                        });
                    if (CheckOnlineDevice.Checked)
                    {
                        if (decoded.last_seen.platform != null) //Checks for last platform seen, possible values 1-8
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
                                temp_text += $"{ TimeShort }, User switched from { counters.platform }  to { newplatform }{ Environment.NewLine }";
                                counters.platform = newplatform;
                                changed = true;
                            }
                        }
                        if (decoded.online_app != null) //Checks for an online app
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
                            if (counters.onlineappid != onlineappid && counters.onlineappid != "Unidentified")
                            {
                                temp_text += $"{ TimeShort }, User switched to another app: from { counters.onlineappid }  to { onlineappid }{ Environment.NewLine }";
                                counters.onlineappid = onlineappid;
                                changed = true;
                            }
                        }
                    }
                    if (!counters.closed) //if profile is closed then theres nothing to check! also it comes with "restrict toggles" so this 'if' statement isnt so necessary
                    {
                        if (CheckAlbums.Checked && decoded.counters.albums != null && counters.albums != (int)decoded.counters.albums)
                        {
                            temp_text += $"{ TimeShort }, Update in albums: { counters.albums }  to { (int)decoded.counters.albums }{ Environment.NewLine }";
                            counters.albums = (int)decoded.counters.albums;
                            changed = true;
                        }
                        if (CheckAudios.Checked && decoded.counters.audios != null && counters.audios != (int)decoded.counters.audios)
                        {
                            temp_text += $"{ TimeShort }, Update in audios: { counters.audios }  to { (int)decoded.counters.audios }{ Environment.NewLine }";
                            counters.audios = (int)decoded.counters.audios;
                            changed = true;
                        }
                        if (CheckGifts.Checked && decoded.counters.gifts != null && counters.gifts != (int)decoded.counters.gifts)
                        {
                            temp_text += $"{ TimeShort }, Update in gifts: { counters.gifts }  to { (int)decoded.counters.gifts }{ Environment.NewLine }";
                            if ((int)decoded.counters.gifts > counters.gifts && continuous)
                            {
                                Thread.Sleep(350);
                                try 
                                {
                                    Get($"{ApiRequestLink}gifts.get?user_id={user_id}&count={(int)decoded.counters.gifts - counters.gifts}&access_token={AuthToken}&v={ApiVersion}"); //get gifts
                                    dynamic gifts = JObject.Parse(server_response);
                                    var giftresponse = gifts.response;
                                    foreach (var item in giftresponse.items) temp_text += $"   Gift from id: { (string)item.from_id }{ Environment.NewLine }   With message: {(string)item.message}{ Environment.NewLine }   Thumbnail: {(string)item.gift.thumb_256}{ Environment.NewLine }";
                                } catch (Exception) { WriteToFile($"Exception thrown at gifts check{Environment.NewLine}"); }
                            }
                            counters.gifts = (int)decoded.counters.gifts;
                            changed = true;
                        }
                        if (CheckPhotos.Checked && decoded.counters.photos != null && counters.photos != (int)decoded.counters.photos)
                        {
                            temp_text += $"{ TimeShort }, Update in photos: { counters.photos }  to { (int)decoded.counters.photos }{ Environment.NewLine }";
                            counters.photos = (int)decoded.counters.photos;
                            changed = true;
                        }
                        if (CheckSubscriptions.Checked && decoded.counters.subscriptions != null && counters.subscriptions != (int)decoded.counters.subscriptions)
                        {
                            temp_text += $"{ TimeShort }, Update in subscpritions: { counters.subscriptions }  to { (int)decoded.counters.subscriptions }{ Environment.NewLine }";
                            counters.subscriptions = (int)decoded.counters.subscriptions;
                            changed = true;
                        }
                        if (CheckVideos.Checked && decoded.counters.videos != null && (int)decoded.counters.videos > 0 && counters.videos != (int)decoded.counters.videos)
                        {
                            temp_text += $"{ TimeShort }, Update in videos: { counters.videos }  to { (int)decoded.counters.videos }{ Environment.NewLine }";
                            if ((int)decoded.counters.videos > counters.videos && continuous)
                            {
                                Thread.Sleep(350); //timeout on api requests
                                try 
                                {
                                    Get($"{ApiRequestLink}video.get?owner_id={user_id}&access_token={AuthToken}&v={ApiVersion}");
                                    dynamic videos = JObject.Parse(server_response);
                                    var videosresponse = videos.response;
                                    for (int i = 0; i < (int)videosresponse.count - counters.videos; i++)
                                        temp_text += $"   Video title: {(string)videosresponse.items[i].title}{ Environment.NewLine }"
                                            + $"   Video link: {(string)videosresponse.items[i].player}{ Environment.NewLine }"
                                            + $"   Video ID & Owner ID: {(string)videosresponse.items[i].id} & {(string)videosresponse.items[i].owner_id}{ Environment.NewLine }";
                                } catch (Exception) { WriteToFile($"Exception thrown at video check{Environment.NewLine}"); }
                            }
                            counters.videos = (int)decoded.counters.videos;
                            changed = true;
                        }
                        if (CheckClips.Checked && decoded.counters.clips_followers != null && counters.clips_followers != (int)decoded.counters.clips_followers)
                        {
                            temp_text += $"{ TimeShort }, Update in clips followers: { counters.clips_followers }  to { (int)decoded.counters.clips_followers }{ Environment.NewLine }";
                            counters.clips_followers = (int)decoded.counters.clips_followers;
                            changed = true;
                        }
                    }
                    if (CheckPages.Checked && decoded.counters.pages != null && counters.pages != (int)decoded.counters.pages) //checks independently from private profile, but only in offline-online online-online online-offline modes
                    {
                        temp_text += $"{ TimeShort }, Update in pages: { counters.pages }  to { (int)decoded.counters.pages }{ Environment.NewLine }";
                        counters.pages = (int)decoded.counters.pages;
                        changed = true;
                    }
                    if (CheckStatus.Checked && decoded.activity != null && counters.activity != (string)decoded.activity) //checks independently from private profile, but only in offline-online online-online online-offline modes
                    {
                        temp_text += $"{ TimeShort }, Update in status: old '{ counters.activity }', new '{ (string)decoded.activity }'{ Environment.NewLine }";
                        counters.activity = (string)decoded.activity;
                        changed = true;
                    }
                    if (CheckFriends.Checked && decoded.counters.friends != null && counters.friends != (int)decoded.counters.friends) //checks in all modes including offline-offline, independetly from private profile
                    {
                        temp_text += $"{ TimeShort }, Update in friends: { counters.friends }  to { (int)decoded.counters.friends }{ Environment.NewLine }";
                        if (!counters.closed && continuous)
                        {
                            Thread.Sleep(350); //timeout on api requests
                            try
                            {
                                Get($"{ApiRequestLink}friends.get?user_id={user_id}&access_token={AuthToken}&v={ApiVersion}");
                                dynamic userfriends = JObject.Parse(server_response);
                                var friendsresponse = userfriends.response;
                                if (friendsresponse.count > counters.friends)
                                    foreach (var addedfriend in friendsresponse.items)
                                        if (!counters.friends_list.Contains(addedfriend.ToString()))
                                        {
                                            temp_text = temp_text.Trim('\n');
                                            temp_text += $", new friend id: {addedfriend.ToString()}{ Environment.NewLine }";
                                            counters.friends_list.Add(addedfriend.ToString());
                                        }
                                        else { }
                                else
                                {
                                    List<string> TempListNew = friendsresponse.items.ToObject<List<string>>();
                                    foreach (string removedfriend in counters.friends_list)
                                        if (!TempListNew.Contains(removedfriend))
                                        {
                                            temp_text = temp_text.Trim('\n');
                                            temp_text += $", removed friend id: {removedfriend}{ Environment.NewLine }";
                                        }
                                    counters.friends_list = TempListNew;
                                }   
                            }
                            catch (Exception) { WriteToFile($"Exception thrown at friends check{Environment.NewLine}"); }
                        }
                        counters.friends = (int)decoded.counters.friends;
                        changed = true;
                    }
                    if (CheckFollowers.Checked && !counters.closed && decoded.counters.followers != null && counters.followers != (int)decoded.counters.followers) //checks in all modes including offline-offline, but depends on private profile
                    {
                        temp_text += $"{ TimeShort }, Update in followers: { counters.followers }  to { (int)decoded.counters.followers }{ Environment.NewLine }";
                        if (!counters.closed && continuous)
                        {
                            Thread.Sleep(350); //timeout on api requests
                            try
                            {
                                Get($"{ApiRequestLink}users.getFollowers?user_id={user_id}&access_token={AuthToken}&v={ApiVersion}");
                                dynamic userfollowers = JObject.Parse(server_response);
                                var followersresponse = userfollowers.response;
                                if (followersresponse.count > counters.followers)
                                    foreach (var addedfollower in followersresponse.items)
                                        if (!counters.followers_list.Contains(addedfollower.ToString()))
                                        {
                                            temp_text = temp_text.Trim('\n');
                                            temp_text += $", new follower id: {addedfollower.ToString()}{ Environment.NewLine }";
                                            counters.followers_list.Add(addedfollower.ToString());
                                        }
                                        else { }
                                else
                                {
                                    List<string> TempListNew = followersresponse.items.ToObject<List<string>>();
                                    foreach (string removedfollower in counters.followers_list)
                                        if (!TempListNew.Contains(removedfollower))
                                        {
                                            temp_text = temp_text.Trim('\n');
                                            temp_text += $", removed follower id: {removedfollower}{ Environment.NewLine }";
                                        }
                                    counters.followers_list = TempListNew;
                                }
                            }
                            catch (Exception) { WriteToFile($"Exception thrown at followers check{Environment.NewLine}"); }
                        }
                        counters.followers = (int)decoded.counters.followers;
                        changed = true;
                    }
                    if (CheckPosts.Checked && !counters.closed && timeout) //depends on private profile
                    {
                        try
                        {
                            Thread.Sleep(350); //timeout on api requests
                            Get($"{ApiRequestLink}wall.get?owner_id={user_id}&access_token={AuthToken}&v={ApiVersion}"); //get wall posts
                            dynamic posts = JObject.Parse(server_response);
                            var response = posts.response;
                            if (response.count != null && counters.posts != (int)response.count)
                            {
                                temp_text += $"{ TimeShort }, Update in posts: { counters.posts }  to { (int)response.count }{ Environment.NewLine }";
                                if ((int)response.count > counters.posts && continuous)
                                {
                                    for (int i = 0; i < (int)response.count - counters.posts; i++)
                                    {
                                        if (response.items[i].text != null && (string)response.items[i].text != "")
                                            temp_text += $"   Post text: {(string)response.items[i].text}{ Environment.NewLine }";
                                        if (response.items[i].attachments != null)
                                            foreach (var attachment in response.items[i].attachments)
                                                switch ((string)attachment.type)
                                                {
                                                    case "photo":
                                                        { temp_text += $"   Attached image: {(string)attachment.photo.sizes[attachment.photo.sizes.Count - 1].url}{ Environment.NewLine }"; }
                                                        break;
                                                    case "video":
                                                        { temp_text += $"   Attached video title: {(string)attachment.video.title}{ Environment.NewLine }   Video ID & Owner ID: {(string)attachment.video.id} & {(string)attachment.video.id}{ Environment.NewLine }"; }
                                                        break;
                                                    case "audio":
                                                        { temp_text += $"   Attached audio: {(string)attachment.audio.artist}   —  {(string)attachment.audio.title}{ Environment.NewLine }"; }
                                                        break;
                                                    case "link":
                                                        { temp_text += $"   Attached link: {(string)attachment.link.url}{ Environment.NewLine }"; }
                                                        break;
                                                    case "doc":
                                                        { temp_text += $"   Attached document: {(string)attachment.doc.title}{ Environment.NewLine }    Document link: {(string)attachment.doc.url}{ Environment.NewLine }"; }
                                                        break;
                                                }
                                    }
                                }
                                counters.posts = (int)response.count;
                                changed = true;
                            }
                            timeout = false;
                            timer.Start();
                        }
                        catch (Exception) { WriteToFile($"Exception thrown at posts, probably too many requests were made{Environment.NewLine}"); CheckPosts.Checked = false; }
                    }
                    if (changed && continuous) EventLogs.Invoke((MethodInvoker)delegate
                    {
                        if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') temp_text = $"  { DateTime }{ Environment.NewLine }" + temp_text;
                        WriteToFile(temp_text);
                        if (counters.online == (bool)decoded.online && counters.online) counters.online = !counters.online;
                    });
                    changed = false;
                    if (!continuous) continuous = true; //if 'while' is not on its first itteration
                                                        //Main ONLINE-OFFLINE checking part
                    if (counters.online != (bool)decoded.online) //if 'online status' changed
                    {
                        if (!counters.online && (bool)decoded.online) //if changes were committed or offline to online transition happened
                            if (CheckOnlineDevice.Checked)
                                if (onlineappid != string.Empty && newplatform != string.Empty) WriteToFile($"Online {newplatform}, {onlineappid}: { DateTime }   —");
                                else WriteToFile($"Online {newplatform}: { DateTime }   —");
                            else WriteToFile($"Online: { DateTime }   —");
                        else WriteToFile($"  { DateTime }{ Environment.NewLine }"); //if transition online to offline happened
                        counters.online = (bool)decoded.online;
                    }
                }
                catch (Exception) { WriteToFile($"Global error, possibly get counters failed{Environment.NewLine}"); }
                Thread.Sleep(350); //timeout on api requests
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
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
            if (Stalking != null) Stop();
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
                    CheckPosts.Checked = CheckPosts.Enabled =
                    CheckSubscriptions.Checked = CheckSubscriptions.Enabled =
                    CheckVideos.Checked = CheckVideos.Enabled = false;
                }));
            }
            else
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    CheckAlbums.Enabled = CheckAudios.Enabled = 
                    CheckClips.Enabled = CheckFollowers.Enabled = 
                    CheckGifts.Enabled = CheckPhotos.Enabled = 
                    CheckPosts.Enabled = CheckSubscriptions.Enabled = 
                    CheckVideos.Enabled = true;
                }));
            }
        }
        private void FriendsIdSelected(object sender, EventArgs e) 
        {
            if (FriendsIdList.Count > 0 && ListOfIDs.SelectedIndex < FriendsIdList.Count) user_id = Convert.ToInt32(FriendsIdList[ListOfIDs.SelectedIndex]);
            else if (CustomIdList.Count > 0 && ListOfIDs.SelectedIndex >= FriendsIdList.Count) user_id = Convert.ToInt32(CustomIdList[ListOfIDs.SelectedIndex - FriendsIdList.Count]);
            if (Logs.Checked)
                if (File.Exists(Application.StartupPath + $"\\{user_id}.txt")) EventLogs.Invoke((MethodInvoker)delegate { EventLogs.Text = File.ReadAllText(Application.StartupPath + $"\\{user_id}.txt"); });
                else
                {
                    File.AppendAllText(Application.StartupPath + $"\\{user_id}.txt", $"Log file for user id{user_id}");
                    EventLogs.Text = File.ReadAllText(Application.StartupPath + $"\\{user_id}.txt");
                }
        }
        private void CustomUserIdEntered(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && int.TryParse(CustomIdInput.Text, out int id))
            {
                try
                {
                    Get($"{ApiRequestLink}users.get?user_ids={id}&access_token={AuthToken}&v={ApiVersion}");
                    dynamic decodedresponse = JObject.Parse(server_response);
                    var decoded = decodedresponse.response[0];
                    if (!CustomIdList.Contains(id.ToString()) && !FriendsIdList.Contains(id.ToString()))
                    {
                        CustomIdList.Add(id.ToString());
                        ListOfIDs.Items.Add($"{decoded.first_name} {decoded.last_name}");
                        CustomIdInput.Text = "";
                    }
                }
                catch (Exception) { }
            }
        }
        private void StartStop_CheckedChanged(object sender, EventArgs e)
        {
            if (StartStop.Checked) Start();  
            else Stop(); 
        }
        private void Start()
        {
            StartStopLabel.Text = "Stop";
            Stalking = new Thread(Poll) { IsBackground = true, Priority = ThreadPriority.Highest };
            Stalking.Start();
            ListOfIDs.Enabled = CustomIdInput.Enabled = Logs.Enabled = false;
            if (Logs.Checked)
            if (!(EventLogs.Text[EventLogs.Text.Length - 3] == '—') && EventLogs.Text[EventLogs.Text.Length - 1] == '\n') 
                WriteToFile($"————————————————————————————————————{ Environment.NewLine }");
            else if (!(EventLogs.Text[EventLogs.Text.Length - 3] == '—')) 
                WriteToFile($"{ Environment.NewLine }————————————————————————————————————{ Environment.NewLine }");
        }
        private void Stop()
        {
            StartStopLabel.Text = "Start";
            if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') 
                WriteToFile($"  { DateTime }{ Environment.NewLine }");
            ListOfIDs.Enabled = CustomIdInput.Enabled = Logs.Enabled = true;
            RestrictToggles(false);
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
            if(OpenSettings.Checked) Size = new System.Drawing.Size(643, WindowHeight);
            else Size = new System.Drawing.Size(509, WindowHeight);
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
            string savetoken = "";
            if (SaveToken.Checked) savetoken = AuthToken;
            var settingslist = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("AuthToken", savetoken),
                new KeyValuePair<string, string>("SaveToken", SaveToken.Checked.ToString()),
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
                new KeyValuePair<string, string>("Posts", CheckPosts.Checked.ToString()),
                new KeyValuePair<string, string>("Status", CheckStatus.Checked.ToString()),
                new KeyValuePair<string, string>("Subscriptions", CheckSubscriptions.Checked.ToString()),
                new KeyValuePair<string, string>("Videos", CheckVideos.Checked.ToString()),
                new KeyValuePair<string, string>("ClipsFollowers", CheckClips.Checked.ToString()),
                new KeyValuePair<string, string>("OpenSettings", OpenSettings.Checked.ToString()),
                new KeyValuePair<string, string>("ClearCookies", ClearCookies.Checked.ToString()),
                new KeyValuePair<string, string>("SelectedUser", ListOfIDs.SelectedIndex.ToString()),
                new KeyValuePair<string, string>("Autostart", Autostart.Checked.ToString()),
            };
            return settingslist;
        }
        private void LoadSettings()
        {
            Size = new System.Drawing.Size(509, WindowHeight);
            SystemEvents.PowerModeChanged += OnPowerChange;
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
            CheckPosts.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Posts"]);
            CheckStatus.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Status"]);
            CheckSubscriptions.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Subscriptions"]);
            CheckVideos.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Videos"]);
            CheckClips.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["ClipsFollowers"]);
            OpenSettings.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["OpenSettings"]);
            SaveToken.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["SaveToken"]);
            if (SaveToken.Checked) AuthToken = ConfigurationManager.AppSettings["AuthToken"];
            else AuthToken = "";
            ClearCookies.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["ClearCookies"]);
            Autostart.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Autostart"]);
            if (!SaveToken.Checked || AuthToken == "") LogInVK();
            FetchFriends();
            int index = Convert.ToInt32(ConfigurationManager.AppSettings["SelectedUser"]);
            if (ListOfIDs.Items.Count > index) ListOfIDs.SelectedIndex = index;
            if (Autostart.Checked) StartStop.Checked = true;
        }
        public Stalker()
        {
            InitializeComponent();
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
        public int posts = 0;
        public int subscriptions = 0;
        public int videos = 0;
        public int clips_followers = 0;
        public string activity = string.Empty;
        public string platform = "Unidentified";
        public string onlineappid = "Unidentified";
        public List<string> friends_list = new List<string>();
        public List<string> followers_list = new List<string>();
        public string LastOnline = string.Empty;
    }
}

