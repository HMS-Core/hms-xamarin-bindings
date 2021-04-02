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
    /// Lite Mode related activity.
    /// </summary>
    [Activity(Label = "LiteModeDemoActivity")]
    public class LiteModeDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "LiteModeDemoActivity";

        private const string MAPVIEW_BUNDLE_KEY = "MapViewBundleKey";

        HuaweiMap hMap;

        MapView mMapView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            HuaweiMapOptions huaweiMapOptions = new HuaweiMapOptions();
            huaweiMapOptions.InvokeLiteMode(true);
            huaweiMapOptions.InvokeCompassEnabled(true);
            huaweiMapOptions.InvokeZoomControlsEnabled(true);
            huaweiMapOptions.InvokeScrollGesturesEnabled(true);
            huaweiMapOptions.InvokeZoomGesturesEnabled(true);
            mMapView = new MapView(this, huaweiMapOptions);
            Bundle mapViewBundle = null;
            if (savedInstanceState != null)
            {
                mapViewBundle = savedInstanceState.GetBundle(MAPVIEW_BUNDLE_KEY);
            }

            mMapView.OnCreate(mapViewBundle);
            mMapView.GetMapAsync(this);
            SetContentView(mMapView);
        }

        protected override void OnStart()
        {
            base.OnStart();
            mMapView.OnStart();
        }

        protected override void OnStop()
        {
            base.OnStop();
            mMapView.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mMapView.OnDestroy();
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 10));
        }

        protected override void OnPause()
        {
            base.OnPause();
            mMapView.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            mMapView.OnResume();
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            mMapView.OnLowMemory();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            mMapView.OnSaveInstanceState(outState);
        }
    }
}