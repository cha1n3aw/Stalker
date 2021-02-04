using MetroFramework.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace Stalker
{
    public partial class Stalker : MetroForm
    {
        private List<string> FriendsIdList = new List<string>();
        private List<string> CustomIdList = new List<string>();
        private int user_id = 0;
        private readonly int WindowHeight = 570;
        string OnlineDateTime = string.Empty;
        string OnlineTime = string.Empty;
        string CurrentTime = string.Empty;
        string[] customids = new string[] { };
        Thread Stalking;
        VkApi api;
        System.Timers.Timer timer;
        RegistryKey AutostartOnBoot = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private string DownloadByLink(Uri link)
        {
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Files"))
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Files");
            string directory = $"{System.Windows.Forms.Application.StartupPath}\\Files\\id{user_id}-{Regex.Match(link.ToString(), @"(?:[^/][\d\w\.]+)(?<=(?:.jpg)|(?:.mp4)|(?:.jpeg)|(?:.png)|(?:.pdf)|(?:.gif)|(?:.doc))").Value}";
            if (!File.Exists(directory))
                using (WebClient wc = new WebClient())
                    wc.DownloadFileAsync(link, directory);
            return $"   Downloaded file: {directory}{Environment.NewLine}";
        }
        private void FetchFriends()
        {
            var friends = api.Friends.Get(new FriendsGetParams { Fields = ProfileFields.Domain });
            if (friends.Count > 0)
                for (int i = 0; i < friends.Count; i++)
                {
                    FriendsIdList.Add(friends[i].Id.ToString());
                    ListOfIDs.Items.Add($"{friends[i].FirstName} {friends[i].LastName}");
                }
            if (customids.Length > 0)
            {
                List<long> userIds = new List<long>();
                foreach (string id in customids) userIds.Add(Int32.Parse(id));
                var customusers = api.Users.Get(userIds, ProfileFields.Domain);
                foreach (var user in customusers)
                {
                    CustomIdList.Add(user.Id.ToString());
                    ListOfIDs.Items.Add($"{user.FirstName} {user.LastName}");
                }
            }
        }
        private void LogInVK(string AuthToken)
        {
            api = new VkApi(new ServiceCollection().AddAudioBypass());
            if (AuthToken == string.Empty) api.Authorize(AuthParams);
            else api.Authorize(new ApiAuthParams { AccessToken = AuthToken });
        }
        private void Poll()
        {
            bool posts_timeout = true;
            bool stories_timeout = true;
            timer = new System.Timers.Timer
            {
                Interval = 17300,
                AutoReset = true
            };
            timer.Elapsed += (sender, data) =>
            {
                posts_timeout = true;
                stories_timeout = true;
            };
            timer.Enabled = true;
            Counters counters = new Counters();
            RestrictToggles(false);

            while (StartStop.Checked)
            {
                if (PreventSleep.Checked) DLLIMPORTS.SetThreadExecutionState(DLLIMPORTS.EXECUTION_STATE.ES_CONTINUOUS | DLLIMPORTS.EXECUTION_STATE.ES_SYSTEM_REQUIRED);
                string temp_text = string.Empty;
                bool changed = false;
                string OnlineApp = string.Empty;
                ReadOnlyCollection<User> ApiCounters;
                try
                {
                    List<long> userIds = new List<long> { user_id };
                    ApiCounters = api.Users.Get(userIds, ProfileFields.Online | ProfileFields.Counters | ProfileFields.LastSeen | ProfileFields.Status);
                }
                catch (Exception e)
                {
                    WriteToFile($"Exception thrown at counters check, {e.Message}{Environment.NewLine}");
                    continue;
                }

                DateTime LastOnlineTime = ((DateTime)ApiCounters[0].LastSeen.Time).AddHours(5);
                OnlineDateTime = LastOnlineTime.ToString("dd MMM yyyy, HH:mm:ss", DateTimeFormatInfo.InvariantInfo); //Get last seen time in string format
                OnlineTime = LastOnlineTime.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
                CurrentTime = DateTime.Now.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
                LastOnlineLabel.Invoke((MethodInvoker)delegate
                {
                    if (counters.LastOnlineTime != LastOnlineTime)
                    {
                        LastOnlineLabel.Text = $"{OnlineTime}";
                        if (DetailedLogs.Checked)
                        {
                            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Logs")) Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Logs");
                            if (!File.Exists(System.Windows.Forms.Application.StartupPath + $"\\Logs\\{user_id}_detailed.txt")) File.AppendAllText(System.Windows.Forms.Application.StartupPath + $"\\Logs\\{user_id}_detailed.txt", $"Detailed online log file for user id{user_id}{Environment.NewLine}");
                            File.AppendAllText(System.Windows.Forms.Application.StartupPath + $"\\Logs\\{user_id}_detailed.txt", $"{OnlineTime}{Environment.NewLine}");
                        }
                        counters.LastOnlineTime = LastOnlineTime;
                    }
                });

                if (counters.ApiCounters == null) //first itteration
                {
                    ApiCounters[0].Online = false;
                    if (ApiCounters[0].LastSeen.Platform != null)
                        switch (ApiCounters[0].LastSeen.Platform)
                        {
                            case "1":
                                { ApiCounters[0].LastSeen.Platform = "Mobile WEB"; }
                                break;
                            case "2":
                                { ApiCounters[0].LastSeen.Platform = "iPhone"; }
                                break;
                            case "3":
                                { ApiCounters[0].LastSeen.Platform = "iPad"; }
                                break;
                            case "4":
                                { ApiCounters[0].LastSeen.Platform = "Android"; }
                                break;
                            case "5":
                                { ApiCounters[0].LastSeen.Platform = "WinPhone"; }
                                break;
                            case "6":
                                { ApiCounters[0].LastSeen.Platform = "Windows 8"; }
                                break;
                            case "7":
                                { ApiCounters[0].LastSeen.Platform = "WEB"; }
                                break;
                            case "8":
                                { ApiCounters[0].LastSeen.Platform = "VK Mobile"; }
                                break;
                            default:
                                { ApiCounters[0].LastSeen.Platform = "Unknown"; }
                                break;
                        }
                    counters.ApiCounters = ApiCounters;
                    if ((bool)ApiCounters[0].IsClosed && !FriendsIdList.Contains(ApiCounters[0].Id.ToString()))
                    {
                        RestrictToggles(true);
                        WriteToFile($"User has private profile, settings were partially restricted{ Environment.NewLine }");
                    }
                    else RestrictToggles(false);
                }
                else //first itteration passed
                {
                    if ((bool)counters.ApiCounters[0].IsClosed != (bool)ApiCounters[0].IsClosed && !FriendsIdList.Contains(ApiCounters[0].Id.ToString())) //profile privacy settings changed
                    {
                        if (!(bool)counters.ApiCounters[0].IsClosed && (bool)ApiCounters[0].IsClosed)
                        {
                            RestrictToggles(true);
                            WriteToFile($"{ CurrentTime }, User switched to private profile, settings were partially restricted{ Environment.NewLine }");
                        }
                        else
                        {
                            RestrictToggles(false);
                            WriteToFile($"{ CurrentTime }, User switched to public profile, settings were unrestricted{ Environment.NewLine }");
                        }
                    }

                    if (CheckOnlineDevice.Checked)
                    {
                        if (ApiCounters[0].LastSeen.Platform != null)
                        {
                            switch (ApiCounters[0].LastSeen.Platform)
                            {
                                case "1":
                                    { ApiCounters[0].LastSeen.Platform = "Mobile WEB"; }
                                    break;
                                case "2":
                                    { ApiCounters[0].LastSeen.Platform = "iPhone"; }
                                    break;
                                case "3":
                                    { ApiCounters[0].LastSeen.Platform = "iPad"; }
                                    break;
                                case "4":
                                    { ApiCounters[0].LastSeen.Platform = "Android"; }
                                    break;
                                case "5":
                                    { ApiCounters[0].LastSeen.Platform = "WinPhone"; }
                                    break;
                                case "6":
                                    { ApiCounters[0].LastSeen.Platform = "Windows 8"; }
                                    break;
                                case "7":
                                    { ApiCounters[0].LastSeen.Platform = "WEB"; }
                                    break;
                                case "8":
                                    { ApiCounters[0].LastSeen.Platform = "VK Mobile"; }
                                    break;
                                default:
                                    { ApiCounters[0].LastSeen.Platform = "Unknown"; }
                                    break;
                            }
                            if (counters.ApiCounters[0].LastSeen.Platform != null && counters.ApiCounters[0].LastSeen.Platform != ApiCounters[0].LastSeen.Platform)
                            {
                                temp_text += $"{ OnlineTime }, User switched from { counters.ApiCounters[0].LastSeen.Platform }  to { ApiCounters[0].LastSeen.Platform }{ Environment.NewLine }";
                                changed = true;
                            }
                        }

                        if (ApiCounters[0].OnlineApp != null)
                        {
                            switch (ApiCounters[0].OnlineApp)
                            {
                                case 2274003:
                                    { OnlineApp = "Android App"; }
                                    break;
                                case 3140623:
                                    { OnlineApp = "iOS App"; }
                                    break;
                                case 3682744:
                                    { OnlineApp = "iPadOS App"; }
                                    break;
                                case 3697615:
                                    { OnlineApp = "Windows Desktop App"; }
                                    break;
                                case 3502557:
                                    { OnlineApp = "Windows Phone App"; }
                                    break;
                                case 2685278:
                                    { OnlineApp = "Kate Mobile"; }
                                    break;
                                default:
                                    { OnlineApp = ApiCounters[0].OnlineApp.ToString(); }
                                    break;
                            }
                            if (counters.ApiCounters[0].OnlineApp != null && (long)counters.ApiCounters[0].OnlineApp != (long)ApiCounters[0].OnlineApp)
                            {
                                temp_text += $"{ OnlineTime }, User switched to another app: from { counters.OnlineApp }  to { OnlineApp }{ Environment.NewLine }";
                                changed = true;
                            }
                        }
                    }

                    if (!(bool)ApiCounters[0].IsClosed && !FriendsIdList.Contains(ApiCounters[0].Id.ToString())) //if profile is closed then theres nothing to check! also it comes with "restrict toggles" so this 'if' statement isnt so necessary
                    {
                        if (CheckAlbums.Checked && ApiCounters[0].Counters.Albums != null && counters.ApiCounters[0].Counters.Albums != null && (counters.Albums == null || (int)ApiCounters[0].Counters.Albums != (int)counters.ApiCounters[0].Counters.Albums))
                        {
                            try
                            {
                                VkCollection<PhotoAlbum> Albums = api.Photo.GetAlbums(new PhotoGetAlbumsParams { OwnerId = user_id });
                                if (counters.Albums == null) counters.Albums = Albums;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in albums: { counters.ApiCounters[0].Counters.Albums }  to { ApiCounters[0].Counters.Albums }{ Environment.NewLine }";
                                    if ((int)ApiCounters[0].Counters.Albums < (int)counters.ApiCounters[0].Counters.Albums)
                                        foreach (var id in Albums.Select(x => x.Id).Except(counters.Albums.Select(x => x.Id)))
                                            foreach (PhotoAlbum album in Albums.Where(i => i.Id == id).ToList())
                                                temp_text += $"   New album:{ Environment.NewLine }" +
                                                $"   Link: vk.com/album{user_id}_{album.Id}{ Environment.NewLine }" +
                                                $"   Title: {album.Title}{ Environment.NewLine }";
                                    else foreach (var id in counters.Albums.Select(x => x.Id).Except(Albums.Select(x => x.Id)))
                                            foreach (PhotoAlbum album in counters.Albums.Where(i => i.Id == id).ToList())
                                                temp_text += $"   Removed album:{ Environment.NewLine }" +
                                                    $"   Link (unavailaible): vk.com/album{user_id}_{album.Id}{ Environment.NewLine }" +
                                                    $"   Title: {album.Title}{ Environment.NewLine }";
                                    changed = true;
                                    counters.Albums = Albums;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at albums check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckAudios.Checked && counters.ApiCounters[0].Counters.Audios != null && ApiCounters[0].Counters.Audios != null && (counters.Audios == null || (int)counters.ApiCounters[0].Counters.Audios != (int)ApiCounters[0].Counters.Audios))
                        {
                            try
                            {
                                VkCollection<Audio> Audios = api.Audio.Get(new AudioGetParams { OwnerId = user_id, Count = 6000 });
                                if (counters.Audios == null) counters.Audios = Audios;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in audios: { counters.ApiCounters[0].Counters.Audios }  to { ApiCounters[0].Counters.Audios }{ Environment.NewLine }";
                                    if ((int)ApiCounters[0].Counters.Albums < (int)counters.ApiCounters[0].Counters.Albums)
                                        foreach (var id in Audios.Select(x => x.Id).Except(counters.Audios.Select(x => x.Id)))
                                            foreach (Audio audio in Audios.Where(i => i.Id == id).ToList())
                                                temp_text += $"   New audio: { audio.Artist } - { audio.Title }{ Environment.NewLine }";
                                    else foreach (var id in counters.Audios.Select(x => x.Id).Except(Audios.Select(x => x.Id)))
                                            foreach (Audio audio in counters.Audios.Where(i => i.Id == id).ToList())
                                                temp_text += $"   Removed audio: { audio.Artist } - { audio.Title }{ Environment.NewLine }";
                                    changed = true;
                                    counters.Audios = Audios;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at audios check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckAudioAlbums.Checked)
                        {
                            try
                            {
                                VkCollection<AudioPlaylist> Playlists = api.Audio.GetPlaylists(user_id);
                                if (counters.Playlists == null) counters.Playlists = Playlists;
                                else if ((int)counters.Playlists.TotalCount != (int)Playlists.TotalCount)
                                {
                                    temp_text += $"{ CurrentTime }, Update in playlists: { counters.Playlists.TotalCount }  to { Playlists.TotalCount }{ Environment.NewLine }";
                                    if ((int)counters.Playlists.TotalCount < (int)Playlists.TotalCount)
                                        foreach (var id in Playlists.Select(x => x.Id).Except(counters.Playlists.Select(x => x.Id)))
                                            foreach (AudioPlaylist playlist in Playlists.Where(i => i.Id == id).ToList())
                                                temp_text += $"   New playlist: { playlist.Title }{ Environment.NewLine }"
                                                    + $"   Description: { playlist.Description }{ Environment.NewLine }"
                                                    + $"   Link: vk.com/music/playlist/{playlist.OwnerId}_{playlist.Id}{ Environment.NewLine }";
                                    else foreach (var id in counters.Playlists.Select(x => x.Id).Except(Playlists.Select(x => x.Id)))
                                            foreach (AudioPlaylist playlist in counters.Playlists.Where(i => i.Id == id).ToList())
                                                temp_text += $"   Removed playlist: { playlist.Title }{ Environment.NewLine }"
                                                    + $"   Description: { playlist.Description }{ Environment.NewLine }"
                                                    + $"   Link: vk.com/music/playlist/{playlist.OwnerId}_{playlist.Id}{ Environment.NewLine }";
                                    changed = true;
                                    counters.Playlists = Playlists;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at playlists check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckGroups.Checked && counters.ApiCounters[0].Counters.Groups != null && ApiCounters[0].Counters.Groups != null && (counters.Groups == null || (int)counters.ApiCounters[0].Counters.Groups != (int)ApiCounters[0].Counters.Groups || (int)counters.ApiCounters[0].Counters.Pages != (int)ApiCounters[0].Counters.Pages))
                        {
                            try
                            {
                                VkCollection<VkNet.Model.Group> Groups = api.Users.GetSubscriptions(user_id);
                                if (counters.Groups == null) counters.Groups = Groups;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in subscpritions: { counters.ApiCounters[0].Counters.Groups }  to { ApiCounters[0].Counters.Groups }{ Environment.NewLine }";
                                    if ((int)counters.ApiCounters[0].Counters.Groups < (int)ApiCounters[0].Counters.Groups)
                                        foreach (var id in Groups.Select(x => x.Id).Except(counters.Groups.Select(x => x.Id)))
                                            foreach (VkNet.Model.Group group in Groups.Where(i => i.Id == id).ToList())
                                                temp_text += $"   New subscription: { group.Name }{ Environment.NewLine }" +
                                                    $"   Type: {group.Type}{Environment.NewLine}" +
                                                    $"   Link: vk.com/public{ group.Id }{ Environment.NewLine }";
                                    else foreach (var id in counters.Groups.Select(x => x.Id).Except(Groups.Select(x => x.Id)))
                                            foreach (VkNet.Model.Group group in counters.Groups.Where(i => i.Id == id).ToList())
                                                temp_text += $"   Removed subscription: { group.Name }{ Environment.NewLine }" +
                                                    $"   Type: {group.Type}{Environment.NewLine}" +
                                                    $"   Link: vk.com/public{ group.Id }{ Environment.NewLine }";
                                    changed = true;
                                    counters.Groups = Groups;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at subscriptions check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckPhotos.Checked && counters.ApiCounters[0].Counters.Photos != null && ApiCounters[0].Counters.Photos != null && (counters.Photos == null || (int)counters.ApiCounters[0].Counters.Photos != (int)ApiCounters[0].Counters.Photos))
                        {
                            try
                            {
                                VkCollection<Photo> Photos = api.Photo.GetAll(new PhotoGetAllParams { OwnerId = user_id, NeedHidden = true }); //default count = 20, be aware of it
                                if (counters.Photos == null) counters.Photos = Photos;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in photos: { counters.ApiCounters[0].Counters.Photos }  to { ApiCounters[0].Counters.Photos }{ Environment.NewLine }";
                                    if ((int)counters.ApiCounters[0].Counters.Photos < (int)ApiCounters[0].Counters.Photos)
                                        foreach (var id in Photos.Select(x => x.Id).Except(counters.Photos.Select(x => x.Id)))
                                            foreach (Photo photo in Photos.Where(i => i.Id == id).ToList())
                                            {
                                                temp_text += $"   New photo: { photo.Sizes[photo.Sizes.Count - 1].Url }{ Environment.NewLine }"
                                                    + $"   Title: { photo.Text }{ Environment.NewLine }";
                                                if (DownloadFiles.Checked) temp_text += $"{ DownloadByLink(photo.Sizes[photo.Sizes.Count - 1].Url) }";
                                            }
                                    else foreach (var id in counters.Photos.Select(x => x.Id).Except(Photos.Select(x => x.Id)))
                                            foreach (Photo photo in counters.Photos.Where(i => i.Id == id).ToList())
                                            {
                                                temp_text += $"   Removed photo: { photo.Sizes[photo.Sizes.Count - 1].Url }{ Environment.NewLine }"
                                                    + $"   Title: { photo.Text }{ Environment.NewLine }";
                                                if (DownloadFiles.Checked) temp_text += $"{ DownloadByLink(photo.Sizes[photo.Sizes.Count - 1].Url) }";
                                            }
                                    changed = true;
                                    counters.Photos = Photos;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at photos check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckGifts.Checked && counters.ApiCounters[0].Counters.Gifts != null && ApiCounters[0].Counters.Gifts != null && (counters.Gifts == null || (int)counters.ApiCounters[0].Counters.Gifts != (int)ApiCounters[0].Counters.Gifts))
                        {
                            try
                            {
                                VkCollection<GiftItem> Gifts = api.Gifts.Get(user_id);
                                if (counters.Gifts == null) counters.Gifts = Gifts;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in gifts: { counters.ApiCounters[0].Counters.Gifts }  to { ApiCounters[0].Counters.Gifts }{ Environment.NewLine }";
                                    if ((int)counters.ApiCounters[0].Counters.Gifts < (int)ApiCounters[0].Counters.Gifts)
                                        foreach (var id in Gifts.Select(x => x.Id).Except(counters.Gifts.Select(x => x.Id)))
                                            foreach (GiftItem gift in Gifts.Where(i => i.Id == id).ToList())
                                                temp_text += $"   New gift from user: vk.com/id{ gift.FromId }{ Environment.NewLine }"
                                                + $"   With message: { gift.Message }{ Environment.NewLine }"
                                                + $"   Thumbnail: { gift.Gift.Thumb256 }{ Environment.NewLine }";
                                    else foreach (var id in counters.Gifts.Select(x => x.Id).Except(Gifts.Select(x => x.Id)))
                                            foreach (GiftItem gift in counters.Gifts.Where(i => i.Id == id).ToList())
                                                temp_text += $"   Removed gift from user: vk.com/id{ gift.FromId }{ Environment.NewLine }"
                                                + $"   With message: { gift.Message }{ Environment.NewLine }"
                                                + $"   Thumbnail: { gift.Gift.Thumb256 }{ Environment.NewLine }";
                                    changed = true;
                                    counters.Gifts = Gifts;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at gifts check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckVideos.Checked && counters.ApiCounters[0].Counters.Videos != null && ApiCounters[0].Counters.Videos != null && (counters.Videos == null || (int)counters.ApiCounters[0].Counters.Videos != (int)ApiCounters[0].Counters.Videos))
                        {
                            try
                            {
                                VkCollection<Video> Videos = api.Video.Get(new VideoGetParams { OwnerId = user_id });
                                if (counters.Videos == null) counters.Videos = Videos;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in videos: { counters.Videos.Count }  to { Videos.Count }{ Environment.NewLine }";
                                    if ((int)counters.ApiCounters[0].Counters.Videos < (int)ApiCounters[0].Counters.Videos)
                                        foreach (var id in Videos.Select(x => x.Id).Except(counters.Videos.Select(x => x.Id)))
                                            foreach (Video video in Videos.Where(i => i.Id == id).ToList())
                                            {
                                                temp_text += $"   New video: { video.Title }{ Environment.NewLine }   Description: { video.Description }{ Environment.NewLine }"
                                                + $"   Link: { video.Player }{ Environment.NewLine }"
                                                + $"   Video ID & Owner ID: { video.Id } & { video.OwnerId }{ Environment.NewLine }";
                                                if (DownloadFiles.Checked) temp_text += DownloadByLink(video.Player);
                                            }
                                    else foreach (var id in counters.Videos.Select(x => x.Id).Except(Videos.Select(x => x.Id)))
                                            foreach (Video video in counters.Videos.Where(i => i.Id == id).ToList())
                                            {
                                                temp_text += $"   Removed video: { video.Title }{ Environment.NewLine }   Description: { video.Description }{ Environment.NewLine }"
                                                     + $"   Link: { video.Player }{ Environment.NewLine }"
                                                     + $"   Video ID & Owner ID: { video.Id } & { video.OwnerId }{ Environment.NewLine }";
                                                if (DownloadFiles.Checked) temp_text += DownloadByLink(video.Player);
                                            }
                                    changed = true;
                                    counters.Videos = Videos;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at videos check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckFriends.Checked && counters.ApiCounters[0].Counters.Friends != null && ApiCounters[0].Counters.Friends != null && (counters.Friends == null || (int)counters.ApiCounters[0].Counters.Friends != (int)ApiCounters[0].Counters.Friends))
                        {
                            try
                            {
                                VkCollection<User> Friends = api.Friends.Get(new FriendsGetParams { UserId = user_id, Fields = ProfileFields.FirstName | ProfileFields.LastName });
                                if (counters.Friends == null) counters.Friends = Friends;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in friends: { counters.ApiCounters[0].Counters.Friends }  to { ApiCounters[0].Counters.Friends }{ Environment.NewLine }";
                                    if ((int)counters.ApiCounters[0].Counters.Friends < (int)ApiCounters[0].Counters.Friends)
                                        foreach (var id in Friends.Select(x => x.Id).Except(counters.Friends.Select(x => x.Id)))
                                            foreach (User friend in Friends.Where(i => i.Id == id).ToList())
                                                temp_text += $"   New friend: {friend.FirstName} {friend.LastName}{ Environment.NewLine }"
                                                    + $"   Link: vk.com/id{friend.Id}{ Environment.NewLine }";
                                    else foreach (var id in counters.Friends.Select(x => x.Id).Except(Friends.Select(x => x.Id)))
                                            foreach (User friend in counters.Friends.Where(i => i.Id == id).ToList())
                                                temp_text += $"   Removed friend: {friend.FirstName} {friend.LastName}{ Environment.NewLine }"
                                                    + $"   Link: vk.com/id{friend.Id}{ Environment.NewLine }";
                                    changed = true;
                                    counters.Friends = Friends;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at friends check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckFollowers.Checked && counters.ApiCounters[0].Counters.Followers != null && ApiCounters[0].Counters.Followers != null && (counters.Followers == null || (int)counters.ApiCounters[0].Counters.Followers != (int)ApiCounters[0].Counters.Followers))
                        {
                            try
                            {
                                VkCollection<User> Followers = api.Users.GetFollowers(user_id);
                                if (counters.Followers == null) counters.Followers = Followers;
                                else
                                {
                                    temp_text += $"{ CurrentTime }, Update in followers: { counters.ApiCounters[0].Counters.Followers }  to { ApiCounters[0].Counters.Followers }{ Environment.NewLine }";
                                    if ((int)counters.ApiCounters[0].Counters.Followers < (int)ApiCounters[0].Counters.Followers)
                                        foreach (var id in Followers.Select(x => x.Id).Except(counters.Followers.Select(x => x.Id)))
                                            foreach (User follower in Followers.Where(i => i.Id == id).ToList())
                                                temp_text += $"   New follower: {follower.FirstName} {follower.LastName}{Environment.NewLine}"
                                                    + $"   Link: vk.com/id{follower.Id}{ Environment.NewLine }";
                                    else foreach (var id in counters.Followers.Select(x => x.Id).Except(Followers.Select(x => x.Id)))
                                            foreach (User follower in counters.Followers.Where(i => i.Id == id).ToList())
                                                temp_text += $"   Removed follower: {follower.FirstName} {follower.LastName}{Environment.NewLine}"
                                                    + $"   Link: vk.com/id{follower.Id}{ Environment.NewLine }";
                                    changed = true;
                                    counters.Followers = Followers;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at followers check, {e.Message}{Environment.NewLine}"); }
                        }

                        if (CheckStatus.Checked && counters.ApiCounters[0].Status != null && ApiCounters[0].Status != null && (counters.Status == null || counters.ApiCounters[0].Status != ApiCounters[0].Status))
                        {
                            try
                            {
                                var Status = api.Status.Get(user_id);
                                if (counters.Status == null) counters.Status = Status;
                                else
                                {
                                    if (counters.Status.Audio == null && Status.Audio != null)
                                        temp_text += $"{ CurrentTime }, Started listening to music: { Status.Audio.Artist } - {  Status.Audio.Title }{ Environment.NewLine }";
                                    else if (counters.Status.Audio != null && Status.Audio != null && counters.Status.Audio.Id != Status.Audio.Id)
                                        temp_text += $"{ CurrentTime }, New track: { Status.Audio.Artist } - { Status.Audio.Title }{ Environment.NewLine }";
                                    else if (counters.Status.Audio != null && Status.Audio == null)
                                        temp_text += $"{ CurrentTime }, Stopped listening to music{ Environment.NewLine }";
                                    else
                                        temp_text += $"{ CurrentTime }, Update in status: old '{ counters.Status.Text }', new '{ Status.Text }'{ Environment.NewLine }";
                                    changed = true;
                                    counters.Status = Status;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at status check, {e.Message}{Environment.NewLine}"); }
                        }
                        /*
                        if (CheckPosts.Checked && posts_timeout)
                        {
                            //try
                            //{
                            WallGetObject Wall = api.Wall.Get(new WallGetParams { OwnerId = user_id, Extended = true, Count = 100 });
                            if (counters.Wall == null)
                            {
                                foreach (Post post in Wall.WallPosts)
                                {
                                    VkCollection<long> Likes = api.Likes.GetList(new LikesGetListParams { Type = LikeObjectType.Post, OwnerId = user_id, ItemId = (long)post.Id });
                                    counters.PostsWithLikesList.Add(new Counters.PostWithLikes { WallPost = post, PostLikes = Likes });
                                }
                                counters.Wall = Wall;
                            }
                            else
                            {
                                if (counters.Wall.TotalCount != Wall.TotalCount)
                                {
                                    temp_text += $"{ CurrentTime }, Update in posts: { counters.Wall.TotalCount }  to { Wall.TotalCount }{ Environment.NewLine }";
                                    if ((int)counters.Wall.TotalCount < (int)Wall.TotalCount)
                                    {
                                        temp_text += $"   New post:{Environment.NewLine}";//   Post author: vk.com/id{post.OwnerId}{ Environment.NewLine }";
                                        counters.PostsWithLikesList.Add()
                                    }
                                    else
                                    {
                                        temp_text += $"   Removed post:{Environment.NewLine}";//   Post author: vk.com/id{post.OwnerId}{ Environment.NewLine }";
                                    }
                                    changed = true;
                                }
                                foreach (Post post in Wall.WallPosts)//(Counters.PostWithLikes postwithlikes in counters.PostsWithLikesList)
                                {
                                    Counters.PostWithLikes OldPost = counters.PostsWithLikesList.FirstOrDefault(Post => Post.WallPost.Id == post.Id);
                                    if (OldPost.PostLikes.Count != post.Likes.Count)
                                    {
                                        var Likes = api.Likes.GetList(new LikesGetListParams { Type = LikeObjectType.Post, OwnerId = user_id, ItemId = (long)post.Id });
                                        if (OldPost.PostLikes.Count < post.Likes.Count)
                                            foreach (long like in Likes.Except(OldPost.PostLikes).ToList())
                                                temp_text += $"   New like on post: { post.Id }{Environment.NewLine}"
                                                    + $"   Like is from: vk.com/id{ like }{ Environment.NewLine }";
                                        else foreach (long like in OldPost.PostLikes.Except(Likes).ToList())
                                                temp_text += $"   User removed like on post: { post.Id }{Environment.NewLine}"
                                                    + $"   Like was from: vk.com/id{ like }{ Environment.NewLine }";
                                    //counters.PostsWithLikesList[counters.PostsWithLikesList.FindIndex(obj => obj.WallPost == post)] = new Counters.PostWithLikes { PostLikes = Likes, WallPost = OldPost.WallPost };
                                        changed = true;
                                    }
                                }
                                counters.Wall = Wall;
                            }
                            //}
                            //catch (Exception e) { WriteToFile($"Exception thrown at posts check, {e.Message}{Environment.NewLine}"); }
                            }
                        /* 

                         //.Select(x => x.Id).Except(counters.Wall.WallPosts.Select(x => x.Id)))
                         foreach (Post post in Wall.WallPosts.Where(i => i.Id == post).ToList())
                     {
                         temp_text += $"   New post:{Environment.NewLine}   Post author: vk.com/id{post.OwnerId}{ Environment.NewLine }";
                         if (post.Text != string.Empty)
                             temp_text += $"   Post text: {post.Text}{ Environment.NewLine }";
                         if (post.Attachments != null)

                         }
                            }
                            else
                            {
                                temp_text += $"{ CurrentTime }, Update in posts: { counters.Wall.TotalCount }  to { Wall.TotalCount }{ Environment.NewLine }";
                                foreach (var id in Wall.WallPosts.Select(x => x.Id).Except(counters.Wall.WallPosts.Select(x => x.Id)))
                                    foreach (Post post in Wall.WallPosts.Where(i => i.Id == id).ToList())
                                    {
                                        temp_text += $"   New post:{Environment.NewLine}   Post author: vk.com/id{post.OwnerId}{ Environment.NewLine }";
                                        if (post.Text != string.Empty)
                                            temp_text += $"   Post text: {post.Text}{ Environment.NewLine }";
                                        if (post.Attachments != null)

                                    }
                            }
                            if (counters.Wall.TotalCount < Wall.TotalCount)
                            {


                            }
                            else if
                            {
                                foreach (var id in Followers.Select(x => x.Id).Except(counters.Followers.Select(x => x.Id)))
                                    foreach (User follower in Followers.Where(i => i.Id == id).ToList())
                                        temp_text += $"   New follower: {follower.FirstName} {follower.LastName}{Environment.NewLine}"
                                            + $"   Link: vk.com/id{follower.Id}{ Environment.NewLine }";
                            }
                            changed = true;
                            counters.Wall = Wall;
                        }

                        Thread.Sleep(350);
                        Get($"{ApiRequestLink}wall.get?owner_id={user_id}&extended=1&access_token={AuthToken}&v={ApiVersion}");
                        JObject posts = JObject.Parse(server_response);
                        var postsresponse = posts["response"];
                        if (counters.posts_response == null) counters.posts_response = (JObject)postsresponse;
                        else
                        {

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
                                temp_text += $"   Removed post:{Environment.NewLine}   Post author: vk.com/id{(string)post["from_id"]}{ Environment.NewLine }";
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
                        posts_timeout = false;
                        catch (Exception e) { WriteToFile($"Exception thrown at posts, probably too many requests were made, {e.Message}{Environment.NewLine}"); BeginInvoke(new MethodInvoker(delegate { CheckPosts.Checked = false; })); }
                        
                    
                        if (CheckStories.Checked && stories_timeout)
                        {
                            try
                            {
                                StoryResult<IEnumerable<Story>> Stories = api.Stories.Get(user_id);
                                if (counters.Stories == null) counters.Stories = Stories;
                                else
                                {
                                    if (counters.Stories.Items.Count() > 0 && Stories.Items.Count() > 0)
                                    {
                                        var OldStoryList = counters.Stories.Items.ToList()[0].ToList();
                                        var NewStoryList = Stories.Items.ToList()[0].ToList();
                                        if (OldStoryList.Count() != NewStoryList.Count())
                                        {
                                            temp_text += $"{ CurrentTime }, Update in stories: { OldStoryList.Count() }  to { NewStoryList.Count() }{ Environment.NewLine }";
                                            changed = true;
                                        }
                                        if (OldStoryList.Count() < NewStoryList.Count())
                                            foreach (var StoryItem in NewStoryList)
                                                if (!OldStoryList.Contains(StoryItem))
                                                    if (StoryItem.Type == StoryType.Photo)
                                                    {
                                                        temp_text += $"   New story: {StoryItem.Photo.Sizes[StoryItem.Photo.Sizes.Count - 1].Url}{Environment.NewLine}";
                                                        if (DownloadFiles.Checked) DownloadByLink(StoryItem.Photo.Sizes[StoryItem.Photo.Sizes.Count - 1].Url);
                                                    }
                                                    else
                                                    {
                                                        temp_text += $"   New story: {StoryItem.Video.Player}{Environment.NewLine}";
                                                        if (DownloadFiles.Checked) DownloadByLink(StoryItem.Video.Player);
                                                    }
                                    }
                                    counters.Stories = Stories;
                                }
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at stories check, {e.Message}{Environment.NewLine}"); }
                        }
                         */
                        if (CheckStories.Checked && stories_timeout)
                        {
                            try
                            {
                                Thread.Sleep(350);
                                string server_response = new WebClient { Encoding = Encoding.UTF8 }.DownloadString($"https://api.vk.com/method/stories.get?owner_id={user_id}&access_token={api.Token}&v=5.126");
                                JObject stories = JObject.Parse(server_response);
                                var storiesresponse = stories["response"];
                                if (counters.stories_response == null) counters.stories_response = (JObject)storiesresponse;
                                else
                                {
                                    if ((int)counters.stories_response["count"] > 0 && (int)storiesresponse["count"] > 0)
                                        if (counters.stories_response["items"][0]["stories"].Count() != storiesresponse["items"][0]["stories"].Count())
                                        {
                                            temp_text += $"{ CurrentTime }, Update in stories: { counters.stories_response["items"][0]["stories"].Count() }  to { storiesresponse["items"][0]["stories"].Count() }{ Environment.NewLine }";
                                            if (counters.stories_response["items"][0]["stories"].Count() < storiesresponse["items"][0]["stories"].Count())
                                            {
                                                List<string> OldStories = new List<string>();
                                                foreach (var story in counters.stories_response["items"][0]["stories"]) OldStories.Add((string)story["id"]);
                                                foreach (var story in storiesresponse["items"][0]["stories"])
                                                    if (!OldStories.Contains((string)story["id"]))
                                                    {
                                                        switch ((string)story["type"])
                                                        {
                                                            case "photo":
                                                                {
                                                                    temp_text += $"   New story: {story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"]}{Environment.NewLine}";
                                                                    if (DownloadFiles.Checked) DownloadByLink(new Uri((string)story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"]));
                                                                }
                                                                break;
                                                            case "video":
                                                                {
                                                                    temp_text += $"   New story: {story["video"]["player"]}{Environment.NewLine}";
                                                                    if (DownloadFiles.Checked) DownloadByLink(new Uri((string)story["video"]["player"]));
                                                                }
                                                                break;
                                                        }
                                                    }
                                            }
                                            else
                                            {
                                                List<string> NewStories = new List<string>();
                                                foreach (var story in storiesresponse["items"][0]["stories"]) NewStories.Add((string)story["id"]);
                                                foreach (var story in counters.stories_response["items"][0]["stories"])
                                                    if (!NewStories.Contains((string)story["id"]))
                                                    {
                                                        switch ((string)story["type"])
                                                        {
                                                            case "photo":
                                                                {
                                                                    temp_text += $"   Removed story: {story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"]}{Environment.NewLine}";
                                                                    if (DownloadFiles.Checked) DownloadByLink(new Uri((string)story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"]));
                                                                }
                                                                break;
                                                            case "video":
                                                                {
                                                                    temp_text += $"   Removed story: {story["video"]["player"]}{Environment.NewLine}";
                                                                    if (DownloadFiles.Checked) DownloadByLink(new Uri((string)story["video"]["player"]));
                                                                }
                                                                break;
                                                        }
                                                    }
                                            }
                                            changed = true;
                                        }
                                        else { }
                                    else if ((int)counters.stories_response["count"] == 0 && (int)storiesresponse["count"] == 1)
                                    {
                                        temp_text += $"{ CurrentTime }, Update in stories: 0  to { storiesresponse["items"][0]["stories"].Count() }{ Environment.NewLine }";
                                        foreach (var story in storiesresponse["items"][0]["stories"])
                                            switch ((string)story["type"])
                                            {
                                                case "photo":
                                                    {
                                                        temp_text += $"   New story: {story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"]}{Environment.NewLine}";
                                                        if (DownloadFiles.Checked) DownloadByLink(new Uri((string)story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"]));
                                                    }
                                                    break;
                                                case "video":
                                                    {
                                                        temp_text += $"   New story: {story["video"]["player"]}{Environment.NewLine}";
                                                        if (DownloadFiles.Checked) DownloadByLink(new Uri((string)story["video"]["player"]));
                                                    }
                                                    break;
                                            }
                                        changed = true;
                                    }
                                    else if ((int)counters.stories_response["count"] == 1 && (int)storiesresponse["count"] == 0)
                                    {
                                        temp_text += $"{ CurrentTime }, Update in stories: { counters.stories_response["items"][0]["stories"].Count() }  to 0{ Environment.NewLine }";
                                        foreach (var story in counters.stories_response["items"][0]["stories"])
                                            switch ((string)story["type"])
                                            {
                                                case "photo":
                                                    {
                                                        temp_text += $"   Removed story: {story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"]}{Environment.NewLine}";
                                                        if (DownloadFiles.Checked) DownloadByLink(new Uri((string)story["photo"]["sizes"][story["photo"]["sizes"].Count() - 1]["url"]));
                                                    }
                                                    break;
                                                case "video":
                                                    {
                                                        temp_text += $"   Removed story: {story["video"]["player"]}{Environment.NewLine}";
                                                        if (DownloadFiles.Checked) DownloadByLink(new Uri((string)story["video"]["player"]));
                                                    }
                                                    break;
                                            }
                                        changed = true;
                                    }
                                    counters.stories_response = (JObject)storiesresponse;
                                }
                                stories_timeout = false;
                            }
                            catch (Exception e) { WriteToFile($"Exception thrown at stories check, {e.Message}{Environment.NewLine}"); }
                        }
                    }

                    if (changed) EventLogs.Invoke((MethodInvoker)delegate
                    {
                        if (EventLogs.Text[EventLogs.Text.Length - 1] == '—') temp_text = $"  { OnlineDateTime }{ Environment.NewLine }" + temp_text;
                        WriteToFile(temp_text);
                        if ((bool)counters.ApiCounters[0].Online && (bool)ApiCounters[0].Online) counters.ApiCounters[0].Online = false;
                        changed = false;
                    });

                    if ((bool)counters.ApiCounters[0].Online != (bool)ApiCounters[0].Online) //if 'online status' changed
                    {
                        if (!(bool)counters.ApiCounters[0].Online && (bool)ApiCounters[0].Online) //if offline to online transition happened
                            if (CheckOnlineDevice.Checked)
                                if (OnlineApp != string.Empty) WriteToFile($"Online { ApiCounters[0].LastSeen.Platform }, { OnlineApp }: { OnlineDateTime }   —");
                                else WriteToFile($"Online { ApiCounters[0].LastSeen.Platform }: { OnlineDateTime }   —");
                            else WriteToFile($"Online: { OnlineDateTime }   —");
                        else WriteToFile($"  { OnlineDateTime }{ Environment.NewLine }"); //if transition online to offline happened
                    }
                    counters.ApiCounters = ApiCounters;
                }      
            }
        }
        private void WriteToFile(string text)
        {
            string[] str = File.ReadAllLines(System.Windows.Forms.Application.StartupPath + $"\\Logs\\{user_id}.txt");
            EventLogs.Invoke((MethodInvoker)delegate
            {
                EventLogs.AppendText(text);
            });
            if (Logs.Checked) File.AppendAllText(System.Windows.Forms.Application.StartupPath + $"\\Logs\\{user_id}.txt", text);
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
                    CheckAudioAlbums.Checked = CheckAudioAlbums.Enabled =
                    CheckFollowers.Checked = CheckFollowers.Enabled =
                    CheckGifts.Checked = CheckGifts.Enabled =
                    CheckPhotos.Checked = CheckPhotos.Enabled =
                    CheckPosts.Checked = CheckPosts.Enabled =
                    CheckGroups.Checked = CheckGroups.Enabled =
                    CheckVideos.Checked = CheckVideos.Enabled = 
                    CheckStories.Checked = CheckStories.Enabled =
                    CheckFriends.Checked = CheckFriends.Enabled = false;
                }));
            }
            else
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    CheckAlbums.Enabled = CheckAudios.Enabled =
                    CheckFollowers.Enabled = CheckGifts.Enabled = 
                    CheckPhotos.Enabled = CheckPosts.Enabled = 
                    CheckGroups.Enabled = CheckVideos.Enabled =
                    CheckStories.Enabled = CheckFriends.Enabled = 
                    CheckAudioAlbums.Enabled = true;
                }));
            }
        }
        private void FriendsIdSelected(object sender, EventArgs e)
        {
            if (FriendsIdList.Count > 0 && ListOfIDs.SelectedIndex < FriendsIdList.Count) user_id = Convert.ToInt32(FriendsIdList[ListOfIDs.SelectedIndex]);
            else if (CustomIdList.Count > 0 && ListOfIDs.SelectedIndex >= FriendsIdList.Count) user_id = Convert.ToInt32(CustomIdList[ListOfIDs.SelectedIndex - FriendsIdList.Count]);
            if (Logs.Checked)
                if (File.Exists(System.Windows.Forms.Application.StartupPath + $"\\Logs\\{user_id}.txt")) EventLogs.Invoke((MethodInvoker)delegate { EventLogs.Text = string.Empty; EventLogs.AppendText(File.ReadAllText(System.Windows.Forms.Application.StartupPath + $"\\Logs\\{user_id}.txt")); });
                else
                {
                    if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Logs")) Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Logs");
                    File.AppendAllText(System.Windows.Forms.Application.StartupPath + $"\\Logs\\{user_id}.txt", $"Log file for user id{user_id}{Environment.NewLine}");
                    EventLogs.Text = string.Empty;
                    EventLogs.AppendText(File.ReadAllText(System.Windows.Forms.Application.StartupPath + $"\\Logs\\{user_id}.txt"));
                }
        }
        private void CustomUserIdEntered(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && int.TryParse(CustomIdInput.Text, out int id))
            {
                try
                {
                    ReadOnlyCollection<User> CustomUsers = api.Users.Get(new List<long> { id });
                    if (!CustomIdList.Contains(id.ToString()) && !FriendsIdList.Contains(id.ToString()))
                    {
                        CustomIdList.Add(id.ToString());
                        ListOfIDs.Items.Add($"{ CustomUsers[0].FirstName } { CustomUsers[0].LastName }");
                        CustomIdInput.Text = string.Empty;
                    }
                }
                catch (Exception ex) { WriteToFile($"Exception thrown while adding custom user, {ex.Message}{Environment.NewLine}"); }
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
            EventLogs.Text = File.ReadAllText(System.Windows.Forms.Application.StartupPath + $"\\Logs\\{user_id}.txt");
        }
        private void Stop()
        {
            StartStopLabel.Text = "Start";
            LastOnlineLabel.Text = "Last Online";
            if (EventLogs.Text[EventLogs.Text.Length - 1] == '—' && Logs.Checked)
                File.AppendAllText(System.Windows.Forms.Application.StartupPath + $"\\Logs\\{user_id}.txt", $"  { OnlineDateTime }{ Environment.NewLine }");
            ListOfIDs.Enabled = CustomIdInput.Enabled = Logs.Enabled = true;
            RestrictToggles(false);
        }
        private void PlaceSplitClicked(object sender, EventArgs e)
        {
            if (Logs.Checked)
                if (EventLogs.Text[EventLogs.Text.Length - 1] == '\n')
                    WriteToFile($"———————————————————————————————{ Environment.NewLine }");
                else WriteToFile($"{ Environment.NewLine }———————————————————————————————{ Environment.NewLine }");
        }
        private void StalkerTrayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) { Show(); WindowState = FormWindowState.Normal; }
            else if (WindowState == FormWindowState.Normal) WindowState = FormWindowState.Minimized;
        }
        private void OpenSettings_CheckedChanged(object sender, EventArgs e)
        {
            if (OpenSettings.Checked) Size = new System.Drawing.Size(586, WindowHeight);
            else Size = new System.Drawing.Size(452, WindowHeight);
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
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
            foreach (KeyValuePair<string, string> pair in settingslist) configuration.AppSettings.Settings[pair.Key].Value = pair.Value;
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }
        private void CustomAppIdToggled(object sender, EventArgs e)
        {
            if (CustomAppIdToggle.Checked) CustomAppId.Enabled = true;
            else CustomAppId.Enabled = false;
        }
        private void StartOnBootChecked(object sender, EventArgs e)
        {
            if (StartOnBoot.Checked) AutostartOnBoot.SetValue("Stalker", System.Windows.Forms.Application.ExecutablePath);
            else AutostartOnBoot.DeleteValue("Stalker", false);
        }
        private List<KeyValuePair<string, string>> SettingsList()
        {
            string temp_ids = string.Empty;
            if (CustomIdList.Count > 0)
            {
                foreach (string id in CustomIdList) temp_ids = temp_ids + id + ",";
                temp_ids = temp_ids.TrimEnd(',');
            }
            string savetoken = string.Empty;
            if (SaveToken.Checked) savetoken = api.Token;
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
                new KeyValuePair<string, string>("AudioAlbums", CheckAudioAlbums.Checked.ToString()),
                new KeyValuePair<string, string>("Followers", CheckFollowers.Checked.ToString()),
                new KeyValuePair<string, string>("Friends", CheckFriends.Checked.ToString()),
                new KeyValuePair<string, string>("Gifts", CheckGifts.Checked.ToString()),
                new KeyValuePair<string, string>("Photos", CheckPhotos.Checked.ToString()),
                new KeyValuePair<string, string>("Posts", CheckPosts.Checked.ToString()),
                new KeyValuePair<string, string>("Status", CheckStatus.Checked.ToString()),
                new KeyValuePair<string, string>("Stories", CheckStories.Checked.ToString()),
                new KeyValuePair<string, string>("Subscriptions", CheckGroups.Checked.ToString()),
                new KeyValuePair<string, string>("Videos", CheckVideos.Checked.ToString()),
                new KeyValuePair<string, string>("OpenSettings", OpenSettings.Checked.ToString()),
                new KeyValuePair<string, string>("SelectedUser", ListOfIDs.SelectedIndex.ToString()),
                new KeyValuePair<string, string>("Autostart", Autostart.Checked.ToString()),
                new KeyValuePair<string, string>("CustomAppIdToggle", CustomAppIdToggle.Checked.ToString()),
                new KeyValuePair<string, string>("CustomAppId", CustomAppId.Text),
                new KeyValuePair<string, string>("DownloadFiles", DownloadFiles.Checked.ToString()),
                new KeyValuePair<string, string>("StartOnBoot", StartOnBoot.Checked.ToString()),
                new KeyValuePair<string, string>("DetailedLogs", DetailedLogs.Checked.ToString()),
            };
            return settingslist;
        }
        private void LoadSettings()
        {
            Size = new System.Drawing.Size(452, WindowHeight);
            SystemEvents.PowerModeChanged += OnPowerChange;
            Logs.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Logs"]);
            PreventSleep.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["NoSleep"]);
            CheckOnlineDevice.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["OnlineDevice"]);
            CheckAlbums.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Albums"]);
            CheckAudios.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Audios"]);
            CheckAudioAlbums.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["AudioAlbums"]);
            CheckFollowers.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Followers"]);
            CheckFriends.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Friends"]);
            CheckGifts.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Gifts"]);
            CheckPhotos.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Photos"]);
            CheckPosts.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Posts"]);
            CheckStatus.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Status"]);
            CheckStories.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Stories"]);
            CheckGroups.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Subscriptions"]);
            CheckVideos.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Videos"]);
            OpenSettings.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["OpenSettings"]);
            SaveToken.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["SaveToken"]);
            DownloadFiles.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["DownloadFiles"]);
            CustomAppIdToggle.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["CustomAppIdToggle"]);
            CustomAppId.Text = ConfigurationManager.AppSettings["CustomAppId"];
            StartOnBoot.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["StartOnBoot"]);
            DetailedLogs.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["DetailedLogs"]);
            Autostart.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["Autostart"]);
            if (ConfigurationManager.AppSettings["CustomIDs"] != String.Empty)
                customids = ConfigurationManager.AppSettings["CustomIDs"].Split(',');
            if (SaveToken.Checked && ConfigurationManager.AppSettings["AuthToken"] != "")
                LogInVK(ConfigurationManager.AppSettings["AuthToken"]);
            else LogInVK(string.Empty);
            FetchFriends();
            int index = Convert.ToInt32(ConfigurationManager.AppSettings["SelectedUser"]);
            if (ListOfIDs.Items.Count > index) ListOfIDs.SelectedIndex = index;
            if (Autostart.Checked) StartStop.Checked = true;
            if (StartOnBoot.Checked) AutostartOnBoot.SetValue("Stalker", System.Windows.Forms.Application.ExecutablePath);
            else AutostartOnBoot.DeleteValue("Stalker", false);
        }
        public Stalker()
        {
            InitializeComponent();
            LoadSettings();
        }
    }
    public class Counters
    {
        public DateTime LastOnlineTime = DateTime.Now;
        public ReadOnlyCollection<User> ApiCounters = null;
        public VkCollection<PhotoAlbum> Albums = null;
        public VkCollection<Audio> Audios = null;
        public VkCollection<AudioPlaylist> Playlists = null;
        public VkCollection<VkNet.Model.Group> Groups = null;
        public VkCollection<Photo> Photos = null;
        public VkCollection<GiftItem> Gifts = null;
        public VkCollection<Video> Videos = null;
        public VkCollection<User> Friends = null;
        public VkCollection<User> Followers = null;
        public StoryResult<IEnumerable<Story>> Stories = null;
        public Status Status = null;
        public string OnlineApp = null;
        public JObject stories_response = null;
        public WallGetObject Wall = null;
        public List<PostWithLikes> PostsWithLikesList = new List<PostWithLikes>();
        public struct PostWithLikes
        {
            public Post WallPost;
            public VkCollection<long> PostLikes;
        }
    }
    
}
