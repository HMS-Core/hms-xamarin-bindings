/*
       Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Huawei.Hms.Iap;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Iap.Entity;
using Android.Content;
using Huawei.Hms.Iap.Util;
using Huawei.Agconnect.Config;
using Android.Util;
using System;

namespace XamarinHmsIAPDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, View.IOnClickListener
    {
        internal static Activity CurrentContext { get; private set; }
        private const String TAG = "MainActivity";

        public MainActivity()
        {
            CurrentContext = this;
        }

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
            SetContentView(Resource.Layout.EntryActivity);

            FindViewById(Resource.Id.button1).SetOnClickListener(this);
            FindViewById(Resource.Id.button2).SetOnClickListener(this);
            FindViewById(Resource.Id.button3).SetOnClickListener(this);

            QueryIsReady();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }
        private void QueryIsReady()
        {
            Log.Info(TAG, "IsReady");
            IIapClient mClient = Iap.GetIapClient(this);
            Task isEnvReady = mClient.IsEnvReady();
            isEnvReady.AddOnSuccessListener(new ListenerImp()).AddOnFailureListener(new ListenerImp());

        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.button1)
            {
                Intent Conusumable = new Intent();
                Conusumable.SetClass(this, typeof(ConsumptionActivity));
                Conusumable.SetFlags(ActivityFlags.ClearTop);
                StartActivity(Conusumable);

            }

            if (v.Id == Resource.Id.button2)
            {
                Intent NonConusumable = new Intent();
                NonConusumable.SetClass(this, typeof(NonConsumptionActivity));
                NonConusumable.SetFlags(ActivityFlags.ClearTop);
                StartActivity(NonConusumable);

            }
            if (v.Id == Resource.Id.button3)
            {
                Intent Subscription = new Intent();
                Subscription.SetClass(this, typeof(SubscriptionActivity));
                Subscription.SetFlags(ActivityFlags.ClearTop);
                StartActivity(Subscription);

            }

        }

        class ListenerImp : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {

            public void OnSuccess(Java.Lang.Object IsEnvReadyResult)
            {
                Log.Info(TAG, "OnSucess");
            }
            public void OnFailure(Java.Lang.Exception e)
            {
                Log.Info(TAG, "Onfailure");
                if (e.GetType()==typeof(IapApiException))
                {
                    IapApiException apiException = (IapApiException)e;
                    if (apiException.Status.StatusCode == OrderStatusCode.OrderHwidNotLogin)
                    {
                        if (apiException.Status.HasResolution)
                        {
                            try
                            {
                                Log.Info(TAG, apiException.Status.StatusMessage);
                                apiException.Status.StartResolutionForResult(MainActivity.CurrentContext, Constants.REQ_CODE_LOGIN);
                            }
                            catch (Java.Lang.Exception exp)
                            {
                                Log.Error(TAG, exp.Message);
                            }
                        }
                    }
                    else if (apiException.Status.StatusCode == OrderStatusCode.OrderAccountAreaNotSupported)
                    {
                        Toast.MakeText(MainActivity.CurrentContext, "This is unavailable in your country/region.", ToastLength.Long).Show();
                    }
                }

            }
        }

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)

        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == Constants.REQ_CODE_LOGIN)
            {
                int returnCode = IapClientHelper.ParseRespCodeFromIntent(data);

                if (returnCode == OrderStatusCode.OrderStateSuccess)
                {
                    Toast.MakeText(this, "Login Success", ToastLength.Long).Show();
                }
                else if (returnCode == OrderStatusCode.OrderAccountAreaNotSupported)
                {
                    Toast.MakeText(this, "This is unavailable in your country/region.", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, "User cancel login.", ToastLength.Long).Show();
                    Toast.MakeText(this, "App will Exit", ToastLength.Long).Show();
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());

                }
                return;
            }

        }

    } 
}