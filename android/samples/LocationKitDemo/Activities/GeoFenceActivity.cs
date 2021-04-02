using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HMSSample;
using Huawei.Hms.Common;
using Huawei.Hms.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocationKitDemo.Activities
{
    [Activity(Label = "GeoFenceActivity")]
    public class GeoFenceActivity : Activity
    {
        #region Fields
        Logger log;

        LocationCallback locationCallback;
        LocationRequest locationRequest;
        FusedLocationProviderClient fusedLocationProviderClient;
        SettingsClient settingsClient;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_geo_fence);
            InitializeViews();
            InitializeFields();
        }

        protected override void OnDestroy()
        {
            // Removed when the location update is no longer required.
            RemoveLocationUpdates();
            base.OnDestroy();
        }

        private void InitializeFields()
        {
            locationCallback = new GeoFenceLocationCallback();

            locationRequest = new LocationRequest();
            locationRequest.SetInterval(5000);
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);

            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);

            settingsClient = LocationServices.GetSettingsClient(this);
        }

        private void InitializeViews()
        {
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.tv_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);
            if (Logger.Instance != log)
                log.Initialize();
            FindViewById<Button>(Resource.Id.getCurrentLocation).Click += OnClickGetCurrentLocation;
            FindViewById<Button>(Resource.Id.geofence_btn).Click += OnClickCreateGeoFence;
            FindViewById<Button>(Resource.Id.showGeoList).Click += OnClickShowGeoList;
        }

        private void OnClickShowGeoList(object sender, EventArgs e)
        {
            string Tag = "GeoFence";
            var geofences = HMSGeofenceData.Geofences;
            if (geofences != null && geofences.Count > 0)
                foreach (IGeofence geofence in geofences)
                    log.Info(Tag, $"Unique ID is {geofence.UniqueId}");
            else
                log.Info(Tag, "No GeoFence Data.");
        }

        private void OnClickCreateGeoFence(object sender, EventArgs e)
        {
            HMSGeofenceData data = new HMSGeofenceData();
            data.Longitude = double.Parse(FindViewById<EditText>(Resource.Id.setlongitude).Text);
            data.Latitude = double.Parse(FindViewById<EditText>(Resource.Id.setlatitude).Text);
            data.Radius = float.Parse(FindViewById<EditText>(Resource.Id.setradius).Text);
            data.UniqueId = FindViewById<EditText>(Resource.Id.setUniqueId).Text;
            data.Conversions = int.Parse(FindViewById<EditText>(Resource.Id.setConversions).Text);
            data.ValidContinueTime = long.Parse(FindViewById<EditText>(Resource.Id.setValidContinueTime).Text);
            data.DwellDelayTime = int.Parse(FindViewById<EditText>(Resource.Id.setDwellDelayTime).Text);
            data.NotificationInterval = int.Parse(FindViewById<EditText>(Resource.Id.setNotificationInterval).Text);

            if (HMSGeofenceData.Geofences == null)
                HMSGeofenceData.CreateNewList();
            data.AddToList();
        }

        private async void OnClickGetCurrentLocation(object sender, EventArgs eventArgs)
        {
            string Tag = "RequestLocationUpdates";

            if (locationRequest.NumUpdates != 1) locationRequest.SetNumUpdates(1);
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
                    var requestLocationUpdatesTask = fusedLocationProviderClient.RequestLocationUpdatesAsync(locationRequest, locationCallback, Looper.MainLooper);
                    try
                    {
                        await requestLocationUpdatesTask;
                        if (requestLocationUpdatesTask.IsCompleted)
                            log.Info(Tag, "RequestLocationUpdates with callback succeeded.");
                        else log.Error(Tag, $"RequestLocationUpdates with callback failed: {requestLocationUpdatesTask.Exception.Message}");
                    }
                    catch (Exception e)
                    {
                        log.Error(Tag, $"RequestLocationUpdates with callback failed: {e.Message}");
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

        private async void RemoveLocationUpdates()
        {
            string Tag = "RemoveLocationUpdates";
            var task = fusedLocationProviderClient.RemoveLocationUpdatesAsync(locationCallback);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, "RemoveLocationUpdates with callback succeeded.");
                else
                    log.Error(Tag, $"RemoveLocationUpdates with callback failed:{task.Exception.Message}");
            }
            catch (Exception e)
            {
                log.Error(Tag, $"RemoveLocationUpdates with callback failed:{e.Message}");
            }
        }
    }
    public class HMSGeofenceData
    {
        private static int requestCode;
        public static int RequestCode { get => requestCode; set => requestCode = value; }
        private static List<IGeofence> geofences;
        public static List<IGeofence> Geofences
        {
            get
            {
                return geofences;
            }
            private set => geofences = value;
        }
        public static void CreateNewList() => Geofences = new List<IGeofence>();
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public float Radius { get; set; }
        public string UniqueId { get; set; }
        public int Conversions { get; set; }
        public long ValidContinueTime { get; set; }
        public int DwellDelayTime { get; set; }
        public int NotificationInterval { get; set; }
        public HMSGeofenceData()
        {
        }
        public void AddToList()
        {
            Logger log = Logger.Instance;
            string Tag = "HMSGeofenceData";
            if (!IsIdUnique())
            {
                log.Error(Tag, "Add GeoFence failed: not UniqueId");
                return;
            }

            GeofenceBuilder builder = new GeofenceBuilder();
            builder.SetRoundArea(Latitude, Longitude, Radius)
                .SetUniqueId(UniqueId)
                .SetConversions(Conversions)
                .SetValidContinueTime(ValidContinueTime)
                .SetDwellDelayTime(DwellDelayTime)
                .SetNotificationInterval(NotificationInterval);
            geofences.Add(builder.Build());
            log.Info(Tag, "Add GeoFence succeeded.");
        }
        private bool IsIdUnique()
        {
            if (geofences == null) return true;
            if (UniqueId == null || UniqueId == string.Empty) return false;
            foreach (IGeofence geofence in geofences)
                if (geofence.UniqueId == UniqueId)
                    return false;
            return true;
        }
    }
    public class GeoFenceLocationCallback : LocationCallback
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