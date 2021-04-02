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
using System.Collections.Generic;
using System.Linq;
using SafetyDetectDemo.HmsSample;
using static Android.Views.View;

namespace SafetyDetectDemo.Fragments
{
    public class UrlCheckFragment : Fragment, IOnClickListener
    {
        public static string TAG = typeof(UrlCheckFragment).Name;

        ISafetyDetectClient client;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fg_urlcheck, container, false);

            Spinner spinner = view.FindViewById<Spinner>(Resource.Id.fg_url_spinner);
            spinner.ItemSelected += Spinner_ItemSelected;
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(Activity, Resource.Array.url_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            client = SafetyDetect.GetClient(Activity);
            view.FindViewById<Button>(Resource.Id.fg_call_url_btn).SetOnClickListener(this);

            return view;
        }
        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string url = spinner.GetItemAtPosition(e.Position).ToString();
            EditText textView = Activity.FindViewById<EditText>(Resource.Id.fg_call_urlCheck_text);
            textView.Text = url;
            TextView textResult = Activity.FindViewById<TextView>(Resource.Id.fg_call_urlResult);
            textResult.Text = string.Empty;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.fg_call_url_btn:
                    CallUrlCheckApi();
                    break;
                default:
                    break;
            }
        }

        private void CallUrlCheckApi()
        {
            string appid = AGConnectServicesConfig.FromContext(Activity).GetString("client/app_id");
            EditText editText = Activity.FindViewById<EditText>(Resource.Id.fg_call_urlCheck_text);
            string realUrl = editText.Text.Trim();
            TextView textResult = Activity.FindViewById<TextView>(Resource.Id.fg_call_urlResult);
            client.UrlCheck(realUrl, appid, UrlCheckThreat.Malware, UrlCheckThreat.Phishing)
                .AddOnSuccessListener(new OnSuccessListener((Result) =>
                {
                    /**
                     * Called after successfully communicating with the SafetyDetect API.
                     * The #onSuccess callback receives an
                     * {@link Com.Huawei.Hms.Support.Api.Entity.Safetydetect.UrlCheckResponse} that contains a
                     * list of UrlCheckThreat that contains the threat type of the Url.
                     *  Indicates communication with the service was successful.
                     *  Identify any detected threats.
                     *  Get UrlCheckResponse property of UrlCheckResponse then you can get List<UrlCheckThreat> .
                     *  If List<UrlCheckThreat> is empty , that means no threats found , else that means threats found
                     */
                    List<UrlCheckThreat> list = ((UrlCheckResponse)Result).GetUrlCheckResponse().ToList();
                    if (list.Count == 0)
                    {
                        textResult.Text = "No threats found.";
                        Log.Info("UrlCheck", $"{textResult.Text}");
                    }
                    else
                    {
                        textResult.Text = "Threats found!";
                        Log.Info("UrlCheck", $"{textResult.Text}");
                        foreach (UrlCheckThreat line in list)
                            Log.Info("UrlCheck", $"Threat type: {line.UrlCheckResult}");
                    }

                })).AddOnFailureListener(new OnFailureListener((Result) =>
                {
                    string errorMessage = null;
                    if (Result is ApiException exception)
                    {
                        errorMessage = $"{SafetyDetectStatusCodes.GetStatusCodeString(exception.StatusCode)}: {exception.Message}";
                    }
                    Log.Error("UrlCheck", errorMessage);
                }));
        }

        public override void OnResume()
        {
            base.OnResume();
            client.InitUrlCheck()
                .AddOnSuccessListener(new OnSuccessListener((Result) => { Log.Info("InitUrlCheck", "InitUrlCheck is succeeded."); }))
                .AddOnFailureListener(new OnFailureListener((Result) => { Log.Error("InitUrlCheck", "InitUrlCheck is failed."); }));
        }
        public override void OnPause()
        {
            base.OnPause();
            client.ShutdownUrlCheck()
                .AddOnSuccessListener(new OnSuccessListener((Result) => { Log.Info("ShutdownUrlCheck", "ShutdownUrlCheck is succeeded."); }))
                .AddOnFailureListener(new OnFailureListener((Result) => { Log.Error("ShutdownUrlCheck", "ShutdownUrlCheck is failed."); }));
        }

    }
}