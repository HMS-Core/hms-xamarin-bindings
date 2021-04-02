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
    [Activity(Label = "SupportMapDemoActivity")]
    public class SupportMapDemoActivity : AppCompatActivity, IOnMapReadyCallback, IOnMapLongClickListener
    {
        private const string TAG = "SupportMapDemoActivity";
        private static readonly LatLng Beijing = new LatLng(48.893478, 2.334595);
        private static readonly LatLng Shanghai = new LatLng(48.7, 2.12);

        private HuaweiMap hMap;

        private Marker mBeijing;

        private Marker mShanghai;

        private SupportMapFragment mSupportMapFragment;

        private bool visible = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_supportmapfragment_demo);
            mSupportMapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.supportMap);
            mSupportMapFragment.GetMapAsync(this);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.UiSettings.CompassEnabled = true;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 14));
            hMap.SetOnMapLongClickListener(this);
        }

        public void OnMapLongClick(LatLng latLng)
        {
            Log.Debug(TAG, "OnMapLongClick: latLng " + " please input latLng");
        }

        [Java.Interop.Export("AddMarker")]
        public void AddMarker(View view)
        {
            if (mBeijing == null && mShanghai == null)
            {
                // Uses a colored icon.
                mBeijing = hMap.AddMarker(new MarkerOptions().InvokePosition(Beijing).InvokeTitle("Beijing").Clusterable(true));
                mShanghai = hMap.AddMarker(new MarkerOptions().InvokePosition(Shanghai)
                    .InvokeAlpha(0.8f)
                    .InvokeIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.badge_ph)));
            }
            if (null != mBeijing)
            {
                mBeijing.Title = "hello";
                mBeijing.Snippet = "world";
                mBeijing.Tag = "huaweimap";
                mBeijing.Draggable = true;
            }
            if (null != mShanghai)
            {
                mShanghai.Title = "Hello";
                mShanghai.Draggable = true;
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            mSupportMapFragment.OnSaveInstanceState(outState);
        }

        [Java.Interop.Export("SetVisibility")]
        public void SetVisibility(View view)
        {
            if (null != mBeijing)
            {
                if (visible)
                {
                    mBeijing.Visible = true;
                    visible = false;
                }
                else
                {
                    mBeijing.Visible = false;
                    visible = true;
                }
            }
        }

        [Java.Interop.Export("SetAlpha")]
        public void SetAlpha(View view)
        {
            if (null != mBeijing)
            {
                if (visible)
                {
                    mBeijing.Alpha  = 0.5f;
                    visible = false;
                }
                else
                {
                    mBeijing.Alpha = 1.0f;
                    visible = true;
                }
            }
        }

        [Java.Interop.Export("SetFlat")]
        public void SetFlat(View view)
        {
            if (null != mBeijing)
            {
                if (visible)
                {
                    mBeijing.Flat = true;
                    visible = false;
                }
                else
                {
                    mBeijing.Flat = false;
                    visible = true;
                }
            }
        }

        [Java.Interop.Export("SetZIndex")]
        public void SetZIndex(View view)
        {
            if (null != mBeijing)
            {
                if (visible)
                {
                    mBeijing.ZIndex = 20f;
                    visible = false;
                }
                else
                {
                    mBeijing.ZIndex = -20f;
                    visible = true;
                }
            }
        }

        [Java.Interop.Export("SetRotation")]
        public void SetRotation(View view)
        {
            if (null != mBeijing)
            {
                if (visible)
                {
                    mBeijing.Rotation = 30.0f;
                    visible = false;
                }
                else
                {
                    mBeijing.Rotation = 60.0f;
                    visible = true;
                }
            }
        }

        [Java.Interop.Export("RemoveMarker")]
        public void RemoveMarker(View view)
        {
            if (null != mBeijing)
            {
                mBeijing.Remove();
                mBeijing = null;
            }

            if (null != mShanghai)
            {
                mShanghai.Remove();
                mShanghai = null;
            }
        }

        [Java.Interop.Export("ShowInfoWindow")]
        public void ShowInfoWindow(View view)
        {
            if (null != mBeijing)
            {
                if (visible)
                {
                    mBeijing.ShowInfoWindow();
                    visible = false;
                }
                else
                {
                    mBeijing.HideInfoWindow();
                    visible = true;
                }
            }
        }

        [Java.Interop.Export("SetAnchor")]
        public void SetAnchor(View view)
        {
            if (null != mBeijing)
            {
                mBeijing.SetMarkerAnchor(0.9F, 0.9F);
            }
        }

        [Java.Interop.Export("SetIcon")]
        public void SetIcon(View view)
        {
            if (null != mBeijing)
            {
                Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.badge_tr);
                BitmapDescriptor bitmapDescriptor = BitmapDescriptorFactory.FromBitmap(bitmap);
                mBeijing.SetIcon(bitmapDescriptor);
            }
        }
    }
}