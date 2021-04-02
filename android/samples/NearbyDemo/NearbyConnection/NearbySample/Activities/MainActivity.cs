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

using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Agconnect.Config;
using Huawei.Hms.Nearby.Transfer;
using Uri = Android.Net.Uri;
using System.IO;
using Huawei.Hms.Nearby.Discovery;
using Huawei.Hms.Nearby.Wifishare;
using Huawei.Hms.Nearby;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Common;
using DataType = Huawei.Hms.Nearby.Transfer.DataType;

namespace XamarinHmsNearbyDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        public static string TAG = "MainActivity";
        BottomNavigationView Navigation;
        Fragment NearbyConnectionFrag;
        Fragment WifiSharingFrag;
        FragmentManager FManager;
        public static DiscoveryEngine mDiscoveryEngine = null;
        public static TransferEngine mTransferEngine = null;
        public static WifiShareEngine mWifiShareEngine = null;
        public static string MyServiceID = "Com.Company.Project";
        public static string MyEndPoint = Android.OS.Build.Model;
        public static List<MessageBean> MsgList = new List<MessageBean>();
        public static IList<Device> AvaliableDevicesList = new List<Device>();
        public IDictionary<long, Data> IncomingFilePayloads = new Dictionary<long, Data>();
        public IDictionary<long, Data> CompletedFilePayloads = new Dictionary<long, Data>();
        public IDictionary<long, string> FilePayloadFilenames = new Dictionary<long, string>();
        public static ExpandableAdapter Adapter;
        private static volatile MainActivity M_SERVICE = null;
        public DialogLoader Dialog;
        protected override void AttachBaseContext(Context context)
        {
            base.AttachBaseContext(context);
            AGConnectServicesConfig config = AGConnectServicesConfig.FromContext(context);
            config.OverlayWith(new HmsLazyInputStream(context));
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.ActivityTabbed);

            Navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            Navigation.SetOnNavigationItemSelectedListener(this);
            FManager = FragmentManager;
            M_SERVICE = this;
            OnNavigationItemSelected(Navigation.Menu.GetItem(0));
            mTransferEngine = Nearby.GetTransferEngine(this);
            mWifiShareEngine = Nearby.GetWifiShareEngine(this);
            mDiscoveryEngine = Nearby.GetDiscoveryEngine(this);
            Adapter = new ExpandableAdapter(this, AvaliableDevicesList);
            RequestPermissions();

            
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            bool result;
            FragmentTransaction fTransaction = FManager.BeginTransaction();
            HideAllFragment(fTransaction);
            switch (item.ItemId)
            {
                case Resource.Id.navigation_NearbyConnection:
                   
                    if (NearbyConnectionFrag == null)
                    {
                        NearbyConnectionFrag = new NearbyConnection();
                        fTransaction.Add(Resource.Id.ly_content, NearbyConnectionFrag);

                    }
                    else { ClearBetweenTaps("Connection");  fTransaction.Show(NearbyConnectionFrag);}
                    result = true;

                    break;
                case Resource.Id.navigation_WifiSharing:
                    ClearBetweenTaps("WIFI");
                    if (WifiSharingFrag == null)
                    {
                        WifiSharingFrag = new WifiSharing();
                        fTransaction.Add(Resource.Id.ly_content, WifiSharingFrag);
                    }
                    else { ClearBetweenTaps("WIFI"); fTransaction.Show(WifiSharingFrag); }
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }
            fTransaction.Commit();
            return result;
        }

        private void HideAllFragment(FragmentTransaction fTransaction)
        {
            if (NearbyConnectionFrag != null) fTransaction.Hide(NearbyConnectionFrag);

            if (WifiSharingFrag != null) fTransaction.Hide(WifiSharingFrag);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == Constants.REQ_CODE)
            {
                bool isAllGranted = true;
                foreach (string result in permissions)
                {
                    if (this.CheckSelfPermission(result) == Permission.Denied)
                    {
                        isAllGranted = false;
                        break;
                    }
                }
                if (isAllGranted)
                {
                    Log.Info(TAG, "All permissions are granted");
                }
                else
                {
                    Toast.MakeText(this, "Cannot start without required permissions", ToastLength.Long).Show();
                }
            }
        }
        protected override void OnStart()
        {
            Log.Info(TAG, "Is support BLE advertiser: " + BluetoothCheckUtil.IsSupportBleAdv());
            Log.Info(TAG, "Is support BLE Scanner: " + BluetoothCheckUtil.IsSupportBleScan());
            base.OnStart();
            if (!BluetoothCheckUtil.IsBlueEnabled())
            {
                ShowWarnDialog(Constants.BLUETOOTH_ERROR);
                return;
            }

            if (!LocationCheckUtil.IsLocationEnabled(this))
            {
                ShowWarnDialog(Constants.LOCATION_SWITCH_ERROR);
                return;
            }
        }
        public string[] GetPermissions()
        {
            return new string[]{
                 Manifest.Permission.Bluetooth,
                 Manifest.Permission.BluetoothAdmin,
                 Manifest.Permission.AccessWifiState,
                 Manifest.Permission.ChangeWifiState,
                 Manifest.Permission.AccessCoarseLocation,
                 Manifest.Permission.AccessFineLocation,
                 Manifest.Permission.ReadExternalStorage,
                 Manifest.Permission.WriteExternalStorage
        };
        }
        private void RequestPermissions()
        {
            string[] deniedPermissions = PermissionUtil.GetDeniedPermissions(this, GetPermissions());

            if (deniedPermissions != null && deniedPermissions.Length > 0)
            {
                PermissionUtil.RequestPermissions(this, deniedPermissions, Constants.REQ_CODE);
            }
            else
            {
                Log.Info(TAG, "All permissions are granted");
            }
        }
        private void ShowWarnDialog(string content)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
            builder.SetTitle(Resource.String.warn);
            builder.SetMessage(content);
            builder.SetNegativeButton(Resource.String.btn_confirm, (senderAlert, args) =>
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            });
            Dialog dialog = builder.Create();
            dialog.Show();
        }
        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)

        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == Constants.REQ_CODE_FILE_PICKER && resultCode == Result.Ok && data != null)
            {
                Uri uri = data.Data;
                Data filePayload;
                Device device = AvaliableDevicesList.ElementAt(data.GetIntExtra("DeviceID", 0));
                try
                {
                    ParcelFileDescriptor pfd = this.ContentResolver.OpenFileDescriptor(uri, "r");
                    filePayload = Data.FromFile(pfd);
                }
                catch (FileNotFoundException e)
                {
                    Log.Error(TAG, "File not found, cause: ", e);
                    return;
                }
                SendFilePayload(filePayload, device.EndPoint, uri);
            }
        }

        public void UpdateAvaliableDevices()
        {

            Adapter.NotifyDataSetChanged();
        }

        public static void InitiateConnection(Device device)
        {
            mDiscoveryEngine.RequestConnect(MyEndPoint, device.EndPoint, new ConnCallBack(M_SERVICE)).AddOnSuccessListener(new TaskListener(M_SERVICE, "Start Connecting")).AddOnFailureListener(new TaskListener(M_SERVICE, "Start Connecting")); ;

        }

        static public Device FindDevice(string endpoint)
        {
            foreach (Device device in AvaliableDevicesList)
                if (device.EndPoint == endpoint)
                    return device;
            return null;
        }

        public void DeleteDevice(string endpoint)
        {
            Log.Debug(TAG, "Delete Device " + endpoint);
            foreach (Device device in AvaliableDevicesList.ToList())
                if (device.EndPoint == endpoint)
                    AvaliableDevicesList.Remove(device);
        }

        public static void DisconnectDevice(string endpoint)
        {
            Log.Debug(TAG, "Disconnect Device " + endpoint);
            Device device = FindDevice(endpoint);
            mDiscoveryEngine.Disconnect(device.EndPoint);
            device.Status = false;
            Adapter.NotifyDataSetChanged();


        }
        public long AddPayloadFilename(string payloadFilenameMessage)
        {
            Log.Debug(TAG, "AddPayloadFilename, payloadFilenameMessage======== " + payloadFilenameMessage);
            string[] parts = payloadFilenameMessage.Split(":");
            long payloadId = long.Parse(parts[0]);
            string filename = parts[1];
            FilePayloadFilenames.Add(payloadId, filename);
            return payloadId;
        }

        public void ReceiveMessage(Data data, string Endpoint)
        {
            if (MsgList.Count == 0 || !ChatActivity.device.EndPoint.Equals(Endpoint))
            {
                MsgList.Clear();
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle("You got a message !");
                builder.SetMessage(Endpoint + " is trying to send a message to you. do you want to accept?");
                builder.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    string msgStr = "Refuse";
                    Data data = Data.FromBytes(System.Text.Encoding.UTF8.GetBytes(msgStr));
                    mTransferEngine.SendData(Endpoint, data).AddOnSuccessListener(new TaskListener(this, "Refuse Connection")).AddOnFailureListener(new TaskListener(this, "Refuse Connection"));
                    return;

                });
                builder.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    string str = System.Text.Encoding.UTF8.GetString(data.AsBytes());
                    Log.Debug(TAG, "OnReceived [Message] success. msgStr-------->>>>" + str);
                    if (!str.EndsWith(":msg input"))
                    {
                        return;
                    }
                    str = str.Split(":")[0];
                    MessageBean item = new MessageBean();
                    item.MyName = MyEndPoint;
                    item.FriendName = FindDevice(Endpoint).Name;
                    item.Msg = str;
                    item.Type = (MessageBean.TYPE_RECEIVE_TEXT);
                    MsgList.Add(item);
                    Intent chat = new Intent();
                    chat.SetClass(this, typeof(ChatActivity));
                    chat.SetFlags(ActivityFlags.ClearTop);
                    chat.PutExtra("DeviceID", AvaliableDevicesList.IndexOf(FindDevice(Endpoint)));
                    StartActivity(chat);
                });
                Dialog dialog = builder.Create();
                dialog.Show();
            }

            else
            {
                string str = System.Text.Encoding.UTF8.GetString(data.AsBytes());
                Log.Debug(TAG, "OnReceived [Message] success. msgStr-------->>>>" + str);
                if (!str.EndsWith(":msg input"))
                {
                    return;
                }
                str = str.Split(":")[0];
                MessageBean item = new MessageBean();
                item.MyName = MyEndPoint;
                item.FriendName = FindDevice(Endpoint).Name;
                item.Msg = str;
                item.Type = (MessageBean.TYPE_RECEIVE_TEXT);
                MsgList.Add(item);
                Intent chat = new Intent();
                chat.SetClass(this, typeof(ChatActivity));
                chat.SetFlags(ActivityFlags.ClearTop);
                chat.PutExtra("DeviceID", AvaliableDevicesList.IndexOf(FindDevice(Endpoint)));
                StartActivity(chat);
            }
        }

        public void SendFilePayload(Data filePayload, string endpointID, Uri uri)
        {
            string fileName = FileUtil.getFileRealNameFromUri(this, uri);
            string filenameMessage = filePayload.Id + ":" + fileName;
            Data filenameBytesPayload = Data.FromBytes(System.Text.Encoding.UTF8.GetBytes(filenameMessage));
            mTransferEngine.SendData(endpointID, filenameBytesPayload).AddOnSuccessListener(new TaskListener(this, "Send File Name "+ fileName)).AddOnFailureListener(new TaskListener(this, "send File Name "+ fileName));
            mTransferEngine.SendData(endpointID, filePayload).AddOnSuccessListener(new TaskListener(this, "Send File "+ filePayload.Id)).AddOnFailureListener(new TaskListener(this, "send File "+ filePayload.Id));
            Dialog = new DialogLoader(this, 0, "Sending File...", filePayload.AsFile().Size, 0, filePayload.Id);
            Dialog.StartDialogLoading();
        }

        public void ProcessFilePayload(long payloadId)
        {
            Log.Debug(TAG, "processFilePayload, payloadId=========" + payloadId);
            Data filePayload = IncomingFilePayloads[payloadId];
            string filename = FilePayloadFilenames[payloadId];
            Log.Debug(TAG, "received file: " + filename);
            if (filePayload != null && filename != null)
            {
                Java.IO.File payloadFile = filePayload.AsFile().AsJavaFile();
                Log.Debug(TAG, "processFilePayload, payloadFile name------>>>>>>: " + payloadFile.Name);
                Dialog = new DialogLoader(this, 0, "Receving File...", 0, 0, payloadId);
                Dialog.StartDialogLoading();
                Java.IO.File targetFileName = new Java.IO.File(payloadFile.ParentFile, filename);

                bool result = payloadFile.RenameTo(targetFileName);
                if (!result)
                {
                    Log.Error(TAG, "rename the file failed.");
                }

            }
        }

        public void ClearBetweenTaps(string Dest)
        {
            AvaliableDevicesList.Clear();
            Adapter.NotifyDataSetChanged();
            if (Dest=="WIFI")
            {
                NearbyConnection.switchBroad.Checked = false;
                NearbyConnection.switchScan.Checked = false;

                mDiscoveryEngine.DisconnectAll();
                Toast.MakeText(this,"All Devices are disconnected", ToastLength.Long).Show();

                mDiscoveryEngine.StopBroadcasting();
                mDiscoveryEngine.StopScan();
            }
            else {
                WifiSharing.switchWifiSet.Checked = false;
                WifiSharing.switchWifiShare.Checked = false;
                mWifiShareEngine.StopWifiShare();
            }
        

        }

        public void UpdateProgress(long transferredBytes,int progress,long totalBytes)
        {
            Dialog.UpdateProgress(transferredBytes, progress,totalBytes);
        }
    }

    class ScanCallBack : ScanEndpointCallback
    {

        private MainActivity context;
        public ScanCallBack(MainActivity Main)
        {
            this.context = Main;
        }
        public override void OnFound(string EndPoint, ScanEndpointInfo scanEndpointInfo)
        {
            Log.Debug(MainActivity.TAG, "OnFound with Endpoint : " + EndPoint+ " scanEndpointInfo : ServiceId: "+scanEndpointInfo.ServiceId);
            Device device = MainActivity.FindDevice(EndPoint);
            if (device == null)
            {
                device = new Device(scanEndpointInfo.Name, EndPoint, false);
                MainActivity.AvaliableDevicesList.Add(device);
                context.UpdateAvaliableDevices();
            }
            
        }

        public override void OnLost(string EndPoint)
        {
            Log.Debug(MainActivity.TAG, "OnLost with Endpoint : " + EndPoint);
            context.DeleteDevice(EndPoint);
            context.UpdateAvaliableDevices();

        }
    }
    class ConnCallBack : ConnectCallback
    {
        private MainActivity context;
        public ConnCallBack(MainActivity Main)
        {
            this.context = Main;
        }

        public override void OnDisconnected(string EndPoint)
        {
            Log.Debug(MainActivity.TAG, "OnDisconnected with Endpoint : " + EndPoint);
            Toast.MakeText(context, EndPoint + " Disconnected ", ToastLength.Long).Show();
            Device device = MainActivity.FindDevice(EndPoint);
            if (device != null) device.Status = false;
            context.UpdateAvaliableDevices();
            NearbyConnection.AvalibleDeviceslistview.CollapseGroup(MainActivity.AvaliableDevicesList.IndexOf(device));

        }

        public override void OnEstablish(string EndPoint, ConnectInfo Coninfo)
        {
            Log.Debug(MainActivity.TAG, "OnEstablish with Endpoint : " + EndPoint+ " ConnectInfo : Endpoint Name " + Coninfo.EndpointName);
            MainActivity.mDiscoveryEngine.AcceptConnect(EndPoint, new DataCallBackImp(context)).AddOnSuccessListener(new TaskListener(context, "Accept Connection")).AddOnFailureListener(new TaskListener(context, "Accept Connection")); ;
            Log.Debug(MainActivity.TAG, "Connection Accepted with Endpoint : " + EndPoint);

           Device device = MainActivity.FindDevice(EndPoint);
            if (device == null)
            {
                device = new Device(Coninfo.EndpointName, EndPoint, true);
                MainActivity.AvaliableDevicesList.Add(device);
            }
        }


        public override void OnResult(string EndPoint, ConnectResult ConnResult)
        {
            Log.Debug(MainActivity.TAG, "OnResult with Endpoint : " + EndPoint+ " ConnectResult : Status: " + ConnResult.Status);
            if (ConnResult.Status.StatusCode == StatusCode.StatusSuccess)
            {
                Toast.MakeText(context, "Connection with " + EndPoint + " has been Established", ToastLength.Long).Show();

                Device device = MainActivity.FindDevice(EndPoint);
                if (device != null)
                    device.Status = true;
                context.UpdateAvaliableDevices();
                }
            else    
               Toast.MakeText(context, "Connection Failed " + ConnResult.Status.StatusMessage, ToastLength.Long).Show();

        }
    }
    class DataCallBackImp : DataCallback
    {
        private MainActivity context;
        public DataCallBackImp(MainActivity Main)
        {
            this.context = Main;
        }

        public override void OnReceived(string endpoint, Data data)
        {
            Log.Debug(MainActivity.TAG, "onReceived, Data.Type = " + data.Type);
            Log.Debug(MainActivity.TAG, "onReceived, string ======== " + endpoint);
            switch (data.Type)
            {
                case DataType.Bytes:
                    string str = System.Text.Encoding.UTF8.GetString(data.AsBytes());
                    if (str.EndsWith(":msg input"))
                    {
                        context.ReceiveMessage(data, endpoint);
                    }
                    else if (str.Contains("Refuse"))
                    {
                        Toast.MakeText(context, endpoint + " has refused to connect. Exiting Chat window", ToastLength.Long).Show();
                        Log.Debug(MainActivity.TAG, "Connection Rejected");
                        MainActivity.MsgList.Clear();
                        Intent chat = new Intent(context, typeof(MainActivity));
                        chat.SetFlags(ActivityFlags.ClearTop);
                        context.StartActivity(chat);
                    }
                    else if (str.EndsWith("ProgressUpdate"))
                    {
                        Log.Debug(MainActivity.TAG, "Feedback recevied>>>>" + str);
                        long transferBytes = long.Parse( str.Split("_")[0]);
                        int Progress = int.Parse(str.Split("_")[1]);
                        long totalBytes= long.Parse(str.Split("_")[2]);
                        if (Progress == 100) context.Dialog.Dismiss();
                        else context.UpdateProgress(transferBytes, Progress,totalBytes);
                    }
                    else
                    {
                        context.AddPayloadFilename(str);
                    }
                    break;
                case DataType.File:
                    context.IncomingFilePayloads.Add(data.Id, data);
                    context.ProcessFilePayload(data.Id);
                    Log.Info(MainActivity.TAG, "onReceived [FilePayload] success, Data.Type.FILE, payloadId ===" + data.Id);
                    break;
                default:
                    Log.Info(MainActivity.TAG, "the other Unknown data type, please check the uploaded file.");
                    return;
            }
        }

        public override void OnTransferUpdate(string Endpoint, TransferStateUpdate update)
        {

            long transferredBytes = update.BytesTransferred;
            long totalBytes = update.TotalBytes;
            long payloadId = update.DataId;
            Log.Debug(MainActivity.TAG, "onTransferUpdate, payloadId============" + payloadId);
            switch (update.Status)
            {
                case TransferStatus.TransferStateSuccess:
                    Log.Debug(MainActivity.TAG, "onTransferUpdate.Status============success.");
                    Data payload = null;
                    if (context.IncomingFilePayloads.ContainsKey(payloadId))
                        payload = context.IncomingFilePayloads[payloadId];

                    if (payload != null)
                    {
                        if (payload.Type == Huawei.Hms.Nearby.Transfer.DataType.File)
                        {
                            Toast.MakeText(context, "Your friend shares a file with you.", ToastLength.Long).Show();
                        }
                        Log.Debug(MainActivity.TAG, "onTransferUpdate, payload.Type " + payload.Type);
                        if (context.Dialog.PayloadID==payloadId)
                        { context.Dialog.Dismiss(); }
                        context.CompletedFilePayloads.Add(payloadId, payload);
                        context.IncomingFilePayloads.Remove(payloadId);
                        context.FilePayloadFilenames.Remove(payloadId);
                    }
                    break;
                case TransferStatus.TransferStateInProgress:
                    Log.Debug(MainActivity.TAG, "onTransferUpdate.Status==========transfer in progress.");
                    if (!context.IncomingFilePayloads.ContainsKey((payloadId)))
                    {
                        if (!context.FilePayloadFilenames.ContainsKey(payloadId))
                        {
                            return;
                        }
                    }
                    int progress = (int)(transferredBytes * 100 / totalBytes);
                    context.UpdateProgress(transferredBytes, progress,totalBytes);
                    string FeedbackUpdate=transferredBytes.ToString()+"_" + progress.ToString()+"_"+totalBytes.ToString()+"_ProgressUpdate";
                    Data FeedbackUpdatePayload = Data.FromBytes(System.Text.Encoding.UTF8.GetBytes(FeedbackUpdate));
                    MainActivity.mTransferEngine.SendData(Endpoint, FeedbackUpdatePayload).AddOnSuccessListener(new TaskListener(context, "Feedback sent")).AddOnFailureListener(new TaskListener(context, "Feedback sent"));
                    break;
                case TransferStatus.TransferStateFailure:
                    Log.Debug(MainActivity.TAG, "onTransferUpdate.Status==========transfer failed.");
                    Log.Debug(MainActivity.TAG, "Cancel Data Transfer");
                    if (context.Dialog.PayloadID == payloadId)
                    { context.Dialog.Dismiss(); MainActivity.mTransferEngine.CancelDataTransfer(payloadId).AddOnSuccessListener(new TaskListener(context, "Cancel Transfer")).AddOnFailureListener(new TaskListener(context, "Cancel Transfer")); }
                    break;

                default:
                    Log.Debug(MainActivity.TAG, "onTransferUpdate.Status=======" + update.Status);
                    return;
            }
        }
    }
    class WifiCallBackImp : WifiShareCallback
    {
        private MainActivity context;
        public WifiCallBackImp(MainActivity Main)
        {
            this.context = Main;
        }

        public override void OnFetchAuthCode(string Endpoint, string authcode)
        {
            Log.Debug(MainActivity.TAG, "OnFetchAuthCode " + authcode);
            Toast.MakeText(context, "verifying with authcode " + authcode, ToastLength.Long).Show();

        }
        public override void OnFound(string Endpoint, ScanEndpointInfo scanEndpointInfo)
        {

            Device device = MainActivity.FindDevice(Endpoint);
            if (device == null)
            {
                device = new Device(scanEndpointInfo.Name, Endpoint, false);
                MainActivity.AvaliableDevicesList.Add(device);
                Log.Debug(MainActivity.TAG, "OnFound : " + Endpoint);
                context.UpdateAvaliableDevices();
            }

        }

        public override void OnLost(string Endpoint)
        {
            Log.Debug(MainActivity.TAG, "OnLost : " + Endpoint);
            context.DeleteDevice(Endpoint);
            context.UpdateAvaliableDevices();
        }

        public override void OnWifiShareResult(string Endpoint, int Status)
        {
            Log.Debug(MainActivity.TAG, "OnWifiShareResult " + Endpoint + " " + StatusCode.GetStatusCode(Status));
            switch (Status)
            {
                case StatusCode.StatusSuccess:
                    Toast.MakeText(context, "Wifi pass is shared Successfully with " +Endpoint, ToastLength.Long).Show();
                    break;
                    case StatusCode.StatusFailure:
                    Toast.MakeText(context, "Wifi sharing failed with " + Endpoint, ToastLength.Long).Show();

                    break;
                default:
                    break;
            }
        }
    }
    class TaskListener : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
    {

        private Context context;
        string Caller;
        public TaskListener(Context context, string caller)
        {
            this.context = context;
            this.Caller = caller;

        }

        public void OnSuccess(Java.Lang.Object result)
        {
            Log.Debug(MainActivity.TAG, Caller + " on Success");
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Log.Error(MainActivity.TAG, Caller + " OnFailure");
            ApiException Excp = (ApiException)e;
            Toast.MakeText(context, Caller + " Failed " +Excp.StatusCode + " " + Excp.StatusMessage, ToastLength.Long).Show();

        }
    }
}