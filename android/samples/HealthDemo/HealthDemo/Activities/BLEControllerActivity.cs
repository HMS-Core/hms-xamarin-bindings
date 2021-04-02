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

using System;
using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Hihealth;
using Huawei.Hms.Hihealth.Data;
using Huawei.Hms.Hihealth.Options;
using Huawei.Hms.Support.Hwid;
using Huawei.Hms.Support.Hwid.Result;
using static Android.Widget.AdapterView;
using DataType = Huawei.Hms.Hihealth.Data.DataType;

namespace XamarinHmsHealthDemo
{
    [Activity(Label = "BLEControllerActivity")]
    public class BLEControllerActivity : Activity
    {
        public static string TAG = "BLEControllerActivity";

        public static ListView AvalibleDeviceslistview;

        // Switch to enble the Scan
        public static Switch switchScan;

        // Create a BleController object to scan external Bluetooth devices.
        private static BleController MyBleController;

        // Create a BleDeviceInfo list to store the scanned devices.
        public static IList<BleDeviceInfo> BleDeviceInfoList;

        // callback object to be used BeginScan
        public static BleScanCallBackImp bleScanCallBack;

        public static FoundDevicesAdapter Adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BLEController_layout);
            RequestPermissions();
            InitBLEController();

            switchScan = FindViewById<Switch>(Resource.Id.switchScan);
            AvalibleDeviceslistview = (ListView)FindViewById(Resource.Id.listView1);

            BleDeviceInfoList = new List<BleDeviceInfo>();
            Adapter = new FoundDevicesAdapter(this, BleDeviceInfoList);
            AvalibleDeviceslistview.Adapter = Adapter;
            Adapter.NotifyDataSetChanged();

            bleScanCallBack = new BleScanCallBackImp();

            RegisterForContextMenu(AvalibleDeviceslistview);
            
