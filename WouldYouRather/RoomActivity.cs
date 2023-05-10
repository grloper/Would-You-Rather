using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireSharp.Interfaces;
using FireSharp.Response;
using FireSharp.Config;
using Newtonsoft.Json;
using WouldYouRather.FireBase;

namespace WouldYouRather
{
    [Activity(Label = "RoomActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RoomActivity : Activity
    {
        IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "AY7hyv1qtUGKDSs2Gud5Ct3sSLX59KYZQZaGJBOq",
            BasePath = "https://eitanprojectwyr-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        private IFirebaseClient client;
        private Handler handler;
        private Button btnBack,btnCreate;
        private string genCode;
        private EditText etDecistionT, etPlayersNum;
        private PrivateRoom privateCode;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            handler = new Handler();
            handler.PostDelayed(ShowToast, 1000);
            SetContentView(Resource.Layout.room_layout);
            btnCreate = FindViewById<Button>(Resource.Id.btnCreateRoom);
            etDecistionT = FindViewById<EditText>(Resource.Id.etDecisionTime);
            etPlayersNum = FindViewById<EditText>(Resource.Id.etPlayersNum);
            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += BtnBack_Click;
            btnCreate.Click += BtnCreate_Click;
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {

                Toast.MakeText(this, "Network Error", ToastLength.Short).Show();
            }
            genCode = Intent.GetStringExtra("genCode");
            genCode = GenCode();


        }

        private async void ShowToast()
        {
            RunOnUiThread(() =>
            {
            });

            // Reschedule the task to run again after the delay
            handler.PostDelayed(ShowToast, 1000);
        }

        private async void BtnCreate_Click(object sender, EventArgs e)//יוצר חדר
        {
            int decistionTime = -1, playerNum = -1;
            int.TryParse(etDecistionT.Text,out decistionTime);
            int.TryParse(etPlayersNum.Text,out playerNum);
            if ((decistionTime<=30&& decistionTime>=5)&&(playerNum>0&&playerNum<=8))
            {
                privateCode = new PrivateRoom
                {
                    myCode = genCode,
                    dicistionTimeSec = int.Parse(etDecistionT.Text),
                    numOfPlayers = int.Parse(etPlayersNum.Text),
                    finishedPlayers = 0,
                    countQuestion = 1,
                    isDone = false
                };
                MainActivity.user.ThirdGuestUid = MainActivity.user.Uid;
                try
                {
                    await client.SetAsync("RoomCodes/" + MainActivity.user.Uid, privateCode);
                    PushResponse pushResponse = await client.PushAsync("RoomCodes/" + MainActivity.user.Uid + "/players", MainActivity.user.Name);
                    MainActivity.user.PushCode = pushResponse.Result.Name;
                    client.Set("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/players/" + MainActivity.user.PushCode + "/ready", true);
                    client.Set("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/players/" + MainActivity.user.PushCode + "/name", MainActivity.user.Name);

                }
                catch
                { }
                Intent intent = new Intent(this, typeof(QuestionCreateActivity));
                intent.PutExtra("code", privateCode.myCode);
                StartActivity(intent);
            }
            else
            {
                Toast.MakeText(this, "Wrong Rules", ToastLength.Short).Show();
            }

        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MenuActivity));
            StartActivity(intent);
        }
        private string GenCode()//מייצר קוד חדש רנדומלי
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            bool nah = finalString.Any(c => char.IsDigit(c));
            if (!finalString.Any(c => char.IsDigit(c)))
                GenCode();
            return finalString;
        }
        public override void OnBackPressed()
        {
            // Do nothing
        }
    }
}