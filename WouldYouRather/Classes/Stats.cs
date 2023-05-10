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
    public class Stats
    {
        public string RedQuestion { get; set; }
        public string BlueQuestion { get; set; }
        public float RedPercentage { get; set; }
        public float BluePercentage { get; set; }
    }

}