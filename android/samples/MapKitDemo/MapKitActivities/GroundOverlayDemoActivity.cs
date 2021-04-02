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
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content.Resources;
using Huawei.Hms.Maps;
using Huawei.Hms.Maps.Model;
using XHms_Map_Kit_Demo_Project.Utils;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// Ground overlay related activity.
    /// </summary>
    [Activity(Label = "GroundOverlayDemoActivity")]
    public class GroundOverlayDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "GroundOverlayDemoActivity";

        private SupportMapFragment mSupportMapFragment;

        private HuaweiMap hMap;

        private GroundOverlay overlay;

        private EditText toprightLatitude;

        private EditText toprightLongtitude;

        private EditText bottomleftLatitude;

        private EditText bottomleftLongtitude;

        private EditText positionLatitude;

        private EditText positionLongtitude;

        private EditText imageWidth;

        private EditText imageHeight;

        private EditText groundOverlayTag;

        private TextView groundOverlayShown;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_groundoverlay_demo);
            mSupportMapFragment =
                (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapInGroundOverlay);
            mSupportMapFragment.GetMapAsync(this);

            toprightLatitude = (EditText)FindViewById(Resource.Id.toprightLatitude);
            toprightLongtitude = (EditText)FindViewById(Resource.Id.toprightLongtitude);
            bottomleftLatitude = (EditText)FindViewById(Resource.Id.bottomleftLatitude);
            bottomleftLongtitude = (EditText)FindViewById(Resource.Id.bottomleftLongtitude);
            positionLatitude = (EditText)FindViewById(Resource.Id.positionLatitude);
            positionLongtitude = (EditText)FindViewById(Resource.Id.positionLongtitude);
            imageWidth = (EditText)FindViewById(Resource.Id.imageWidth);
            imageHeight = (EditText)FindViewById(Resource.Id.imageHeight);
            groundOverlayTag = (EditText)FindViewById(Resource.Id.groundOverlayTag);
            groundOverlayShown = (TextView)FindViewById(Resource.Id.groundOverlayShown);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Info(TAG, "OnMapReady: ");
            hMap = map;
            hMap.MyLocationEnabled = true;
            hMap.UiSettings.ZoomControlsEnabled =false;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 10));
        }

        /// <summary>
        /// Create a GroundOverlay using the images in the assets directory.
        /// </summary>
        [Java.Interop.Export("AddFromAsset")]
        public void AddFromAsset(View view)
        {
            if (hMap == null)
            {
                return;
            }
            if (null != overlay)
            {
                overlay.Remove();
            }
            Log.Debug(TAG, "addFromAsset: ");
            GroundOverlayOptions options = new GroundOverlayOptions().Position(MapUtils.FRANCE2, 50, 50)
                .InvokeImage(BitmapDescriptorFactory.FromAsset("HuaweiIcon.png"));
            overlay = hMap.AddGroundOverlay(options);
            CameraPosition cameraPosition = new
                CameraPosition.Builder().Target(MapUtils.FRANCE2).Zoom(18).Bearing(0f).Tilt(0f).Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            hMap.MoveCamera(cameraUpdate);
        }

        /// <summary>
        /// Create a GroundOverlay using the resources of the bitmap image.
        /// </summary>
        [Java.Interop.Export("AddFromResource")]
        public void AddFromResource(View view)
        {
            if (hMap == null)
            {
                return;
            }
            if (null != overlay)
            {
                overlay.Remove();
            }
            Log.Debug(TAG, "AddFromResource: ");
            GroundOverlayOptions options = new GroundOverlayOptions().Position(MapUtils.FRANCE2, 50, 50)
                .InvokeImage(BitmapDescriptorFactory.FromResource(Resource.Drawable.niuyouguo));
            overlay = hMap.AddGroundOverlay(options);
            CameraPosition cameraPosition = new
                CameraPosition.Builder().Target(MapUtils.FRANCE2).Zoom(18).Bearing(0f).Tilt(0f).Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            hMap.MoveCamera(cameraUpdate);
        }

        /// <summary>
        /// Create GroundOverlay.
        /// </summary>
        [Java.Interop.Export("AddFromBitmap")]
        public void AddFromBitmap(View view)
        {
            if (hMap == null)
            {
                return;
            }
            if (null != overlay)
            {
                overlay.Remove();
            }
            Log.Debug(TAG, "AddFromBitmap: ");
            Drawable vectorDrawable = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.niuyouguo, null);
            Bitmap bitmap = Bitmap.CreateBitmap(vectorDrawable.IntrinsicWidth, vectorDrawable.IntrinsicHeight,
                Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            vectorDrawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            vectorDrawable.Draw(canvas);
            GroundOverlayOptions options = new GroundOverlayOptions().Position(MapUtils.FRANCE2, 50, 50)
                .InvokeImage(BitmapDescriptorFactory.FromBitmap(bitmap));
            overlay = hMap.AddGroundOverlay(options);
            CameraPosition cameraPosition = new
                CameraPosition.Builder().Target(MapUtils.FRANCE2).Zoom(18).Bearing(0f).Tilt(0f).Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            hMap.MoveCamera(cameraUpdate);
        }

        /// <summary>
        /// Remove the Groundoverlay.
        /// </summary>
        [Java.Interop.Export("RemoveGroundOverlay")]
        public void RemoveGroundOverlay(View view)
        {
            Log.Debug(TAG, "RemoveGroundoverlay: ");
            if (null != overlay)
            {
                overlay.Remove();
            }
        }

        /// <summary>
        /// Get the properties of the GroundOverlay.
        /// </summary>
        [Java.Interop.Export("GetAttributes")]
        public void GetAttributes(View view)
        {
            if (null != overlay)
            {
                String bounds = null;
                String position = null;
                if (overlay.Bounds == null)
                {
                    bounds = "null";
                }
                else
                {
                    bounds = overlay.Bounds.ToString();
                }
                if (overlay.Position == null)
                {
                    position = "null";
                }
                else
                {
                    position = overlay.Position.ToString();
                }

                Toast
                    .MakeText(this,
                        "position:" + position + "width:" + overlay.Width + "height:" + overlay.Height + "bounds:"
                            + bounds,
                        ToastLength.Long)
                    .Show();
            }
        }

        /// <summary>
        /// Set the scope of GroundOverlay.
        /// </summary>
        [Java.Interop.Export("SetPointsBy2Points")]
        public void SetPointsBy2Points(View view)
        {
            if (null != overlay)
            {
                string northeastLatitude = toprightLatitude.Text.ToString().Trim();
                string northeastLongtitude = toprightLongtitude.Text.ToString().Trim();
                string southwestLatitude = bottomleftLatitude.Text.ToString().Trim();
                string southwestLontitude = bottomleftLongtitude.Text.ToString().Trim();
                if (CheckUtils.CheckIsEdit(northeastLatitude) || CheckUtils.CheckIsEdit(northeastLongtitude) || CheckUtils.CheckIsEdit(southwestLatitude)
                    || CheckUtils.CheckIsEdit(southwestLontitude))
                {
                    Toast.MakeText(this, "Please make sure these latlng are Edited", ToastLength.Short).Show();
                }
                else
                {
                    if (!CheckUtils.CheckIsRight(northeastLatitude) || !CheckUtils.CheckIsRight(northeastLongtitude)
                        || !CheckUtils.CheckIsRight(southwestLatitude) || !CheckUtils.CheckIsRight(southwestLontitude))
                    {
                        Toast.MakeText(this, "Please make sure these latlng are right", ToastLength.Short).Show();
                    }
                    else
                    {
                        try
                        {
                            overlay.SetPositionFromBounds(new LatLngBounds(
                                new LatLng(Double.Parse(southwestLatitude), Double.Parse(southwestLontitude)),
                                new LatLng(Double.Parse(northeastLatitude), Double.Parse(northeastLongtitude))));
                            CameraPosition cameraPosition = new CameraPosition.Builder()
                                .Target(overlay.Position)
                                .Zoom(18)
                                .Bearing(0f)
                                .Tilt(0f)
                                .Build();
                            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                            hMap.MoveCamera(cameraUpdate);
                        }
                        catch (Java.Lang.IllegalArgumentException e)
                        {
                            Toast.MakeText(this, "IllegalArgumentException ", ToastLength.Short).Show();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the scope of GroundOverlay.
        /// </summary>
        [Java.Interop.Export("GetPointsBy2Points")]
        public void GetPointsBy2Points(View view)
        {
            if (null != overlay)
            {
                if (overlay.Bounds == null)
                {
                    Toast.MakeText(this, "the groundoverlay is added by the other function", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "LatlngBounds :" + overlay.Bounds.ToString(), ToastLength.Short).Show();
                }
            }
        }

        /// <summary>
        /// Set the height and width of the GroundOverlay.
        /// </summary>
        [Java.Interop.Export("SetPointsBy1PointsWidthHeight")]
        public void SetPointsBy1PointsWidthHeight(View view)
        {
            if (null != overlay)
            {
                String width = imageWidth.Text.ToString().Trim();
                String height = imageHeight.Text.ToString().Trim();
                String latitude = positionLatitude.Text.ToString().Trim();
                String longtitude = positionLongtitude.Text.ToString().Trim();
                if (CheckUtils.CheckIsEdit(width) || CheckUtils.CheckIsEdit(height) || CheckUtils.CheckIsEdit(latitude) 
                    || CheckUtils.CheckIsEdit(longtitude))
                {
                    Toast.MakeText(this, "Please make sure the width & height & position is Edited", ToastLength.Short).Show();
                }
                else
                {
                    if (!CheckUtils.CheckIsRight(width) || !CheckUtils.CheckIsRight(height) || !CheckUtils.CheckIsRight(latitude)
                        || !CheckUtils.CheckIsRight(longtitude))
                    {
                        Toast.MakeText(this, "Please make sure the width & height & position is right", ToastLength.Short).Show();
                    }
                    else
                    {
                        try
                        {
                            if (Single.Parse(width) < 0.0F || Single.Parse(height) < 0.0F)
                            {
                                Toast
                                    .MakeText(this, "Please make sure the width & height is right, this value must be non-negative",
                                        ToastLength.Short).Show();
                                return;
                            }
                            LatLng position = new LatLng(Double.Parse(latitude), Double.Parse(longtitude));
                            overlay.Position = position;
                            overlay.SetDimensions(Single.Parse(width), Single.Parse(height));
                            CameraPosition cameraPosition = new
                                CameraPosition.Builder().Target(position).Zoom(18).Bearing(0f).Tilt(0f).Build();
                            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                            hMap.MoveCamera(cameraUpdate);
                        }
                        catch (Java.Lang.IllegalArgumentException e)
                        {
                            Toast.MakeText(this, "IllegalArgumentException:" + e.Message, ToastLength.Short).Show();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set the height, width, and position of the GroundOverlay.
        /// </summary>
        [Java.Interop.Export("GetPointsBy1PointsWidthHeight")]
        public void GetPointsBy1PointsWidthHeight(View view)
        {
            if (null != overlay)
            {
                if (overlay.Position == null || overlay.Height == 0 || overlay.Width == 0)
                {
                    Toast.MakeText(this, "the groundoverlay is added by the other function", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this,
                            "Position :" + overlay.Position.ToString() + "With :" + overlay.Width + "Height :"
                                + overlay.Height,
                            ToastLength.Short).Show();
                }
            }
        }

        /// <summary>
        /// Change the image of GroundOverlay.
        /// </summary>
        [Java.Interop.Export("SetImage")]
        public void SetImage(View view)
        {
            if (null != overlay)
            {
                overlay.SetImage(BitmapDescriptorFactory.FromResource(Resource.Drawable.makalong));
            }
        }

        /// <summary>
        /// Get the tag of GroundOverlay.
        /// </summary>
        [Java.Interop.Export("GetTag")]
        public void GetTag(View v)
        {
            if (null != overlay)
            {
                groundOverlayShown.Text = "Overlay tag is " + overlay.Tag;
            }
        }

        /// <summary>
        /// Set the tag of GroundOverlay.
        /// </summary>
        [Java.Interop.Export("SetTag")]
        public void SetTag(View v)
        {
            if (null != overlay)
            {
                string tag = groundOverlayTag.Text.ToString().Trim();
                if (CheckUtils.CheckIsEdit(tag))
                {
                    Toast.MakeText(this, "Please make sure the tag is Edited", ToastLength.Short).Show();
                }
                else
                {
                    overlay.Tag = tag;
                }
            }
        }

        /// <summary>
        /// Set GroundOverlay visible.
        /// </summary>
        [Java.Interop.Export("SetVisibleTrue")]
        public void SetVisibleTrue(View view)
        {
            if (null != overlay)
            {
                overlay.Visible = true;
            }
        }

        /// <summary>
        /// Setting GroundOverlay is not visible.
        /// </summary>
        [Java.Interop.Export("SetVisibleFalse")]
        public void SetVisibleFalse(View view)
        {
            if (null != overlay)
            {
                overlay.Visible = false;
            }
        }
    }
}