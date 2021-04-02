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
using Huawei.Hms.Maps.Model;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// Create a simple activity with a map and a marker on the map.
    /// </summary>
    [Activity(Label = "SupportMapCodeDemoActivity")]
    public class SupportMapCodeDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "SupportMapCodeActivity";

        private HuaweiMap hMap;

        private SupportMapFragment mSupportMapFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "onCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_supportmapcode_demo);

            Initialize();
        }

        public void Initialize()
        {
            LatLng southwest = new LatLng(30, 118);
            CameraPosition.Builder builder = new CameraPosition.Builder();
            CameraPosition cameraPosition =
                    builder.Target(southwest).Zoom(2).Bearing(2.0f).Tilt(2.5f).Build();
            HuaweiMapOptions huaweiMapOptions = new HuaweiMapOptions().InvokeCamera(cameraPosition);
            mSupportMapFragment = SupportMapFragment.NewInstance(huaweiMapOptions);
            mSupportMapFragment.GetMapAsync(this);
            AndroidX.Fragment.App.FragmentManager fragmentManager = SupportFragmentManager;
            AndroidX.Fragment.App.FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.frame_supportmapcode, mSupportMapFragment);
            fragmentTransaction.Commit();
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 10));
        }

    }
}