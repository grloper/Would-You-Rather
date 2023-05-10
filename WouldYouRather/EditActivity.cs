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

namespace WouldYouRather
{
    [Activity(Label = "EditLayout")]
    public class EditLayout : Activity
    {
        Button btnSave;
        EditText et1, et2;
        int pos = -1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.editLayout);
            pos = Intent.GetIntExtra("pos", -1);//-1 is default
            btnSave = FindViewById<Button>(Resource.Id.btnSaveEdit);
            et1 = FindViewById<EditText>(Resource.Id.etQ1);
            et2 = FindViewById<EditText>(Resource.Id.etQ2);
            btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            String question1 = et1.Text;
            String question2 = et2.Text;
            QuestionClass t = null;
            if (pos != -1)  //update Existing item
            {
                t = new QuestionClass(question2, question1);
                QuestionCreateActivity.questionList[pos] = t;
                Finish();
            }
            else // create new toy
            {
                t = new QuestionClass(question2, question1);
                QuestionCreateActivity.questionList.Add(t); // Add to the end of the list 
                Finish();
            }

        }
    }
}