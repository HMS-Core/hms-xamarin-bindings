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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Ads.Identifier;
using Java.IO;

namespace XamarinAdsOAIDDemo.Oaid
{
    public class OaidSdkUtil
    {
        private static readonly string TAG = "OaidSdkUtil";
        public static void SetVerifyID(Context context, string adId)
        {
            SetVerifyIdTask setVerifyIdTask = new SetVerifyIdTask(context, adId);
            setVerifyIdTask.Execute();
        }
        public static void GetOaid(Context context, OaidCallback callback)
        {
            if(null == context || null == callback)
            {
                Log.Error(TAG, "invalid input param");
                return;
            }
            try
            {
                
                //Get advertising id information. Do not call this method in the main thread.
                AdvertisingIdClient.Info info = AdvertisingIdClient.GetAdvertisingIdInfo(context);
                Log.Info(TAG, "AdvertisingIdClient.GetAdvertisingIdInfo function called successfully.");
                
                if (null != info)
                {
                    Log.Info(TAG, "AdvertisingIdClient.Info.Id: " + info.Id);
                    Log.Info(TAG, "AdvertisingIdClient.Info.IsLimitAdTrackingEnabled: " + info.IsLimitAdTrackingEnabled);
                    callback.OnSuccess(info.Id, info.IsLimitAdTrackingEnabled);
                }
                else
                {
                    callback.OnFail("OAID is null");
                }
            }
            catch (IOException ex)
            {
                Log.Error(TAG, "AdvertisingIdInfo IOException");
                callback.OnFail("AdvertisingIdInfo IOException:" + ex.Message);
            }
        }

        public class SetVerifyIdTask : AsyncTask
        {
            Context context;
            string adId;
            public SetVerifyIdTask(Context context,string adId)
            {
                this.context = context;
                this.adId = adId;
            }
            protected override void OnPreExecute()
            {
                Log.Info("SetVerifyIdTask", "OnPreExecute");
            }
            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
            {
                try
                {
                    AdvertisingIdClient.VerifyAdId(context, adId, true);
                }
                catch (AdIdVerifyException ex)
                {
                    Log.Info("SetVerifyIdTask", ex.Message);
                    return null;
                }

                return "success";
            }

            protected override void OnPostExecute(Java.Lang.Object result)
            {
                if (result.Equals("success"))
                {
                    Log.Info("SetVerifyIdTask", "AdvertisingIdClient.VerifyAdId called successfully.");
                }
                else
                {
                    Log.Info("SetVerifyIdTask", "error");
                }
            }
        }
    }
}