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
using Huawei.Hms.Maps;
using Huawei.Hms.Maps.Model;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    [Activity(Label = "MapViewDemoActivity")]
    public class MapViewDemoActivity : Activity, IOnMapReadyCallback
    {
        private const string TAG = "MapViewDemoActivity";

        private const string MAPVIEW_BUNDLE_KEY = "MapViewBundleKey";

        private HuaweiMap hMap;

        private MapView mMapView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_mapview_demo);
            mMapView = (MapView)FindViewById(Resource.Id.mapView);
            Bundle mapViewBundle = null;
            if (savedInstanceState != null)
            {
                mapViewBundle = savedInstanceState.GetBundle(MAPVIEW_BUNDLE_KEY);
            }
            // please replace "Your API key" with api_key field value in
            // agconnect-services.json if the field is null.
            MapsInitializer.SetApiKey(Constants.API_KEY);
            mMapView.OnCreate(mapViewBundle);
            mMapView.GetMapAsync(this);
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

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "onMapReady: ");
            hMap = map;
            hMap.MyLocationEnabled = false;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 10));
        }
    }
}