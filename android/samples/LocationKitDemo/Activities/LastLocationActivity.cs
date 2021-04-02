using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HMSSample;
using Huawei.Hms.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocationKitDemo.Activities
{
    [Activity(Label = "LastLocationActivity")]
    public class LastLocationActivity : Activity
    {
        #region Fields
        Logger log;

        FusedLocationProviderClient fusedLocationProviderClient;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_get_last_location);
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
            FindViewById<Button>(Resource.Id.location_getLastLocation).Click += OnClickGetLastLocation;
        }

        private async void OnClickGetLastLocation(object sender, EventArgs eventArgs)
        {
            string Tag = "LastLocation";
            var lastLocationTask = fusedLocationProviderClient.GetLastLocationAsync();
            try
            {
                await lastLocationTask;
                if (lastLocationTask.IsCompleted && lastLocationTask.Result != null)
                {
                    Location location = lastLocationTask.Result;
                    if (location == null)
                        log.Info(Tag, "Last Location is null.");
                    else
                        log.Info("Longitude,Latitude", $"{location.Longitude},{location.Latitude}");
                }
                else
                    log.Error(Tag, $"GetLastLocationAsync failed: {lastLocationTask.Exception.Message}");
            }
            catch (Exception e)
            {
                log.Error(Tag, $"GetLastLocationAsync exception: {e.Message}");
            }
        }
    }
}