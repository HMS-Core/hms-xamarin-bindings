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
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Huawei.Hms.Maps;
using Huawei.Hms.Maps.Model;
using Java.Lang;
using XHms_Map_Kit_Demo_Project.Utils;
using static Huawei.Hms.Maps.HuaweiMap;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// Camera related activity.
    /// </summary>
    [Activity(Label = "CameraDemoActivity")]
    public class CameraDemoActivity : AppCompatActivity, IOnMapReadyCallback, View.IOnClickListener,
    IOnCameraMoveStartedListener, IOnCameraMoveListener, IOnCameraIdleListener, IOnMapLoadedCallback
    {
        private const string TAG = "CameraDemoActivity";

        public static readonly int REQUEST_CODE = 0X01;

        private static readonly string[] PERMISSIONS =
            {Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation};

        private static readonly float ZOOM_DELTA = 2.0f;

        private SupportMapFragment mSupportMapFragment;

        private HuaweiMap hMap;

        private TextView cameraChange;

        private float mMaxZoom = 20.0f;

        private float mMinZoom = 3.0f;

        private EditText cameraLat;

        private EditText cameraLng;

        private EditText cameraZoom;

        private EditText cameraTilt;

        private EditText cameraBearing;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_camera_demo);
            if (!HasLocationPermission())
            {
                ActivityCompat.RequestPermissions(this, PERMISSIONS, REQUEST_CODE);
            }
            var fragment = SupportFragmentManager.FindFragmentById(Resource.Id.mapInCamera);
            if (fragment is SupportMapFragment)
            {
                mSupportMapFragment = (SupportMapFragment)fragment;
                mSupportMapFragment.GetMapAsync(this);
            }
            cameraLat = (EditText)FindViewById(Resource.Id.cameraLat);
            cameraLng = (EditText)FindViewById(Resource.Id.cameraLng);
            cameraZoom = (EditText)FindViewById(Resource.Id.cameraZoom);
            cameraTilt = (EditText)FindViewById(Resource.Id.cameraTilt);
            cameraBearing = (EditText)FindViewById(Resource.Id.cameraBearing);
            Button btn1 = (Button)FindViewById(Resource.Id.animateCamera);
            btn1.SetOnClickListener(this);
            Button btn2 = (Button)FindViewById(Resource.Id.getCameraPosition);
            btn2.SetOnClickListener(this);
            Button btn3 = (Button)FindViewById(Resource.Id.moveCamera);
            btn3.SetOnClickListener(this);
            Button btn4 = (Button)FindViewById(Resource.Id.ZoomBy);
            btn4.SetOnClickListener(this);
            Button btn5 = (Button)FindViewById(Resource.Id.newLatLngBounds);
            btn5.SetOnClickListener(this);
            Button btn6 = (Button)FindViewById(Resource.Id.setCameraPosition);
            btn6.SetOnClickListener(this);

            cameraChange = (TextView)FindViewById(Resource.Id.cameraChange);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode == REQUEST_CODE)
            {
                for (int i = 0; i < permissions.Length; i++)
                {
                    if (grantResults[i] == Permission.Granted)
                    {
                        Toast.MakeText(this, permissions[i] + " permission setting successfully", ToastLength.Long).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, permissions[i] + " permission setting failed", ToastLength.Long).Show();
                        Finish();
                    }
                }
            }
        }

        /// <summary>
        /// Determine if you have the location permission.
        /// </summary>
        private bool HasLocationPermission()
        {
            foreach (string permission in PERMISSIONS)
            {
                if (ActivityCompat.CheckSelfPermission(this, permission) != Permission.Granted)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determine whether to turn on GPS.
        /// </summary>
        private bool IsGPSOpen(Context context)
        {
            LocationManager locationManager = (LocationManager)context.GetSystemService(Context.LocationService);
            bool gps = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            bool network = locationManager.IsProviderEnabled(LocationManager.NetworkProvider);
            if (gps || network)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OnMapReady(HuaweiMap huaweiMap)
        {
            Log.Info(TAG, "OnMapReady: ");
            hMap = huaweiMap;
            if (IsGPSOpen(this))
            {
                hMap.MyLocationEnabled = true;
                hMap.UiSettings.MyLocationButtonEnabled = true;
            }
            else
            {
                hMap.MyLocationEnabled = false;
                hMap.UiSettings.MyLocationButtonEnabled = false;
            }
            hMap.SetOnCameraMoveStartedListener(this);
            hMap.SetOnCameraIdleListener(this);
            hMap.SetOnCameraMoveListener(this);
            hMap.SetOnMapLoadedCallback(this);
        }

        public void OnClick(View v)
        {
            if (hMap == null)
            {
                Log.Warn(TAG, "map is null");
                return;
            }
            if (v.Id == Resource.Id.animateCamera)
            {
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLng(new LatLng(20, 120));
                Toast.MakeText(this, hMap.CameraPosition.Target.ToString(), ToastLength.Long).Show();
                hMap.AnimateCamera(cameraUpdate);
            }
            if (v.Id == Resource.Id.getCameraPosition)
            {
                CameraPosition position = hMap.CameraPosition;
                Toast.MakeText(ApplicationContext, position.ToString(), ToastLength.Long).Show();

                // Displays the maximum zoom level and minimum scaling level of the current camera.
                Log.Info(TAG, position.ToString());
                Log.Info(TAG, "MaxZoomLevel:" + hMap.MaxZoomLevel + " MinZoomLevel:" + hMap.MinZoomLevel);
            }
            if (v.Id == Resource.Id.moveCamera  )
            {
                CameraPosition build = new CameraPosition.Builder().Target(new LatLng(60, 60)).Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(build);
                Toast.MakeText(this, hMap.CameraPosition.ToString(), ToastLength.Long).Show();
                hMap.MoveCamera(cameraUpdate);
            }
            if (v.Id == Resource.Id.ZoomBy)
            {
                CameraUpdate cameraUpdate = CameraUpdateFactory.ZoomBy(2);
                Toast.MakeText(this, "amount = 2", ToastLength.Long).Show();
                hMap.MoveCamera(cameraUpdate);
            }
            if (v.Id == Resource.Id.newLatLngBounds)
            {
                LatLng southwest = new LatLng(30, 60);
                LatLng northeast = new LatLng(60, 120);
                LatLngBounds latLngBounds = new LatLngBounds(southwest, northeast);
                Toast.MakeText(this,"southwest =" + southwest.ToString() + " northeast=" + northeast.ToString() + " padding=2",
                        ToastLength.Long).Show();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngBounds(latLngBounds, 2);
                hMap.MoveCamera(cameraUpdate);
            }
            if (v.Id == Resource.Id.setCameraPosition)
            {
                LatLng southwest = new LatLng(30, 60);
                CameraPosition cameraPosition = 
                    new CameraPosition.Builder().Target(southwest).Zoom(10).Bearing(2.0f).Tilt(2.5f).Build();
                Toast.MakeText(this, cameraPosition.ToString(), ToastLength.Long).Show();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                hMap.MoveCamera(cameraUpdate);
            }
        }

        /// <summary>
        /// Callback when the camera starts moving.
        /// </summary>
        public void OnCameraMoveStarted(int reason)
        {
            Log.Info(TAG, "OnCameraMoveStarted: susccessful");
            if (reason == OnCameraMoveStartedListener.ReasonDeveloperAnimation)
            {
                Log.Info(TAG, "OnCameraMove");
            }
        }

        /// <summary>
        /// Set camera move callback.
        /// </summary>
        public void OnCameraMove()
        {
            Log.Info(TAG, "OnCameraMove: successful");
        }

        /// <summary>
        /// Callback when the map loaded.
        /// </summary>
        public void OnMapLoaded()
        {
            Log.Info(TAG, "OnMapLoaded:successful");
        }

        /// <summary>
        /// Callback when the camera move ends.
        /// </summary>
        public void OnCameraIdle()
        {
            cameraChange.Text = (hMap.CameraPosition.ToString());
            Log.Info(TAG, "OnCameraIdle: successful");
        }

        /// <summary>
        /// Set the upper limit of camera zoom.
        /// </summary>
        [Java.Interop.Export("OnSetMaxZoomClamp")]
        public void OnSetMaxZoomClamp(View view)
        {
            mMaxZoom -= ZOOM_DELTA;
            if (mMaxZoom < MapUtils.MIN_ZOOM_LEVEL)
            {
                Toast.MakeText(this, "The minimum zoom level is " + MapUtils.MIN_ZOOM_LEVEL + ", and cannot be decreased.",
                        ToastLength.Short).Show();
                mMaxZoom = MapUtils.MIN_ZOOM_LEVEL;
                hMap.SetMaxZoomPreference(mMaxZoom);
                return;
            }
            // Constrains the maximum zoom level.
            hMap.SetMaxZoomPreference(mMaxZoom);
            Toast.MakeText(this, "Max zoom preference set to: " + mMaxZoom, ToastLength.Short).Show();
        }

        /// <summary>
        /// Set the lower limit of camera zoom.
        /// </summary>
        [Java.Interop.Export("OnSetMinZoomClamp")]
        public void OnSetMinZoomClamp(View view)
        {
            mMinZoom += ZOOM_DELTA;
            if (mMinZoom > MapUtils.MAX_ZOOM_LEVEL)
            {
                Toast.MakeText(this, "The maximum zoom level is " + MapUtils.MAX_ZOOM_LEVEL + ", and cannot be increased.",
                                ToastLength.Short).Show();
                mMinZoom = MapUtils.MAX_ZOOM_LEVEL;
                hMap.SetMinZoomPreference(mMinZoom);
                return;
            }
            // Constrains the minimum zoom level.
            hMap.SetMinZoomPreference(mMinZoom);
            Toast.MakeText(this, "Min zoom preference set to: " + mMinZoom,
                            ToastLength.Short).Show();
        }

        [Java.Interop.Export("SetCameraPosition")]
        public void SetCameraPosition(View view)
        {
            try
            {
                LatLng latLng = null;
                float zoom = 2.0f;
                float bearing = 2.0f;
                float tilt = 2.0f;
                if (!TextUtils.IsEmpty(cameraLng.Text) && !TextUtils.IsEmpty(cameraLat.Text))
                {
                    latLng = new LatLng(float.Parse(cameraLat.Text.ToString().Trim()),
                        float.Parse(cameraLng.Text.ToString().Trim()));
                }
                if (!TextUtils.IsEmpty(cameraZoom.Text))
                {
                    zoom = float.Parse(cameraZoom.Text.ToString().Trim());
                }
                if (!TextUtils.IsEmpty(cameraBearing.Text))
                {
                    bearing = float.Parse(cameraBearing.Text.ToString().Trim());
                }
                if (!TextUtils.IsEmpty(cameraTilt.Text))
                {
                    tilt = float.Parse(cameraTilt.Text.ToString().Trim());
                }
                CameraPosition.Builder builder = new CameraPosition.Builder();
                CameraPosition cameraPosition =
                    builder.Target(latLng).Zoom(zoom).Bearing(bearing).Tilt(tilt).Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                hMap.MoveCamera(cameraUpdate);
            }
            catch (IllegalArgumentException e)
            {
                Log.Error(TAG, "IllegalArgumentException " + e.ToString());
                Toast.MakeText(this, "IllegalArgumentException", ToastLength.Short).Show();
            }
            catch (NullPointerException e)
            {
                Log.Error(TAG, "NullPointerException " + e.ToString());
                Toast.MakeText(this, "NullPointerException", ToastLength.Short).Show();

            }
        }

    }

    
}
