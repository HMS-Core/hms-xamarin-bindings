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
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Huawei.Hms.Maps;
using Huawei.Hms.Maps.Model;
using static Huawei.Hms.Maps.HuaweiMap;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// This shows how to listen to some HuaweiMap events.
    /// </summary>
    [Activity(Label = "EventsDemoActivity")]
    public class EventsDemoActivity : AppCompatActivity, HuaweiMap.IOnMapClickListener,
    IOnMapLongClickListener, IOnCameraIdleListener, IOnMapReadyCallback, HuaweiMap.IOnMyLocationButtonClickListener
    {
        private const string TAG = "EventsDemoActivity";

        private TextView mTapView;

        private TextView mToLatLngView;

        private TextView mToPointView;

        private HuaweiMap hMap;

        private SupportMapFragment mSupportMapFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_events_demo);
            mSupportMapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mSupportMapFragment.GetMapAsync(this);
            mTapView = (TextView)FindViewById(Resource.Id.tap_text);
            mToPointView = (TextView)FindViewById(Resource.Id.toPoint);
            mToLatLngView = (TextView)FindViewById(Resource.Id.toLatlng);
        }

        public void OnMapReady(HuaweiMap map)
        {
            hMap = map;
            hMap.SetOnMapClickListener(this);
            hMap.SetOnMapLongClickListener(this);
            hMap.SetOnCameraIdleListener(this);
            hMap.MyLocationEnabled = true;
            hMap.SetOnMyLocationButtonClickListener(this);
        }

        //Map click event.
        public void OnMapClick(LatLng latLng)
        {
            mTapView.Text = "point=" + latLng + "is tapped";
            Point point = hMap.Projection.ToScreenLocation(latLng);
            mToPointView.Text = "to point, point=" + point;
            LatLng newLatlng = hMap.Projection.FromScreenLocation(point);
            mToLatLngView.Text = "to latlng, latlng=" + newLatlng;
            VisibleRegion visibleRegion = hMap.Projection.VisibleRegion;
            Log.Info(TAG, visibleRegion.ToString());
        }

        //Map long click event.
        public void OnMapLongClick(LatLng point)
        {
            mTapView.Text = "long pressed, point=" + point;
        }

        //Callback when the camera move ends.
        public void OnCameraIdle()
        {
            Toast.MakeText(this, "camera move stoped", ToastLength.Long).Show();
        }

        //Map click event.
        public bool OnMyLocationButtonClick()
        {
            Toast.MakeText(this, "MyLocation button clicked", ToastLength.Short).Show();
            return false;
        }
    }
}