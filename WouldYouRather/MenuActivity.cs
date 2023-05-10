using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WouldYouRather.FireBase;

namespace WouldYouRather
{
    [Activity(Label = "MenuActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MenuActivity : Activity
    {
        private IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "AY7hyv1qtUGKDSs2Gud5Ct3sSLX59KYZQZaGJBOq",
            BasePath = "https://eitanprojectwyr-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        private IFirebaseClient client;
        private Button btnBack, btnCreate, btnOffline, btnJoin;
        private EditText etCode;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.menu_layout);
            btnOffline = FindViewById<Button>(Resource.Id.btnPlayOffline);
            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += BtnBack_Click;
            btnCreate = FindViewById<Button>(Resource.Id.btnCreateLobby);
            btnJoin= FindViewById<Button>(Resource.Id.btnJoin);
            etCode = FindViewById<EditText>(Resource.Id.etCode);
            btnOffline.Click += BtnOffline_Click;
            btnCreate.Click += BtnCreate_Click;
            btnJoin.Click += BtnJoin_Click;
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {

                Toast.MakeText(this, "Network Error", ToastLength.Short).Show();
            }
            // Create your application here
        }

        private void BtnOffline_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(QuestionAnswerActivity));
            StartActivity(intent);
        }

        private async void BtnJoin_Click(object sender, EventArgs e)
        {
             FirebaseResponse response = await client.GetAsync(@"RoomCodes");
            Dictionary<string, PrivateRoom> data = JsonConvert.DeserializeObject<Dictionary<string, PrivateRoom>>(response.Body.ToString());
            CheckCode(data);
        }

        private async void CheckCode(Dictionary<string, PrivateRoom> data)//פעולה שבודקת האם הקוד נמצא
        {
            bool found = false;
            foreach (var item in data)
            {
                if (etCode.Text == item.Value.myCode)
                {
                    MainActivity.user.ThirdGuestUid=item.Key;
                    Intent intent = new Intent(this, typeof(QuestionCreateActivity));
                    intent.PutExtra("code", etCode.Text);
                    intent.PutExtra("roomUid", item.Key);
                    StartActivity(intent);
                    try
                    {
                        PushResponse pushResponse = await client.PushAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/players", MainActivity.user.Name);
                        MainActivity.user.PushCode = pushResponse.Result.Name;
                        PlayersOnline d = new PlayersOnline
                        {
                            name = MainActivity.user.Name,
                            ready = true
                        };
                        client.Set("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/players/" + MainActivity.user.PushCode, d);
                    }

                    catch
                    { }
                           found = true;
                }
            }
            if (!found)
                Toast.MakeText(this, "Code Not Found!", ToastLength.Short).Show();
        }
        private void BtnCreate_Click(object sender, EventArgs e)
        {

            Intent intent = new Intent(this, typeof(RoomActivity));
            StartActivity(intent); 
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            FirebaseAuth auth = FirebaseAuth.Instance;
            auth.SignOut();
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
        public override void OnBackPressed()
        {
            // Do nothing
        }
    }
}