using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using HMSSample;
using Huawei.Hms.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMSLocationKit
{
    [BroadcastReceiver]
    public class LocationBroadcastReceiver : BroadcastReceiver
    {
        public static readonly string ActionProcessLocation = "com.huawei.hms.location.ACTION_PROCESS_LOCATION";

        public static bool IsListenActivityIdentification = false;

        public static bool IsListenActivityConversion = false;

        private Logger log;
        public override void OnReceive(Context context, Intent intent)
        {
            string Tag = "RequestLocationUpdatesWithIntent";
            if (intent == null) return;
            string action = intent.Action;
            if (ActionProcessLocation != action) return;

            log = Logger.Instance;

            string logMessage = string.Empty;

            ActivityConversionResponse activityConversionResponse = ActivityConversionResponse.GetDataFromIntent(intent);
            if (activityConversionResponse != null && IsListenActivityConversion)
            {
                List<ActivityConversionData> list = activityConversionResponse.ActivityConversionDatas.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    log.Info(Tag, $"ActivityConversionEvent[{i}]\n{list[i].ACDToString()}");
                }
            }

            ActivityIdentificationResponse activityIdentificationResponse = ActivityIdentificationResponse.GetDataFromIntent(intent);
            if (activityIdentificationResponse != null && IsListenActivityIdentification)
            {
                log.Info(Tag, "activityIdentificationResponse:", activityIdentificationResponse);
                //List<ActivityIdentificationData> list = activityIdentificationResponse.ActivityIdentificationDatas.ToList();
                //log.Info("ActivityIdentification",);
            }
            if (LocationResult.HasResult(intent))
            {
                LocationResult result = LocationResult.ExtractResult(intent);
                if (result != null)
                {
                    List<Location> locations = result.Locations.ToList();
                    if (locations.Count > 0)
                    {
                        logMessage += "[Longitude,Latitude,Accuracy]:";
                        foreach (Location location in locations)
                        {
                            logMessage += $"{System.Environment.NewLine}{location.Longitude}, {location.Latitude}, {location.Accuracy}";
                        }
                    }
                }
            }

            //Processing LocationAvailability information
            if (LocationAvailability.HasLocationAvailability(intent))
            {
                LocationAvailability locationAvailability =
                    LocationAvailability.ExtractLocationAvailability(intent);
                if (locationAvailability != null)
                {
                    logMessage += $"[locationAvailability]: {locationAvailability.IsLocationAvailable}";
                }
            }

            if (logMessage != string.Empty && log != null)
                log.Info(Tag, logMessage);
        }
        public static void AddConversionListener()
        {
            IsListenActivityConversion = true;
        }
        public static void RemoveConversionListener()
        {
            IsListenActivityConversion = false;
        }
        public static void AddIdentificationListener()
        {
            IsListenActivityIdentification = true;
        }
        public static void RemoveIdentificationListener()
        {
            IsListenActivityIdentification = false;
        }
    }
}