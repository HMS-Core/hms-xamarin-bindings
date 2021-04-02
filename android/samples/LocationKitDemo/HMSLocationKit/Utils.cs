using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Location;
using Java.IO;
using Java.Nio.Charset;
using Java.Util;
using Org.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HMSLocationKit
{
    public static class Utils
    {
        public static void RequestPermissions(Context context)
        {
            // check location permisiion
            if (Build.VERSION.SdkInt <= BuildVersionCodes.P)
            {
                if (ActivityCompat.CheckSelfPermission(context, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                    ActivityCompat.CheckSelfPermission(context, Manifest.Permission.AccessCoarseLocation) != Permission.Granted &&
                    ActivityCompat.CheckSelfPermission(context, "com.huawei.hms.permission.ACTIVITY_RECOGNITION") != Permission.Granted)
                {
                    string[] permissions = {
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.AccessCoarseLocation,
                        "com.huawei.hms.permission.ACTIVITY_RECOGNITION"
                    };
                    ActivityCompat.RequestPermissions((Activity)context, permissions, 1);
                }
            }
            else
            {
                if (ActivityCompat.CheckSelfPermission(context, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                    ActivityCompat.CheckSelfPermission(context, Manifest.Permission.AccessCoarseLocation) != Permission.Granted &&
                    ActivityCompat.CheckSelfPermission(context, "android.permission.ACCESS_BACKGROUND_LOCATION") != Permission.Granted &&
                    ActivityCompat.CheckSelfPermission(context, "android.permission.ACTIVITY_RECOGNITION") != Permission.Granted)
                {
                    string[] permissions = {
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.AccessCoarseLocation,
                        "android.permission.ACCESS_BACKGROUND_LOCATION",
                        "android.permission.ACTIVITY_RECOGNITION"
                    };
                    ActivityCompat.RequestPermissions((Activity)context, permissions, 2);
                }
            }
        }
        public static void RequestLocationPermission(Context context)
        {
            // check location permisiion
            if (Build.VERSION.SdkInt <= BuildVersionCodes.P)
            {
                if (ActivityCompat.CheckSelfPermission(context, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                    ActivityCompat.CheckSelfPermission(context, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
                {
                    string[] permissions = {
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.AccessCoarseLocation
                    };
                    ActivityCompat.RequestPermissions((Activity)context, permissions, 1);
                }
            }
            else
            {
                if (ActivityCompat.CheckSelfPermission(context, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                    ActivityCompat.CheckSelfPermission(context, Manifest.Permission.AccessCoarseLocation) != Permission.Granted &&
                    ActivityCompat.CheckSelfPermission(context, "android.permission.ACCESS_BACKGROUND_LOCATION") != Permission.Granted)
                {
                    string[] permissions = {
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.AccessCoarseLocation,
                        "android.permission.ACCESS_BACKGROUND_LOCATION"
                    };
                    ActivityCompat.RequestPermissions((Activity)context, permissions, 2);
                }
            }
        }
        public static void RequestActivityTransitionPermission(Context context)
        {
            // check location permisiion
            if (Build.VERSION.SdkInt <= BuildVersionCodes.P)
            {
                if (ActivityCompat.CheckSelfPermission(context, "com.huawei.hms.permission.ACTIVITY_RECOGNITION") != Permission.Granted)
                {
                    string[] permissions = { "com.huawei.hms.permission.ACTIVITY_RECOGNITION" };
                    ActivityCompat.RequestPermissions((Activity)context, permissions, 1);
                }
            }
            else
            {
                if (ActivityCompat.CheckSelfPermission(context, "android.permission.ACTIVITY_RECOGNITION") != Permission.Granted)
                {
                    string[] permissions = { "android.permission.ACTIVITY_RECOGNITION" };
                    ActivityCompat.RequestPermissions((Activity)context, permissions, 2);
                }
            }
        }
        public static string AIDToString(this ActivityIdentificationData data)
        {
            string result = string.Empty;

            result += $"IdentificationActivity: {data.IdentificationActivity.ToActivityType()}\n";
            result += $"Possibility: {data.Possibility}";

            return result;
        }
        public static string ACDToString(this ActivityConversionData data)
        {
            string result = string.Empty;

            result += $"ActivityType: {data.ActivityType.ToActivityType()}\n";
            result += $"ConversionType: {data.ConversionType.ToConversionType()}";

            return result;
        }
        public static string LSSToString(this LocationSettingsStates states)
        {
            string result = string.Empty;

            result += $"BlePresent: {states.BlePresent}\n";
            result += $"BleUsable: {states.BleUsable}\n";
            result += $"GnssPresent: {states.GnssPresent}\n";
            result += $"GnssUsable: {states.GnssUsable}\n";
            result += $"LocationPresent: {states.LocationPresent}\n";
            result += $"LocationUsable: {states.LocationUsable}\n";
            result += $"NetworkLocationPresent: {states.NetworkLocationPresent}\n";
            result += $"NetworkLocationUsable: {states.NetworkLocationUsable}\n";
            result += $"HMSLocationPresent: {states.HMSLocationPresent}\n";
            result += $"IsHMSLocationUsable: {states.HMSLocationUsable}";

            return result;
        }
        public static string ToActivityType(this int code)
        {
            Type type = typeof(ActivityIdentificationData);
            FieldInfo[] fieldInfos = type.GetFields();

            string result = fieldInfos.ToList().Where(x => Convert.ToInt32(type.GetField(x.Name).GetValue(null)) == code).First().Name;

            return result;
        }
        public static string ToConversionType(this int code)
        {
            Type type = typeof(ActivityConversionInfo);
            FieldInfo[] fieldInfos = type.GetFields();

            string result = fieldInfos.ToList().Where(x => Convert.ToInt32(type.GetField(x.Name).GetValue(null)) == code).First().Name;

            return result;
        }
        public static void InitDataDisplayView(Context context, TableLayout table, string fileName)
        {
            try
            {
                JSONObject jsonObject = new JSONObject(GetJson(context, fileName));
                IIterator iterator = jsonObject.Keys();
                while (iterator.HasNext)
                {
                    string key = (string)iterator.Next();
                    string value = jsonObject.GetString(key);

                    TableRow tableRow = new TableRow(context);

                    TextView textView = new TextView(context);
                    textView.Text = key;
                    textView.SetTextColor(Color.Gray);
                    textView.Id = context.Resources.GetIdentifier(key + "_key", "id", context.PackageName);
                    tableRow.AddView(textView);

                    EditText editText = new EditText(context);
                    editText.Text = value;
                    editText.Id = context.Resources
                            .GetIdentifier(key + "_value", "id", context.PackageName);
                    editText.SetTextColor(Color.DarkGray);
                    tableRow.AddView(editText);
                    table.AddView(tableRow);
                }
            }
            catch (JSONException e)
            {
                Log.Error("Utils.InitDataDisplayView", $"JSONException occured: {e.Message}");
            }
        }
        public static string GetJson(Context context, string fileName)
        {
            string result = string.Empty;
            try
            {
                AssetManager assetManager = context.Assets;
                using (StreamReader sr = new StreamReader(assetManager.Open(fileName)))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Log.Error("Utils.GetJson", $"Exception occured: {e.Message}");
            }
            return result;
        }
    }
}