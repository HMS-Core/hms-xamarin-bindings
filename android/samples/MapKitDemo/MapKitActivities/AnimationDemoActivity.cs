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
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Views.Animations;
using AndroidX.AppCompat.App;
using Huawei.Hms.Maps;
using Huawei.Hms.Maps.Model;
using Huawei.Hms.Maps.Model.Animation;
using static Huawei.Hms.Maps.Model.Animation.Animation;
using AlphaAnimation = Huawei.Hms.Maps.Model.Animation.AlphaAnimation;
using AnimationSet = Huawei.Hms.Maps.Model.Animation.AnimationSet;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// Animation related activity.
    /// </summary>
    [Activity(Label = "AnimationDemoActivity")]
    public class AnimationDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "AnimationDemoActivity";

        private HuaweiMap hMap;

        private MapFragment mMapFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_animation_demo);
            mMapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.mapFragment);
            mMapFragment.GetMapAsync(this);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.UiSettings.MyLocationButtonEnabled = false;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 10));

            // Add a marker on the map.
            Marker mParis;
            mParis = hMap.AddMarker(new MarkerOptions().InvokePosition(new LatLng(48.893478, 2.334595)).InvokeTitle("paris").InvokeSnippet("hello"));

            // Define the animation transparency effect.
            Huawei.Hms.Maps.Model.Animation.Animation alphaAnimation = new AlphaAnimation(0.2f, 1.0f);
            alphaAnimation.RepeatCount = 5;
            alphaAnimation.SetDuration(1000L);
            alphaAnimation.SetAnimationListener(new AnimationListener());

            AnimationSet animationSet = new AnimationSet(true);
            animationSet.SetInterpolator(new LinearInterpolator());
            animationSet.AddAnimation(alphaAnimation);

            // Set the animation effect for a marker.
            mParis.SetAnimation(animationSet);
            // Start the animation.
            mParis.StartAnimation();

        }
    }

    public class AnimationListener : Java.Lang.Object, IAnimationListener
    {
        public void OnAnimationEnd()
        {
            Log.Debug("AnimationListener", "Alpha Animation End");
        }

        public void OnAnimationStart()
        {
            Log.Debug("AnimationListener", "Alpha Animation Start");
        }
    }
}