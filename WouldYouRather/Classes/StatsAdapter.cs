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
    public class StatsAdapter : BaseAdapter

    {
        private List<Stats> statsList;
        private Context context;

        public StatsAdapter(Context context, List<Stats> statsList)
        {
            this.context = context;
            this.statsList = statsList;
        }

        public override int Count
        {
            get { return statsList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(context).Inflate(Resource.Layout.custom_stats_view, null, false);
            }

            TextView tvRedStats = view.FindViewById<TextView>(Resource.Id.tvRedStats);
            TextView tvBlueStats = view.FindViewById<TextView>(Resource.Id.tvBlueStats);

            Stats stats = statsList[position];
            tvRedStats.Text = stats.RedQuestion+" - " + stats.RedPercentage + "%";
            tvBlueStats.Text = stats.BlueQuestion + " - " + stats.BluePercentage + "%";

            return view;
        }
    }
}