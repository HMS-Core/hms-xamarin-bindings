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
using Org.Json;
using System;
using System.Text;
using SafetyDetectDemo.HmsSample;
using static Android.Views.View;

namespace SafetyDetectDemo.Fragments
{
    public class SysIntegrityFragment : Fragment, IOnClickListener
    {
        public static string TAG = typeof(SysIntegrityFragment).Name;

        Button theButton;
        TextView basicIntegrityTextView;
        TextView adviceTextView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fg_sysintegrity, container, false);

            basicIntegrityTextView = view.FindViewById<TextView>(Resource.Id.fg_payloadBasicIntegrity);
            adviceTextView = view.FindViewById<TextView>(Resource.Id.fg_payloadAdvice);

            theButton = view.FindViewById<Button>(Resource.Id.fg_button_sys_integrity_go);
            theButton.SetOnClickListener(this);

            return view;
        }

        private void SysIntegrity()
        {
            byte[] nonce = new byte[24];
            string appid = AGConnectServicesConfig.FromContext(Activity).GetString("client/app_id");
            SafetyDetect.GetClient(Activity).SysIntegrity(nonce, appid).AddOnSuccessListener(new OnSuccessListener((Result) =>
            {
                SysIntegrityResp response = (SysIntegrityResp)Result;
                string jwStr = response.Result;
                string[] jwsSplit = jwStr.Split(".");
                string jwsPayloadStr = jwsSplit[1];
                byte[] data = Convert.FromBase64String(jwsPayloadStr);
                string jsonString = Encoding.UTF8.GetString(data);
                JSONObject jObject = new JSONObject(jsonString);
                Log.Info("SysIntegrity", jsonString.Replace(",",",\n"));

                string basicIntegrityText = null;
                try
                {
                    bool basicIntegrity = jObject.GetBoolean("basicIntegrity");
                    if (basicIntegrity)
                    {
                        theButton.SetBackgroundResource(Resource.Drawable.btn_round_green);
                        basicIntegrityText = "Basic Integrity is Success.";
                    }
                    else
                    {
                        theButton.SetBackgroundResource(Resource.Drawable.btn_round_red);
                        basicIntegrityText = "Basic Integrity is Failure.";
                        adviceTextView.Text = $"Advice: {jObject.GetString("advice")}";
                    }
                }
                catch (JSONException e)
                {
                    Android.Util.Log.Error("SysIntegrity", e.Message);
                }
                basicIntegrityTextView.Text = basicIntegrityText;
                theButton.SetText(Resource.String.rerun);

            })).AddOnFailureListener(new OnFailureListener((Result) =>
            {
                string errorMessage = null;
                if (Result is ApiException exception)
                {
                    errorMessage = $"{SafetyDetectStatusCodes.GetStatusCodeString(exception.StatusCode)}: {exception.Message}";
                }
                else
                {
                    errorMessage = ((Java.Lang.Exception)Result).Message;
                }
                Log.Error("SysIntegrity", errorMessage);
                theButton.SetBackgroundResource(Resource.Drawable.btn_round_yellow);
            }));
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.fg_button_sys_integrity_go:
                    ProcessView();
                    SysIntegrity();
                    break;
                default:
                    break;
            }
        }
        public void ProcessView()
        {
            basicIntegrityTextView.Text = string.Empty;
            adviceTextView.Text = string.Empty;
            theButton.SetText(Resource.String.processing);
            theButton.SetBackgroundResource(Resource.Drawable.btn_round_proccessing);
        }
    }
}