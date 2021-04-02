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
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Huawei.Hms.Maps;
using Huawei.Hms.Maps.Model;
using Java.Lang;
using XHms_Map_Kit_Demo_Project.Utils;
using static Huawei.Hms.Maps.HuaweiMap;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// Circle shape related activity.
    /// </summary>
    [Activity(Label = "CircleDemoActivity")]
    public class CircleDemoActivity : AppCompatActivity, IOnMapReadyCallback, IOnCircleClickListener
    {
        private const string TAG = "CircleDemoActivity";

        private SupportMapFragment mSupportMapFragment;

        private HuaweiMap hMap;

        private Circle mCircle;

        private TextView circleShown;

        private EditText centerLatitude;

        private EditText centerLongtitude;

        private EditText circleRadius;

        private EditText circleStokeWidth;

        private EditText circleTag;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_circle_demo);
            mSupportMapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapInCircle);
            mSupportMapFragment.GetMapAsync(this);

            circleShown = (TextView)FindViewById(Resource.Id.circleShown);
            centerLatitude = (EditText)FindViewById(Resource.Id.centerLatitude);
            centerLongtitude = (EditText)FindViewById(Resource.Id.centerLongtitude);
            circleRadius = (EditText)FindViewById(Resource.Id.circleRadius);
            circleStokeWidth = (EditText)FindViewById(Resource.Id.circleStokeWidth);
            circleTag = (EditText)FindViewById(Resource.Id.circleTag);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Info(TAG, "onMapReady: ");
            hMap = map;
            hMap.MyLocationEnabled = (true);
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 14));
        }

        /// <summary>
        /// Remove the circle.
        /// </summary>
        [Java.Interop.Export("RemoveCircle")]
        public void RemoveCircle(View view)
        {
            if (null != mCircle)
            {
                mCircle.Remove();
            }
        }

        /// <summary>
        /// Add a circle on the map.
        /// </summary>
        [Java.Interop.Export("AddCircle")]
        public void AddCircle(View view)
        {
            if (null == hMap)
            {
                return;
            }
            if (null != mCircle)
            {
                mCircle.Remove();
            }
            mCircle = hMap.AddCircle(new CircleOptions().InvokeCenter(new LatLng(48.893478, 2.334595))
                .InvokeRadius(500)
                .InvokeFillColor(unchecked((int)0XFF00FFFF))
                .InvokeStrokeWidth(10)
                .InvokeStrokeColor(Color.Red));
        }

        /// <summary>
        /// Set the center of the circle.
        /// </summary>
        [Java.Interop.Export("SetCenter")]
        public void SetCenter(View v)
        {
            if (null != mCircle)
            {
                LatLng center = null;
                if (!TextUtils.IsEmpty(centerLatitude.Text) && !TextUtils.IsEmpty(centerLongtitude.Text))
                {
                    string latitude = centerLatitude.Text.ToString().Trim();
                    string longtitude = centerLongtitude.Text.ToString().Trim();
                    center = new LatLng(System.Double.Parse(latitude), System.Double.Parse(longtitude));
                }
                try
                {
                    mCircle.Center = center;
                }
                catch (NullPointerException e)
                {
                    Log.Error(TAG, "NullPointerException " + e.ToString());
                    Toast.MakeText(this, "NullPointerException", ToastLength.Short).Show();
                }
            }
        }

        /// <summary>
        /// Get the center coordinates.
        /// </summary>
        [Java.Interop.Export("GetCenter")]
        public void GetCenter(View v)
        {
            if (null != mCircle)
            {
                circleShown.Text = "Circle center is " + mCircle.Center.ToString();
            }
        }

        /// <summary>
        /// Set the radius of the circle.
        /// </summary>
        [Java.Interop.Export("SetRadius")]
        public void SetRadius(View v)
        {
            if (null != mCircle)
            {
                string radius = circleRadius.Text.ToString().Trim();
                if (CheckUtils.CheckIsEdit(radius))
                {
                    Toast.MakeText(this, "Please make sure the radius is Edited", ToastLength.Short).Show();
                }
                else
                {
                    if (!CheckUtils.CheckIsRight(radius))
                    {
                        Toast.MakeText(this, "Please make sure the radius is right", ToastLength.Short).Show();
                    }
                    else
                    {
                        try
                        {
                            mCircle.Radius = System.Double.Parse(radius);
                        }
                        catch (IllegalArgumentException e)
                        {
                            Toast.MakeText(this, "IllegalArgumentException ", ToastLength.Short).Show();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the radius of the circle.
        /// </summary>
        [Java.Interop.Export("GetRadius")]
        public void GetRadius(View v)
        {
            if (null != mCircle)
            {
                circleShown.Text = ("Circle radius is " + mCircle.Radius);
            }
        }

        /// <summary>
        /// Get the fill color of the circle.
        /// </summary>
        [Java.Interop.Export("SetFillColor")]
        public void SetFillColor(View v)
        {
            if (null != mCircle)
            {
                mCircle.FillColor = Color.Red;
            }
        }

        /// <summary>
        /// Set the fill color of the circle.
        /// </summary>
        [Java.Interop.Export("GetFillColor")]
        public void GetFillColor(View v)
        {
            if (null != mCircle)
            {
                circleShown.Text = "Circle fill color is " + Integer.ToHexString(mCircle.FillColor);
            }
        }

        bool flag = true;

        [Java.Interop.Export("SetStokeColor")]
        public void SetStokeColor(View v)
        {
            if (null != mCircle)
            {
                if (flag)
                {
                    mCircle.StrokeColor = Color.Red;
                    flag = false;
                }
                else
                {
                    mCircle.StrokeColor = Color.Gray;
                    flag = true;
                }
            }
        }

        /// <summary>
        /// Get the outline color of the circle.
        /// </summary>
        [Java.Interop.Export("GetStokeColor")]
        public void GetStokeColor(View v)
        {
            if (null != mCircle)
            {
                circleShown.Text = "Circle stroke color is " + Integer.ToHexString(mCircle.StrokeColor);
            }
        }

        /// <summary>
        /// Set the outline width of the circle.
        /// </summary>
        [Java.Interop.Export("SetWidth")]
        public void SetWidth(View v)
        {
            if (null != mCircle)
            {
                string width = circleStokeWidth.Text.ToString().Trim();
                if (CheckUtils.CheckIsEdit(width))
                {
                    Toast.MakeText(this, "Please make sure the width is Edited", ToastLength.Short).Show();
                }
                else
                {
                    if (!CheckUtils.CheckIsRight(width))
                    {
                        Toast.MakeText(this, "Please make sure the width is right", ToastLength.Short).Show();
                    }
                    else
                    {
                        try
                        {
                            mCircle.StrokeWidth =  Single.Parse(width);
                        }
                        catch (IllegalArgumentException e)
                        {
                            Log.Error(TAG, "IllegalArgumentException " + e.ToString());
                            Toast.MakeText(this, "IllegalArgumentException", ToastLength.Short).Show();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the outline width of the circle.
        /// </summary>
        [Java.Interop.Export("GetWidth")]
        public void GetWidth(View v)
        {
            if (null != mCircle)
            {
                circleShown.Text = "Circle stroke width is " + mCircle.StrokeWidth;
            }
        }

        /// <summary>
        /// Set the tag of the circle.
        /// </summary>
        [Java.Interop.Export("SetTag")]
        public void SetTag(View v)
        {
            if (null != mCircle)
            {
                string tag = circleTag.Text.ToString().Trim();
                if (CheckUtils.CheckIsEdit(tag))
                {
                    Toast.MakeText(this, "Please make sure the tag is Edited", ToastLength.Short).Show();
                }
                else
                {
                    mCircle.Tag = tag;
                }
            }
        }

        /// <summary>
        /// Get the tag of the circle.
        /// </summary>
        [Java.Interop.Export("GetTag")]
        public void GetTag(View v)
        {
            if (null != mCircle)
            {
                circleShown.Text = (mCircle.Tag).ToString();
            }
        }

        /// <summary>
        /// Set click event for circle.
        /// </summary>
        [Java.Interop.Export("AddClickEvent")]
        public void AddClickEvent(View v)
        {
            if (null != mCircle)
            {
                hMap.SetOnCircleClickListener(this);
            }
        }

        public void OnCircleClick(Circle circle)
        {
            if (mCircle.Equals(circle))
            {
                Toast.MakeText(ApplicationContext, "Circle is clicked.", ToastLength.Long).Show();
            }
        }

        /// <summary>
        /// Set the circle clickable to true.
        /// </summary>
        [Java.Interop.Export("SetClickableTrue")]
        public void SetClickableTrue(View v)
        {
            if (null != mCircle)
            {
                mCircle.Clickable = true;
            }
        }

        /// <summary>
        /// Set the circle clickable to false.
        /// </summary>
        [Java.Interop.Export("SetClickableFalse")]
        public void SetClickableFalse(View v)
        {
            if (null != mCircle)
            {
                mCircle.Clickable = false;
            }
        }

    }
}