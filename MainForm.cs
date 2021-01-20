using MetroFramework.Forms;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
        private readonly int WindowHeight = 549;
        string OnlineDateTime = string.Empty;
        string OnlineTime = string.Empty;
        string CurrentTime = string.Empty;
        Thread Stalking;
        System.Timers.Timer timer;
        private string _link;
        private string link
        { 
            set 
            { 
                if (!Directory.Exists(Application.StartupPath + "\\Files"))
                    Directory.CreateDirectory(Application.StartupPath + "\\Files");
                string directory = $"{Application.StartupPath}\\Files\\id{user_id}-{Regex.Match(value, @"(?:[^/][\d\w\.]+)(?<=(?:.jpg)|(?:.mp4)|(?:.jpeg)|(?:.png)|(?:.pdf)|(?:.gif)|(?:.doc))").Value}";
                if (!File.Exists(directory))
                using (WebClient wc = new WebClient())
                        wc.DownloadFileAsync(new Uri(value), directory);
                _link = $"   Downloaded file: {directory}";
            } 
        }

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
            if (CustomAppIdToggle.Checked && int.TryParse(CustomAppId.Text, out int appid)) AuthForm.APPid = appid;
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
            RestrictToggles(false);
            while (StartStop.Checked)
            {
                string temp_text = string.Empty;
                bool changed = false;
                try
                {
                    Get($"{ApiRequestLink}users.get?user_ids={user_id}&fields=online,counters,last_seen,status&access_token={AuthToken}&v={ApiVersion}");
                    JObject decodedresponse = JObject.Parse(server_response);
                    var decoded = decodedresponse["response"][0];
                    if (counters.decoded == null)
                    {
                        counters.decoded = (JObject)decoded;
                        counters.decoded["online"] = false;
                        counters.decoded["counters"]["photos"] = -1;
                        counters.decoded["counters"]["gifts"] = -1;
                        counters.decoded["counters"]["videos"] = -1;
                        counters.decoded["counters"]["followers"] = -1;
                        counters.decoded["counters"]["friends"] = -1;
                        counters.decoded["counters"]["albums"] = -1;
                        counters.decoded["counters"]["subscriptions"] = -1;
                        counters.decoded["is_closed"] = false;
                        if (counters.decoded["status_audio"] != null) counters.decoded.Remove("status_audio");
                    }

                    if ((bool)counters.decoded["is_closed"] != (bool)decoded["is_closed"]) //Check for private profile, if so - then restrict unavailaible toggles
                    {
                        if ((bool)decoded["is_closed"])
                        {
                            RestrictToggles(true);
                            WriteToFile($"User has private profile, settings were partially restricted{ Environment.NewLine }");
                        }
                        else RestrictToggles(false);
                    }
                    DateTime LastOnlineTime = new DateTime(1970, 1, 1, 5, 0, 0, 0).AddSeconds((double)decoded["last_seen"]["time"]);
                    OnlineDateTime = LastOnlineTime.ToString("dd MMM yyyy, HH:mm:ss", DateTimeFormatInfo.InvariantInfo); //Get last seen time in string format
                    OnlineTime = LastOnlineTime.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    CurrentTime = DateTime.Now.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    
                    LastOnlineLabel.Invoke((MethodInvoker)delegate
                    {
                        if (counters.LastOnlineTime != LastOnlineTime) LastOnlineLabel.Text = $"{OnlineTime}";
                        counters.LastOnlineTime = LastOnlineTime;
                    });

                    if (CheckOnlineDevice.Checked)
                    {
                        if (decoded["last_seen"]["platform"] != null)
                        {
                            switch ((string)decoded["last_seen"]["platform"])
                            {
                                case "1":
                                    { decoded["last_seen"]["platform"] = "Mobile WEB"; }
                                    break;
                                case "2":
                                    { decoded["last_seen"]["platform"] = "iPhone"; }
                                    break;
                                case "3":
                                    { decoded["last_seen"]["platform"] = "iPad"; }
                                    break;
                                case "4":
                                    { decoded["last_seen"]["platform"] = "Android"; }
                                    break;
                                case "5":
                                    { decoded["last_seen"]["platform"] = "WinPhone"; }
                                    break;
                                case "6":
                                    { decoded["last_seen"]["platform"] = "Windows 8"; }
                                    break;
                                case "7":
                                    { decoded["last_seen"]["platform"] = "WEB"; }
                                    break;
                                case "8":
                                    { decoded["last_seen"]["platform"] = "VK Mobile"; }
                                    break;
                                default:
                                    { decoded["last_seen"]["platform"] = "Unknown"; }
                                    break;
                            }
                            if (counters.decoded["last_seen"]["platform"] != null && decoded["last_seen"]["platform"] != null && (string)counters.decoded["last_seen"]["platform"] != (string)decoded["last_seen"]["platform"])
                            {
                                temp_text += $"{ OnlineTime }, User switched from { counters.decoded["last_seen"]["platform"] }  to { decoded["last_seen"]["platform"] }{ Environment.NewLine }";
                                changed = true;
                            }
                        }
                        if (decoded["online_app"] != null)
                        {
                            switch ((string)decoded["online_app"])
                            {
                                case "2274003":
                                    { decoded["online_app"] = "Android App"; }
                                    break;
                                case "3140623":
                                    { decoded["online_app"] = "iOS App"; }
                                    break;
                                case "3682744":
                                    { decoded["online_app"] = "iPadOS App"; }
                                    break;
                                case "3697615":
                                    { decoded["online_app"] = "Windows Desktop App"; }
                                    break;
                                case "3502557":
                                    { decoded["online_app"] = "Windows Phone App"; }
                                    break;
                                case "2685278":
                                    { decoded["online_app"] = "Kate Mobile"; }
                                    break;
                                default:
                                    { decoded["online_app"] = (string)decoded["online_app"]; }
                                    break;
                            }
                            if (counters.decoded["online_app"] != null && decoded["online_app"] != null && (string)counters.decoded["online_app"] != (string)decoded["online_app"])
                            {
                                temp_text += $"{ OnlineTime }, User switched to another app: from { counters.decoded["online_app"] }  to { decoded["online_app"] }{ Environment.NewLine }";
                                changed = true;
                            }
                        }
                    }
                    if (!(bool)counters.decoded["is_closed"]) //if profile is closed then theres nothing to check! also it comes with "restrict toggles" so this 'if' statement isnt so necessary
                    {
                        if (CheckAlbums.Checked && decoded["counters"]["albums"] != null && counters.decoded["counters"]["albums"] != null && (int)counters.decoded["counters"]["albums"] != (int)decoded["counters"]["albums"])
                        {
                            try
                            {
                                Thread.Sleep(350);
                                Get($"{ApiRequestLink}photos.getAlbums?owner_id={user_id}&access_token={AuthToken}&v={ApiVersion}");
                                JObject albums = JObject.Parse(server_response);
                                var albumsresponse = albums["response"];
                                if (counters.albums_response == null) counters.albums_response = (JObject)albumsresponse;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in albums: { counters.decoded["counters"]["albums"] }  to { decoded["counters"]["albums"] }{ Environment.NewLine }";
                                    if ((int)counters.decoded["counters"]["albums"] < (int)decoded["counters"]["albums"])
                                    {
                                        List<string> OldAlbums = new List<string>();
                                        foreach (var album in counters.albums_response["items"]) OldAlbums.Add((string)album["id"]);
                                        foreach (var album in albumsresponse["items"])
                                            if (!OldAlbums.Contains((string)album["id"]))
                                                temp_text += $"   New album:{ Environment.NewLine }   Link: vk.com/album{user_id}_{album["id"]}{ Environment.NewLine }   Title: {album["title"]}{ Environment.NewLine }";
                                    }
                                    else
                                    {
                                        List<string> NewAlbums = new List<string>();
                                        foreach (var album in albumsresponse["items"]) NewAlbums.Add((string)album["id"]);
                                        foreach (var album in counters.albums_response["items"])
                                            if (!NewAlbums.Contains((string)album["id"]))
                                                temp_text += $"   Removed album:{ Environment.NewLine }   Link (unavailaible): vk.com/album{user_id}_{album["id"]}{ Environment.NewLine }   Title: {album["title"]}{ Environment.NewLine }";
                                    }
                                    changed = true;
                                    counters.albums_response = (JObject)albumsresponse;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at albums check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckAudios.Checked && decoded["counters"]["audios"] != null && counters.decoded["counters"]["audios"] != null && (int)counters.decoded["counters"]["audios"] != (int)decoded["counters"]["audios"])
                        {
                            temp_text += $"{ CurrentTime }, Update in audios: { counters.decoded["counters"]["audios"] }  to { decoded["counters"]["audios"] }{ Environment.NewLine }";
                            changed = true;
                        }

                        if (CheckSubscriptions.Checked && decoded["counters"]["subscriptions"] != null && counters.decoded["counters"]["subscriptions"] != null && (int)counters.decoded["counters"]["subscriptions"] != (int)decoded["counters"]["subscriptions"])
                        {
                            try
                            {
                                Thread.Sleep(350);
                                Get($"{ApiRequestLink}users.getSubscriptions?user_id={user_id}&extended=1&access_token={AuthToken}&v={ApiVersion}");
                                JObject subscriptions = JObject.Parse(server_response);
                                var subscriptionsresponse = subscriptions["response"];
                                if (counters.subscriptions_response == null) counters.subscriptions_response = (JObject)subscriptionsresponse;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in subscpritions: { counters.decoded["counters"]["subscriptions"] }  to { decoded["counters"]["subscriptions"] }{ Environment.NewLine }";
                                    if ((int)counters.decoded["counters"]["subscriptions"] < (int)decoded["counters"]["subscriptions"])
                                    {
                                        List<string> OldSubscriptions = new List<string>();
                                        foreach (var subscription in counters.subscriptions_response["items"]) OldSubscriptions.Add((string)subscription["id"]);
                                        foreach (var subscription in subscriptionsresponse["items"])
                                            if (!OldSubscriptions.Contains((string)subscription["id"]))
                                                temp_text += $"   New subscription:{ Environment.NewLine }   Link: vk.com/public{subscription["id"]}{ Environment.NewLine }   Name: {subscription["name"]}";
                                    }
                                    else
                                    {
                                        List<string> NewSubscriptions = new List<string>();
                                        foreach (var subscription in subscriptionsresponse["items"]) NewSubscriptions.Add((string)subscription["id"]);
                                        foreach (var subscription in counters.subscriptions_response["items"])
                                            if (!NewSubscriptions.Contains((string)subscription["id"]))
                                                temp_text += $"   Removed subscription:{ Environment.NewLine }   Link: vk.com/public{subscription["id"]}{ Environment.NewLine }   Name: {subscription["name"]}";
                                    }
                                    changed = true;
                                    counters.subscriptions_response = (JObject)subscriptionsresponse;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at subscriptions check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckClips.Checked && decoded["counters"]["clips_followers"] != null && counters.decoded["counters"]["clips_followers"] != null && (int)counters.decoded["counters"]["clips_followers"] != (int)decoded["counters"]["clips_followers"])
                        {
                            temp_text += $"{ CurrentTime }, Update in clips followers: { counters.decoded["counters"]["clips_followers"] }  to { decoded["counters"]["clips_followers"] }{ Environment.NewLine }";
                            changed = true;
                        }

                        if (CheckPhotos.Checked && decoded["counters"]["photos"] != null && counters.decoded["counters"]["photos"] != null && (int)counters.decoded["counters"]["photos"] != (int)decoded["counters"]["photos"])
                        {
                            try
                            {
                                Thread.Sleep(350);
                                Get($"{ApiRequestLink}photos.getAll?owner_id={user_id}&need_hidden=1&access_token={AuthToken}&v={ApiVersion}");
                                JObject photos = JObject.Parse(server_response);
                                var photosresponse = photos["response"];
                                if (counters.photos_response == null) counters.photos_response = (JObject)photosresponse;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in photos: { counters.decoded["counters"]["photos"] }  to { decoded["counters"]["photos"] }{ Environment.NewLine }";
                                    if ((int)counters.decoded["counters"]["photos"] < (int)decoded["counters"]["photos"])
                                    {
                                        List<string> OldPhotos = new List<string>();
                                        foreach (var photo in counters.photos_response["items"]) OldPhotos.Add((string)photo["sizes"][photo["sizes"].Count() - 1]["url"]);
                                        foreach (var photo in photosresponse["items"])
                                            if (!OldPhotos.Contains((string)photo["sizes"][photo["sizes"].Count() - 1]["url"]))
                                            {
                                                temp_text += $"   New photo: {photo["sizes"][photo["sizes"].Count() - 1]["url"]}{ Environment.NewLine }";
                                                if (DownloadFiles.Checked)
                                                {
                                                    link = (string)photo["sizes"][photo["sizes"].Count() - 1]["url"];
                                                    temp_text += $"{_link}{Environment.NewLine}";
                                                }
                                            }
                                    }
                                    else
                                    {
                                        List<string> NewPhotos = new List<string>();
                                        foreach (var photo in photosresponse["items"]) NewPhotos.Add((string)photo["sizes"][photo["sizes"].Count() - 1]["url"]);
                                        foreach (var photo in counters.photos_response["items"])
                                            if (!NewPhotos.Contains((string)photo["sizes"][photo["sizes"].Count() - 1]["url"]))
                                            { 
                                                temp_text += $"   Removed photo: {photo["sizes"][photo["sizes"].Count() - 1]["url"]}{ Environment.NewLine }";
                                                if (DownloadFiles.Checked)
                                                {
                                                    link = (string)photo["sizes"][photo["sizes"].Count() - 1]["url"];
                                                    temp_text += $"{_link}{Environment.NewLine}";
                                                }
                                            }  
                                    }
                                    changed = true;
                                    counters.photos_response = (JObject)photosresponse;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at photo check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckGifts.Checked && decoded["counters"]["gifts"] != null && counters.decoded["counters"]["gifts"] != null && (int)counters.decoded["counters"]["gifts"] != (int)decoded["counters"]["gifts"])
                        {
                            try 
                            {
                                Thread.Sleep(350);
                                Get($"{ApiRequestLink}gifts.get?user_id={user_id}&access_token={AuthToken}&v={ApiVersion}"); //get gifts
                                JObject gifts = JObject.Parse(server_response);
                                var giftsresponse = gifts["response"];
                                if (counters.gifts_response == null) counters.gifts_response = (JObject)giftsresponse;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in gifts: { counters.decoded["counters"]["gifts"] }  to { decoded["counters"]["gifts"] }{ Environment.NewLine }";
                                    if ((int)counters.decoded["counters"]["gifts"] < (int)decoded["counters"]["gifts"])
                                    {
                                        List<string> OldGifts = new List<string>();
                                        foreach (var gift in counters.gifts_response["items"]) OldGifts.Add((string)gift["from_id"]);
                                        foreach (var gift in giftsresponse["items"])
                                            if (!OldGifts.Contains((string)gift["from_id"]))
                                                temp_text += $"   New gift from user: vk.com/id{ gift["from_id"] }{ Environment.NewLine }   With message: { gift["message"] }{ Environment.NewLine }   Thumbnail: { gift["gift"]["thumb_256"] }{ Environment.NewLine }";
                                    }
                                    else
                                    {
                                        List<string> NewGifts = new List<string>();
                                        foreach (var gift in giftsresponse["items"]) NewGifts.Add((string)gift["from_id"]);
                                        foreach (var gift in counters.gifts_response["items"])
                                            if (!NewGifts.Contains((string)gift["from_id"]))
                                                temp_text += $"   Removed gift from user: vk.com/id{ gift["from_id"] }{ Environment.NewLine }   With message: { gift["message"] }{ Environment.NewLine }   Thumbnail: { gift["gift"]["thumb_256"] }{ Environment.NewLine }";
                                    }
                                    changed = true;
                                    counters.gifts_response = (JObject)giftsresponse;
                                }
                            } catch (Exception e) { WriteToFile($"Exception thrown at gifts check, {e.Message}{Environment.NewLine}"); }
                        }
                    
                        if (CheckVideos.Checked && decoded["counters"]["videos"] != null && counters.decoded["counters"]["videos"] != null && (int)counters.decoded["counters"]["videos"] != (int)decoded["counters"]["videos"])
                        {
                            try 
                            {
                                Thread.Sleep(350); //timeout on api requests
                                Get($"{ApiRequestLink}video.get?owner_id={user_id}&access_token={AuthToken}&v={ApiVersion}");
                                JObject videos = JObject.Parse(server_response);
                                var videosresponse = videos["response"];
                                if (counters.videos_response == null) counters.videos_response = (JObject)videosresponse;
                                else 
                                {
                                    temp_text += $"{ CurrentTime }, Update in videos: { counters.decoded["counters"]["videos"] }  to { decoded["counters"]["videos"] }{ Environment.NewLine }";
                                    if ((int)counters.decoded["counters"]["videos"] < (int)decoded["counters"]["videos"])
                                    {
                                        List<string> OldVideos = new List<string>();
                                        foreach (var video in counters.videos_response["items"]) OldVideos.Add((string)video["id"]);
                                        foreach (var video in videosresponse["items"])
                                            if (!OldVideos.Contains((string)video["id"]))
                                            {
                                                link = (string)video["player"];
                                                temp_text += $"   New video:{ Environment.NewLine }   Title: {video["title"]}{ Environment.NewLine }"
                                                    + $"   Video link: {video["player"]}{ Environment.NewLine }"
                                                    + $"   Video ID & Owner ID: {video["id"]} & {video["owner_id"]}{ Environment.NewLine }";
                                            }
                                    }
                                    else
                                    {
                                        List<string> NewVideos = new List<string>();
                                        foreach (var video in videosresponse["items"]) NewVideos.Add((string)video["id"]);
                                        foreach (var video in counters.videos_response["items"])
                                            if (!NewVideos.Contains((string)video["id"]))
                                            {
                                                link = (string)video["player"];
                                                temp_text += $"   Removed video:{ Environment.NewLine }   Title: {video["title"]}{ Environment.NewLine }"
                                                    + $"   Video link: {video["player"]}{ Environment.NewLine }"
                                                    + $"   Video ID & Owner ID: {video["id"]} & {video["owner_id"]}{ Environment.NewLine }";
                                            }
                                    }
                                    changed = true;
                                    counters.videos_response = (JObject)videosresponse;
                                }
                            } catch (Exception e) { WriteToFile($"Exception thrown at video check, {e.Message}{Environment.NewLine}"); }  
                        }

                        if (CheckFriends.Checked && decoded["counters"]["friends"] != null && counters.decoded["counters"]["friends"] != null && (int)counters.decoded["counters"]["friends"] != (int)decoded["counters"]["friends"])
                        {
                            try
                            {
                                Thread.Sleep(350); //timeout on api requests
                                Get($"{ApiRequestLink}friends.get?user_id={user_id}&fields=name&access_token={AuthToken}&v={ApiVersion}");
                                JObject friends = JObject.Parse(server_response);
                                var friendsresponse = friends["response"];
                                if (counters.friends_response == null) counters.friends_response = (JObject)friendsresponse;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in friends: { counters.decoded["counters"]["friends"] }  to { decoded["counters"]["friends"] }{ Environment.NewLine }";
                                    if ((int)counters.decoded["counters"]["friends"] < (int)decoded["counters"]["friends"])
                                    {
                                        List<string> OldFriends = new List<string>();
                                        foreach (var friend in counters.friends_response["items"]) OldFriends.Add((string)friend["id"]);
                                        foreach (var friend in friendsresponse["items"])
                                            if (!OldFriends.Contains((string)friend["id"]))
                                                temp_text += $"   New friend: {friend["first_name"]} {friend["last_name"]}, vk.com/id{friend["id"]}{ Environment.NewLine }";
                                    }
                                    else
                                    {
                                        List<string> NewFriends = new List<string>();
                                        foreach (var friend in friendsresponse["items"]) NewFriends.Add((string)friend["id"]);
                                        foreach (var friend in counters.friends_response["items"])
                                            if (!NewFriends.Contains((string)friend["id"]))
                                                temp_text += $"   Removed friend: {friend["first_name"]} {friend["last_name"]}, vk.com/id{friend["id"]}{ Environment.NewLine }";
                                    }
                                }
                                changed = true;
                                counters.friends_response = (JObject)friendsresponse;
                            } catch (Exception e) { WriteToFile($"Exception thrown at friends check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckFollowers.Checked && decoded["counters"]["followers"] != null && counters.decoded["counters"]["followers"] != null && (int)counters.decoded["counters"]["followers"] != (int)decoded["counters"]["followers"]) //checks in all modes including offline-offline, but depends on private profile
                        {
                            try
                            {
                                Thread.Sleep(350);
                                Get($"{ApiRequestLink}users.getFollowers?user_id={user_id}&access_token={AuthToken}&v={ApiVersion}");
                                JObject followers = JObject.Parse(server_response);
                                var followersresponse = followers["response"];
                                if (counters.followers_response == null) counters.followers_response = (JObject)followersresponse;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in followers: { counters.decoded["counters"]["followers"] }  to { decoded["counters"]["followers"] }{ Environment.NewLine }";
                                    if ((int)counters.decoded["counters"]["followers"] < (int)decoded["counters"]["followers"])
                                    {
                                        List<string> OldFollowers = new List<string>();
                                        foreach (var follower in counters.followers_response["items"]) OldFollowers.Add((string)follower["id"]);
                                        foreach (var follower in followersresponse["items"])
                                            if (!OldFollowers.Contains((string)follower["id"]))
                                                temp_text += $"   New follower: {follower["first_name"]} {follower["last_name"]}, vk.com/id{follower["id"]}{ Environment.NewLine }";
                                    }
                                    else
                                    {
                                        List<string> NewFollowers = new List<string>();
                                        foreach (var follower in followersresponse["items"]) NewFollowers.Add((string)follower["id"]);
                                        foreach (var follower in counters.followers_response["items"])
                                            if (!NewFollowers.Contains((string)follower["id"]))
                                                temp_text += $"   Removed follower: {follower["first_name"]} {follower["last_name"]}, vk.com/id{follower["id"]}{ Environment.NewLine }";
                                    }
                                }
                                changed = true;
                                counters.followers_response = (JObject)followersresponse;
                            } catch (Exception e) { WriteToFile($"Exception thrown at followers check, {e.Message}{Environment.NewLine}"); }
                        }
                        if (CheckPosts.Checked && timeout)
                        {
                            try
                            {
                                Thread.Sleep(350);
                                Get($"{ApiRequestLink}wall.get?owner_id={user_id}&access_token={AuthToken}&v={ApiVersion}");
                                JObject posts = JObject.Parse(server_response);
                                var postsresponse = posts["response"];
                                if (counters.posts_response == null) counters.posts_response = (JObject)postsresponse;
                                else if ((int)counters.posts_response["count"] != (int)postsresponse["count"])
                                {
                                    temp_text += $"{ CurrentTime }, Update in posts: { counters.posts_response["count"] }  to { postsresponse["count"] }{ Environment.NewLine }";
                                    if ((int)counters.posts_response["count"] < (int)postsresponse["count"])
                                    {
                                        List<string> OldPosts = new List<string>();
                                        foreach (var post in counters.posts_response["items"]) OldPosts.Add((string)post["id"]);
                                        foreach (var post in postsresponse["items"])
                                            if (!OldPosts.Contains((string)post["id"]))
                                            {
                                                temp_text += $"   New post:{Environment.NewLine}   Post author: vk.com/id{(string)post["from_id"]}{ Environment.NewLine }";
                                                if (post["text"] != null && (string)post["text"] != "")
                                                    temp_text += $"   Post text: {(string)post["text"]}{ Environment.NewLine }";
                                                if (post["attachments"] != null)
                                                    foreach (var attachment in post["attachments"])
                                                        switch ((string)attachment["type"])
                                                        {
                                                            case "photo":
                                                                {
                                                                    temp_text += $"   Attached image: {(string)attachment["photo"]["sizes"][attachment["photo"]["sizes"].Count() - 1]["url"]}{ Environment.NewLine }";
                                                                    if (DownloadFiles.Checked)
                                                                    {
                                                                        link = (string)attachment["photo"]["sizes"][attachment["photo"]["sizes"].Count() - 1]["url"];
                                                                        temp_text += $"{_link}{Environment.NewLine}";
                                                                    }
                                                                }
                                                                break;
                                                            case "video":
                                                                { temp_text += $"   Attached video title: {(string)attachment["video"]["title"]}{ Environment.NewLine }   Video ID & Owner ID: {(string)attachment["video"]["id"]} & {(string)attachment["video"]["id"]}{ Environment.NewLine }"; }
                                                                break;
                                                            case "audio":
                                                                { temp_text += $"   Attached audio: {(string)attachment["audio"]["artist"]}   —  {(string)attachment["audio"]["title"]}{ Environment.NewLine }"; }
                                                                break;
                                                            case "link":
                                                                { temp_text += $"   Attached link: {(string)attachment["link"]["url"]}{ Environment.NewLine }"; }
                                                                break;
                                                            case "doc":
                                                                {
                                                                    temp_text += $"   Attached document: {(string)attachment["doc"]["title"]}{ Environment.NewLine }    Document link: {(string)attachment["doc"]["url"]}{ Environment.NewLine }";
                                                                    if (DownloadFiles.Checked)
                                                                    {
                                                                        link = (string)attachment["doc"]["url"];
                                                                        temp_text += $"{_link}{Environment.NewLine}";
                                                                    }
                                                                }
                                                                break;
                                                        }
                                            }
                                    }
                                    else
                                    {
                                        List<string> NewPosts = new List<string>();
                                        foreach (var post in postsresponse["items"]) NewPosts.Add((string)post["id"]);
                                        foreach (var post in counters.posts_response["items"])
                                            if (!NewPosts.Contains((string)post["id"]))
                                            {
                                                temp_text += $"   New post:{Environment.NewLine}   Post author: vk.com/id{(string)post["from_id"]}{ Environment.NewLine }";
                                                if (post["text"] != null && (string)post["text"] != "")
                                                    temp_text += $"   Post text: {(string)post["text"]}{ Environment.NewLine }";
                                                if (post["attachments"] != null)
                                                    foreach (var attachment in post["attachments"])
                                                        switch ((string)attachment["type"])
                                                        {
                                                            case "photo":
                                                                {
                                                                    temp_text += $"   Attached image: {(string)attachment["photo"]["sizes"][attachment["photo"]["sizes"].Count() - 1]["url"]}{ Environment.NewLine }";
                                                                    if (DownloadFiles.Checked)
                                                                    {
                                                                        link = (string)attachment["photo"]["sizes"][attachment["photo"]["sizes"].Count() - 1]["url"];
                                                                        temp_text += $"{_link}{Environment.NewLine}";
                                                                    }
                                                                }
                                                                break;
                                                            case "video":
                                                                { temp_text += $"   Attached video title: {(string)attachment["video"]["title"]}{ Environment.NewLine }   Video ID & Owner ID: {(string)attachment["video"]["id"]} & {(string)attachment["video"]["id"]}{ Environment.NewLine }"; }
                                                                break;
                                                            case "audio":
                                                                { temp_text += $"   Attached audio: {(string)attachment["audio"]["artist"]}   —  {(string)attachment["audio"]["title"]}{ Environment.NewLine }"; }
                                                                break;
                                                            case "link":
                                                                { temp_text += $"   Attached link: {(string)attachment["link"]["url"]}{ Environment.NewLine }"; }
                                                                break;
                                                            case "doc":
                                                                {
                                                                    temp_text += $"   Attached document: {(string)attachment["doc"]["title"]}{ Environment.NewLine }    Document link: {(string)attachment["doc"]["url"]}{ Environment.NewLine }";
                                                                    if (DownloadFiles.Checked)
                                                                    {
                                                                        link = (string)attachment["doc"]["url"];
                                                                        temp_text += $"{_link}{Environment.NewLine}";
                                                                    }
                                                                }
                                                                break;
                                                        }
                                            }
                                    }
                                    changed = true;
                                    counters.posts_response = (JObject)postsresponse;
                                }
                                timeout = false;
                                timer.Start();
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at posts, probably too many requests were made, {e.Message}{Environment.NewLine}"); BeginInvoke(new MethodInvoker(delegate { CheckPosts.Checked = false; }));  }
                        }
                        if (CheckStories.Checked && timeout)
                        {
                            try
                            {
                                Thread.Sleep(350);
                                Get($"{ApiRequestLink}stories.get?owner_id={user_id}&access_token={AuthToken}&v={ApiVersion}");
                                JObject stories = JObject.Parse(server_response);
                                var storiesresponse = stories["response"]["items"][0];
                                if (counters.stories_response == null) counters.stories_response = (JObject)storiesresponse;
                                else if (counters.stories_response["stories"].Count() != storiesresponse["stories"].Count())
                                {
                                    temp_text += $"{ CurrentTime }, Update in stories: { counters.stories_response["stories"].Count() }  to { storiesresponse["stories"].Count() }{ Environment.NewLine }";
                                    if (counters.stories_response["stories"].Count() < storiesresponse["stories"].Count())
                                    {
                                        List<string> OldStories = new List<string>();
                                        foreach (var story in counters.stories_response["stories"]) OldStories.Add((string)story["id"]);
                                        foreach (var story in storiesresponse["stories"])
                                            if (!OldStories.Contains((string)story["id"]))
                                            {
                                                switch ((string)story["type"])
                                                {
                                                    case "photo":
                                                        {
                                                            temp_text += $"   New story: {story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"]}{Environment.NewLine}";
                                                            if (DownloadFiles.Checked)
                                                            {
                                                                link = (string)story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"];
                                                                temp_text += $"{_link}{Environment.NewLine}";
                                                            }
                                                        } 
                                                        break;
                                                    case "video":
                                                        {
                                                            temp_text += $"   New story: {story["video"]["player"]}{Environment.NewLine}";
                                                            if (DownloadFiles.Checked)
                                                            {
                                                                link = (string)story["video"]["player"];
                                                                temp_text += $"{_link}{Environment.NewLine}";
                                                            }
                                                        } 
                                                        break;
                                                }
                                            }
                                    }
                                    else
                                    {
                                        List<string> NewStories = new List<string>();
                                        foreach (var story in storiesresponse["stories"]) NewStories.Add((string)story["id"]);
                                        foreach (var story in counters.stories_response["stories"])
                                            if (!NewStories.Contains((string)story["id"]))
                                            {
                                                switch ((string)story["type"])
                                                {
                                                    case "photo":
                                                        {
                                                            temp_text += $"   Removed story: {story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"]}{Environment.NewLine}";
                                                            if (DownloadFiles.Checked)
                                                            {
                                                                link = (string)story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"];
                                                                temp_text += $"{_link}{Environment.NewLine}";
                                                            }
                                                        }
                                                        break;
                                                    case "video":
                                                        {
                                                            temp_text += $"   Removed story: {story["video"]["player"]}{Environment.NewLine}";
                                                            if (DownloadFiles.Checked)
                                                            {
                                                                link = (string)story["video"]["player"];
                                                                temp_text += $"{_link}{Environment.NewLine}";
                                                            }
                                                        }
                                                        break;
                                                }
                                            }    
                                    }
                                    changed = true;
                                    counters.stories_response = (JObject)storiesresponse;
                                }
                                timeout = false;
                                timer.Start();
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at stories check, {e.Message}{Environment.NewLine}"); }
                        }
                    }
                
                    if (CheckPages.Checked && decoded["counters"]["pages"] != null && (int)counters.decoded["counters"]["pages"] != (int)decoded["counters"]["pages"]) //checks independently from private profile, but only in offline-online online-online online-offline modes
                    {
                        temp_text += $"{ CurrentTime }, Update in pages: { counters.decoded["counters"]["pages"] }  to { decoded["counters"]["pages"] }{ Environment.NewLine }";
                        changed = true;
                    }

                    if (CheckStatus.Checked) //checks independently from private profile, but only in offline-online online-online online-offline modes
                    {
                        try
                        {
                            if (decoded["status"] != null && counters.decoded["status"] != null && (string)counters.decoded["status"] != (string)decoded["status"])
                            {
                                temp_text += $"{ CurrentTime }, Update in status: old '{ counters.decoded["status"] }', new '{ decoded["status"] }'{ Environment.NewLine }";
                                changed = true;
                            }
                            if (counters.decoded["status_audio"] != null)
                            {
                                if (decoded["status_audio"] == null)
                                {
                                    temp_text += $"{ CurrentTime }, Stopped listening to music{ Environment.NewLine }";
                                    changed = true;
                                }
                                else if ((string)counters.decoded["status_audio"]["id"] != (string)decoded["status_audio"]["id"])
                                {
                                    temp_text += $"{ CurrentTime }, New track: { decoded["status_audio"]["artist"] } - { decoded["status_audio"]["title"] }{ Environment.NewLine }";
                                    changed = true;
                                }
                            }
                            else if (decoded["status_audio"] != null)
                            { 
                                temp_text += $"{ CurrentTime }, Started listening to music: { decoded["status_audio"]["artist"] } - { decoded["status_audio"]["title"] }{ Environment.NewLine }";
                                changed = true;
                            }
                        }
                        catch (Exception e) { WriteToFile($"Exception thrown at status check, {e.Message}{Environment.NewLine}"); }
                    }

                    if (changed) EventLogs.Invoke((MethodInvoker)delegate
                    {
                        if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') temp_text = $"  { OnlineDateTime }{ Environment.NewLine }" + temp_text;
                        WriteToFile(temp_text);
                        if ((bool)counters.decoded["online"] == (bool)decoded["online"] && (bool)counters.decoded["online"]) counters.decoded["online"] = !(bool)counters.decoded["online"];
                        changed = false;
                    });

                    if ((bool)counters.decoded["online"] != (bool)decoded["online"]) //if 'online status' changed
                    {
                        if (!(bool)counters.decoded["online"] && (bool)decoded["online"]) //if changes were committed or offline to online transition happened
                            if (CheckOnlineDevice.Checked)
                                if (decoded["online_app"] != null && decoded["last_seen"]["platform"] != null) WriteToFile($"Online {decoded["last_seen"]["platform"]}, {decoded["online_app"]}: { OnlineDateTime }   —");
                                else WriteToFile($"Online {decoded["last_seen"]["platform"]}: { OnlineDateTime }   —");
                            else WriteToFile($"Online: { OnlineDateTime }   —");
                        else WriteToFile($"  { OnlineDateTime }{ Environment.NewLine }"); //if transition online to offline happened
                    }
                    counters.decoded = (JObject)decoded;
                } catch (Exception e) { WriteToFile($"Global error, possibly get counters failed, {e.Message}{Environment.NewLine}"); }
                Thread.Sleep(350);
            }
        }
        private void WriteToFile(string text)
        {
            string[] str = File.ReadAllLines(Application.StartupPath + $"\\Logs\\{user_id}.txt");
            EventLogs.Invoke((MethodInvoker)delegate 
            { 
                EventLogs.AppendText(text); 
            });
            if (Logs.Checked) File.AppendAllText(Application.StartupPath + $"\\Logs\\{user_id}.txt", text);
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
                if (File.Exists(Application.StartupPath + $"\\Logs\\{user_id}.txt")) EventLogs.Invoke((MethodInvoker)delegate { EventLogs.Text = ""; EventLogs.AppendText(File.ReadAllText(Application.StartupPath + $"\\Logs\\{user_id}.txt")); });
                else 
                {
                    if (!Directory.Exists(Application.StartupPath + "\\Logs")) Directory.CreateDirectory(Application.StartupPath + "\\Logs");
                    File.AppendAllText(Application.StartupPath + $"\\Logs\\{user_id}.txt", $"Log file for user id{user_id}");
                    EventLogs.Text = "";
                    EventLogs.AppendText(File.ReadAllText(Application.StartupPath + $"\\Logs\\{user_id}.txt"));
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
            LastOnlineLabel.Text = "Last Online";
            if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') 
                WriteToFile($"  { OnlineDateTime }{ Environment.NewLine }");
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
        private void CustomAppIdToggled(object sender, EventArgs e)
        {
            if (CustomAppIdToggle.Checked) CustomAppId.Enabled = true;
            else CustomAppId.Enabled = false;
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
                new KeyValuePair<string, string>("Stories", CheckStories.Checked.ToString()),
                new KeyValuePair<string, string>("Subscriptions", CheckSubscriptions.Checked.ToString()),
                new KeyValuePair<string, string>("Videos", CheckVideos.Checked.ToString()),
                new KeyValuePair<string, string>("ClipsFollowers", CheckClips.Checked.ToString()),
                new KeyValuePair<string, string>("OpenSettings", OpenSettings.Checked.ToString()),
                new KeyValuePair<string, string>("ClearCookies", ClearCookies.Checked.ToString()),
                new KeyValuePair<string, string>("SelectedUser", ListOfIDs.SelectedIndex.ToString()),
                new KeyValuePair<string, string>("Autostart", Autostart.Checked.ToString()),
                new KeyValuePair<string, string>("CustomAppIdToggle", CustomAppIdToggle.Checked.ToString()),
                new KeyValuePair<string, string>("CustomAppId", CustomAppId.Text),
                new KeyValuePair<string, string>("DownloadFiles", DownloadFiles.Checked.ToString()),
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
            CheckStories.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Stories"]);
            CheckSubscriptions.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Subscriptions"]);
            CheckVideos.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Videos"]);
            CheckClips.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["ClipsFollowers"]);
            OpenSettings.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["OpenSettings"]);
            SaveToken.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["SaveToken"]);
            DownloadFiles.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["DownloadFiles"]);
            CustomAppIdToggle.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["CustomAppIdToggle"]);
            CustomAppId.Text = ConfigurationManager.AppSettings["CustomAppId"];
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
        public JObject decoded = null;
        public JObject photos_response = null;
        public JObject gifts_response = null;
        public JObject videos_response = null;
        public JObject friends_response = null;
        public JObject followers_response = null;
        public JObject posts_response = null;
        public JObject albums_response = null;
        public JObject subscriptions_response = null;
        public JObject stories_response = null;
        public DateTime LastOnlineTime = DateTime.Now;
    }
}

