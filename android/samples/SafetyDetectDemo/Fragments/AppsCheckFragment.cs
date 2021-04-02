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
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Common;
using Huawei.Hms.Support.Api.Entity.Core;
using Huawei.Hms.Support.Api.Entity.Safetydetect;
using Huawei.Hms.Support.Api.Safetydetect;
using System.Collections.Generic;
using System.Linq;
using SafetyDetectDemo.HmsSample;

namespace SafetyDetectDemo.Fragments
{
    public class AppsCheckFragment : Fragment, View.IOnClickListener
    {
        public static string TAG = typeof(AppsCheckFragment).Name;

        private ListView maliciousAppListView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fg_appscheck, container, false);

            view.FindViewById<Button>(Resource.Id.fg_enable_appscheck).SetOnClickListener(this);
            view.FindViewById<Button>(Resource.Id.fg_verify_appscheck).SetOnClickListener(this);
            view.FindViewById<Button>(Resource.Id.fg_get_malicious_apps).SetOnClickListener(this);

            maliciousAppListView = view.FindViewById<ListView>(Resource.Id.fg_list_app);

            return view;
        }
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.fg_enable_appscheck:
                    EnableAppsCheck();
                    break;
                case Resource.Id.fg_verify_appscheck:
                    VerifyAppsCheckEnabled();
                    break;
                case Resource.Id.fg_get_malicious_apps:
                    GetMaliciousApps();
                    break;
                default:
                    break;
            }
        }

        private void EnableAppsCheck()
        {
            SafetyDetect.GetClient(Activity).EnableAppsCheck()
                .AddOnSuccessListener(new OnSuccessListener((Result) =>
                {
                    VerifyAppsCheckEnabledResp appsCheckResp = (VerifyAppsCheckEnabledResp)Result;
                    bool isEnableAppsCheck = appsCheckResp.Result;
                    if (isEnableAppsCheck)
                    {
                        Toast.MakeText(Activity.ApplicationContext, "The AppsCheck feature is enabled.", ToastLength.Short).Show();
                        Log.Info("EnableAppsCheck", "The AppsCheck feature is enabled.");
                    }
                }))
                .AddOnFailureListener(new OnFailureListener((Result) =>
                {
                    string errorMessage = null;
                    if (Result is ApiException exception)
                    {
                        errorMessage = $"{SafetyDetectStatusCodes.GetStatusCodeString(exception.StatusCode)}: {exception.Message}";
                    }
                    else errorMessage = ((Java.Lang.Exception)Result).Message;
                    Log.Error("EnableAppsCheck", errorMessage);
                    Toast.MakeText(Activity.ApplicationContext, $"Enable AppsCheck failed! Message: {errorMessage}", ToastLength.Short).Show();
                }));
        }

        private void VerifyAppsCheckEnabled()
        {
            SafetyDetect.GetClient(Activity).IsVerifyAppsCheck()
                .AddOnSuccessListener(new OnSuccessListener((Result) =>
                {
                    VerifyAppsCheckEnabledResp appsCheckResp = (VerifyAppsCheckEnabledResp)Result;
                    bool isEnableAppsCheck = appsCheckResp.Result;
                    if (isEnableAppsCheck)
                    {
                        Toast.MakeText(Activity.ApplicationContext, "The AppsCheck feature is enabled.", ToastLength.Short).Show();
                        Log.Info("VerifyAppsCheckEnabled", "The AppsCheck feature is enabled.");
                    }
                }))
                .AddOnFailureListener(new OnFailureListener((Result) =>
                {
                    string errorMessage = null;
                    if (Result is ApiException exception)
                    {
                        errorMessage = $"{SafetyDetectStatusCodes.GetStatusCodeString(exception.StatusCode)}: {exception.Message}";
                    }
                    else errorMessage = ((Java.Lang.Exception)Result).Message;
                    Log.Error("VerifyAppsCheckEnabled", errorMessage);
                    Toast.MakeText(Activity.ApplicationContext, $"Verfy AppsCheck Enabled failed! Message: {errorMessage}", ToastLength.Short).Show();
                }));
        }

        private void GetMaliciousApps()
        {
            SafetyDetect.GetClient(Activity).MaliciousAppsList
                .AddOnSuccessListener(new OnSuccessListener((Result) =>
                {
                    MaliciousAppsListResp maliciousAppsListResp = (MaliciousAppsListResp)Result;
                    List<MaliciousAppsData> appsDataList = maliciousAppsListResp.MaliciousAppsList.ToList();
                    if (maliciousAppsListResp.RtnCode == CommonCode.Ok)
                    {
                        if (appsDataList.Count == 0)
                            Toast.MakeText(Activity.ApplicationContext, "No known potentially malicious apps are installed.", ToastLength.Short).Show();
                        else
                        {
                            foreach (MaliciousAppsData maliciousApp in appsDataList)
                            {
                                Log.Info("GetMaliciousApps", "Information about a malicious app:");
                                Log.Info("GetMaliciousApps", $"APK: {maliciousApp.ApkPackageName}");
                                Log.Info("GetMaliciousApps", $"SHA-256: {maliciousApp.ApkSha256}");
                                Log.Info("GetMaliciousApps", $"Category: {maliciousApp.ApkCategory}");
                            }
                            MaliciousAppsDataListAdapter maliciousAppAdapter = new MaliciousAppsDataListAdapter(appsDataList, Activity.ApplicationContext);
                            maliciousAppListView.Adapter = maliciousAppAdapter;
                        }
                    }
                }))
                .AddOnFailureListener(new OnFailureListener((Result) =>
                {
                    string errorMessage = null;
                    if (Result is ApiException exception)
                    {
                        errorMessage = $"{SafetyDetectStatusCodes.GetStatusCodeString(exception.StatusCode)}: {exception.Message}";
                    }
                    else errorMessage = ((Java.Lang.Exception)Result).Message;
                    Log.Error("GetMaliciousApps", errorMessage);
                    Toast.MakeText(Activity.ApplicationContext, $"Verfy AppsCheck Enabled failed! Message: {errorMessage}", ToastLength.Short).Show();
                }));
        }
    }
    public class MaliciousAppsDataListAdapter : BaseAdapter<MaliciousAppsData>
    {
        private List<MaliciousAppsData> maliciousAppsData;
        private Context context;

        public MaliciousAppsDataListAdapter(List<MaliciousAppsData> data, Context context)
        {
            maliciousAppsData = (data);
            this.context = context;
        }

        public override int Count => maliciousAppsData.Count;

        public override MaliciousAppsData this[int position] => maliciousAppsData[position];

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }


        public override long GetItemId(int position)
        {
            return position;
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.item_list_app, parent, false);
            TextView txtAppPackageName = convertView.FindViewById<TextView>(Resource.Id.txt_aName);//ignore error
            TextView txtAppCategory = convertView.FindViewById<TextView>(Resource.Id.txt_aCategory);//ignore error
            MaliciousAppsData oneMaliciousAppsData = maliciousAppsData[position];
            txtAppPackageName.Text = (oneMaliciousAppsData.ApkPackageName);
            txtAppCategory.Text = (GetCategory(oneMaliciousAppsData.ApkCategory));
            return convertView;
        }

        private string GetCategory(int apkCategory)
        {
            if (apkCategory == AppsCheckConstants.VirusLevelRisk)
            {
                return context.GetString(Resource.String.app_type_risk);
            }
            else if (apkCategory == AppsCheckConstants.VirusLevelVirus)
            {
                return context.GetString(Resource.String.app_type_virus);
            }
            else
            {
                return context.GetString(Resource.String.app_type_unknown);
            }
        }
    }

}