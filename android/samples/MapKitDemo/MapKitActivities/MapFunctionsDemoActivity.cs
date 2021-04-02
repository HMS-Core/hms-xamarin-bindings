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
using Java.Util;
using XHms_Map_Kit_Demo_Project.Utils;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    [Activity(Label = "MapFunctionsDemoActivity")]
    public class MapFunctionsDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "MapFunctionsDemoActivity";

        private SupportMapFragment mSupportMapFragment;

        private HuaweiMap hMap;

        private EditText left;

        private EditText right;

        private EditText top;

        private EditText bottom;

        private EditText minZoomlevel;

        private EditText maxZoomlevel;

        private TextView text;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.acitivity_map_founctions_demo);
            mSupportMapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapInFunctions);
            mSupportMapFragment.GetMapAsync(this);

            left = (EditText)FindViewById(Resource.Id.paddingleft);
            right = (EditText)FindViewById(Resource.Id.paddingright);
            top = (EditText)FindViewById(Resource.Id.paddingtop);
            bottom = (EditText)FindViewById(Resource.Id.paddingbottom);
            text = (TextView)FindViewById(Resource.Id.founctionsshow);
            minZoomlevel = (EditText)FindViewById(Resource.Id.minZoomlevel);
            maxZoomlevel = (EditText)FindViewById(Resource.Id.maxZoomlevel);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.MyLocationEnabled = true;
            hMap.ResetMinMaxZoomPreference();
        }

        /// <summary>
        /// Get the maximum zoom level parameter.
        /// </summary>
        [Java.Interop.Export("GetMaxZoomLevel")]
        public void GetMaxZoomLevel(View view)
        {
            if (null != hMap)
            {
                text.Text = (hMap.MaxZoomLevel.ToString());
            }
        }

        /// <summary>
        /// Get the minimum zoom level parameter.
        /// </summary>
        [Java.Interop.Export("GetMinZoomLevel")]
        public void GetMinZoomLevel(View view)
        {
            if (null != hMap)
            {
                text.Text = (hMap.MinZoomLevel.ToString());
            }
        }

        /// <summary>
        /// Get map type.
        /// </summary>
        [Java.Interop.Export("GetMapType")]
        public void GetMapType(View view)
        {
            if (null != hMap)
            {
                text.Text = ((hMap.MapType == MapUtils.MAP_TYPE_NONE) ? "MAP_TYPE_NONE" : "MAP_TYPE_NORMAL");
            }
        }

        /// <summary>
        /// Set map type.
        /// </summary>
        [Java.Interop.Export("SetMapType")]
        public void SetMapType(View view)
        {
            if (null != hMap)
            {
                lock (hMap)
                {
                    if (hMap.MapType == MapUtils.MAP_TYPE_NORMAL)
                    {
                        hMap.MapType = HuaweiMap.MapTypeNone;
                    }
                    else
                    {
                        hMap.MapType = HuaweiMap.MapTypeNormal;
                    }
                }
            }
        }

        /// <summary>
        /// Get 3D mode settings.
        /// </summary>
        [Java.Interop.Export("Is3DMode")]
        public void Is3DMode(View view)
        {
            if (null != hMap)
            {
                text.Text = (hMap.BuildingsEnabled.ToString());
            }
        }

        /// <summary>
        /// Turn on the 3D switch.
        /// </summary>
        [Java.Interop.Export("Set3DMode")]
        public void Set3DMode(View view)
        {
            if (null != hMap)
            {
                if (hMap.BuildingsEnabled)
                {
                    hMap.BuildingsEnabled = false;
                }
                else
                {
                    hMap.BuildingsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Set the maximum value of the desired camera zoom level.
        /// </summary>
        [Java.Interop.Export("SetMaxZoomPreference")]
        public void SetMaxZoomPreference(View view)
        {
            string text = maxZoomlevel.Text.ToString();
            if ((text.Trim().Length == 0) || (text == null) || ("".Equals(text)))
            {
                Toast.MakeText(this, "Please make sure the maxZoom is Edited", ToastLength.Short).Show();
            }
            else
            {
                if (!CheckUtils.IsNumber(text.Trim()))
                {
                    Toast.MakeText(this, "Please make sure the maxZoom is right", ToastLength.Short).Show();
                    return;
                }
                if (Single.Parse(text.Trim()) > MapUtils.MAX_ZOOM_LEVEL
                    || Single.Parse(text.Trim()) < MapUtils.MIN_ZOOM_LEVEL)
                {
                    Toast
                        .MakeText(this, String.Format(Locale.English.ToString(), "The zoom level ranges from %s to %s.",
                            MapUtils.MIN_ZOOM_LEVEL, MapUtils.MAX_ZOOM_LEVEL), ToastLength.Short).Show();
                }
                else
                {
                    float maxZoom = Single.Parse(maxZoomlevel.Text.ToString());
                    Log.Info(TAG, "SetMaxZoomPreference: " + maxZoom);
                    if (null != hMap)
                    {
                        hMap.SetMaxZoomPreference(maxZoom);
                    }
                }
            }
        }

        /// <summary>
        /// Test the maximum zoom parameter.
        /// </summary>
        [Java.Interop.Export("TestMaxZoom")]
        public void TestMaxZoom(View view)
        {
            CameraUpdate cameraUpdate = CameraUpdateFactory.ZoomBy(1.0f);
            if (null != hMap)
            {
                hMap.MoveCamera(cameraUpdate);
            }
        }

        /// <summary>
        /// Set the minimum value of the desired camera zoom level.
        /// </summary>
        [Java.Interop.Export("SetMinZoomPreference")]
        public void SetMinZoomPreference(View view)
        {
            string text = minZoomlevel.Text.ToString();
            if ((text.Trim().Length == 0)  || (text == null) || ("".Equals(text)))
            {
                Toast.MakeText(this, "Please make sure the minZoom is Edited", ToastLength.Short).Show();
            }
            else
            {
                if (!CheckUtils.IsNumber(text.Trim()))
                {
                    Toast.MakeText(this, "Please make sure the minZoom is right", ToastLength.Short).Show();
                    return;
                }
                if (Single.Parse(text.Trim()) > MapUtils.MAX_ZOOM_LEVEL
                    || Single.Parse(text.Trim()) < MapUtils.MIN_ZOOM_LEVEL)
                {
                    Toast.MakeText(this, String.Format(Locale.English.ToString(), "The zoom level ranges from %s to %s.",
                                    MapUtils.MIN_ZOOM_LEVEL, MapUtils.MAX_ZOOM_LEVEL), ToastLength.Short).Show();
                }
                else
                {
                    if (null != hMap)
                    {
                        hMap.SetMinZoomPreference(Single.Parse(minZoomlevel.Text.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Remove the previously set zoom level upper and lower boundary values.
        /// </summary>
        [Java.Interop.Export("ResetMinMaxZoomPreference")]
        public void ResetMinMaxZoomPreference(View view)
        {
            if (null != hMap)
            {
                hMap.ResetMinMaxZoomPreference();
            }
        }

        /// <summary>
        /// Set the map border fill width for the map.
        /// </summary>
        [Java.Interop.Export("SetPadding")]
        public void SetPadding(View view)
        {
            string leftString = left.Text.ToString();
            string topString = top.Text.ToString();
            string rightString = right.Text.ToString();
            string bottomString = bottom.Text.ToString();

            if ((leftString.Trim().Length == 0) || (leftString == null)
                || ("".Equals(leftString)) || (topString.Trim().Length == 0) 
                || (topString == null)     || ("".Equals(topString)) || (rightString.Trim().Length == 0)
                || (rightString == null)   || ("".Equals(rightString))
                || (bottomString.Trim().Length == 0) || (bottomString == null)
                || ("".Equals(bottomString)))
            {
            }
            else
            {
                if (!CheckUtils.IsNumber(leftString.Trim()) || !CheckUtils.IsNumber(topString.Trim()) || !CheckUtils.IsNumber(rightString.Trim())
                    || !CheckUtils.IsNumber(bottomString.Trim()))
                {
                    Toast.MakeText(this, "Please make sure the padding value is right", ToastLength.Short).Show();
                }
                else
                {
                    if (null != hMap)
                    {
                        hMap.SetPadding(int.Parse(left.Text.ToString()),
                            int.Parse(top.Text.ToString()), int.Parse(right.Text.ToString()),
                            int.Parse(bottom.Text.ToString()));
                    }
                }
            }
        }
    }
}