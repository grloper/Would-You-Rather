using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WouldYouRather.Classes;

namespace WouldYouRather
{
    [Activity(Label = "StatisticsActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class StatisticsActivity : Activity
    {
        private IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "AY7hyv1qtUGKDSs2Gud5Ct3sSLX59KYZQZaGJBOq",
            BasePath = "https://eitanprojectwyr-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        private IFirebaseClient client;
        private List<string> keysList;
        private Button btnBack;
        private ListView lv;
        private List<Stats> statsList;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.statistics_layout);
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {
                Toast.MakeText(this, "Network Error", ToastLength.Short).Show();
            }
            FirebaseResponse res = await client.GetAsync(@"RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions");
            Dictionary<string, object> data = res.ResultAs<Dictionary<string, object>>();
            //GetKeys
            IEnumerable<string> qKeys = data.Keys;
            keysList = qKeys.ToList();
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle("Wait for players to finish");
            alert.SetMessage("Wait for other players to finish voting before proceeding.");
            alert.Show();
            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += BtnBack_Click;
            lv = FindViewById<ListView>(Resource.Id.lvstats);
            statsList = new List<Stats>();
            foreach (var key in keysList)
            {
                //getting questions
                FirebaseResponse questionRes = await client.GetAsync(@"RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + key + "/O1/Question");
                string blueQuestion = questionRes.ResultAs<string>();
                questionRes = await client.GetAsync(@"RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + key + "/O2/Question");
                string redQuestion = questionRes.ResultAs<string>();
                //getting votes
                FirebaseResponse voteRes = await client.GetAsync(@"RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + key + "/O1/votes");
                int blueVotes = int.Parse(voteRes.ResultAs<string>());
                voteRes = await client.GetAsync(@"RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + key + "/O2/votes");
                int redVotes = int.Parse(voteRes.ResultAs<string>());
                //votes to %
                int totalVotes = redVotes + blueVotes;
                float redPercentage = (float)redVotes / totalVotes * 100;
                float bluePercentage = (float)blueVotes / totalVotes * 100;
                Math.Round(redPercentage, 2);
                Math.Round(bluePercentage, 2);
                statsList.Add(new Stats { RedQuestion=redQuestion,BluePercentage= bluePercentage,BlueQuestion=blueQuestion,RedPercentage=redPercentage });
            }
            alert.Dismiss();

            lv.Adapter = new StatsAdapter(this, statsList);

            // Create your application here
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
        public override void OnBackPressed()
        {
            // Do nothing
        }
    }
}