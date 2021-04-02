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
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android;
using Android.Content;
using Android.Support.V4.App;
using Android.Content.PM;
using Android.Util;
using Huawei.Hms.Maps.Model;
using XHms_Map_Kit_Demo_Project.MapKitActivities;

namespace XHms_Map_Kit_Demo_Project
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity , View.IOnClickListener
    {
        private const string TAG = "MainActivity";

        //Required permissions
        private static readonly string[] RUNTIME_PERMISSIONS = {
        Manifest.Permission.WriteExternalStorage,
        Manifest.Permission.ReadExternalStorage, 
        Manifest.Permission.AccessCoarseLocation,
        Manifest.Permission.AccessFineLocation, 
        Manifest.Permission.Internet
        };

        //Permission request code.
        private const int REQUEST_CODE = 100;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            //Check permissions
            if (!HasPermissions(this, RUNTIME_PERMISSIONS))
            {
                ActivityCompat.RequestPermissions(this, RUNTIME_PERMISSIONS, REQUEST_CODE);
            }

            Button Camera = (Button)FindViewById(Resource.Id.Camera);
            Button BasicMap = (Button)FindViewById(Resource.Id.BasicMap);
            Button GestureDemo = (Button)FindViewById(Resource.Id.GestureDemo);
            Button CircleDemo = (Button)FindViewById(Resource.Id.CircleDemo);
            Button PolygonDemo = (Button)FindViewById(Resource.Id.PolygonDemo);
            Button PolylineDemo = (Button)FindViewById(Resource.Id.PolylineDemo);
            Button GroudOverlayDemo = (Button)FindViewById(Resource.Id.GroundOverlayDemo);
            Button TileOverlayDemo = (Button)FindViewById(Resource.Id.TileOverlayDemo);
            Button LiteModeDemo = (Button)FindViewById(Resource.Id.LiteModeDemo);
            Button MoreLanguageDemo = (Button)FindViewById(Resource.Id.MoreLanguageDemo);
            Button MapFounctions = (Button)FindViewById(Resource.Id.MapFunctions);
            Button addMarkerDemo = (Button)FindViewById(Resource.Id.AddMarkerDemo);
            Button addAnimationDemo = (Button)FindViewById(Resource.Id.AddAnimationDemo);
            Button eventsDemo = (Button)FindViewById(Resource.Id.EventsDemo);
            Button MapStyleDemo = (Button)FindViewById(Resource.Id.MapStyle);
            Button locationSourceDemo = (Button)FindViewById(Resource.Id.LocationSourceDemo);
            Button RoutePlanningDemo = (Button)FindViewById(Resource.Id.RoutePlanningDemo);
            
            Camera.SetOnClickListener(this);
            BasicMap.SetOnClickListener(this);
            GestureDemo.SetOnClickListener(this);
            CircleDemo.SetOnClickListener(this);
            PolygonDemo.SetOnClickListener(this);
            PolylineDemo.SetOnClickListener(this);
            GroudOverlayDemo.SetOnClickListener(this);
            TileOverlayDemo.SetOnClickListener(this);
            LiteModeDemo.SetOnClickListener(this);
            MoreLanguageDemo.SetOnClickListener(this);
            MapFounctions.SetOnClickListener(this);
            addMarkerDemo.SetOnClickListener(this);
            addAnimationDemo.SetOnClickListener(this);
            eventsDemo.SetOnClickListener(this);
            MapStyleDemo.SetOnClickListener(this);
            locationSourceDemo.SetOnClickListener(this);
            RoutePlanningDemo.SetOnClickListener(this);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.BasicMap:
                    Log.Info(TAG, "OnClick: BasicMap");
                    StartActivity(new Intent(this, typeof(BasicMapDemoActivity)));
                    break;
                case Resource.Id.Camera:
                    Log.Info(TAG, "OnClick: CameraDemo");
                    StartActivity(new Intent(this, typeof(CameraDemoActivity)));
                    break;
                case Resource.Id.MapFunctions:
                    Log.Info(TAG, "OnClick: MapFunctions");
                    StartActivity(new Intent(this, typeof(MapFunctionsDemoActivity)));
                    break;
                case Resource.Id.GestureDemo:
                    Log.Info(TAG, "OnClick: GestureDemo");
                    StartActivity(new Intent(this, typeof(GestureDemoActivity)));
                    break;
                case Resource.Id.LiteModeDemo:
                    Log.Info(TAG, "OnClick: LiteModeDemo");
                    StartActivity(new Intent(this, typeof(LiteModeDemoActivity)));
                    break;
                case Resource.Id.MoreLanguageDemo:
                    Log.Info(TAG, "OnClick: MoreLanguageDemo");
                    StartActivity(new Intent(this, typeof(MoreLanguageDemoActivity)));
                    break;
                case Resource.Id.CircleDemo:
                    Log.Info(TAG, "OnClick: CircleDemo");
                    StartActivity(new Intent(this, typeof(CircleDemoActivity)));
                    break;
                case Resource.Id.PolylineDemo:
                    Log.Info(TAG, "OnClick: PolylineDemo");
                    StartActivity(new Intent(this, typeof(PolylineDemoActivity)));
                    break;
                case Resource.Id.PolygonDemo:
                    Log.Info(TAG, "OnClick: PolygonDemo");
                    StartActivity(new Intent(this, typeof(PolygonDemoActivity)));
                    break;
                case Resource.Id.GroundOverlayDemo:
                    Log.Info(TAG, "OnClick: GroundOverlayDemo");
                    StartActivity(new Intent(this, typeof(GroundOverlayDemoActivity)));
                    break;
                case Resource.Id.TileOverlayDemo:
                    Log.Info(TAG, "OnClick: TileOverlayDemo");
                    StartActivity(new Intent(this, typeof(TileOverlayDemoActivity)));
                    break;
                case Resource.Id.AddMarkerDemo:
                    Log.Info(TAG, "OnClick: AddMarkerDemo");
                    StartActivity(new Intent(this, typeof(MarkerDemoActivity)));
                    break;
                case Resource.Id.AddAnimationDemo:
                    Log.Info(TAG, "OnClick: AddAnimationDemo");
                    StartActivity(new Intent(this, typeof(AnimationDemoActivity)));
                    break;
                case Resource.Id.EventsDemo:
                    Log.Info(TAG, "OnClick: EventsDemo");
                    StartActivity(new Intent(this, typeof(EventsDemoActivity)));
                    break;
                case Resource.Id.MapStyle:
                    Log.Info(TAG, "OnClick: MapStyle");
                    StartActivity(new Intent(this, typeof(StyleMapDemoActivity)));
                    break;
                case Resource.Id.LocationSourceDemo:
                    Log.Info(TAG, "OnClick: LocationSourceDemo");
                    StartActivity(new Intent(this, typeof(LocationSourceDemoActivity)));
                    break;
                case Resource.Id.RoutePlanningDemo:
                    Log.Info(TAG, "OnClick: RoutePlanningDemo");
                    StartActivity(new Intent(this, typeof(RoutePlanningDemoActivity)));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Check whether permissions are granted
        /// </summary>
        private static bool HasPermissions(Context context, string[] permissions)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M && permissions != null)
            {
                foreach (string permission in permissions)
                {
                    if (ActivityCompat.CheckSelfPermission(context, permission) != Permission.Granted)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}