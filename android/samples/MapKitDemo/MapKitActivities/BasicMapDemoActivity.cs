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

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// This shows how we create a basic activity with a map.
    /// </summary>
    [Activity(Label = "BasicMapDemoActivity")]
    public class BasicMapDemoActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string TAG = "BasicMapDemoActivity";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_basic_demo);

            Button mapView = (Button)FindViewById(Resource.Id.btn_mapView);
            Button mapViewCode = (Button)FindViewById(Resource.Id.btn_mapViewCode);
            Button mapFragment = (Button)FindViewById(Resource.Id.btn_mapFragment);
            Button mapFragmentCode = (Button)FindViewById(Resource.Id.btn_mapFragmentCode);
            Button supportMapFragment = (Button)FindViewById(Resource.Id.btn_supportMapFragment);
            Button supportMapFragmentCode = (Button)FindViewById(Resource.Id.btn_supportMapFragmentCode);
            mapView.SetOnClickListener(this);
            mapViewCode.SetOnClickListener(this);
            mapFragment.SetOnClickListener(this);
            mapFragmentCode.SetOnClickListener(this);
            supportMapFragment.SetOnClickListener(this);
            supportMapFragmentCode.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.btn_mapView:
                    Log.Debug(TAG, "MapView: ");
                    StartActivity(new Intent(this, typeof(MapViewDemoActivity)));
                    break;
                case Resource.Id.btn_mapViewCode:
                    Log.Debug(TAG, "MapViewCode: ");
                    StartActivity(new Intent(this, typeof(MapViewCodeDemoActivity)));
                    break;
                case Resource.Id.btn_mapFragment:
                    Log.Debug(TAG, "MapFragment: ");
                    StartActivity(new Intent(this, typeof(MapFragmentDemoActivity)));
                    break;
                case Resource.Id.btn_mapFragmentCode:
                    Log.Debug(TAG, "MapFragmentCode: ");
                    StartActivity(new Intent(this, typeof(MapFragmentCodeDemoActivity)));
                    break;
                case Resource.Id.btn_supportMapFragment:
                    Log.Debug(TAG, "SupportMapFragment: ");
                    StartActivity(new Intent(this, typeof(SupportMapDemoActivity)));
                    break;
                case Resource.Id.btn_supportMapFragmentCode:
                    Log.Debug(TAG, "SupportMapFragmentCode: ");
                    StartActivity(new Intent(this, typeof(SupportMapCodeDemoActivity)));
                    break;
                default:
                    break;
            }
        }
    }
}