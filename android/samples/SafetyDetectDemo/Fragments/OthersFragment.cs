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
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Agconnect.Config;
using Huawei.Hms.Common;
using Huawei.Hms.Support.Api.Entity.Safetydetect;
using Huawei.Hms.Support.Api.Safetydetect;
using SafetyDetectDemo.HmsSample;

namespace SafetyDetectDemo.Fragments
{
    public class OthersFragment : Fragment, View.IOnClickListener
    {
        ISafetyDetectClient client;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fg_others, container, false);
            string appid = AGConnectServicesConfig.FromContext(Activity).GetString("client/app_id");

            view.FindViewById<Button>(Resource.Id.fg_wifidetect).SetOnClickListener(this);
            view.FindViewById<Button>(Resource.Id.fg_userdetect).SetOnClickListener(this);
            view.FindViewById<Button>(Resource.Id.fg_risktoken).SetOnClickListener(this);

            client = SafetyDetect.GetClient(Activity);

            client.InitUserDetect()
                .AddOnSuccessListener(new OnSuccessListener((Result) => { Log.Info("InitUserDetect", "InitUserDetect is succeeded."); }))
                .AddOnFailureListener(new OnFailureListener((Result) => { Log.Error("InitUserDetect", "InitUserDetect is failed."); }));
            client.InitAntiFraud(appid)
                .AddOnSuccessListener(new OnSuccessListener((Result) => { Log.Info("InitAntiFraud", "InitAntiFraud is succeeded."); }))
                .AddOnFailureListener(new OnFailureListener((Result) => { Log.Error("InitAntiFraud", "InitAntiFraud is failed."); }));

            return view;
        }
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.fg_wifidetect:
                    GetWifiDetectStatus();
                    break;
                case Resource.Id.fg_userdetect:
                    DetectUser();
                    break;
                case Resource.Id.fg_risktoken:
                    GetRiskToken();
                    break;
                default:
                    break;
            }
        }

        private void GetWifiDetectStatus()
        {
            SafetyDetect.GetClient(Activity).WifiDetectStatus
                .AddOnSuccessListener(new OnSuccessListener((Result) =>
                {
                    WifiDetectResponse wifiDetectResponse = (WifiDetectResponse)Result;
                    Toast.MakeText(Activity.ApplicationContext, $"WifiDetect status: {wifiDetectResponse.WifiDetectStatus}", ToastLength.Short).Show();
                    Log.Info("WifiDetectStatus", $"{wifiDetectResponse.WifiDetectStatus}");
                }))
                .AddOnFailureListener(new OnFailureListener((Result) =>
                {
                    string errorMessage = null;
                    if (Result is ApiException exception)
                    {
                        errorMessage = $"{SafetyDetectStatusCodes.GetStatusCodeString(exception.StatusCode)}: {exception.Message}";
                    }
                    else errorMessage = ((Java.Lang.Exception)Result).Message;
                    Log.Error("WifiDetectStatus", errorMessage);
                    Toast.MakeText(Activity.ApplicationContext, $"GetWifiDetect status failed! Message: {errorMessage}", ToastLength.Short).Show();
                    //If Value is 19003 that means the user device area does not support the Safety Detect service.
                }));
        }
        private void DetectUser()
        {
            string appid = AGConnectServicesConfig.FromContext(Activity).GetString("client/app_id");
            SafetyDetect.GetClient(Activity).UserDetection(appid)
                .AddOnSuccessListener(new OnSuccessListener((Result) =>
                {
                    UserDetectResponse userDetectResponse = (UserDetectResponse)Result;
                    Log.Info("DetectUser", $"User detection succeed, response {userDetectResponse.ResponseToken}");
                    Toast.MakeText(Activity.ApplicationContext, $"User detection succeed.", ToastLength.Short).Show();
                }))
                .AddOnFailureListener(new OnFailureListener((Result) =>
                {
                    string errorMessage = null;
                    if (Result is ApiException exception)
                    {
                        errorMessage = $"{SafetyDetectStatusCodes.GetStatusCodeString(exception.StatusCode)}: {exception.Message}";
                    }
                    else errorMessage = ((Java.Lang.Exception)Result).Message;
                    Log.Error("DetectUser", errorMessage);
                    Toast.MakeText(Activity.ApplicationContext, $"User detection failed! Message: {errorMessage}", ToastLength.Short).Show();
                    // If value is 19800, this means Fake user detection failed. It also be throwed when user clicks cancel button.
                }));
        }

        private void GetRiskToken()
        {
            SafetyDetect.GetClient(Activity).RiskToken
                .AddOnSuccessListener(new OnSuccessListener((Result) =>
                {
                    RiskTokenResponse riskTokenResponse = (RiskTokenResponse)Result;
                    Log.Info("GetRiskToken", $"GetRiskToken succeed, response {riskTokenResponse.RiskToken}");
                    Toast.MakeText(Activity.ApplicationContext, $"GetRiskToken succeed response {riskTokenResponse.RiskToken}", ToastLength.Short).Show();
                }))
                .AddOnFailureListener(new OnFailureListener((Result) =>
                {
                    string errorMessage = null;
                    if (Result is ApiException exception)
                    {
                        errorMessage = $"{SafetyDetectStatusCodes.GetStatusCodeString(exception.StatusCode)}: {exception.Message}";
                    }
                    else errorMessage = ((Java.Lang.Exception)Result).Message;
                    Log.Error("GetRiskToken", errorMessage);
                    Toast.MakeText(Activity.ApplicationContext, $"GetRiskToken failed! Message: {errorMessage}", ToastLength.Short).Show();
                }));
        }

        public override void OnDestroyView()
        {
            client.ReleaseAntiFraud()
                .AddOnSuccessListener(new OnSuccessListener((Result) => { Log.Info("ReleaseAntiFraud", "ReleaseAntiFraud is succeeded."); }))
                .AddOnFailureListener(new OnFailureListener((Result) => { Log.Error("ReleaseAntiFraud", "ReleaseAntiFraud is failed."); }));
            client.ShutdownUserDetect()
                .AddOnSuccessListener(new OnSuccessListener((Result) => { Log.Info("ShutdownUserDetect", "ShutdownUserDetect is succeeded."); }))
                .AddOnFailureListener(new OnFailureListener((Result) => { Log.Error("ShutdownUserDetect", "ShutdownUserDetect is failed."); }));
            base.OnDestroyView();
        }
    }
}