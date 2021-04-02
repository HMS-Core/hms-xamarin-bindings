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
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Agconnect.Config;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Common;
using Huawei.Hms.Nearby;
using Huawei.Hms.Nearby.Message;

namespace XamarinHmsNearbyDemoMessage
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        public static string TAG = "MainActivity";
        FragmentManager FManager;
        Fragment Publish;
        Fragment Subscribe;
        public static MessageEngine sMessageEngine;

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
            SetContentView(Resource.Layout.activity_main);

            FManager = FragmentManager;
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            OnNavigationItemSelected(navigation.Menu.GetItem(0));
            sMessageEngine = Nearby.GetMessageEngine(this);
            RequestPermissions();
            sMessageEngine.RegisterStatusCallback(new MyStatusCallback()).AddOnCompleteListener(new TaskListener(this, " RegisterStatusCallback")).AddOnFailureListener(new TaskListener(this, " RegisterStatusCallback"));
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            bool result;
            FragmentTransaction FTransaction = FManager.BeginTransaction();
            HideAllFragment(FTransaction);
            switch (item.ItemId)
            {
                case Resource.Id.navigation_publish:
                    if (Publish == null)
                    {
                        Publish = new PublishFragment();
                        FTransaction.Add(Resource.Id.ly_content, Publish);

                    }
                    else { FTransaction.Show(Publish); }
                    result = true;

                    break;
                 case Resource.Id.navigation_subscribe:
                    if (Subscribe == null)
                    {
                        Subscribe = new SubscribeFragment();
                        FTransaction.Add(Resource.Id.ly_content, Subscribe);
                    }
                    else { FTransaction.Show(Subscribe); }
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }
            FTransaction.Commit();
            return result;
        }

        private void HideAllFragment(FragmentTransaction FTransaction)
        {
            if (Subscribe != null) FTransaction.Hide(Subscribe);

            if (Publish != null) FTransaction.Hide(Publish);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == Constants.REQ_CODE)
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

            if (!NetCheckUtil.IsNetworkAvailable(this))
            {
                ShowWarnDialog(Constants.NETWORK_ERROR);
                return;
            }
        }

        public string[] GetPermissions()
        {
            return new string[]{
                 Manifest.Permission.Bluetooth,
                 Manifest.Permission.BluetoothAdmin,
                 Manifest.Permission.AccessCoarseLocation,
                 Manifest.Permission.AccessFineLocation,
                 Manifest.Permission.AccessNetworkState
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
            builder.SetNegativeButton("Confirm", (senderAlert, args) =>
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            });
            Dialog dialog = builder.Create();
            dialog.Show();
        }

    }

    class MyPutCallback : PutCallback
    {
        override public void OnTimeout()
    {
        Log.Info(MainActivity.TAG, "HMS publish expired");
    }
}

    class MyGetCallback : GetCallback
    {
    override public void OnTimeout()
    {
    Log.Info(MainActivity.TAG, "HMS subscribe expired");
    }

}

    class MyStatusCallback : StatusCallback
    {
        public override void OnPermissionChanged(bool GrantPermission)
        {
            base.OnPermissionChanged(GrantPermission);
            Log.Info(MainActivity.TAG, "OnPermissionChanged " + GrantPermission);
        }
    }

    class TaskListener : Java.Lang.Object, IOnFailureListener, IOnCompleteListener
    {

        private Context context;
        string Caller;
        public TaskListener(Context context, string caller)
        {
            this.context = context;
            this.Caller = caller;

        }

        public void OnFailure(Java.Lang.Exception e)
        { 
            ApiException Excp = (ApiException)e;
            Toast.MakeText(context, Caller + " Failed "+ Excp.StatusCode + " " + Excp.StatusMessage, ToastLength.Long).Show();
            Log.Error(MainActivity.TAG, Caller + " OnFailure " + Excp.StatusMessage);

        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful && Caller.Contains("Put Message"))
            { PublishFragment.MsgList.Add(PublishFragment.Message.Text); PublishFragment.Message.Text = ""; PublishFragment.MsglistArrayAdapter.NotifyDataSetChanged(); }
            Log.Debug(MainActivity.TAG, Caller + " OnComplete " + task.IsSuccessful);
            Toast.MakeText(context, Caller + task.IsSuccessful, ToastLength.Long).Show();
        }
    }
}

