using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using System;
using LocationKitDemo.Activities;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Content;
using Android.Support.Design.Widget;
using System.Collections.Generic;
using HMSSample;
using HMSLocationKit;

namespace LocationKitDemo
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            InitializeViews();
            Utils.RequestPermissions(this);
        }

        private void InitializeViews()
        {
            FindViewById<Button>(Resource.Id.location_requestLocationUpdatesWithIntent).Click += OnClickRequestLocationUpdatesWithIntent;
            FindViewById<Button>(Resource.Id.location_requestLocationUpdatesWithCallback).Click += OnClickRequestLocationUpdatesWithCallback;
            FindViewById<Button>(Resource.Id.location_setMockMode).Click += OnClickSetMockMode;
            FindViewById<Button>(Resource.Id.location_setMockLocation).Click += OnClickSetMockLocation;
            FindViewById<Button>(Resource.Id.location_getLocationAvailability).Click += OnClickLocationAvailability;
            FindViewById<Button>(Resource.Id.location_getLastLocation).Click += OnClickLastLocation;
            FindViewById<Button>(Resource.Id.location_activity_update).Click += OnClickActivityRecognitionUpdate;
            FindViewById<Button>(Resource.Id.location_activity_transition_update).Click += OnClickActivityTransitionUpdate;
            FindViewById<Button>(Resource.Id.locationHD).Click += OnClickLocationHD;
            FindViewById<Button>(Resource.Id.check_setting).Click += OnClickCheckSettings;
            FindViewById<Button>(Resource.Id.getNavigationContextState).Click += OnClickNavigationContextState;
            FindViewById<Button>(Resource.Id.GeoFence).Click += OnClickGeoFence;
        }

        private void OnClickGeoFence(object sender, EventArgs e)
        {
            StartActivity(typeof(OperateGeoFenceActivity));
        }

        private void OnClickNavigationContextState(object sender, EventArgs e)
        {
            StartActivity(typeof(NavigationContextStateActivity));
        }

        private void OnClickCheckSettings(object sender, EventArgs e)
        {
            StartActivity(typeof(CheckSettingActivity));
        }

        private void OnClickLocationHD(object sender, EventArgs e)
        {
            StartActivity(typeof(RequestLocationUpdatesHDWithCallbackActivity));
        }

        private void OnClickActivityTransitionUpdate(object sender, EventArgs e)
        {
            StartActivity(typeof(ActivityConversionActivity));
        }

        private void OnClickActivityRecognitionUpdate(object sender, EventArgs e)
        {
            StartActivity(typeof(ActivityIdentificationActivity));
        }

        private void OnClickLastLocation(object sender, EventArgs e)
        {
            StartActivity(typeof(LastLocationActivity));
        }

        private void OnClickLocationAvailability(object sender, EventArgs e)
        {
            StartActivity(typeof(LocationAvailabilityActivity));
        }

        private void OnClickSetMockLocation(object sender, EventArgs e)
        {
            StartActivity(typeof(SetMockLocationActivity));
        }

        private void OnClickSetMockMode(object sender, EventArgs e)
        {
            StartActivity(typeof(SetMockModeActivity));
        }

        private void OnClickRequestLocationUpdatesWithIntent(object sender, EventArgs e)
        {
            StartActivity(typeof(RequestLocationUpdatesWithIntentActivity));
        }

        private void OnClickRequestLocationUpdatesWithCallback(object sender, EventArgs e)
        {
            StartActivity(typeof(RequestLocationUpdatesWithCallbackActivity));
        }
    }
}