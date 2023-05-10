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
using Firebase.Database.Streaming;
using AndroidX.AppCompat.App;
using AlertDialog = Android.App.AlertDialog;
using Java.Interop;
using System.Security.Cryptography.Xml;
using System.Drawing.Imaging;
using WouldYouRather.Classes;

namespace WouldYouRather
{
    [Activity(Label = "QuestionAnswerActivity", Theme = "@style/AppTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class QuestionAnswerActivity : AppCompatActivity
    {
        private Button btnQ1,btnQ2;
        private IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "AY7hyv1qtUGKDSs2Gud5Ct3sSLX59KYZQZaGJBOq",
            BasePath = "https://eitanprojectwyr-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        private IFirebaseClient client;
        private int count;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            InvalidateOptionsMenu();
            SetContentView(Resource.Layout.questionAnswer_layout);
            btnQ1 = FindViewById<Button>(Resource.Id.btnQ1);
            btnQ2 = FindViewById<Button>(Resource.Id.btnQ2);
            btnQ1.Click += BtnQ1_Click;
            btnQ2.Click += BtnQ2_Click;
            count = 1;
            // Create your application here
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {

                Toast.MakeText(this, "Network Error", ToastLength.Short).Show();
            }
            FirebaseResponse res=await client.GetAsync("Questions/Q1/O1/Question");
            btnQ1.Text = res.ResultAs<string>();
            res = await client.GetAsync("Questions/Q1/O2/Question");
            btnQ2.Text = res.ResultAs<string>();
        }

        private async void BtnQ2_Click(object sender, EventArgs e)
        {
            
            try
            {
                FirebaseResponse res = await client.GetAsync("Questions/Q" + count + "/O2/votes");//get the votes count
                await client.SetAsync("Questions/Q" + count + "/O2/votes", res.ResultAs<int>() + 1);//set the new votes count
                GetPerscentes(count);
                count++;
                res = await client.GetAsync("Questions/Q" + count + "/O1/Question");//get next qo1
                btnQ1.Text = res.ResultAs<string>();
                res = await client.GetAsync("Questions/Q" + count + "/O2/Question");//get next qo2
                btnQ2.Text = res.ResultAs<string>();
                if (btnQ1.Text == "")
                {   Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    AlertDialog alert = dialog.Create();
                    alert.Window.SetSoftInputMode(Android.Views.SoftInput.StateAlwaysVisible);
                    alert.Window.GetJniTypeName();
                    alert.SetCancelable(false);
                    alert.SetCanceledOnTouchOutside(false); alert.SetTitle("Oops Run Out Of Questions!");
                    alert.SetMessage("What Do You Wish To Do?");
                    alert.SetButton("Add Question!", (c, ev) =>
                    {
                        AddQuestion();

                    });
                    alert.SetButton3("Go Back To Menu!", (c, ev) => {
                        Intent i = new Intent(this, typeof(MenuActivity));
                        StartActivity(i);
                    });
                    alert.Show();
                }
            }
            catch (Exception)
            {
              
            }
           
           
        }

        private async void BtnQ1_Click(object sender, EventArgs e)
        {

            try
            {
                FirebaseResponse res = await client.GetAsync("Questions/Q" + count + "/O1/votes");//get the votes count
                await client.SetAsync("Questions/Q" + count + "/O1/votes", res.ResultAs<int>() + 1);//set the new votes count
                GetPerscentes(count);
                count++;
                res = await client.GetAsync("Questions/Q" + count + "/O1/Question");//get next qo1
                btnQ1.Text = res.ResultAs<string>();
                res = await client.GetAsync("Questions/Q" + count + "/O2/Question");//get next qo2
                btnQ2.Text = res.ResultAs<string>();
                if (btnQ1.Text== "")
                {   Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    AlertDialog alert = dialog.Create();
                    alert.Window.SetSoftInputMode(Android.Views.SoftInput.StateAlwaysVisible);
                    alert.Window.GetJniTypeName();
                    alert.SetCancelable(false);
                    alert.SetCanceledOnTouchOutside(false);
                    alert.SetTitle("Oops Run Out Of Questions!");
                    alert.SetMessage("What Do You Wish To Do?");
                    alert.SetButton("Add Question!", (c, ev) =>
                    {
                        AddQuestion();

                    });
                    alert.SetButton3("Go Back To Menu!", (c, ev) => {
                        Intent i = new Intent(this, typeof(MenuActivity));
                        StartActivity(i);
                    });
                    alert.Show();
                }
            }
            catch (Exception)
            {
              
            }
        }
      
        private async void GetPerscentes(int v)//מחשב אחוזים
        {
            FirebaseResponse res = await client.GetAsync("Questions/Q" + v + "/O1/votes");
            int countvoteo1 = res.ResultAs<int>();
            res = await client.GetAsync("Questions/Q" + v + "/O2/votes");
            int countvoteo2 = res.ResultAs<int>();
            int totalos = countvoteo1 + countvoteo2;
            double op1 = (countvoteo1 / (double)totalos) * 100;
            double op2 = (countvoteo2 / (double)totalos) * 100;
            op1 = Math.Round(op1);
            op2 = Math.Round(op2);
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Percentages");
            alert.SetMessage("Option 1: " + op1 + "%" + "\n" + "Option 2: " + op2 + "%");
            alert.SetPositiveButton("OK", (senderAlert, args) =>
            {
                //Do something here when OK is clicked
            });
            RunOnUiThread(() => {
                alert.Show();
            });
        }
        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.mymenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {

            if (item.ItemId == Resource.Id.action_add_question)
            {
                AddQuestion();

                return true;

            }
            else if (item.ItemId == Resource.Id.action_skip)
            {
                ActionSkip();
                return true;
            }
            else if (item.ItemId == Resource.Id.action_back)
            {
                Intent intent = new Intent(this, typeof(MenuActivity));
                StartActivity(intent);
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private async void ActionSkip()//מעביר שאלה הבאה מבלי להוסיף הצבעה
        {
            count++;
            FirebaseResponse res = await client.GetAsync("Questions/Q" + count + "/O1/Question");//get next qo1
            btnQ1.Text = res.ResultAs<string>();
            res = await client.GetAsync("Questions/Q" + count + "/O2/Question");//get next qo2
            btnQ2.Text = res.ResultAs<string>();
        }

        private async void AddQuestion()//הוספת שאלה
        {
            FirebaseResponse res = await client.GetAsync("Questions/count");//get next qo1
            int key = res.ResultAs<int>();
            // Code is valid, allow user to join lobby
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            View view = LayoutInflater.Inflate(Resource.Layout.custom_second_dialog, null);
            dialog.SetView(view);
            AlertDialog alert = dialog.Create();
            var quest1 = view.FindViewById<EditText>(Resource.Id.CostumDia_etq1);
            var quest2 = view.FindViewById<EditText>(Resource.Id.CostumDia_etq2);
            alert.Window.SetSoftInputMode(Android.Views.SoftInput.StateAlwaysVisible);
            alert.Window.GetJniTypeName();
            alert.SetTitle("Add Question");
            alert.SetMessage("Please Enter Your Questions");
            alert.SetButton("Add!", async (c, ev) =>
            {
                try
                {
                    await client.SetAsync("Questions/Q" + key + "/O1/Question", quest1.Text);
                    await client.SetAsync("Questions/Q" + key + "/O2/Question", quest2.Text);
                    await client.SetAsync("Questions/Q" + key + "/O1/votes", 0);
                    await client.SetAsync("Questions/Q" + key + "/O2/votes", 0);
                    key++;
                    await client.SetAsync("Questions/count", key);

                }
                catch
                { }
            });
            alert.SetButton3("Cancel!", (c, ev) => { });
            alert.Show();

        }
        public override void OnBackPressed()
        {
            // Do nothing
        }

    }
}