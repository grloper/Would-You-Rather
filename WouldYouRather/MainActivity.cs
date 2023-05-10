using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using Android.Gms.Tasks;
using FbAuthentication;
using Android.Content;
using System;
using AlertDialog = Android.App.AlertDialog;
using Firebase.Auth;
using Java.Interop;
using Android.Views;
using System.Net.Mail;
using WouldYouRather.Classes;

namespace WouldYouRather
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity,IOnCompleteListener
    {
        private Button login, register;
        private EditText name, email, password;
        private CheckBox checkboxPassword;
        private TextView resultPassword, guest, reset;
        public static User user;
        private FB_Data fbd;
        private Task tskLogin, tskReset ,tskGuest;
        private ISharedPreferences sp;

    protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            login = FindViewById<Button>(Resource.Id.btnLogin);
            reset = FindViewById<TextView>(Resource.Id.btnReset);
            guest= FindViewById<TextView>(Resource.Id.btnGuest);
            register = FindViewById<Button>(Resource.Id.btnReg);
            name = FindViewById<EditText>(Resource.Id.etUser);
            email= FindViewById<EditText>(Resource.Id.etMail);
            password = FindViewById<EditText>(Resource.Id.etPass);
            checkboxPassword = FindViewById<CheckBox>(Resource.Id.cbPassword);
            resultPassword = FindViewById<TextView>(Resource.Id.tvResult);
            user = new User(this);
            fbd = new FB_Data();
            login.Click += Login_Click;
            reset.Click += Reset_Click;
            guest.Click += Guest_Click;
            register.Click += Register_Click;
            if (user.Exist)
                ShowUserData();
            sp = this.GetSharedPreferences("details", FileCreationMode.Private);
            Intent intent = new Intent(this, typeof(MusicService));
            StartService(intent);

        }

        private void Guest_Click(object sender, EventArgs e)
        {
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            View view = LayoutInflater.Inflate(Resource.Layout.custom_dialog_layout, null);
            dialog.SetView(view);
            AlertDialog alert = dialog.Create();
            var userdata = view.FindViewById<EditText>(Resource.Id.CostumDia_etDia);
            alert.Window.SetSoftInputMode(Android.Views.SoftInput.StateAlwaysVisible);
            alert.Window.GetJniTypeName();
            alert.SetTitle("Login Anonymously");
            alert.SetMessage("Please Mention Your Username");
            alert.SetButton("Yes!", (c, ev) =>
            {
                user.Name = userdata.Text;
                if (ValidName(user.Name))
                {
                    tskGuest = fbd.SignInAnonymously();
                    tskGuest.AddOnCompleteListener(this);
                }
                else
                {
                    Toast.MakeText(this, "Name Rules: words or numbers", ToastLength.Long).Show();
                }

            });
            alert.SetButton3("No!", (c, ev) => { });
            alert.Show();
        }

        private bool ValidName(string name)
        {
            if (name == "")
                return false;
            char[] valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890 ".ToCharArray();
            char[] chars = name.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                for (int j = 0; j < valid.Length; j++)
                    if (!(Array.IndexOf(valid, chars[i]) > -1))
                        return false;
            return true;
        }

        private void OpenRegisterActivity()
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivityForResult(intent, General.REQUEST_REGISTER);
        }

        private void ShowUserData()
        {
            email.Text = user.Mail;
            name.Text = user.Name;
            password.Text = user.Pwd;
            checkboxPassword.Checked = (user.Pwd != string.Empty);
        }

        private void Register_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivityForResult(intent, General.REQUEST_REGISTER);
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            ResetPassword();
        }

        private void ResetPassword()
        {
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            View view = LayoutInflater.Inflate(Resource.Layout.custom_dialog_layout, null);
            dialog.SetView(view);
            AlertDialog alert = dialog.Create();
            var userdata = view.FindViewById<EditText>(Resource.Id.CostumDia_etDia);
            alert.Window.SetSoftInputMode(Android.Views.SoftInput.StateAlwaysVisible);
            alert.Window.GetJniTypeName();
            alert.SetTitle("Reset Password?");
            alert.SetMessage("Please Mention Your Email Address");
            alert.SetButton("Yes!", (c, ev) =>
            {
                if (IsValidMail(userdata.Text))
                {
                    tskReset = fbd.ResetPassword(userdata.Text);
                    tskReset.AddOnCompleteListener(this);
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Reset Successful");
                    alert.SetMessage("We Sent a Password Reset Link To: " + userdata.Text);
                    alert.SetButton("Ok", (c, ev) => { });
                    alert.Show();
                }
                else
                {
                    Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                    AlertDialog alert = dialog.Create();
                    alert.SetTitle("Reset Failed");
                    alert.SetMessage("Please Mention A Valid Email Address");
                    alert.SetButton("Ok", (c, ev) => { });
                    alert.Show();
                }
            });
            alert.Show();
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

        private void Login_Click(object sender, EventArgs e)
        {
            LoginUser();

        }

        private void LoginUser()
        {
            if (password.Text != string.Empty)
            {
                tskLogin = fbd.SignIn(user.Mail, password.Text);
                tskLogin.AddOnCompleteListener(this);
                user.Pwd = checkboxPassword.Checked ? password.Text : string.Empty;
                if (!user.Save())
                    Toast.MakeText(this, "Error", ToastLength.Long).Show();
            }
            else
                Toast.MakeText(this, "Enter Password", ToastLength.Long).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnComplete(Task task)
        {
            FirebaseAuth auth = FirebaseAuth.Instance;
            string msg = string.Empty;
            if (task.IsSuccessful)
            {
                if (task == tskLogin)
                {
                    msg = "Login successful";
                    Intent intent = new Intent(this,typeof(MenuActivity));
                    StartActivity(intent);
                    user.Uid = auth.Uid;
                }
                else if(task == tskGuest)
                {
                    msg = "Login Anonymously successful";
                    user.Uid = auth.Uid;
                    Intent intent = new Intent(this, typeof(MenuActivity));
                    StartActivity(intent);
                    user.IsGuest = true;
                    fbd.DelteUser();
                }
                else if (task == tskReset)
                    msg = "Reset successful";
            }
            else
                msg = task.Exception.Message;
            resultPassword.Text = msg;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == General.REQUEST_REGISTER)
                if (resultCode == Result.Ok)
                {
                    user.Name = data.GetStringExtra(General.KEY_NAME);
                    user.Mail = data.GetStringExtra(General.KEY_MAIL);
                    user.Pwd = data.GetStringExtra(General.KEY_PWD);
                    ShowUserData();
                    user.Pwd = string.Empty;
                    if (!user.Save())
                        Toast.MakeText(this, "Error", ToastLength.Long).Show();
                    checkboxPassword.Checked = false;
                }
        }
        public override void OnBackPressed()
        {
            // Do nothing
        }
        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.music_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {

            if (item.ItemId == Resource.Id.action_music_controller)
            {
                ShowVolumeControlDialog();
            }
            return base.OnOptionsItemSelected(item);
        }
        private void ShowVolumeControlDialog()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            LayoutInflater inflater = LayoutInflater.From(this);
            View view = inflater.Inflate(Resource.Layout.music_controller, null);
            builder.SetView(view);
            TextView volumeTextView = view.FindViewById<TextView>(Resource.Id.tvVol);

            SeekBar volumeSeekBar = view.FindViewById<SeekBar>(Resource.Id.seekBar);
            int volumeProgress = sp.GetInt("volumeProgress", 50); // default volume at 50%
            volumeSeekBar.Progress = volumeProgress;
            volumeTextView.Text = "Volume: " + volumeProgress; // update text view with current volume

            volumeSeekBar.ProgressChanged += (sender, e) =>
            {
                int progress = volumeSeekBar.Progress;
                ISharedPreferencesEditor editor = sp.Edit();
                editor.PutInt("volumeProgress", progress);
                editor.Apply();
                volumeTextView.Text = "Volume: " + progress;

                float volume = progress / 100f;
                MusicService.mediaPlayer.SetVolume(volume, volume);
            };



            AlertDialog dialog = builder.Create();
            dialog.Show();
        }


    }
}