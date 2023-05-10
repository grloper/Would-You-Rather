using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Android.Widget;
using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.App;
using Firebase.Database.Streaming;
using AndroidX.AppCompat.App;
using AlertDialog = Android.App.AlertDialog;
using Java.Interop;
using System.Security.Cryptography.Xml;
using System.Drawing.Imaging;
using WouldYouRather.Classes;
using Java.Lang;

namespace WouldYouRather
{
    [Activity(Label = "QuestionAnswerOnlineActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class QuestionAnswerOnlineActivity : Activity
    {
        private SeekBar _seekBar;
        int progress = 0, time = 100;
        Runnable runnable;
        private Handler handler;
        private Button btnQ1, btnQ2;
        private IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "AY7hyv1qtUGKDSs2Gud5Ct3sSLX59KYZQZaGJBOq",
            BasePath = "https://eitanprojectwyr-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        private IFirebaseClient client;
        private int counter;
        private List<string> keysList;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.questoinansweronline_layout);
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {

                Toast.MakeText(this, "Network Error", ToastLength.Short).Show();
            }
#pragma warning disable CS0618 // Type or member is obsolete
            handler = new Handler();
#pragma warning restore CS0618 // Type or member is obsolete
            handler.PostDelayed(MyThread, 1000);
            btnQ1 = FindViewById<Button>(Resource.Id.online_btnQ1);
            btnQ2 = FindViewById<Button>(Resource.Id.online_btnQ2);
            counter = 0;
            btnQ1.Click += BtnQ1_Click;
            btnQ2.Click += BtnQ2_Click;
            FirebaseResponse res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/dicistionTimeSec");
            time = res.ResultAs<int>();
            time *= 10;
            _seekBar = FindViewById<SeekBar>(Resource.Id.seekBar);
            _seekBar.Max = 100;
            _seekBar.Enabled = false;
            runnable = new Runnable(UpdateSeekBar);
            _seekBar.ProgressChanged += (s, e) =>
            {
                if (e.FromUser)
                {
                    progress = e.Progress;
                }
            };
            handler.PostDelayed(runnable, 1000);
       

             res = await client.GetAsync(@"RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions");
            Dictionary<string, object> data = res.ResultAs<Dictionary<string, object>>();
            IEnumerable<string> qKeys = data.Keys;
            keysList = qKeys.ToList();
            FirebaseResponse result = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O1/Question");//get next qo1
            btnQ1.Text = result.ResultAs<string>();
            result = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O2/Question");//get next qo2
            btnQ2.Text = result.ResultAs<string>();
        }
        private async void UpdateSeekBar()//מעדכן את הווליום
        {
            progress++;
            _seekBar.Progress = progress;

            if (progress >= 100)
            {
                try
                {
                    counter++;
                    if (counter >= keysList.Count)
                    {
                        Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                        AlertDialog alert = dialog.Create();
                        alert.SetTitle("Wait for players to finish");
                        alert.SetMessage("Wait for other players to finish voting before proceeding.");
                        alert.Show();
                       FirebaseResponse res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/finishedPlayers");
                        await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/finishedPlayers", res.ResultAs<int>() + 1);
                    }
                    else
                    {
                        FirebaseResponse res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O1/Question");//get next qo1
                        btnQ1.Text = res.ResultAs<string>();
                        res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O2/Question");//get next qo2
                        btnQ2.Text = res.ResultAs<string>();
                        progress = 0;
                    }
                }
                catch (System.Exception)
                {

                }
                return;

            }

            handler.PostDelayed(runnable, time);
        }
        private async void BtnQ2_Click(object sender, EventArgs e)
        {
            try
            {
                progress = 0;
                FirebaseResponse res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O2/votes");//get the votes count
                await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O2/votes", res.ResultAs<int>() + 1);//set the new votes count
                counter++;
                if (counter >= keysList.Count)
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Wait for players to finish");
                    alert.SetMessage("Wait for other players to finish voting before proceeding.");
                    alert.Show();
                    res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/finishedPlayers");
                    await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/finishedPlayers", res.ResultAs<int>() + 1);
                }
                else
                {
                    res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O1/Question");//get next qo1
                    btnQ1.Text = res.ResultAs<string>();
                    res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O2/Question");//get next qo2
                    btnQ2.Text = res.ResultAs<string>();
                }
            }
            catch (System.Exception)
            {

            }
        }
        private bool isStopped = false;
        private async void BtnQ1_Click(object sender, EventArgs e)
        {
            try
            {
                progress = 0;
                FirebaseResponse res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O1/votes");//get the votes count
                await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O1/votes", res.ResultAs<int>() + 1);//set the new votes count
                counter++;
                if (counter >= keysList.Count)
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Wait for players to finish");
                    alert.SetMessage("Wait for other players to finish voting before proceeding.");
                    alert.Show();
                    res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/finishedPlayers");
                    await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/finishedPlayers", res.ResultAs<int>() + 1);

                }
                else
                {
                    res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O1/Question");//get next qo1
                    btnQ1.Text = res.ResultAs<string>();
                    res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + keysList[counter] + "/O2/Question");//get next qo2
                    btnQ2.Text = res.ResultAs<string>();
                }
            }
            catch (System.Exception)
            {

            }
        }
        private async void MyThread()//בודק אם כל המשתתפים סיימו לענות על השאלות
        {
            FirebaseResponse res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/numOfPlayers");
            int totalPlayers = res.ResultAs<int>();
            res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/finishedPlayers");
            int finishedPlayers = res.ResultAs<int>();
            Intent intent;
            if (isStopped)
            {
                return;
            }
            RunOnUiThread(async () =>
            {
                if (finishedPlayers == totalPlayers)
                {
                    isStopped = true;
                    intent = new Intent(this, typeof(StatisticsActivity));
                    StartActivity(intent);
                }
            });

            // Reschedule the task to run again after the delay
            handler.PostDelayed(MyThread, 1000);
        }
    }
}