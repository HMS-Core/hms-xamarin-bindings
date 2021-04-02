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
using Android.Text.Style;
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
    /// Marker functions related activity.
    /// </summary>
    [Activity(Label = "MarkerDemoActivity")]
    public class MarkerDemoActivity : AppCompatActivity, IOnMapReadyCallback, IOnMarkerClickListener, IOnMapLongClickListener
        , IOnMarkerDragListener, IOnInfoWindowClickListener, IOnInfoWindowCloseListener, IOnInfoWindowLongClickListener
    {
        private const string TAG = "MarkerDemoActivity";

        private static readonly LatLng PARIS = new LatLng(48.893478, 2.334595);
        private static readonly LatLng SERRIS = new LatLng(48.7, 2.31);
        private static readonly LatLng ORSAY = new LatLng(48.85, 2.78);

        private SupportMapFragment mSupportMapFragment;

        private HuaweiMap hMap;

        private Marker mOrsay;

        private Marker mParis;

        private Marker mSerris;

        public int mWindowType;

        private EditText edtTitle;

        private EditText edtSnippet;

        private EditText edtTag;

        private TextView txtvResultShown;

        private EditText edtCameraLat;

        private EditText edtCameraLng;

        private EditText edtCameraZoom;

        private EditText edtCameraTilt;

        private EditText edtCameraBearing;

        IList<Marker> markerList = new List<Marker>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_marker_demo);
            var fragment = SupportFragmentManager.FindFragmentById(Resource.Id.mapfragment_markerdemo);
            if (fragment is SupportMapFragment) {
                mSupportMapFragment = (SupportMapFragment)fragment;
                mSupportMapFragment.GetMapAsync(this);
            }
            edtTitle = (EditText)FindViewById(Resource.Id.edt_title);
            edtSnippet = (EditText)FindViewById(Resource.Id.edt_snippet);
            edtTag = (EditText)FindViewById(Resource.Id.edt_tag);
            txtvResultShown = (TextView)FindViewById(Resource.Id.markerdemo_result_shown);

            edtCameraLat = (EditText)FindViewById(Resource.Id.edt_camera_lat);
            edtCameraLng = (EditText)FindViewById(Resource.Id.edt_camera_lng);
            edtCameraZoom = (EditText)FindViewById(Resource.Id.edt_camera_zoom);
            edtCameraTilt = (EditText)FindViewById(Resource.Id.edt_camera_tilt);
            edtCameraBearing = (EditText)FindViewById(Resource.Id.edt_camera_bearing);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.MyLocationEnabled = true;
            hMap.SetInfoWindowAdapter(new CustomInfoWindowAdapter(this));
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 14));
        }

        public bool OnMarkerClick(Marker marker)
        {
            bool clusterable = marker.IsClusterable;
            Toast.MakeText(ApplicationContext, clusterable.ToString(), ToastLength.Short).Show();
            return true;
        }

        /// <summary>
        /// Add a marker to the map.
        /// </summary>
        [Java.Interop.Export("AddMarker")]
        public void AddMarker(View view)
        {
            if (null == hMap)
            {
                return;
            }
            if (mParis == null && mOrsay == null && mSerris == null)
            {
                // Uses a colored icon.
                mParis =
                    hMap.AddMarker(new MarkerOptions().InvokePosition(PARIS).InvokeTitle("paris").InvokeSnippet("hello").Clusterable(true));
                mOrsay = hMap.AddMarker(new MarkerOptions().InvokePosition(ORSAY)
                    .InvokeAlpha(0.5f)
                    .InvokeTitle("Orsay")
                    .InvokeSnippet("hello")
                    .Clusterable(true)
                    .InvokeIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.badge_ph)));
                mSerris = hMap.AddMarker(new MarkerOptions().InvokePosition(SERRIS)
                    .InvokeTitle("Serris")
                    .InvokeSnippet("Can be dragged after DragMarker.")
                    .Clusterable(true));
                hMap.SetOnMarkerClickListener(this);
            }
            hMap.SetOnMapLongClickListener(this);
            AddMarkerListener();
        }

        /// <summary>
        /// Add a marker when the point is long clicked.
        /// </summary>
        public void OnMapLongClick(LatLng latLng)
        {
            Log.Debug(TAG, "Map is long clicked.");
            Marker mMarker = hMap.AddMarker(new MarkerOptions().InvokePosition(latLng).InvokeTitle("I am Marker!"));
            markerList.Add(mMarker);
            Log.Info(TAG, "markerList size is." + markerList.Count);
        }

        //Implemented from HuaweiMap.IOnMarkerDragListener
        public void OnMarkerDrag(Marker marker)
        {
            Log.Debug(TAG, "OnMarkerDrag: ");
        }

        //Implemented from HuaweiMap.IOnMarkerDragListener
        public void OnMarkerDragEnd(Marker marker)
        {
            Log.Debug(TAG, "OnMarkerDragEnd: ");
        }

        //Implemented from HuaweiMap.IOnMarkerDragListener
        public void OnMarkerDragStart(Marker marker)
        {
            Log.Debug(TAG, "OnMarkerDragStart: ");
        }

        public void OnInfoWindowClick(Marker marker)
        {
            if (marker.Equals(mSerris))
            {
                Toast.MakeText(ApplicationContext, "mMelbourne infowindow is clicked", ToastLength.Short).Show();
            }

            if (marker.Equals(mOrsay))
            {
                Toast.MakeText(ApplicationContext, "mSydney infowindow is clicked", ToastLength.Short).Show();
            }

            if (marker.Equals(mParis))
            {
                Toast.MakeText(ApplicationContext, "mBrisbane infowindow is clicked", ToastLength.Short).Show();
            }
        }

        public void OnInfoWindowClose(Marker marker)
        {
            Toast.MakeText(ApplicationContext, "InfoWindowClose", ToastLength.Short).Show();
        }

        public void OnInfoWindowLongClick(Marker marker)
        {
            Toast.MakeText(ApplicationContext, "InfoWindowLongClick", ToastLength.Short).Show();
        }

        /// <summary>
        /// Set the listener associated with the marker
        /// </summary>
        private void AddMarkerListener()
        {
            hMap.SetOnMarkerDragListener(this);
            hMap.SetOnInfoWindowClickListener(this);
            hMap.SetOnInfoWindowCloseListener(this);
            hMap.SetOnInfoWindowLongClickListener(this);
        }

        /// <summary>
        /// Remove the marker from the map
        /// </summary>
        /// <param name="view"></param>
        [Java.Interop.Export("DeleteMarker")]
        public void DeleteMarker(View view)
        {
            if (null != mSerris)
            {
                mSerris.Remove();
                mSerris = null;
            }

            if (null != mOrsay)
            {
                mOrsay.Remove();
                mOrsay = null;
            }

            if (null != mParis)
            {
                mParis.Remove();
                mParis = null;
            }

            // remove the markers added by long click.
            if (null != markerList && markerList.Count > 0)
            {
                foreach (Marker iMarker in markerList.ToList())
                {
                    iMarker.Remove();
                    markerList.Remove(iMarker);
                }
                markerList.Clear();
            }
        }

        /// <summary>
        /// Set the tag attribute of the marker.
        /// </summary>
        [Java.Interop.Export("SetTag")]
        public void SetTag(View view)
        {
            string tagStr = edtTag.Text.ToString();
            if (mParis != null && tagStr != null && !"".Equals(tagStr))
            {
                mParis.Tag = tagStr;
            }
        }

        /// <summary>
        /// Set the snippet attribute of the marker.
        /// </summary>
        [Java.Interop.Export("SetSnippet")]
        public void SetSnippet(View view)
        {
            string snippetStr = edtSnippet.Text.ToString();
            if (mOrsay != null && snippetStr != null && !"".Equals(snippetStr))
            {
                mOrsay.Snippet = snippetStr;
            }
        }

        /// <summary>
        /// Set the icon attribute of the marker.
        /// </summary>
        [Java.Interop.Export("SetMarkerIcon")]
        public void SetMarkerIcon(View view)
        {
            if (null != mOrsay)
            {
                Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.badge_tr);
                BitmapDescriptor bitmapDescriptor = BitmapDescriptorFactory.FromBitmap(bitmap);

                mOrsay.SetIcon(bitmapDescriptor);
            }
        }

        /// <summary>
        /// Set the anchor attribute of the marker.
        /// </summary>
        [Java.Interop.Export("SetMarkerAnchor")]
        public void SetMarkerAnchor(View view)
        {
            if (mParis != null)
            {
                mParis.SetMarkerAnchor(0.9F, 0.9F);
            }
        }

        /// <summary>
        /// Get the latitude and longitude of the marker.
        /// </summary>
        [Java.Interop.Export("GetPosition")]
        public void GetPosition(View view)
        {
            if (mParis != null)
            {
                LatLng latLng = mParis.Position;
                Double latitude = latLng.Latitude;
                Double longitude = latLng.Longitude;
                txtvResultShown.Text = "mBrisbane " + latitude.ToString() + " " + longitude.ToString();
            }
        }

        /// <summary>
        /// Hide the information window of the marker.
        /// </summary>
        [Java.Interop.Export("HideInfoWindow")]
        public void HideInfoWindow(View view)
        {
            if (null != mParis)
            {
                mParis.HideInfoWindow();
            }
        }

        /// <summary>
        /// Show the information window of the marker.
        /// </summary>
        [Java.Interop.Export("ShowInfoWindow")]
        public void ShowInfoWindow(View view)
        {
            if (null != mParis)
            {
                mParis.ShowInfoWindow();
            }
        }

        /// <summary>
        /// Repositions the camera according to the instructions defined in the update.
        /// </summary>
        [Java.Interop.Export("SetCamera")]
        public void SetCamera(View view)
        {
            try
            {
                LatLng latLng = null;
                float zoom = 0f;
                float bearing = 0f;
                float tilt = 0f;
                if (!TextUtils.IsEmpty(edtCameraLng.Text) && !TextUtils.IsEmpty(edtCameraLat.Text))
                {
                    latLng = new LatLng(Single.Parse(edtCameraLat.Text.ToString().Trim()),
                        Single.Parse(edtCameraLng.Text.ToString().Trim()));
                }
                if (!TextUtils.IsEmpty(edtCameraZoom.Text))
                {
                    zoom = Single.Parse(edtCameraZoom.Text.ToString().Trim());
                }
                if (!TextUtils.IsEmpty(edtCameraBearing.Text))
                {
                    bearing = Single.Parse(edtCameraBearing.Text.ToString().Trim());
                }
                if (!TextUtils.IsEmpty(edtCameraTilt.Text))
                {
                    tilt = Single.Parse(edtCameraTilt.Text.ToString().Trim());
                }
                CameraPosition cameraPosition = new
                    CameraPosition.Builder().Target(latLng).Zoom(zoom).Bearing(bearing).Tilt(tilt).Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                hMap.MoveCamera(cameraUpdate);
            }
            catch (Java.Lang.IllegalArgumentException e)
            {
                Log.Error(TAG, "IllegalArgumentException " + e.ToString());
                Toast.MakeText(this, "IllegalArgumentException", ToastLength.Short).Show();
            }
            catch (Java.Lang.NullPointerException e)
            {
                Log.Error(TAG, "NullPointerException " + e.ToString());
                Toast.MakeText(this, "NullPointerException", ToastLength.Short).Show();
            }
        }

        /// <summary>
        /// Set the title attribute of the marker.
        /// </summary>
        [Java.Interop.Export("SetTitle")]
        public void SetTitle(View view)
        {
            string titleStr = edtTitle.Text.ToString();
            if (mParis != null && titleStr != null && !"".Equals(titleStr))
            {
                mParis.Title = titleStr;
                Toast.MakeText(this, "BJ title is " + mParis.Title, ToastLength.Short).Show();
            }
        }

        [Java.Interop.Export("DefaultWindow")]
        public void DefaultWindow(View view)
        {
            mWindowType = 1;
        }

        [Java.Interop.Export("ContentWindow")]
        public void ContentWindow(View view)
        {
            mWindowType = 2;
        }

        [Java.Interop.Export("CustomWindow")]
        public void CustomWindow(View view)
        {
            mWindowType = 3;
        }

        /// <summary>
        /// Set the marker to drag.
        /// </summary>
        [Java.Interop.Export("DragMarker")]
        public void DragMarker(View view)
        {
            if (null == mSerris)
            {
                return;
            }
            mSerris.Draggable = true;
        }

        private class CustomInfoWindowAdapter : Java.Lang.Object, HuaweiMap.IInfoWindowAdapter
        {
            private readonly View mWindowView;

            private readonly View mContentsView;

            private MarkerDemoActivity activity;

            public CustomInfoWindowAdapter(MarkerDemoActivity activity)
            {
                mWindowView = activity.LayoutInflater.Inflate(Resource.Layout.custom_info_window, null);
                mContentsView = activity.LayoutInflater.Inflate(Resource.Layout.custom_info_contents, null);
                this.activity = activity;
            }

            public View GetInfoContents(Marker marker)
            {
                View view = null;
                Log.Debug(TAG, "GetInfoContents");
                if (activity.mWindowType != 2)
                {
                    return view;
                }
                Render(marker, mContentsView);
                return mContentsView;
            }

            public View GetInfoWindow(Marker marker)
            {
                View view = null;
                Log.Debug(TAG, "GetInfoWindow");
                if (activity.mWindowType != 3)
                {
                    return view;
                }
                Render(marker, mWindowView);
                return mWindowView;
            }

            private void Render(Marker marker, View view)
            {
                SetMarkerBadge(marker, view);

                SetMarkerTextView(marker, view);

                SetMarkerSnippet(marker, view);
            }

            private void SetMarkerBadge(Marker marker, View view)
            {
                int markerBadge;
                // Use the equals method to determine if the marker is the same ,do not use"=="
                if (marker.Equals(activity.mParis))
                {
                    markerBadge = Resource.Drawable.badge_bj;
                }
                else if (marker.Equals(activity.mOrsay))
                {
                    markerBadge = Resource.Drawable.badge_sh;
                }
                else if (marker.Equals(activity.mSerris))
                {
                    markerBadge = Resource.Drawable.badge_nj;
                }
                else
                {
                    markerBadge = 0;
                }
                ((ImageView)view.FindViewById(Resource.Id.imgv_badge)).SetImageResource(markerBadge);
            }

            private void SetMarkerTextView(Marker marker, View view)
            {
                string markerTitle = marker.Title;

                TextView titleView = null;

                var obj = view.FindViewById(Resource.Id.txtv_titlee);
                if (obj is TextView) {
                    titleView = (TextView)obj;
                }
                if (markerTitle == null)
                {
                    titleView.Text = "";
                }
                else
                {
                    SpannableString titleText = new SpannableString(markerTitle);
                    titleText.SetSpan(new ForegroundColorSpan(Color.Blue), 0, titleText.Length(), 0);
                    titleView.Text = titleText.ToString();
                }
            }

            private void SetMarkerSnippet(Marker marker, View view)
            {
                string markerSnippet = marker.Snippet;
                if (marker.Tag != null)
                {
                    markerSnippet = (string)marker.Tag;
                }
                TextView snippetView = ((TextView)view.FindViewById(Resource.Id.txtv_snippett));
                if (!String.IsNullOrEmpty(markerSnippet))
                {
                    SpannableString snippetText = new SpannableString(markerSnippet);
                    snippetText.SetSpan(new ForegroundColorSpan(Color.Red), 0, markerSnippet.Length, 0);
                    snippetView.Text = snippetText.ToString();
                }
                else
                {
                    snippetView.Text = ("");
                }
            }
        }
    }
}