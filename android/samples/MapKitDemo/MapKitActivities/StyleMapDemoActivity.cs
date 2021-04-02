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
    /// Map style related activity.
    /// </summary>
    [Activity(Label = "StyleMapDemoActivity")]
    public class StyleMapDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "StyleMapDemoActivity";

        private HuaweiMap hMap;

        private MapFragment mMapFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_style_map_demo);
            mMapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.mapFragment_mapstyle);
            mMapFragment.GetMapAsync(this);
        }
        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.UiSettings.MyLocationButtonEnabled = false;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 10));
        }

        [Java.Interop.Export("SetNightStyle")]
        public void SetNightStyle(View view)
        {
            MapStyleOptions styleOptions = MapStyleOptions.LoadRawResourceStyle(this, Resource.Drawable.mapstyle_night_hms);
            hMap.SetMapStyle(styleOptions);
        }

        [Java.Interop.Export("SetRetroStyle")]
        public void SetRetroStyle(View view)
        {
            MapStyleOptions styleOptions = MapStyleOptions.LoadRawResourceStyle(this, Resource.Drawable.mapstyle_retro_hms);
            hMap.SetMapStyle(styleOptions);
        }
    }
}