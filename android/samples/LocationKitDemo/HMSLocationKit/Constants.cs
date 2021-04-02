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

namespace LocationKitDemo
{
    public class LocationRequestConstants
    {
        public const string Priority = "Priority";
        public const string Interval = "Interval";
        public const string FastestInterval = "FastestInterval";
        public const string IsFastestIntervalExplicitlySet = "IsFastestIntervalExplicitlySet";
        public const string ExpirationTime = "ExpirationTime";
        public const string ExpirationDuration = "ExpirationDuration";
        public const string NumUpdates = "NumUpdates";
        public const string SmallestDisplacement = "SmallestDisplacement";
        public const string MaxWaitTime = "MaxWaitTime";
        public const string NeedAddress = "NeedAddress";
        public const string Language = "Language";
        public const string CountryCode = "CountryCode";
    }
}