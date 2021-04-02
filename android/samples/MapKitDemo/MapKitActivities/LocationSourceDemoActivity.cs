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
using Android.Locations;
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
    /// <summary>
    /// Location source information related activity.
    /// </summary>
    [Activity(Label = "LocationSourceDemoActivity")]
    public class LocationSourceDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "LocationSourceDemoActiv";

        private PressLocationSource pressLocationSource;

        /// <summary>
        /// Report LocationSource for new location whenever the user taps on the map.
        /// </summary>
        private class PressLocationSource : Java.Lang.Object, ILocationSource, IOnMapClickListener
        {
            private ILocationSourceOnLocationChangedListener locationChangedListener;

            private bool mPaused;

            private Context context;
            public PressLocationSource(Context appContext)
            {
                this.context = appContext;
            }


            public void Activate(ILocationSourceOnLocationChangedListener listener)
            {
                Log.Debug(TAG, "activate listener " + listener);
                locationChangedListener = listener;
            }

            public void Deactivate()
            {
                Log.Debug(TAG, "deactivate listener ");
                locationChangedListener = null;
            }

            public void OnPause()
            {
                mPaused = true;
            }

            public void OnResume()
            {
                mPaused = false;
            }

            public void OnMapClick(LatLng latLng)
            {
                if (locationChangedListener != null && !mPaused)
                {
                    locationChangedListener.OnLocationChanged(GetLocation(latLng));
                    Toast.MakeText(context, "Latitude =" + latLng.Latitude.ToString() + " Longitude=" + latLng.Longitude.ToString(),
                        ToastLength.Long).Show();
                }
            }

            private Location GetLocation(LatLng latLng)
            {
                Location location = new Location("Provider");
                location.Latitude = latLng.Latitude;
                location.Longitude = latLng.Longitude;
                location.Accuracy = 200;
                return location;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_locationsource_demo);

            pressLocationSource = new PressLocationSource(ApplicationContext);

            SupportMapFragment mapFragment =
                    (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapForLocationDemo);
            mapFragment.GetMapAsync(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            pressLocationSource.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            pressLocationSource.OnPause();
        }
        public void OnMapReady(HuaweiMap map)
        {
            map.SetOnMapClickListener(pressLocationSource);
            map.MyLocationEnabled = true;
            map.SetLocationSource(pressLocationSource);
        }
    }
}