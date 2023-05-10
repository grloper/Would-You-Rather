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

namespace WouldYouRather.FireBase
{
    internal class PrivateRoom
    {
        public string myCode { get; set; }
        public int numOfPlayers { get; set; }
        public bool isDone { get; set; }
        public int dicistionTimeSec { get; set; }
        public int finishedPlayers { get; set; }
        public int countQuestion { get; set; }
    }
}