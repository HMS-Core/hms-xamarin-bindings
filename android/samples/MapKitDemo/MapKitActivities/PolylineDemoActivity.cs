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
using Java.Lang;
using XHms_Map_Kit_Demo_Project.Utils;
using static Huawei.Hms.Maps.HuaweiMap;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// Polyline related activity.
    /// </summary>
    [Activity(Label = "PolylineDemoActivity")]
    public class PolylineDemoActivity : AppCompatActivity, IOnMapReadyCallback, IOnPolylineClickListener
    {
        private const string TAG = "PolylineDemoActivity";

        private SupportMapFragment mSupportMapFragment;

        private HuaweiMap hMap;

        private Polyline mPolyline;

        private TextView polylineShown;

        private EditText oneLatitude;

        private EditText oneLongtitude;

        private EditText polylineStokeWidth;

        private EditText polylineTag;

        private IList<LatLng> points = new List<LatLng>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_polyline_demo);
            mSupportMapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapInPolyline);
            mSupportMapFragment.GetMapAsync(this);

            polylineShown = (TextView)FindViewById(Resource.Id.polylineShown);
            oneLatitude = (EditText)FindViewById(Resource.Id.oneLatitude);
            oneLongtitude = (EditText)FindViewById(Resource.Id.oneLongtitude);
            polylineStokeWidth = (EditText)FindViewById(Resource.Id.polylineStokeWidth);
            polylineTag = (EditText)FindViewById(Resource.Id.polylineTag);

            points.Add(MapUtils.HUAWEI_CENTER);
            points.Add(MapUtils.APARTMENT_CENTER);
            points.Add(MapUtils.EPARK_CENTER);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.MyLocationEnabled = true;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 10));
        }

        /// <summary>
        /// Add polyline to the map.
        /// </summary>
        [Java.Interop.Export("AddPolyline")]
        public void AddPolyline(View view)
        {
            if (hMap == null)
            {
                return;
            }
            if (mPolyline != null)
            {
                mPolyline.Remove();
            }
            mPolyline = hMap.AddPolyline(
                new PolylineOptions().Add(MapUtils.FRANCE, MapUtils.FRANCE1, MapUtils.FRANCE2, MapUtils.FRANCE3)
                    .InvokeColor(Color.Blue)
                    .InvokeWidth(3));
            hMap.SetOnPolylineClickListener(this);
        }

        public void OnPolylineClick(Polyline polyline)
        {
            Log.Info(TAG, "OnMapReady:OnPolylineClick ");
            Toast.MakeText(ApplicationContext, "Polyline is clicked.", ToastLength.Long).Show();
        }


        /// <summary>
        /// Remove the polyline
        /// </summary>
        [Java.Interop.Export("RemovePolyline")]
        public void RemovePolyline(View view)
        {
            if (null != mPolyline)
            {
                mPolyline.Remove();
            }
            points.Clear();
            points.Add(MapUtils.HUAWEI_CENTER);
            points.Add(MapUtils.APARTMENT_CENTER);
            points.Add(MapUtils.EPARK_CENTER);
        }

        /// <summary>
        /// Set the point position information of the polyline.
        /// </summary>
        [Java.Interop.Export("SetOnePoint")]
        public void SetOnePoint(View v)
        {
            if (null != mPolyline)
            {
                string latitude = oneLatitude.Text.ToString().Trim();
                string longtitude = oneLongtitude.Text.ToString().Trim();
                if (CheckUtils.CheckIsEdit(latitude) || CheckUtils.CheckIsEdit(longtitude))
                {
                    Toast.MakeText(this, "Please make sure the latitude & longtitude is Edited", ToastLength.Short).Show();
                }
                else
                {
                    if (!CheckUtils.CheckIsRight(latitude) || !CheckUtils.CheckIsRight(longtitude))
                    {
                        Toast.MakeText(this, "Please make sure the latitude & longtitude is right", ToastLength.Short).Show();
                            
                    }
                    else
                    {
                        points.Add(new LatLng(System.Double.Parse(latitude), System.Double.Parse(longtitude)));
                        mPolyline.Points = points;
                    }
                }
            }
        }

        /// <summary>
        /// Get the point position information of the polyline.
        /// </summary>
        [Java.Interop.Export("GetPoints")]
        public void GetPoints(View v)
        {
            if (null != mPolyline)
            {
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                for (int i = 0; i < mPolyline.Points.ToArray().Length; i++)
                {
                    stringBuilder.Append(mPolyline.Points.ElementAt(i).ToString());
                }
                Toast.MakeText(this, "Polyline points is " + stringBuilder, ToastLength.Long).Show();
            }
        }

        /// <summary>
        /// Set the outline color of the polyline.
        /// </summary>
        [Java.Interop.Export("SetStokeColor")]
        public void SetStokeColor(View v)
        {
            if (null != mPolyline)
            {
                mPolyline.Color = Color.Yellow;
            }
        }

        /// <summary>
        /// Get the outline color of the polyline.
        /// </summary>
        [Java.Interop.Export("GetStokeColor")]
        public void GetStokeColor(View v)
        {
            if (null != mPolyline)
            {
                polylineShown.Text = "Polyline color is " + Integer.ToHexString(mPolyline.Color);
            }
        }

        /// <summary>
        /// Set the width of the polyline.
        /// </summary>
        [Java.Interop.Export("SetWidth")]
        public void SetWidth(View v)
        {
            if (null != mPolyline)
            {
                string width = polylineStokeWidth.Text.ToString().Trim();
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
                        if (Single.Parse(width) < 0.0F)
                        {
                            Toast.MakeText(this,
                                            "Please make sure the width is right, this value must be non-negative",
                                            ToastLength.Short).Show();
                            return;
                        }
                        mPolyline.Width = Single.Parse(width);
                    }
                }
            }
        }

        /// <summary>
        /// Get the width of the polyline.
        /// </summary>
        [Java.Interop.Export("GetWidth")]
        public void GetWidth(View v)
        {
            if (null != mPolyline)
            {
                polylineShown.Text = ("Polyline width is " + mPolyline.Width);
            }
        }

        /// <summary>
        /// Set the tag of the polyline.
        /// </summary>
        [Java.Interop.Export("SetTag")]
        public void SetTag(View v)
        {
            if (null != mPolyline)
            {
                string tag = polylineTag.Text.ToString().Trim();
                if (CheckUtils.CheckIsEdit(tag))
                {
                    Toast.MakeText(this, "Please make sure the tag is Edited", ToastLength.Short).Show();
                }
                else
                {
                    mPolyline.Tag = tag;
                }
            }
        }

        /// <summary>
        /// Get the tag of the polyline.
        /// </summary>
        [Java.Interop.Export("GetTag")]
        public void GetTag(View v)
        {
            if (null != mPolyline)
            {
                polylineShown.Text = ((mPolyline.Tag == null ? "Tag is null" : mPolyline.Tag).ToString());
            }
        }

        /// <summary>
        /// Add polyline click event.
        /// </summary>
        [Java.Interop.Export("AddClickEvent")]
        public void AddClickEvent(View v)
        {
            if (null != mPolyline)
            {
                hMap.SetOnPolylineClickListener(this);
            }
        }

        /// <summary>
        /// Set polyline clickable.
        /// </summary>
        [Java.Interop.Export("SetClickableTrue")]
        public void SetClickableTrue(View v)
        {
            if (null != mPolyline)
            {
                mPolyline.Clickable = true;
            }
        }

        /// <summary>
        /// Set polyline are not clickable.
        /// </summary>
        [Java.Interop.Export("SetClickableFalse")]
        public void SetClickableFalse(View v)
        {
            if (null != mPolyline)
            {
                mPolyline.Clickable = false;
            }
        }
    }
}