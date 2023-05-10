using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Widget;
using Android.Views;
using Android.Content;
using System.Collections.Generic;
using System;
using System.Threading;
using FireSharp.Response;
using FireSharp.Config;
using FireSharp.Interfaces;
using Firebase.Database;
using Newtonsoft.Json;
using WouldYouRather.FireBase;
using Firebase;
using AlertDialog = Android.App.AlertDialog;
using WouldYouRather.Classes;
using Firebase.Auth;
using Java.Interop;
using Java.Util;
using System.Linq;
using System.Threading.Tasks;

namespace WouldYouRather
{
    [Activity(Label = "QuestionCreateActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class QuestionCreateActivity : AppCompatActivity
    {
        private Handler handler;
        private IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "AY7hyv1qtUGKDSs2Gud5Ct3sSLX59KYZQZaGJBOq",
            BasePath = "https://eitanprojectwyr-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        private IFirebaseClient client;
        private ListView lv;
        private Button btnAdd, btnDone, btnLeave;
        private TextView tvCode, tvPlayer;
        private List<string> pushKeyArray;
        private int count;
        public static List<QuestionClass> questionList { get; set; }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            handler = new Handler();
            handler.PostDelayed(MyThread, 1000);
            count = 0;
            pushKeyArray= new List<string>();
            SetContentView(Resource.Layout.questionCreate_layout);
            btnAdd = FindViewById<Button>(Resource.Id.btnAddQuestion);
            btnDone = FindViewById<Button>(Resource.Id.btnDone);
            tvCode = FindViewById<TextView>(Resource.Id.tvRCode);
            tvPlayer = FindViewById<TextView>(Resource.Id.tvPlayer);
            btnLeave = FindViewById<Button>(Resource.Id.btnLeave);
            tvCode.Text = "Room code: " + Intent.GetStringExtra("code");
            btnLeave.Click += BtnLeave_Click;
            btnAdd.Click += BtnAdd_Click;
            btnDone.Click += BtnDone_Click;
            if (MainActivity.user.Uid != MainActivity.user.ThirdGuestUid)
                btnDone.Visibility = ViewStates.Gone;
            lv = FindViewById<ListView>(Resource.Id.lvxml);
            questionList = new System.Collections.Generic.List<QuestionClass>();
            MyQuestionAdapter listAdapter = new MyQuestionAdapter(this, questionList);
            lv.Adapter = listAdapter;
            lv.ItemClick += Lv_ItemClick;
            lv.ItemLongClick += Lv_ItemLongClick;
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {
                Toast.MakeText(this, "Network Error", ToastLength.Short).Show();
            }
        }

        private async void BtnLeave_Click(object sender, EventArgs e)
        {
            isStopped = true;
            Finish();
            if (MainActivity.user.Uid == MainActivity.user.ThirdGuestUid)
            await client.DeleteAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid);
            else
                await client.DeleteAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/players/" + MainActivity.user.PushCode);
        }

        private bool isStopped = false;
        private async void MyThread()
        {
            if (isStopped)
            {
                return;
            }
            RunOnUiThread(async () =>
            {
                FirebaseResponse response = await client.GetAsync(@"RoomCodes/"+MainActivity.user.ThirdGuestUid+"/players");
                Dictionary<string, PlayersOnline> data = JsonConvert.DeserializeObject<Dictionary<string, PlayersOnline>>(response.Body.ToString());
                CheckCode(data);
                FirebaseResponse res = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/isDone");
                bool IsDone = res.ResultAs<bool>();
                if (IsDone)
                {
                    isStopped = true;
                    handler.RemoveCallbacks(MyThread);
                    Intent intent = new Intent(this, typeof(QuestionAnswerOnlineActivity));
                    StartActivity(intent);
                }
            });

            // Reschedule the task to run again after the delay
            handler.PostDelayed(MyThread, 1000);
        }

        private void CheckCode(Dictionary<string, PlayersOnline> data)
        {
            tvPlayer.Text = "Players List:\n";
            bool first = true;
            foreach (var item in data)
            {
                if (first)
                {
                    tvPlayer.Text += " " + item.Value.name;
                    first = false;
                }
                else
                {
                    tvPlayer.Text += ", " + item.Value.name;
                }
            }
        }

        private void Lv_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)//מחיקת שאלה
        {
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            View view = LayoutInflater.Inflate(Resource.Layout.emptydialog, null);
            dialog.SetView(view);
            AlertDialog alert = dialog.Create();
            alert.Window.SetSoftInputMode(Android.Views.SoftInput.StateAlwaysVisible);
            alert.Window.GetJniTypeName();
            alert.SetTitle("Delete Question");
            alert.SetMessage("Are you sure you want to delete your question?");
            alert.SetButton("Delete!", async (c, ev) =>
            {
                await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/players/" + MainActivity.user.PushCode + "/ready", false);
                MyQuestionAdapter listAdapter = (MyQuestionAdapter)lv.Adapter;
                questionList.RemoveAt(e.Position);
                listAdapter.NotifyDataSetChanged();
                FirebaseResponse response = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/countQuestion");
                count = response.ResultAs<int>() - 1;
                try
                {
                    await client.DeleteAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + pushKeyArray[e.Position]);
                    await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/countQuestion", count);
                }
                catch
                { }
            });
            count++;
            alert.SetButton3("Cancel!", (c, ev) => { });
            alert.Show();
        }

        private void Lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)//עריכת שאלה
        {
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            View view = LayoutInflater.Inflate(Resource.Layout.custom_second_dialog, null);
            dialog.SetView(view);
            AlertDialog alert = dialog.Create();
            var quest = view.FindViewById<EditText>(Resource.Id.CostumDia_etq1);
            var quest2 = view.FindViewById<EditText>(Resource.Id.CostumDia_etq2);
            alert.Window.SetSoftInputMode(Android.Views.SoftInput.StateAlwaysVisible);
            alert.Window.GetJniTypeName();
            alert.SetTitle("Edit Question");
            alert.SetMessage("Please Enter Your Questions");
            alert.SetButton("Edit!", async (c, ev) =>
            {
                await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/players/" + MainActivity.user.PushCode + "/ready", false);
                QuestionClass questionClass = new QuestionClass(quest.Text, quest2.Text);
                questionList[e.Position] = questionClass;
                MyQuestionAdapter listAdapter = (MyQuestionAdapter)lv.Adapter;
                listAdapter.NotifyDataSetChanged();
                FirebaseResponse response = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/countQuestion");
                int count = response.ResultAs<int>() - 1;
                try
                {
                    await client.UpdateAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + pushKeyArray[e.Position], new
                    {
                        O1 = new { Question = quest.Text },
                        O2 = new { Question = quest2.Text }
                    });
                }
                catch
                { }
            });
            alert.SetButton3("Cancel!", (c, ev) => { });
            alert.Show();
        }

        private async void BtnDone_Click(object sender, EventArgs e)
        {
            FirebaseResponse response = await client.GetAsync(@"RoomCodes/" + MainActivity.user.ThirdGuestUid + "/players");
            Dictionary<string, PlayersOnline> data = JsonConvert.DeserializeObject<Dictionary<string, PlayersOnline>>(response.Body.ToString());
            CheckDone(data);
        }

        private async void CheckDone(Dictionary<string, PlayersOnline> data)//בודק האם אפשר להתחיל משחק
        {
            bool flag = true;
            bool isQuestions = true;

            foreach (var item in data)
            {
                if (item.Value.ready == false)
                    flag = false;
            }
            FirebaseResponse response = await client.GetAsync(@"RoomCodes/" + MainActivity.user.ThirdGuestUid);
            Dictionary<string, object> uidList = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Body.ToString());
            foreach (var item in uidList)
            {
                if (item.Key == "Questions")
                    isQuestions= false;
            }
            if (isQuestions)//צריך לבדוק אם יש שאלות או אין)
            {
                flag = false;
                Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                AlertDialog alert = dialog.Create();
                alert.Window.SetSoftInputMode(Android.Views.SoftInput.StateAlwaysVisible);
                alert.Window.GetJniTypeName();
                alert.SetCancelable(false);
                alert.SetCanceledOnTouchOutside(false);
                alert.SetTitle("Notice!");
                alert.SetMessage("No question has been added");
                alert.SetButton("Noticed!", (c, ev) =>
                {

                });
                alert.Show();
            }
            
            if (flag)
            {
                await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/isDone", true);
            }
            else if(flag==false&&isQuestions==false)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Notice!");
                alert.SetMessage("All Players must leave 'Add Question Form'");
                alert.SetPositiveButton("OK", (senderAlert, args) =>
                {
                    //Do something here when OK is clicked
                });
                RunOnUiThread(() => {
                    alert.Show();
                });
            }
        }



        private async void BtnAdd_Click(object sender, EventArgs e)//הוספת שאלה
        {
            await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/players/" + MainActivity.user.PushCode + "/ready", false);
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            View view = LayoutInflater.Inflate(Resource.Layout.custom_second_dialog, null);
            dialog.SetView(view);
            AlertDialog alert = dialog.Create();
            var quest1 = view.FindViewById<EditText>(Resource.Id.CostumDia_etq1);
            var quest2 = view.FindViewById<EditText>(Resource.Id.CostumDia_etq2);
            alert.SetCancelable(false);
            alert.SetCanceledOnTouchOutside(false);
            alert.Window.SetSoftInputMode(Android.Views.SoftInput.StateAlwaysVisible);
            alert.Window.GetJniTypeName();
            alert.SetTitle("Add Question");
            alert.SetMessage("Please Enter Your Questions");
            alert.SetButton("Add!", async (c, ev) =>
            {
                if(quest1.Text!="" && quest2.Text!="")
                {
                    await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/players/" + MainActivity.user.PushCode + "/ready", true);
                    QuestionClass questionClass = new QuestionClass(quest1.Text, quest2.Text);
                    questionList.Add(questionClass);
                    MyQuestionAdapter listAdapter = (MyQuestionAdapter)lv.Adapter;
                    listAdapter.NotifyDataSetChanged();
                    FirebaseResponse response = await client.GetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/countQuestion");
                    count = response.ResultAs<int>();


                    try
                    {
                        PushResponse res = await client.PushAsync("push",2);
                        await client.DeleteAsync("push");
                        await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/"+res.Result.Name+"/O1/Question", quest1.Text);
                        await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + res.Result.Name + "/O2/Question", quest2.Text);
                        await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + res.Result.Name + "/O1/votes", 0);
                        await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/Questions/" + res.Result.Name + "/O2/votes", 0);
                        pushKeyArray.Add(res.Result.Name);
                        count++;

                    }
                    catch
                    { }
                }
                else
                {
                    Toast.MakeText(this, "One or more of the question is empty", ToastLength.Short).Show();
                }
            });
            count++;
            alert.SetButton3("Cancel!", async (c, ev) => { await client.SetAsync("RoomCodes/" + MainActivity.user.ThirdGuestUid + "/players/" + MainActivity.user.PushCode + "/ready", true); });
            alert.Show();
        }
        public override void OnBackPressed()
        {

        }
    }
}