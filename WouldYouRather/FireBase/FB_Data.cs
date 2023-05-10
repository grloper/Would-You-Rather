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
using Firebase;
using Firebase.Auth;

namespace WouldYouRather
{
    internal class FB_Data
    {
        private readonly FirebaseApp app;
        private readonly FirebaseAuth auth;
        public FB_Data()
        {
            app = FirebaseApp.InitializeApp(Application.Context);
            if (app is null)
            {
                FirebaseOptions options = GetMyOptions();
                app = FirebaseApp.InitializeApp(Application.Context, options);
            }
            auth = FirebaseAuth.Instance;
        }
        public Android.Gms.Tasks.Task SignInAnonymously()
        {
            return auth.SignInAnonymously();
        }
        public void DelteUser()
        {
            auth.CurrentUser.Delete();
        }
        public Android.Gms.Tasks.Task CreateUser(string email, string password)
        {
            return auth.CreateUserWithEmailAndPassword(email, password);
        }
        public Android.Gms.Tasks.Task SignIn(string email, string password)
        {
            return auth.SignInWithEmailAndPassword(email, password);
        }
        public Android.Gms.Tasks.Task ResetPassword(string email)
        {
            return auth.SendPasswordResetEmail(email);
        }
        private FirebaseOptions GetMyOptions()
        {
            return new FirebaseOptions.Builder()
                .SetProjectId("eitanprojectwyr")
                .SetApplicationId("eitanprojectwyr")
                .SetApiKey("AIzaSyCi05_nUPL5Eseao_VhbXwH41qObm9ZpBE")
                .SetStorageBucket("eitanprojectwyr.appspot.com").Build();
        }
    }
}