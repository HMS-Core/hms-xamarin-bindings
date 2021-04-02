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
    /// Polygon shape related activity.
    /// </summary>
    [Activity(Label = "PolygonDemoActivity")]
    public class PolygonDemoActivity : AppCompatActivity, IOnMapReadyCallback, IOnPolygonClickListener
    {
        private const string TAG = "PolygonDemoActivity";

        private SupportMapFragment mSupportMapFragment;

        private HuaweiMap hMap;

        private Polygon mPolygon;

        private TextView polygonShown;

        private EditText oneLatitude;

        private EditText oneLongtitude;

        private EditText polygonTag;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_polygon_demo);
            var fragment = SupportFragmentManager.FindFragmentById(Resource.Id.mapInPolygon);
            if (fragment is SupportMapFragment) {
                mSupportMapFragment = (SupportMapFragment)fragment;
                mSupportMapFragment.GetMapAsync(this);
            }
            polygonShown = (TextView)FindViewById(Resource.Id.polygonShown);
            oneLatitude = (EditText)FindViewById(Resource.Id.oneLatitude);
            oneLongtitude = (EditText)FindViewById(Resource.Id.oneLongtitude);
            polygonTag = (EditText)FindViewById(Resource.Id.polygonTag);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.MyLocationEnabled = true;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 10));
        }

        /// <summary>
        /// Add polygons to the map.
        /// </summary>
        [Java.Interop.Export("AddPolygon")]
        public void AddPolygon(View view)
        {
            if (null == hMap)
            {
                return;
            }
            if (null != mPolygon)
            {
                mPolygon.Remove();
            }

            PolygonOptions polygonOptions = new PolygonOptions();
            var rectangleLatLng = MapUtils.CreateRectangle(new LatLng(48.893478, 2.334595), 0.1, 0.1);
            foreach(LatLng item in rectangleLatLng)
            {
                polygonOptions.Add(item);
            }
            polygonOptions.InvokeFillColor(Color.Green).InvokeStrokeColor(Color.Black);
            mPolygon = hMap.AddPolygon(polygonOptions);
            hMap.SetOnPolygonClickListener(this);
        }

        /// <summary>
        /// OnPolygonClickListener implemented by IOnPolygonClickListener
        /// </summary>
        public void OnPolygonClick(Polygon polygon)
        {
            Log.Info(TAG, "AddPolygon and OnPolygonClick start ");
            Toast.MakeText(ApplicationContext, "Polygon is clicked.", ToastLength.Long).Show();
        }

        /// <summary>
        /// Remove the polygon.
        /// </summary>
        [Java.Interop.Export("RemovePolygon")]
        public void RemovePolygon(View view)
        {
            if (null != mPolygon)
            {
                mPolygon.Remove();
            }
        }

        [Java.Interop.Export("SetPoints")]
        public void SetPoints(View v)
        {
            if (null != mPolygon)
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
                        mPolygon.Points = (MapUtils
                            .CreateRectangle(new LatLng(System.Double.Parse(latitude), System.Double.Parse(longtitude)), 0.5, 0.5));
                    }
                }
            }
        }

        /// <summary>
        /// Get the point position information of the polygon.
        /// </summary>
        [Java.Interop.Export("GetPoints")]
        public void GetPoints(View v)
        {
            if (null != mPolygon)
            {
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                for (int i = 0; i < mPolygon.Points.ToArray().Length; i++)
                {
                    stringBuilder.Append(mPolygon.Points.ElementAt(i).ToString());
                }
                Toast.MakeText(this, "Polygon points is " + stringBuilder, ToastLength.Long).Show();
            }
        }

        /// <summary>
        /// Set the outline color of the polygon.
        /// </summary>
        [Java.Interop.Export("SetStokeColor")]
        public void SetStokeColor(View v)
        {
            if (null != mPolygon)
            {
                mPolygon.StrokeColor = Color.Yellow;
            }
        }

        /// <summary>
        /// Get the outline color of the polygon.
        /// </summary>
        [Java.Interop.Export("GetStokeColor")]
        public void GetStokeColor(View v)
        {
            if (null != mPolygon)
            {
                polygonShown.Text = ("Polygon color is " + Integer.ToHexString(mPolygon.StrokeColor));
            }
        }

        /// <summary>
        /// Set the fill color of the polygon.
        /// </summary>
        [Java.Interop.Export("SetFillColor")]
        public void SetFillColor(View v)
        {
            if (null != mPolygon)
            {
                mPolygon.FillColor = Color.Cyan;
            }
        }

        /// <summary>
        /// Get the fill color of the polygon.
        /// </summary>
        [Java.Interop.Export("GetFillColor")]
        public void GetFillColor(View v)
        {
            if (null != mPolygon)
            {
                polygonShown.Text = "Polygon color is " + Integer.ToHexString(mPolygon.FillColor);
            }
        }

        /// <summary>
        /// Set the tag of the polygon.
        /// </summary>
        [Java.Interop.Export("SetTag")]
        public void SetTag(View v)
        {
            if (null != mPolygon)
            {
                string tag = polygonTag.Text.ToString().Trim();
                if (CheckUtils.CheckIsEdit(tag))
                {
                    Toast.MakeText(this, "Please make sure the tag is Edited", ToastLength.Short).Show();
                }
                else
                {
                    mPolygon.Tag = tag;
                }
            }
        }

        /// <summary>
        /// Get the tag of the polygon.
        /// </summary>
        [Java.Interop.Export("GetTag")]
        public void GetTag(View v)
        {
            if (null != mPolygon)
            {
                polygonShown.Text = (mPolygon.Tag == null ? "Tag is null" : mPolygon.Tag).ToString();
            }
        }

        /// <summary>
        /// Add polygon click event.
        /// </summary>
        [Java.Interop.Export("AddClickEvent")]
        public void AddClickEvent(View v)
        {
            if (null != mPolygon)
            {
                hMap.SetOnPolygonClickListener(this);
            }
        }

        /// <summary>
        /// Set polygons clickable.
        /// </summary>
        [Java.Interop.Export("SetClickableTrue")]
        public void SetClickableTrue(View v)
        {
            if (null != mPolygon)
            {
                mPolygon.Clickable = true;
            }
        }

        /// <summary>
        /// Set polygons are not clickable.
        /// </summary>
        [Java.Interop.Export("SetClickableFalse")]
        public void SetClickableFalse(View v)
        {
            if (null != mPolygon)
            {
                mPolygon.Clickable = false;
            }
        }


}
}