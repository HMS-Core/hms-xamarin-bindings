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
using System.Threading.Tasks;

namespace LocationKitDemo.Activities
{
    [Activity(Label = "SetMockLocationActivity")]
    public class SetMockLocationActivity : Activity
    {
        #region Fields
        Logger log;

        FusedLocationProviderClient fusedLocationProviderClient;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_set_mock_location);
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
            FindViewById<Button>(Resource.Id.location_setMockLocation).Click += OnClickSetMockLocation;
        }

        private async void OnClickSetMockLocation(object sender, EventArgs eventArgs)
        {
            string Tag = "SetMockLocation";

            //Fill in the information sources such as gps and network based on the application situation.
            Location mockLocation = new Location(LocationManager.GpsProvider);
            mockLocation.Longitude = 34.34;
            mockLocation.Latitude = 34.34;

            // Note: To enable the mock function, enable the android.permission.ACCESS_MOCK_LOCATION permission in the AndroidManifest.xml file,
            // and set the application to the mock location app in the device setting.
            Task task = fusedLocationProviderClient.SetMockLocationAsync(mockLocation);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, $"{Tag} is succeeded");
                else
                    log.Error(Tag, $"{Tag} failed: {task.Exception.Message}");

            }
            catch (Exception e)
            {
                log.Error(Tag, $"{Tag} exception: {e.Message}");
            }
        }
    }
}