using Android.App;
using Android.Locations;
using Android.OS;
using Android.Widget;
using HMSLocationKit;
using HMSSample;
using Huawei.Hms.Location;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocationKitDemo.Activities
{
    [Activity(Label = "RequestLocationUpdatesHDWithCallbackActivity")]
    public class RequestLocationUpdatesHDWithCallbackActivity : Activity
    {
        #region Fields
        Logger log;
        PendingIntent pendingIntent;

        FusedLocationProviderClient fusedLocationProviderClient;
        LocationHDCallback locationHDCallback;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_hms_hd);
            InitializeViews();
            InitializeFields();
        }

        private void InitializeFields()
        {
            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
        }

        private void InitializeViews()
        {
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.tv_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);
            if (Logger.Instance != log)
                log.Initialize();
            FindViewById<Button>(Resource.Id.btn_hd).Click += OnClickGetLocationWithHD;
            FindViewById<Button>(Resource.Id.btn_remove_hd).Click += OnClickRemoveLocationWithHD;
            Utils.InitDataDisplayView(this, FindViewById<TableLayout>(Resource.Id.callback_table_layout_show), "LocationRequest.json");
        }

        private async void OnClickGetLocationWithHD(object sender, EventArgs eventArgs)
        {
            string Tag = "GetLocationWithHD";

            LocationRequest request = new LocationRequest();
            SetLocationRequest(request);

            locationHDCallback = new LocationHDCallback();
            var task = fusedLocationProviderClient.RequestLocationUpdatesExAsync(request, locationHDCallback, Looper.MainLooper);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, "RequestLocationUpdatesEx with callback succeeded.");
                else log.Error(Tag, $"RequestLocationUpdatesEx with callback failed: {task.Exception.Message}");
            }
            catch (Exception e)
            {
                log.Error(Tag, $"RequestLocationUpdatesEx with callback failed: {e.Message}");
            }
        }

        private async void OnClickRemoveLocationWithHD(object sender, EventArgs eventArgs)
        {
            string Tag = "RemoveLocationUpdatesWithHD";
            var task = fusedLocationProviderClient.RemoveLocationUpdatesAsync(locationHDCallback);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, "RequestLocationUpdatesEx with callback succeeded.");
                else
                    log.Error(Tag, $"RequestLocationUpdatesEx with callback failed:{task.Exception.Message}");
            }
            catch (Exception e)
            {
                log.Error(Tag, $"RequestLocationUpdatesEx with callback failed:{e.Message}");
            }
        }

        private void SetLocationRequest(LocationRequest request)
        {
            TableLayout table = FindViewById<TableLayout>(Resource.Id.callback_table_layout_show);
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            TableRow[] rows = new TableRow[table.ChildCount];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = (TableRow)table.GetChildAt(i);
                paramList[((TextView)rows[i].GetChildAt(0)).Text] = ((EditText)rows[i].GetChildAt(1)).Text;
            }
            request.SetPriority(int.Parse(paramList[LocationRequestConstants.Priority]));
            request.SetInterval(long.Parse(paramList[LocationRequestConstants.Interval]));
            request.SetFastestInterval(long.Parse(paramList[LocationRequestConstants.FastestInterval]));
            request.SetExpirationTime(long.Parse(paramList[LocationRequestConstants.ExpirationTime]));
            request.SetExpirationDuration(long.Parse(paramList[LocationRequestConstants.ExpirationDuration]));
            request.SetNumUpdates(int.Parse(paramList[LocationRequestConstants.NumUpdates]));
            request.SetSmallestDisplacement(float.Parse(paramList[LocationRequestConstants.SmallestDisplacement]));
            request.SetMaxWaitTime(long.Parse(paramList[LocationRequestConstants.MaxWaitTime]));
            request.SetNeedAddress(bool.Parse(paramList[LocationRequestConstants.NeedAddress]));
            request.SetLanguage(paramList[LocationRequestConstants.Language]);
            request.SetCountryCode(paramList[LocationRequestConstants.CountryCode]);
        }
    }
    public class LocationHDCallback : LocationCallback
    {
        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            if (locationAvailability != null)
            {
                bool flag = locationAvailability.IsLocationAvailable;
                Logger.Instance.Info("OnLocationAvailability", $"OnLocationAvailability IsLocationAvailable:{flag}");
            }
        }
        public override void OnLocationResult(LocationResult locationResult)
        {
            if (locationResult != null)
            {
                List<Location> locations = locationResult.Locations.ToList();
                if (locations.Count != 0)
                    foreach (Location location in locations)
                        Logger.Instance.Info("OnLocationResult", $"[Longitude,Latitude,Accuracy]:{System.Environment.NewLine}{location.Longitude}, {location.Latitude}, {location.Accuracy}");

            }
        }
    }
}