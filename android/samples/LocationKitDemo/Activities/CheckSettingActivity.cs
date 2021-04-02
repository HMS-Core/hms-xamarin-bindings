using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using HMSLocationKit;
using HMSSample;
using Huawei.Hms.Common;
using Huawei.Hms.Location;
using System;

namespace LocationKitDemo.Activities
{
    [Activity(Label = "CheckSettingActivity")]
    public class CheckSettingActivity : Activity
    {
        #region Fields
        Logger log;

        SettingsClient settingsClient;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_check_setting);
            InitializeViews();
            InitializeFields();
        }

        protected override void OnActivityResult(int requestCode, [Android.Runtime.GeneratedEnum] Result resultCode, Intent data)
        {
            string Tag = "CheckSettingActivity.OnActivityResult";
            base.OnActivityResult(requestCode, resultCode, data);
            if (data == null) return;
            LocationSettingsStates locationSettingsStates = LocationSettingsStates.FromIntent(data);
            log.Info(Tag, $"CheckLocationSettings completed: {locationSettingsStates.LSSToString()}");
            switch (requestCode)
            {
                // Check for the integer request code originally supplied to StartResolutionForResult().
                case 0:
                    switch (resultCode)
                    {
                        case Result.Ok:
                            log.Info(Tag, "User agreed to make required location settings changes.");
                            // Nothing to do. StartLocationupdates() gets called in OnResume again.
                            break;
                        case Result.Canceled:
                            log.Info(Tag, "User chose not to make required location settings changes.");
                            break;
                    }
                    break;
            }
        }

        private void InitializeFields()
        {
            settingsClient = LocationServices.GetSettingsClient(this);
        }

        private void InitializeViews()
        {
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.tv_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);
            FindViewById<Button>(Resource.Id.checkLocationSetting).Click += OnClickCheckSetting;
            Utils.InitDataDisplayView(this, FindViewById<TableLayout>(Resource.Id.check_setting_table_layout_show), "LocationRequest.json");
            Utils.InitDataDisplayView(this, FindViewById<TableLayout>(Resource.Id.check_setting_table_layout_show), "CheckLocationSettings.json");
        }

        private async void OnClickCheckSetting(object sender, EventArgs eventArgs)
        {
            string Tag = "CheckSetting";

            LocationRequest locationRequest = new LocationRequest();
            LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder();
            builder.AddLocationRequest(locationRequest)
                .SetAlwaysShow(false)
                .SetNeedBle(false);
            var locationSettingsResponseTask = settingsClient.CheckLocationSettingsAsync(builder.Build());
            try
            {
                await locationSettingsResponseTask;
                if (locationSettingsResponseTask.IsCompleted && locationSettingsResponseTask.Result != null)
                {
                    LocationSettingsResponse response = locationSettingsResponseTask.Result;
                    LocationSettingsStates locationSettingsStates = response.LocationSettingsStates;
                    log.Info(Tag, $"CheckLocationSettings completed: {locationSettingsStates.LSSToString()}");
                }
            }
            catch (Exception e)
            {
                if (e is ApiException apiException)
                {
                    log.Error(Tag, $"CheckLocationSetting Failed. ErrorMessage: {apiException.Message} ErrorCode: {apiException.StatusCode}");

                    int statuesCode = apiException.StatusCode;
                    if (statuesCode == LocationSettingsStatusCodes.ResolutionRequired)
                        try
                        {
                            //When the StartResolutionForResult is invoked, a dialog box is displayed, asking you to open the corresponding permission.
                            ResolvableApiException resolvableApiException = (ResolvableApiException)e;
                            resolvableApiException.StartResolutionForResult(this, 0);
                        }
                        catch (IntentSender.SendIntentException sendIntentException)
                        {
                            log.Error(Tag, "PendingIntent unable to execute request.");
                        }
                }
                else
                    log.Error(Tag, $"CheckLocationSetting Failed: {e.Message}");
            }
        }
    }
}