            switchScan.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                if (e.IsChecked)
                {

                    StartScan();
                    Log.Debug(TAG, "BeginScan");
                    Toast.MakeText(this, " Starting Scanning ", ToastLength.Short).Show();
                }
                else
                {
                    StopScan();
                    Log.Debug(TAG, "EndScan");
                    Toast.MakeText(this, " Stopping Scanning ", ToastLength.Short).Show();
                }
            };

            GetSavedDevices();

        }
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            MenuInflater inflater = new MenuInflater(this);
            inflater.Inflate(Resource.Menu.ContextMenu, menu);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            AdapterContextMenuInfo info = (AdapterContextMenuInfo)item.MenuInfo;
            if (item.ItemId == Resource.Id.SaveDevice)
            {
                SaveDevice(BleDeviceInfoList.ElementAt(info.Position));
            }
            if (item.ItemId == Resource.Id.DeleteDevice)
            {

                DeleteDevice(BleDeviceInfoList.ElementAt(info.Position), info.Position);
            }
            return base.OnContextItemSelected(item);
        }
        public string[] GetPermissions()
        {
            return new string[]{
                 Manifest.Permission.Bluetooth,
                 Manifest.Permission.BluetoothAdmin,
                 Manifest.Permission.AccessCoarseLocation,
                 Manifest.Permission.AccessFineLocation,
                 Manifest.Permission.AccessNetworkState,
                 Manifest.Permission.BodySensors
        };
        }

        private void RequestPermissions()
        {
            string[] deniedPermissions = PermissionUtil.GetDeniedPermissions(this, GetPermissions());

            if (deniedPermissions != null && deniedPermissions.Length > 0)
            {
                PermissionUtil.RequestPermissions(this, deniedPermissions, 2020);
            }
            else
            {
                Log.Info(TAG, "All permissions are granted");
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 2020)
            {
                bool IsAllGranted = true;
                foreach (string result in permissions)
                {
                    if (this.CheckSelfPermission(result) == Android.Content.PM.Permission.Denied)
                    {
                        IsAllGranted = false;
                        break;
                    }
                }
                if (IsAllGranted)
                {
                    Log.Info(TAG, "All permissions are granted");
                    InitBLEController();
                  
                }
                else
                {
                    Toast.MakeText(this, "Cannot start without required permissions", ToastLength.Long).Show();
                }
            }
        }
        
        //Initiate BleController
        private void InitBLEController()
        {

            if (MyBleController == null)
            {
                HiHealthOptions hiHealthOptions = HiHealthOptions.HiHealthOptionsBulider().Build();
                AuthHuaweiId signInHuaweiId = HuaweiIdAuthManager.GetExtendedAuthResult(hiHealthOptions);
                MyBleController = HuaweiHiHealth.GetBleController(this, signInHuaweiId);
            }
        }

        //Get The saved Blutooth devices 
        public async void GetSavedDevices()
        {
            var GetSavedDevicesTask = MyBleController.GetSavedDevicesAsync();

            try
            {
                await GetSavedDevicesTask;

                if (GetSavedDevicesTask.IsCompleted && GetSavedDevicesTask.Result != null)
                {

                    if (GetSavedDevicesTask.Exception == null)
                    {
                        Log.Debug(TAG, "GetSaveDevices OnSuccess");
                        foreach (BleDeviceInfo bleDevice in GetSavedDevicesTask.Result)
                        {
                            if (!IsAdded(bleDevice))
                                BleDeviceInfoList.Add(bleDevice);
                        }
                        Adapter.NotifyDataSetChanged();

                    }
                    else
                    {
                        Log.Debug(TAG, "GetSaveDevices Failed " + GetSavedDevicesTask.Exception.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Debug(TAG, "GetSaveDevices Failed " + ex.Message);
            }

        }

        //Check if the device is added 
        static public bool IsAdded(BleDeviceInfo Device)
        {
            foreach (BleDeviceInfo device in BleDeviceInfoList)
                if (device.DeviceAddress == Device.DeviceAddress)
                    return true;
            return false;
        }

        // Delete device
        public async void DeleteDevice(BleDeviceInfo Device,int pos)
        {
            Log.Debug(TAG, "Delete Device " + Device.DeviceAddress);
            var DeleteTask = MyBleController.DeleteDeviceAsync(Device);

            try
            {
                await DeleteTask;

                if (DeleteTask.IsCompleted)
                {

                    if (DeleteTask.Exception == null)
                    {
                        Log.Debug(TAG, "Delete Device success");
                        BleDeviceInfoList.RemoveAt(pos);
                        Adapter.NotifyDataSetChanged();
         
                    }
                    else
                    {
                        Log.Debug(TAG, "Delete Device Failed " + DeleteTask.Exception.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Debug(TAG, "Delete Device Failed  " + ex.Message);
            }

        }

        // Save device
        public async void SaveDevice(BleDeviceInfo Device)
        {
            Log.Debug(TAG, "Save Device " + Device.DeviceAddress);
            var SaveTask = MyBleController.SaveDeviceAsync(Device);

            try
            {
                await SaveTask;

                if (SaveTask.IsCompleted)
                {

                    if (SaveTask.Exception == null)
                    {
                        Log.Debug(TAG, "Save Device success");
                      

                    }
                    else
                    {
                        Log.Debug(TAG, "Save Device Failed " + SaveTask.Exception.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Debug(TAG, "Save Device Failed  " + ex.Message);
            }

        }

        //Start Scan for devices
        public static async void StartScan()
        {
            var StartTask = MyBleController.BeginScanAsync(new DataType[] { DataType.DtInstantaneousHeartRate}, 30, bleScanCallBack);
           
            try
            {
                await StartTask;

                if (StartTask.IsCompleted)
                {

                    if (StartTask.Exception == null)
                    {
                        Log.Debug(TAG, "Start Scan success");

                    }
                    else
                    {
                        Log.Debug(TAG, "Start Scan Failed " + StartTask.Exception.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Debug(TAG, "Start Scan Failed  " + ex.Message);
            }

        }

        //Stop Scan for devices
        public async void StopScan()
        {
            var StopTask = MyBleController.EndScanAsync(bleScanCallBack);

            try
            {
                await StopTask;

                if (StopTask.IsCompleted)
                {

                    if (StopTask.Exception == null)
                    {
                        Log.Debug(TAG, "Stop Scan is "+ StopTask.Result);

                    }
                    else
                    {
                        Log.Debug(TAG, "Stop Scan Failed " + StopTask.Exception.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Debug(TAG, "Stop Scan Failed  " + ex.Message);
            }

        }
    }

    public class BleScanCallBackImp : BleScanCallback
    {
        public override void OnDeviceDiscover(BleDeviceInfo Device)
        {
            Log.Debug("BleController", "onDeviceDiscover : " + Device.DeviceName);
            if (!BLEControllerActivity.IsAdded(Device))
            BLEControllerActivity.BleDeviceInfoList.Add(Device);
            BLEControllerActivity.Adapter.NotifyDataSetChanged();
        }

        public override void OnScanEnd()
        {
            Log.Debug("BleController", "OnScanEnd ");
            if (BLEControllerActivity.switchScan.Checked)
                BLEControllerActivity.StartScan();
        }
    }
}