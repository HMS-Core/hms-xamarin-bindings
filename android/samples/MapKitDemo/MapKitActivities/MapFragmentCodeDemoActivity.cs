﻿/*
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
    [Activity(Label = "MapFragmentCodeDemoActivity")]
    public class MapFragmentCodeDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "MapFragmentCodeActivity";

        private HuaweiMap hMap;

        private MapFragment mMapFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_mapfragmentcode_demo);
            HuaweiMapOptions huaweiMapOptions = new HuaweiMapOptions();
            huaweiMapOptions.InvokeCompassEnabled(true);
            huaweiMapOptions.InvokeZoomGesturesEnabled(true);
            mMapFragment = MapFragment.NewInstance(huaweiMapOptions);
            FragmentManager fragmentManager = FragmentManager;
            FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.frame_mapfragmentcode, mMapFragment);
            fragmentTransaction.Commit();

            mMapFragment.GetMapAsync(this);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.BuildingsEnabled = true;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 10));
        }
    }
}