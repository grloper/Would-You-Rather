using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FbAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace WouldYouRather
{
    [Activity(Label = "RegisterActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RegisterActivity : Activity, IOnCompleteListener
    {
        private Button back, register;
        private EditText name, email, password, conpass;
        private TextView resultPassword;
        private FB_Data fbd;
        private User user;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register_layout);

            // Create your application here
            back = FindViewById<Button>(Resource.Id.btnBack);
            register = FindViewById<Button>(Resource.Id.btnReg);
            name = FindViewById<EditText>(Resource.Id.etUser);
            email = FindViewById<EditText>(Resource.Id.etMail);
            conpass = FindViewById<EditText>(Resource.Id.etPassConfirm);
            password = FindViewById<EditText>(Resource.Id.etPass);
            resultPassword = FindViewById<TextView>(Resource.Id.tvResult);
            fbd = new FB_Data();
            back.Click += Back_Click;
            register.Click += Register_Click;
        }

        private void Register_Click(object sender, EventArgs e)
        {
            RegisterUser();
        }

        private void RegisterUser()
        {
            user = new User(name.Text, email.Text, password.Text, false);
            if (user.Name != string.Empty && user.Pwd != string.Empty && password.Text==conpass.Text && IsValidMail(email.Text))
                fbd.CreateUser(user.Mail, user.Pwd).AddOnCompleteListener(this);
            else
                Toast.MakeText(this, "Enter all values", ToastLength.Short).Show();
        }

        private bool IsValidMail(string email)
        {
            var valid = true;
            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }
            return valid;
        }

        private void Back_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                Intent i = new Intent();
                i.PutExtra(General.KEY_NAME, user.Name);
                i.PutExtra(General.KEY_MAIL, user.Mail);
                i.PutExtra(General.KEY_PWD, user.Pwd);
                SetResult(Result.Ok, i);
                Finish();
            }
            else
            {
                resultPassword.Text = task.Exception.Message; // יקבל את הסיבה לכישלון שהגיעה מהפיירבייס
            }
        }
        public override void OnBackPressed()
        {
            // Do nothing
        }
    }
}