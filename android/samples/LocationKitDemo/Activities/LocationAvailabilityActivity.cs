using Android.App;
using Android.Content;
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
    [Activity(Label = "LocationAvailabilityActivity")]
    public class LocationAvailabilityActivity : Activity
    {
        #region Fields
        Logger log;

        FusedLocationProviderClient fusedLocationProviderClient;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_get_location_availability);
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
            FindViewById<Button>(Resource.Id.location_getLocationAvailability).Click += OnClickGetLocationAvailability;
        }

        private async void OnClickGetLocationAvailability(object sender, EventArgs eventArgs)
        {
            string Tag = "LocationAvailability";
            var locationAvailabilityTask = fusedLocationProviderClient.GetLocationAvailabilityAsync();
            try
            {
                await locationAvailabilityTask;
                if (locationAvailabilityTask.IsCompleted && locationAvailabilityTask.Result != null)
                {
                    LocationAvailability locationAvailability = locationAvailabilityTask.Result;
                    log.Info(Tag, "Location Availability:", locationAvailability);
                }
                else
                    log.Error(Tag, $"GetLocationAvailabilityAsync failed: {locationAvailabilityTask.Exception.Message}");
            }
            catch (Exception e)
            {
                log.Error(Tag, $"GetLocationAvailabilityAsync exception: {e.Message}");
            }
        }
    }
}