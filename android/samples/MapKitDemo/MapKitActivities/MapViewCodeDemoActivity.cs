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
    [Activity(Label = "MapViewCodeDemoActivity")]
    public class MapViewCodeDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "MapViewCodeDemoActivity";

        private HuaweiMap hMap;

        private MapView mMapView;

        private const string MAPVIEW_BUNDLE_KEY = "MapViewBundleKey";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            HuaweiMapOptions huaweiMapOptions = new HuaweiMapOptions();
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
            // please replace "Your API key" with api_key field value in
            // agconnect-services.json if the field is null.
            MapsInitializer.SetApiKey(Constants.API_KEY);
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

            HuaweiMapOptions huaweiMapOptions = new HuaweiMapOptions();


            var AmbientEnabled = huaweiMapOptions.AmbientEnabled;
            Log.Debug(TAG, "AmbientEnabled: " + AmbientEnabled.ToString());



            huaweiMapOptions.InvokeCamera(hMap.CameraPosition);
            huaweiMapOptions.InvokeLatLngBoundsForCameraTarget(new LatLngBounds(new LatLng(10,21), new LatLng(15, 21)));


            var Camera = huaweiMapOptions.Camera;
            Log.Debug(TAG, "Camera: " + Camera.ToString());



            var CompassEnabled = huaweiMapOptions.CompassEnabled;
            Log.Debug(TAG, "CompassEnabled: " + CompassEnabled.ToString());



            var LatLngBoundsForCameraTarget = huaweiMapOptions.LatLngBoundsForCameraTarget;
            Log.Debug(TAG, "LatLngBoundsForCameraTarget: " + LatLngBoundsForCameraTarget.ToString());


            var LiteMode = huaweiMapOptions.LiteMode;
            Log.Debug(TAG, "LiteMode: " + LiteMode.ToString());


            var MapToolbarEnabled = huaweiMapOptions.MapToolbarEnabled;
            Log.Debug(TAG, "MapToolbarEnabled: " + MapToolbarEnabled.ToString());


            var MapType = huaweiMapOptions.MapType;
            Log.Debug(TAG, "MapType: " + MapType.ToString());


            var MinZoomPreference = huaweiMapOptions.MinZoomPreference;
            Log.Debug(TAG, "MinZoomPreference: " + MinZoomPreference.ToString());


            var RotateGesturesEnabled = huaweiMapOptions.RotateGesturesEnabled;
            Log.Debug(TAG, "RotateGesturesEnabled: " + RotateGesturesEnabled.ToString());



            var ScrollGesturesEnabled = huaweiMapOptions.ScrollGesturesEnabled;
            Log.Debug(TAG, "ScrollGesturesEnabled: " + ScrollGesturesEnabled.ToString());



            var TiltGesturesEnabled = huaweiMapOptions.TiltGesturesEnabled;
            Log.Debug(TAG, "TiltGesturesEnabled: " + TiltGesturesEnabled.ToString());


            huaweiMapOptions.InvokeUseViewLifecycleInFragment(false);
            var UseViewLifecycleInFragment = huaweiMapOptions.UseViewLifecycleInFragment;
            Log.Debug(TAG, "UseViewLifecycleInFragment: " + UseViewLifecycleInFragment.ToString());


            huaweiMapOptions.InvokeZOrderOnTop(true);
            var ZOrderOnTop = huaweiMapOptions.ZOrderOnTop;
            Log.Debug(TAG, "ZOrderOnTop: " + ZOrderOnTop.ToString());



            var MaxZoomPreference = huaweiMapOptions.MaxZoomPreference;
            Log.Debug(TAG, "MaxZoomPreference: " + MaxZoomPreference.ToString());



            var ZoomControlsEnabled = huaweiMapOptions.ZoomControlsEnabled;
            Log.Debug(TAG, "ZoomControlsEnabled: " + ZoomControlsEnabled.ToString());



            var ZoomGesturesEnabled = huaweiMapOptions.ZoomGesturesEnabled;
            Log.Debug(TAG, "ZoomGesturesEnabled: " + ZoomGesturesEnabled.ToString());




            var DescribeContents = huaweiMapOptions.DescribeContents();
            Log.Debug(TAG, "DescribeContents: " + DescribeContents.ToString());
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