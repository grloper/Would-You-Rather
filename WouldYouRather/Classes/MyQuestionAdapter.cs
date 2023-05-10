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

namespace WouldYouRather.Classes
{
    internal class MyQuestionAdapter : BaseAdapter<QuestionClass>
    {
        private Context context;
        private List<QuestionClass> objects;

        public MyQuestionAdapter(Context context, List<QuestionClass> objects)
        {
            this.context = context;
            this.objects = objects;
        }
      
        public override long GetItemId(int position)
        {
            return position;
        }

        public override QuestionClass this[int position]
        {
            get { return this.objects[position]; }
        }
        public List<QuestionClass> GetList()
        {
            return this.objects;
        }
        public override int Count
        {
            get { return this.objects.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Android.Views.LayoutInflater layoutInflater = ((QuestionCreateActivity)context).LayoutInflater;
            Android.Views.View view = layoutInflater.Inflate(Resource.Layout.custom_view, parent, false);
            TextView tvOption = view.FindViewById<TextView>(Resource.Id.tvOption);
            TextView tvQ1 = view.FindViewById<TextView>(Resource.Id.tvQ1);
            TextView tvQ2 = view.FindViewById<TextView>(Resource.Id.tvQ2);
            QuestionClass temp = objects[position];
            if (temp != null)
            {
                tvOption.Text = "Option " + (position + 1); // update the option name to the correct index
                tvQ1.Text = temp.GetOption1();
                tvQ2.Text = temp.GetOption2();
            }
            return view;
        }

    }
}