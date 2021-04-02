/*
 		Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

		Licensed under the Apache License, Version 2.0 (the "License");
		you may not use this file except in compliance with the License.
		You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

		Unless required by applicable law or agreed to in writing, software
		distributed under the License is distributed on an "AS IS" BASIS,
		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
		See the License for the specific language governing permissions and
		limitations under the License.
 */
using Android.App;
using Android.Locations;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Kit.Awareness;
using Huawei.Hms.Kit.Awareness.Base;
using Huawei.Hms.Kit.Awareness.Capture;
using Huawei.Hms.Kit.Awareness.Status;
using Huawei.Hms.Kit.Awareness.Status.Weather;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AwarenessKitDemo.HMSSample;

namespace AwarenessKitDemo.Activities
{
    [Activity(Label = "CaptureActivity")]
    public class CaptureActivity : Activity
    {
        Logger log;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_capture);

            InitializeButtons();

            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);
        }

        public void InitializeButtons()
        {
            FindViewById<Button>(Resource.Id.capture_getTimeCategories).Click += GetTimeCategories;
            FindViewById<Button>(Resource.Id.capture_getHeadsetStatus).Click += GetHeadsetStatus;
            FindViewById<Button>(Resource.Id.capture_getLocation).Click += GetLocation;
            FindViewById<Button>(Resource.Id.capture_getBehaviorStatus).Click += GetBehaviorStatus;
            FindViewById<Button>(Resource.Id.capture_getLightIntensity).Click += GetLightIntensity;
            FindViewById<Button>(Resource.Id.capture_getWeatherStatus).Click += GetWeatherStatus;
            FindViewById<Button>(Resource.Id.capture_getBluetoothStatus).Click += GetBluetoothStatus;
            FindViewById<Button>(Resource.Id.capture_getBeaconStatus).Click += GetBeaconStatus;
        }

        private async void GetTimeCategories(object sender, EventArgs e)
        {
            string MethodName = "GetTimeCategories";
            var timeCategoriesTask = Awareness.GetCaptureClient(this).GetTimeCategoriesAsync();
            try
            {
                await timeCategoriesTask;
                if (timeCategoriesTask.IsCompleted && timeCategoriesTask.Result != null)
                {
                    ITimeCategories timeCategories = timeCategoriesTask.Result.TimeCategories;
                    foreach (int timeCode in timeCategories.GetTimeCategories())
                    {
                        log.ShowLog(Constant.GetTimeCategory(timeCode));
                        Log.Info(MethodName, timeCode.ToString());
                    }
                }
                else
                {
                    var exception = timeCategoriesTask.Exception;
                    string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                    log.ShowLog(errorMessage);
                    Log.Error(MethodName, errorMessage);
                }
            }
            catch (Exception exception)
            {
                string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                log.ShowLog(errorMessage);
                Log.Error(MethodName, errorMessage);
            }
        }

        private async void GetHeadsetStatus(object sender, EventArgs e)
        {
            string MethodName = "GetHeadsetStatus";
            var headsetStatusTask = Awareness.GetCaptureClient(this).GetHeadsetStatusAsync();
            try
            {
                await headsetStatusTask;
                if (headsetStatusTask.IsCompleted && headsetStatusTask.Result != null)
                {
                    IHeadsetStatus headsetStatus = headsetStatusTask.Result.HeadsetStatus;
                    int status = headsetStatus.Status;
                    string logMessage = $"Headsets are {(status == HeadsetStatus.Connected ? "connected" : "disconnected")}";
                    log.ShowLog(logMessage);
                    Log.Info(MethodName, logMessage);
                }
                else
                {
                    var exception = headsetStatusTask.Exception;
                    string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                    log.ShowLog(errorMessage);
                    Log.Error(MethodName, errorMessage);
                }
            }
            catch (Exception exception)
            {
                string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                log.ShowLog(errorMessage);
                Log.Error(MethodName, errorMessage);
            }
        }

        private async void GetLocation(object sender, EventArgs e)
        {
            string MethodName = "GetLocation";
            var locationTask = Awareness.GetCaptureClient(this).GetLocationAsync();
            try
            {
                await locationTask;
                if (locationTask.IsCompleted && locationTask.Result != null)
                {
                    Location location = locationTask.Result.Location;
                    string logMessage = $"Longtitude: {location.Longitude},Latitude: {location.Latitude}";
                    log.ShowLog(logMessage);
                    Log.Info(MethodName, logMessage);
                }
                else
                {
                    var exception = locationTask.Exception;
                    string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                    log.ShowLog(errorMessage);
                    Log.Error(MethodName, errorMessage);
                }
            }
            catch (Exception exception)
            {
                string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                log.ShowLog(errorMessage);
                Log.Error(MethodName, errorMessage);
            }
        }

        private async void GetBehaviorStatus(object sender, EventArgs e)
        {
            string MethodName = "GetBehaviorStatus";
            var behaviorTask = Awareness.GetCaptureClient(this).GetBehaviorAsync();
            try
            {
                await behaviorTask;
                if (behaviorTask.IsCompleted && behaviorTask.Result != null)
                {
                    BehaviorStatus behaviorStatus = behaviorTask.Result.BehaviorStatus;
                    DetectedBehavior mostLikelyBehavior = behaviorStatus.MostLikelyBehavior;
                    string logMessage = $"Most likely behavior is {Constant.GetBehavior(mostLikelyBehavior.Type)},the confidence is { mostLikelyBehavior.Confidence}";
                    log.ShowLog(logMessage);
                    Log.Info(MethodName, logMessage);
                }
                else
                {
                    var exception = behaviorTask.Exception;
                    string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                    log.ShowLog(errorMessage);
                    Log.Error(MethodName, errorMessage);
                }
            }
            catch (Exception exception)
            {
                string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                log.ShowLog(errorMessage);
                Log.Error(MethodName, errorMessage);
            }
        }

        private async void GetLightIntensity(object sender, EventArgs e)
        {
            string MethodName = "GetLightIntensity";
            var ambientLightTask = Awareness.GetCaptureClient(this).GetLightIntensityAsync();
            try
            {
                await ambientLightTask;
                if (ambientLightTask.IsCompleted && ambientLightTask.Result != null)
                {
                    IAmbientLightStatus ambientLightStatus = ambientLightTask.Result.AmbientLightStatus; 
                    string logMessage = $"Light intensity is {ambientLightStatus.LightIntensity} lux";
                    log.ShowLog(logMessage);
                    Log.Info(MethodName, logMessage);
                }
                else
                {
                    var exception = ambientLightTask.Exception;
                    string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                    log.ShowLog(errorMessage);
                    Log.Error(MethodName, errorMessage);
                }
            }
            catch (Exception exception)
            {
                string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                log.ShowLog(errorMessage);
                Log.Error(MethodName, errorMessage);
            }
        }

        private async void GetWeatherStatus(object sender, EventArgs e)
        {
            string MethodName = "GetWeatherStatus";
            var weatherTask = Awareness.GetCaptureClient(this).GetWeatherByDeviceAsync();
            try
            {
                await weatherTask;
                if (weatherTask.IsCompleted && weatherTask.Result != null)
                {
                    IWeatherStatus weatherStatus = weatherTask.Result.WeatherStatus;
                    WeatherSituation weatherSituation = weatherStatus.WeatherSituation;
                    Situation situation = weatherSituation.Situation;
                    string logMessage = $"City:{weatherSituation.City.Name}\n";
                    logMessage += $"Weather id is {situation.WeatherId}\n";
                    logMessage += $"CN Weather id is {situation.CnWeatherId}\n";
                    logMessage += $"Temperature is {situation.TemperatureC}℃";
                    logMessage += $",{situation.TemperatureF}℉\n";
                    logMessage += $"Wind speed is {situation.WindSpeed}km/h\n";
                    logMessage += $"Wind direction is {situation.WindDir}\n";
                    logMessage += $"Humidity is {situation.Humidity}%";
                    log.ShowLog(logMessage);
                    Log.Info(MethodName, logMessage);
                }
                else
                {
                    var exception = weatherTask.Exception;
                    string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                    log.ShowLog(errorMessage);
                    Log.Error(MethodName, errorMessage);
                }
            }
            catch (Exception exception)
            {
                string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                log.ShowLog(errorMessage);
                Log.Error(MethodName, errorMessage);
            }
        }

        private async void GetBluetoothStatus(object sender, EventArgs e)
        {
            string MethodName = "GetBluetoothStatus";
            var bluetoothStatusTask = Awareness.GetCaptureClient(this).GetBluetoothStatusAsync(BluetoothStatus.DeviceCar);
            try
            {
                await bluetoothStatusTask;
                if (bluetoothStatusTask.IsCompleted && bluetoothStatusTask.Result != null)
                {
                    IBluetoothStatus bluetoothStatus = bluetoothStatusTask.Result.BluetoothStatus;
                    int status = bluetoothStatus.Status;
                    string logMessage = $"The Bluetooth car stereo are {(status == BluetoothStatus.Connected ? "connected" : "disconnected")}";
                    log.ShowLog(logMessage);
                    Log.Info(MethodName, logMessage);
                }
                else
                {
                    var exception = bluetoothStatusTask.Exception;
                    string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                    log.ShowLog(errorMessage);
                    Log.Error(MethodName, errorMessage);
                }
            }
            catch (Exception exception)
            {
                string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                log.ShowLog(errorMessage);
                Log.Error(MethodName, errorMessage);
            }
        }

        private async void GetBeaconStatus(object sender, EventArgs e)
        {
            string MethodName = "GetBeaconStatus";
            string ns = "sample namespace";
            string type = "sample type";
            byte[] content = Encoding.ASCII.GetBytes("sample");
            BeaconStatusFilter filter = BeaconStatusFilter.Match(ns, type, content);
            var beaconTask = Awareness.GetCaptureClient(this).GetBeaconStatusAsync(filter);
            try
            {
                await beaconTask;
                if (beaconTask.IsCompleted && beaconTask.Result != null)
                {
                    List<IBeaconStatusBeaconData> beaconDataList = beaconTask.Result.BeaconStatus.BeaconData.ToList();
                    string logMessage = string.Empty;
                    if (beaconDataList != null && beaconDataList.Count != 0)
                    {
                        int i = 1;
                        foreach (IBeaconStatusBeaconData beaconData in beaconDataList)
                        {
                            logMessage += $"Beacon Data {i++}\n";
                            logMessage += $"namespace: {beaconData.Namespace}\n";
                            logMessage += $"type: {beaconData.Type}\n";
                            logMessage += $"content: {BitConverter.ToString(beaconData.GetContent())}\n";
                        }
                    }
                    else
                    {
                        logMessage = "No beacon matches filters nearby.";
                    }
                    log.ShowLog(logMessage);
                    Log.Info(MethodName, logMessage);
                }
                else
                {
                    var exception = beaconTask.Exception;
                    string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                    log.ShowLog(errorMessage);
                    Log.Error(MethodName, errorMessage);
                }
            }
            catch (Exception exception)
            {
                string errorMessage = $"{AwarenessStatusCodes.GetMessage(exception.GetStatusCode())}: {exception.Message}";
                log.ShowLog(errorMessage);
                Log.Error(MethodName, errorMessage);
            }
        }
    }
    public static class MyExtensions
    {
        public static int GetStatusCode(this Exception exception)
        {
            string msg = exception.Message;
            int startIndex = 12;
            int length = msg.IndexOf('D') - startIndex;
            if (exception is AwarenessException)
                return Convert.ToInt32(msg.Substring(startIndex, length));
            return -1;
        }
    }

}