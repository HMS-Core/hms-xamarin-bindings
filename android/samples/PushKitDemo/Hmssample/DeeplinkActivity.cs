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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Util;
using Java.Lang;

namespace XamarinHmsPushDemo.Hmssample
{
    [Activity(Name = "com.huawei.xahmspushdemo.DeeplinkActivity", Label = "DeeplinkActivity")]
    public class DeeplinkActivity : Activity
    {
        private static readonly string TAG = "Deep Link Activity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_deeplink);
            GetIntentData(Intent);
        }

        private void GetIntentData(Intent intent)
        {
            if (intent != null)
            {
                int iAge1 = 0;
                string name1 = null;
                try
                {
                    Android.Net.Uri uri = intent.Data;
                    if (uri == null)
                    {
                        Log.Info(TAG, "getData null");
                        return;
                    }
                    string age1 = uri.GetQueryParameter("age");
                    name1 = uri.GetQueryParameter("name");

                    if (age1 != null)
                    {
                        int intAge = Convert.ToInt32(age1);
                        iAge1 = intAge;
                    }
                }
                catch (NullPointerException e)
                {
                    Log.Error(TAG, "NullPointer," + e);
                }
                catch (NumberFormatException e)
                {
                    Log.Error(TAG, "NumberFormatException," + e);
                }
                catch (UnsupportedOperationException e)
                {
                    Log.Error(TAG, "UnsupportedOperationException," + e);
                }
                finally
                {
                    Log.Info(TAG, "name " + name1 + ",age " + iAge1);
                    Toast.MakeText(this, "name " + name1 + ", age " + iAge1, ToastLength.Short).Show();
                }

            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            GetIntentData(intent);
        }
    }



}