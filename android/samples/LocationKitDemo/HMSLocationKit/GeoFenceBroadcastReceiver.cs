using Android.Content;
using Android.Locations;
using HMSSample;
using Huawei.Hms.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocationKitDemo.HMSLocationKit
{
    [BroadcastReceiver]
    public class GeoFenceBroadcastReceiver : BroadcastReceiver
    {
        public static readonly string ActionProcessLocation = "com.huawei.hmssample.geofence.GeoFenceBroadcastReceiver.ACTION_PROCESS_LOCATION";
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent != null)
            {
                string action = intent.Action;
                StringBuilder sb = new StringBuilder();
                string next = Environment.NewLine;
                if (ActionProcessLocation.Equals(action))
                {
                    GeofenceData geofenceData = GeofenceData.GetDataFromIntent(intent);
                    if (geofenceData != null)
                    {
                        int errorCode = geofenceData.ErrorCode;
                        int conversion = geofenceData.Conversion;
                        List<IGeofence> list = geofenceData.ConvertingGeofenceList.ToList();
                        Location myLocation = geofenceData.ConvertingLocation;
                        bool status = geofenceData.IsSuccess;
                        sb.Append($"ErrorCode: {errorCode}{next}");
                        sb.Append($"Conversion: {conversion}{next}");
                        if (list != null)
                        {
                            foreach (IGeofence geofence in list)
                            {
                                sb.Append($"GeoFence id: {geofence.UniqueId}{next}");
                            }
                        }
                        if (myLocation != null)
                        {
                            sb.Append($"location is : {myLocation.Longitude} {myLocation.Latitude}{next}");
                        }
                        sb.Append("is successful :" + status);
                        Logger.GeofenceInstance.Info("GeoFenceBroadcastReceiver", sb.ToString());
                    }
                }
            }
        }
    }
}