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
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using Huawei.Hms.Maps;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// Gestures related activity.
    /// </summary>
    [Activity(Label = "GestureDemoActivity")]
    public class GestureDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "GestureDemoActivity";

        private SupportMapFragment mSupportMapFragment;

        private HuaweiMap hMap;

        private UiSettings mUiSettings;

        private CheckBox mMyLocationButtonCheckbox;

        private CheckBox mMyLocationLayerCheckbox;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_gestures_demo);
            mSupportMapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapInGestures);
            mSupportMapFragment.GetMapAsync(this);

            mMyLocationButtonCheckbox = (CheckBox)FindViewById(Resource.Id.isShowMylocationButton);
            mMyLocationLayerCheckbox = (CheckBox)FindViewById(Resource.Id.isMyLocationLayerEnabled);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.MyLocationEnabled = false;
            hMap.UiSettings.CompassEnabled = false;
            hMap.UiSettings.ZoomControlsEnabled = false;
            hMap.UiSettings.MyLocationButtonEnabled = false;
            mUiSettings = hMap.UiSettings;
        }

        private bool CheckReady()
        {
            if (hMap == null)
            {
                Toast.MakeText(this, "Map is not ready yet", ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Set map zoom button available.
        /// </summary>
        [Java.Interop.Export("SetZoomButtonsEnabled")]
        public void SetZoomButtonsEnabled(View v)
        {
            if (!CheckReady())
            {
                return;
            }
            mUiSettings.ZoomControlsEnabled = (((CheckBox)v).Checked);
        }

        /// <summary>
        /// Set compass available.
        /// </summary>
        [Java.Interop.Export("SetCompassEnabled")]
        public void SetCompassEnabled(View v)
        {
            if (!CheckReady())
            {
                return;
            }
            mUiSettings.CompassEnabled = (((CheckBox)v).Checked);
        }

        /// <summary>
        /// Set my location button is available.
        /// </summary>
        [Java.Interop.Export("SetMyLocationButtonEnabled")]
        public void SetMyLocationButtonEnabled(View v)
        {
            if (!CheckReady())
            {
                return;
            }
            if (ContextCompat.CheckSelfPermission(this,
                Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted)
            {
                if (mMyLocationLayerCheckbox.Checked)
                {
                    mUiSettings.MyLocationButtonEnabled = mMyLocationButtonCheckbox.Checked;
                }
                else
                {
                    Toast.MakeText(this, "Please open My Location Layer first", ToastLength.Long).Show();
                    mMyLocationButtonCheckbox.Checked = false;
                }

            }
            else
            {
                Toast.MakeText(this,
                    "System positioning permission was not obtained, please turn on system positioning permission first",
                    ToastLength.Long).Show();
                mMyLocationButtonCheckbox.Checked = false;
            }
        }

        /// <summary>
        /// Set my location layer available.
        /// </summary>
        [Java.Interop.Export("SetMyLocationLayerEnabled")]
        public void SetMyLocationLayerEnabled(View v)
        {
            if (!CheckReady())
            {
                return;
            }
            if (ContextCompat.CheckSelfPermission(this,
                Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted)
            {
                hMap.MyLocationEnabled = mMyLocationLayerCheckbox.Checked;
            }
            else
            {
                mMyLocationLayerCheckbox.Checked = false;
            }
        }

        /// <summary>
        /// Set scroll gestures available.
        /// </summary>
        [Java.Interop.Export("SetScrollGesturesEnabled")]
        public void SetScrollGesturesEnabled(View v)
        {
            if (!CheckReady())
            {
                return;
            }
            mUiSettings.ScrollGesturesEnabled = (((CheckBox)v).Checked);
        }

        /// <summary>
        /// Set zoom gestures available.
        /// </summary>
        [Java.Interop.Export("SetZoomGesturesEnabled")]
        public void SetZoomGesturesEnabled(View v)
        {
            if (!CheckReady())
            {
                return;
            }
            mUiSettings.ZoomGesturesEnabled = (((CheckBox)v).Checked);
        }

        /// <summary>
        /// Set tilt gestures available.
        /// </summary>
        [Java.Interop.Export("SetTiltGesturesEnabled")]
        public void SetTiltGesturesEnabled(View v)
        {
            if (!CheckReady())
            {
                return;
            }
            mUiSettings.TiltGesturesEnabled = ((CheckBox)v).Checked;
        }

        /// <summary>
        /// Set the rotation gesture available.
        /// </summary>
        [Java.Interop.Export("SetRotateGesturesEnabled")]
        public void SetRotateGesturesEnabled(View v)
        {
            if (!CheckReady())
            {
                return;
            }
            mUiSettings.RotateGesturesEnabled = ((CheckBox)v).Checked;
        }

    }
}