using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using HMSSample;
using Huawei.Hms.Location;
using LocationKitDemo.HMSLocationKit;
using System.Collections.Generic;
using System.Linq;

namespace LocationKitDemo.Activities
{
    [Activity(Label = "OperateGeoFenceActivity")]
    public class OperateGeoFenceActivity : Activity
    {
        #region Fields
        Logger log;
        TextView geoFenceData;
        TextView geoRequestData;
        EditText trigger;

        GeofenceService geofenceService;
        List<Request> requestList;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_operate_geo_fence);
            InitializeViews();
            InitializeFields();
        }

        private void InitializeFields()
        {
            geofenceService = LocationServices.GetGeofenceService(this);
            requestList = new List<Request>();
        }

        private void InitializeViews()
        {
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.tv_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);
            if (Logger.GeofenceInstance != log)
                log.GeofenceInitialize();

            geoFenceData = FindViewById<TextView>(Resource.Id.GeoFenceData);
            geoRequestData = FindViewById<TextView>(Resource.Id.GeoRequestData);
            trigger = FindViewById<EditText>(Resource.Id.trigger);

            FindViewById<Button>(Resource.Id.CreateGeofence).Click += OnClickCreateGeoFence;
            FindViewById<Button>(Resource.Id.removeGeofence).Click += OnClickRemoveGeoFence;
            FindViewById<Button>(Resource.Id.getGeoFenceData).Click += OnClickGeoFenceData;
            FindViewById<Button>(Resource.Id.sendRequest).Click += OnClickSendRequest;
            FindViewById<Button>(Resource.Id.sendRequestWithNew).Click += OnClickSendRequestWithNewIntent;
            FindViewById<Button>(Resource.Id.GetRequestMessage).Click += OnClickGetRequestMessage;
            FindViewById<Button>(Resource.Id.removeWithIntent).Click += OnClickRemoveWithIntent;
            FindViewById<Button>(Resource.Id.removeWithID).Click += OnClickRemoveWithID;
        }

        private async void OnClickRemoveWithID(object sender, System.EventArgs eventArgs)
        {
            string Tag = "RemoveWithID";
            string input = FindViewById<EditText>(Resource.Id.removeWithIDInput).Text;
            List<string> ids = new List<string>(input.Split(' '));
            var task = geofenceService.DeleteGeofenceListAsync(ids);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, "Delete GeoFence succeeded.");
                else
                    log.Error(Tag, $"Delete GeoFence failed: {task.Exception.Message}");
            }
            catch (System.Exception e)
            {
                log.Error(Tag, $"Delete GeoFence exception: {e.Message}");
            }
            foreach (Request request in requestList)
            {
                request.RemoveID(ids.ToArray());
            }
        }

        private async void OnClickRemoveWithIntent(object sender, System.EventArgs eventArgs)
        {
            string Tag = "RemoveWithIntent";
            int id;
            if (int.TryParse(FindViewById<EditText>(Resource.Id.removeWithPendingIntentInput).Text, out id))
            {
                Request request = requestList.Where(x => x.requestCode == id).First();
                requestList.Remove(request);

                PendingIntent pendingIntent = request.pendingIntent;
                if (pendingIntent == null)
                {
                    geoRequestData.Text = "No such intent.";
                    return;
                }
                var task = geofenceService.DeleteGeofenceListAsync(pendingIntent);
                try
                {
                    await task;
                    if (task.IsCompleted)
                        log.Info(Tag, "Delete GeoFence succeeded.");
                    else
                        log.Error(Tag, $"Delete GeoFence failed: {task.Exception.Message}");
                }
                catch (System.Exception e)
                {
                    log.Error(Tag, $"Delete GeoFence exception: {e.Message}");
                }
            }
        }

        private void OnClickGetRequestMessage(object sender, System.EventArgs e)
        {
            string text = string.Empty;
            foreach (Request request in requestList)
                text += request.ToString();

            if (text == string.Empty) text = "No Request.";
            geoRequestData.Text = text;
        }

        private async void OnClickSendRequestWithNewIntent(object sender, System.EventArgs eventArgs)
        {
            string Tag = "GeoFenceSendRequestWithNewIntent";
            var geofences = HMSGeofenceData.Geofences;

            if (geofences == null || geofences.Count == 0)
            {
                geoRequestData.Text = "No new request to add.";
                return;
            }
            if (requestList.Any(x => x.IsIDExist()))
            {
                geoRequestData.Text = "ID already exist, please remove and add it again.";
                return;
            }
            GeofenceRequest.Builder geofenceRequest = new GeofenceRequest.Builder();
            geofenceRequest.CreateGeofenceList(geofences);
            int intTrigger;
            if (int.TryParse(trigger.Text, out intTrigger))
            {
                geofenceRequest.SetInitConversions(intTrigger);
                log.Info(Tag, $"Trigger is {intTrigger}");
            }
            else
            {
                geofenceRequest.SetInitConversions(4);
                log.Info(Tag, "Trigger is 4");
            }
            PendingIntent pendingIntent = GetPendingIntent();
            requestList.Add(new Request(pendingIntent, HMSGeofenceData.RequestCode, geofences));

            var task = geofenceService.CreateGeofenceListAsync(geofenceRequest.Build(), pendingIntent);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, "Add GeoFence succeeded.");
                else
                    log.Error(Tag, $"Add GeoFence failed: {task.Exception.Message}");
            }
            catch (System.Exception e)
            {
                log.Error(Tag, $"Add GeoFence exception: {e.Message}");
            }
            HMSGeofenceData.CreateNewList();
        }

        private async void OnClickSendRequest(object sender, System.EventArgs eventArgs)
        {
            string Tag = "GeoFenceSendRequest";
            var geofences = HMSGeofenceData.Geofences;

            if (geofences.Count == 0)
            {
                geoRequestData.Text = "No new request to add.";
                return;
            }
            if (requestList.Count == 0)
            {
                geoRequestData.Text = "No PendingIntent to send.";
                return;
            }
            if (requestList.Any(x => x.IsIDExist()))
            {
                geoRequestData.Text = "ID already exist, please remove and add it again.";
                return;
            }
            GeofenceRequest.Builder geofenceRequest = new GeofenceRequest.Builder();
            geofenceRequest.CreateGeofenceList(geofences);
            int intTrigger;
            if (int.TryParse(trigger.Text, out intTrigger))
            {
                geofenceRequest.SetInitConversions(intTrigger);
                log.Info(Tag, $"Trigger is {intTrigger}");
            }
            else
            {
                geofenceRequest.SetInitConversions(5);
                log.Info(Tag, "Trigger is 5");
            }
            Request tmp = requestList[requestList.Count - 1];
            PendingIntent pendingIntent = tmp.pendingIntent;
            requestList.Add(new Request(pendingIntent, tmp.requestCode, geofences));

            var task = geofenceService.CreateGeofenceListAsync(geofenceRequest.Build(), pendingIntent);
            try
            {
                await task;
                if (task.IsCompleted)
                    log.Info(Tag, "Add GeoFence succeeded.");
                else
                    log.Error(Tag, $"Add GeoFence failed: {task.Exception.Message}");
            }
            catch (System.Exception e)
            {
                log.Error(Tag, $"Add GeoFence exception: {e.Message}");
            }
            HMSGeofenceData.CreateNewList();
        }

        private void OnClickGeoFenceData(object sender, System.EventArgs e)
        {
            string text = string.Empty;
            var geofences = HMSGeofenceData.Geofences;
            if (geofences.Count == 0) text = "No GeoFence Data.";
            foreach (IGeofence geofence in geofences)
            {
                text += $"Unique ID is {geofence.UniqueId}\n";
            }
            geoFenceData.Text = text;
        }

        private void OnClickRemoveGeoFence(object sender, System.EventArgs e)
        {
            HMSGeofenceData.CreateNewList();
        }

        private void OnClickCreateGeoFence(object sender, System.EventArgs e)
        {
            StartActivity(typeof(GeoFenceActivity));
        }

        private PendingIntent GetPendingIntent()
        {
            Intent intent = new Intent(this, typeof(GeoFenceBroadcastReceiver));
            intent.SetAction(GeoFenceBroadcastReceiver.ActionProcessLocation);
            HMSGeofenceData.RequestCode++;
            return PendingIntent.GetBroadcast(this, HMSGeofenceData.RequestCode, intent, PendingIntentFlags.UpdateCurrent);
        }
    }
    class Request
    {
        public PendingIntent pendingIntent;
        public int requestCode;
        public List<IGeofence> geofences;

        public Request(PendingIntent pendingIntent, int requestCode, List<IGeofence> geofences)
        {
            this.pendingIntent = pendingIntent;
            this.requestCode = requestCode;
            this.geofences = geofences;
        }

        public override string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < geofences.Count; i++)
            {
                result += $"PendingIntent: {requestCode} UniqueID: {geofences[i].UniqueId}\n";
            }
            return result;
        }

        public bool IsIDExist()
        {
            List<IGeofence> list = HMSGeofenceData.Geofences;
            for (int j = 0; j < list.Count; j++)
            {
                string s = list[j].UniqueId;
                foreach (IGeofence geofence in geofences)
                {
                    if (s.Equals(geofence.UniqueId))
                    {
                        return true;
                        //id already exist
                    }
                }
            }
            return false;
        }

        public void RemoveID(string[] str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                string s = str[i];
                for (int j = geofences.Count - 1; j >= 0; j--)
                {
                    if (s.Equals(geofences[j].UniqueId))
                    {
                        geofences.RemoveAt(j);
                    }
                }
            }
        }
    }
}