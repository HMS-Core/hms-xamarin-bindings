/*
*       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Huawei.Hms.Maps;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// Multilanguage related activity.
    /// </summary>
    [Activity(Label = "MoreLanguageDemoActivity")]
    public class MoreLanguageDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "MoreLanguageDemoActivity";

        private SupportMapFragment mSupportMapFragment;

        private HuaweiMap hMap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG,"OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_more_language_demo);
            var fragment = SupportFragmentManager.FindFragmentById(Resource.Id.mapInLanguage);
            if (fragment is SupportMapFragment) {
                mSupportMapFragment = (SupportMapFragment)fragment;
            }
            mSupportMapFragment.GetMapAsync(this);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.SetLanguage("en");
            hMap.MyLocationEnabled = true;
        }

        [Java.Interop.Export("SetTurkish")]
        public void SetTurkish(View view)
        {
            hMap.SetLanguage("tr");
        }

        [Java.Interop.Export("SetChinese")]
        public void SetChinese(View view)
        {
            hMap.SetLanguage("zh");
        }

        [Java.Interop.Export("SetArabic")]
        public void SetArabic(View view)
        {
            hMap.SetLanguage("ar");
        }

        [Java.Interop.Export("SetEnglish")]
        public void SetEnglish(View view)
        {
            hMap.SetLanguage("en");
        }
    }
}