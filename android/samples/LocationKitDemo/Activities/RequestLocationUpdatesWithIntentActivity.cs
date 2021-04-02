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
    [Activity(Label = "RequestLocationUpdatesWithIntentActivity")]
    public class RequestLocationUpdatesWithIntentActivity : Activity
    {
        #region Fields
        Logger log;

        LocationRequest locationRequest;
        FusedLocationProviderClient fusedLocationProviderClient;
        SettingsClient settingsClient;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_request_location_updates_intent);
            InitializeViews();
            InitializeFields();
        }

        protected override void OnDestroy()
        {
            // Removed when the location update is no longer required.
            OnClickRemoveLocationUpdates(this, null);
            base.OnDestroy();
        }

        private void InitializeFields()
        {
            locationRequest = new LocationRequest();
            //Sets the interval for location update (unit: Millisecond)
            locationRequest.SetInterval(10000);
            //Sets the priority
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);

            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);

            settingsClient = LocationServices.GetSettingsClient(this);
        }

        private void InitializeViews()
        {
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.tv_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);
            if (Logger.Instance != log)
                log.Initialize();
            FindViewById<Button>(Resource.Id.location_requestLocationUpdatesWithIntent).Click += OnClickRequestLocationUpdates;
            FindViewById<Button>(Resource.Id.location_removeLocationUpdatesWithIntent).Click += OnClickRemoveLocationUpdates;
        }

        private async void OnClickRequestLocationUpdates(object sender, EventArgs eventArgs)
        {
            string Tag = "RequestLocationUpdates";

            LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder();
            builder.AddLocationRequest(locationRequest);
            LocationSettingsRequest request = builder.Build();
            //Before requesting location update, invoke CheckLocationSettings to check device settings.
            var locationSettingsResponseTask = settingsClient.CheckLocationSettingsAsync(request);
            try
            {
                await locationSettingsResponseTask;
                if (locationSettingsResponseTask.IsCompleted && locationSettingsResponseTask != null)
                {
                    LocationSettingsResponse response = locationSettingsResponseTask.Result;
                    var requestLocationUpdatesTask = fusedLocationProviderClient.RequestLocationUpdatesAsync(locationRequest, GetPendingIntent());
                    try
                    {
                        await requestLocationUpdatesTask;
                        if (requestLocationUpdatesTask.IsCompleted)
                            log.Info(Tag, "RequestLocationUpdates with intent succeeded.");
                        else log.Error(Tag, $"RequestLocationUpdates with intent failed: {requestLocationUpdatesTask.Exception.Message}");
                    }
                    catch (Exception e)
                    {
                        if (e is ApiException apiException)
                        {
                            if (apiException.StatusCode == 10803)
                            {
                                //PermissionDenied
                                Utils.RequestLocationPermission(this);
                            }
                        }
                        else
                            log.Error(Tag, $"RequestLocationUpdates with intent exception: {e.Message}");
                    }
                }
                else
                {
                    var exception = locationSettingsResponseTask.Exception;
                    log.Error(Tag, $"CheckLocationSetting Failed: {exception.Message}");
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
                            //When the startResolutionForResult is invoked, a dialog box is displayed, asking you to open the corresponding permission.
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

        private async void OnClickRemoveLocationUpdates(object sender, EventArgs eventArgs)
        {
            string Tag = "RemoveLocationUpdates";
            var task = fusedLocationProviderClient.RemoveLocationUpdatesAsync(GetPendingIntent());
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, "RemoveLocationUpdates with intent succeeded.");
                else
                    log.Error(Tag, $"RemoveLocationUpdates with intent failed:{task.Exception.Message}");
            }
            catch (Exception e)
            {
                log.Error(Tag, $"RemoveLocationUpdates with intent failed:{e.Message}");
            }
        }

        private PendingIntent GetPendingIntent()
        {
            Intent intent = new Intent(this, typeof(LocationBroadcastReceiver));
            intent.SetAction(LocationBroadcastReceiver.ActionProcessLocation);
            return PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
        }
    }
}