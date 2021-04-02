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
using Org.Json;
using XHms_Map_Kit_Demo_Project.Utils;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// RoutePlanning related activity.
    /// </summary>
    [Activity(Label = "RoutePlanningDemoActivity")]
    public class RoutePlanningDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "RoutePlanningDemoActivity";

        private SupportMapFragment mSupportMapFragment;

        private HuaweiMap hMap;

        private Marker mMarkerOrigin;

        private Marker mMarkerDestination;

        private EditText edtOriginLat;

        private EditText edtOriginLng;

        private EditText edtDestinationLat;

        private EditText edtDestinationLng;

        private LatLng latLng1 = new LatLng(54.216608, -4.66529);

        private LatLng latLng2 = new LatLng(54.209673, -4.64002);

        private List<Polyline> mPolylines = new List<Polyline>();

        private List<List<LatLng>> mPaths = new List<List<LatLng>>();

        private LatLngBounds mLatLngBounds;

        private Handler mHandler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_route_planning_demo);
            var fragment = SupportFragmentManager.FindFragmentById(Resource.Id.mapfragment_routeplanningdemo);
            if (fragment is SupportMapFragment) {
                mSupportMapFragment = (SupportMapFragment)fragment;
                mSupportMapFragment.GetMapAsync(this);
            }
            edtOriginLat = (EditText)FindViewById(Resource.Id.edt_origin_lat);
            edtOriginLng = (EditText)FindViewById(Resource.Id.edt_origin_lng);
            edtDestinationLat = (EditText)FindViewById(Resource.Id.edt_destination_lat);
            edtDestinationLng = (EditText)FindViewById(Resource.Id.edt_destination_lng);

            mHandler = new HandlerCallback(
                    delegate (Message msg)
                    {
                        switch (msg.What)
                        {
                            case 0:
                                renderRoute(mPaths, mLatLngBounds);
                                break;
                            case 1:
                                Bundle bundle = msg.Data;
                                string errorMsg = bundle.GetString("errorMsg");
                                Toast.MakeText(this, errorMsg, ToastLength.Short).Show();
                                break;
                            default:
                                break;
                        }
                    }
                );
        }

        public void OnMapReady(HuaweiMap map)
        {
            hMap = map;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(latLng1, 13));
            AddOriginMarker(latLng1);
            AddDestinationMarker(latLng2);
        }

        [Java.Interop.Export("GetWalkingRouteResult")]
        public void GetWalkingRouteResult(View view)
        {
            RemovePolylines();
            NetworkRequestManager.GetWalkingRoutePlanningResult(latLng1, latLng2,
                new OnNetworkListenerClass(
                                delegate (string result)
                                {
                                    // Request Success
                                    GenerateRoute(result);

                                },
                                delegate (string errorMsg)
                                {
                                    // Request Fail
                                    Message msg = Message.Obtain();
                                    Bundle bundle = new Bundle();
                                    bundle.PutString("errorMsg", errorMsg);
                                    msg.What = 1;
                                    msg.Data = bundle;
                                    mHandler.SendMessage(msg);

                                }
                ));
        }

        [Java.Interop.Export("GetBicyclingRouteResult")]
        public void GetBicyclingRouteResult(View view)
        {
            RemovePolylines();
            NetworkRequestManager.GetBicyclingRoutePlanningResult(latLng1, latLng2,
                new OnNetworkListenerClass(
                                delegate (string result)
                                {
                                    // Request Success
                                    GenerateRoute(result);

                                },
                                delegate (string errorMsg)
                                {
                                    // Request Fail
                                    Message msg = Message.Obtain();
                                    Bundle bundle = new Bundle();
                                    bundle.PutString("errorMsg", errorMsg);
                                    msg.What = 1;
                                    msg.Data = bundle;
                                    mHandler.SendMessage(msg);
                                }
                ));
        }

        [Java.Interop.Export("GetDrivingRouteResult")]
        public void GetDrivingRouteResult(View view)
        {
            RemovePolylines();
            NetworkRequestManager.GetDrivingRoutePlanningResult(latLng1, latLng2, 
                new OnNetworkListenerClass(
                                delegate (string result)
                                {
                                    // Request Success
                                    GenerateRoute(result);

                                },   
                                delegate (string errorMsg)
                                {
                                    // Request Fail
                                    Message msg = Message.Obtain();
                                    Bundle bundle = new Bundle();
                                    bundle.PutString("errorMsg", errorMsg);
                                    msg.What = 1;
                                    msg.Data = bundle;
                                    mHandler.SendMessage(msg);
                                }
                ));
        }

        private void GenerateRoute(string json)
        {
            try
            {
                JSONObject jsonObject = new JSONObject(json);
                JSONArray routes = jsonObject.OptJSONArray("routes");
                if (null == routes || routes.Length() == 0)
                {
                    return;
                }
                JSONObject route = routes.GetJSONObject(0);

                // get route bounds
                JSONObject bounds = route.OptJSONObject("bounds");
                if (null != bounds && bounds.Has("southwest") && bounds.Has("northeast"))
                {
                    JSONObject southwest = bounds.OptJSONObject("southwest");
                    JSONObject northeast = bounds.OptJSONObject("northeast");
                    LatLng sw = new LatLng(southwest.OptDouble("lat"), southwest.OptDouble("lng"));
                    LatLng ne = new LatLng(northeast.OptDouble("lat"), northeast.OptDouble("lng"));
                    mLatLngBounds = new LatLngBounds(sw, ne);
                }

                // get paths
                JSONArray paths = route.OptJSONArray("paths");
                for (int i = 0; i < paths.Length(); i++)
                {
                    JSONObject path = paths.OptJSONObject(i);
                    List<LatLng> mPath = new List<LatLng>();

                    JSONArray steps = path.OptJSONArray("steps");
                    for (int j = 0; j < steps.Length(); j++)
                    {
                        JSONObject step = steps.OptJSONObject(j);

                        JSONArray polyline = step.OptJSONArray("polyline");
                        for (int k = 0; k < polyline.Length(); k++)
                        {
                            if (j > 0 && k == 0)
                            {
                                continue;
                            }
                            JSONObject line = polyline.GetJSONObject(k);
                            double lat = line.OptDouble("lat");
                            double lng = line.OptDouble("lng");
                            LatLng latLng = new LatLng(lat, lng);
                            mPath.Add(latLng);
                        }
                    }
                    mPaths.Add(mPath);
                }
                mHandler.SendEmptyMessage(0);

            }
            catch (JSONException e)
            {
                Log.Error(TAG, "JSONException" + e.ToString());
            }
        }

        /// <summary>
        /// Render the route planning result
        /// </summary>
        /// <param name="paths">paths</param>
        /// <param name="latLngBounds">latLngBounds</param>
        private void renderRoute(List<List<LatLng>> paths, LatLngBounds latLngBounds)
        {
            if (paths == null  || paths.Count <= 0 || paths.ElementAt(0).Count <= 0)
            {
                return;
            }

            for (int i = 0; i < paths.Count; i++)
            {
                List<LatLng> path = paths.ElementAt(i);
                PolylineOptions options = new PolylineOptions().InvokeColor(Color.Blue).InvokeWidth(5);
                foreach (LatLng latLng in path)
                {
                    options.Add(latLng);
                }

                Polyline polyline = hMap.AddPolyline(options);
                mPolylines.Add(polyline);
            }

            AddOriginMarker(paths.ElementAt(0).ElementAt(0));
            AddDestinationMarker(paths.ElementAt(0).ElementAt(paths.ElementAt(0).Count - 1));

            if (null != latLngBounds)
            {
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngBounds(latLngBounds, 5);
                hMap.MoveCamera(cameraUpdate);
            }
            else
            {
                hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(paths.ElementAt(0).ElementAt(0), 13));
            }

        }

        [Java.Interop.Export("SetOrigin")]
        public void SetOrigin(View view)
        {
            if (!String.IsNullOrEmpty(edtOriginLat.Text) && !String.IsNullOrEmpty(edtOriginLng.Text))
            {
                latLng1 = new LatLng(Double.Parse(edtOriginLat.Text.ToString().Trim()),
                    Double.Parse(edtOriginLng.Text.ToString().Trim()));

                RemovePolylines();
                AddOriginMarker(latLng1);
                hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(latLng1, 13));
                mMarkerOrigin.ShowInfoWindow();
            }
        }

        [Java.Interop.Export("SetDestination")]
        public void SetDestination(View view)
        {
            if (!String.IsNullOrEmpty(edtDestinationLat.Text) && !String.IsNullOrEmpty(edtDestinationLng.Text))
            {
                latLng2 = new LatLng(Double.Parse(edtDestinationLat.Text.ToString().Trim()),
                    Double.Parse(edtDestinationLng.Text.ToString().Trim()));

                RemovePolylines();
                AddDestinationMarker(latLng2);
                hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(latLng2, 13));
                mMarkerDestination.ShowInfoWindow();
            }
        }

        private void AddOriginMarker(LatLng latLng)
        {
            if (null != mMarkerOrigin)
            {
                mMarkerOrigin.Remove();
            }
            mMarkerOrigin = hMap.AddMarker(new MarkerOptions().InvokePosition(latLng)
                .Anchor(0.5f, 0.9f)
                // .anchorMarker(0.5f, 0.9f)
                .InvokeTitle("Origin")
                .InvokeSnippet(latLng.ToString()));
        }

        private void AddDestinationMarker(LatLng latLng)
        {
            if (null != mMarkerDestination)
            {
                mMarkerDestination.Remove();
            }
            mMarkerDestination = hMap.AddMarker(
                new MarkerOptions()
                .InvokePosition(latLng)
                .Anchor(0.5f, 0.9f)
                .InvokeTitle("Destination")
                .InvokeSnippet(latLng.ToString()));
        }

        private void RemovePolylines()
        {
            foreach (Polyline polyline in mPolylines)
            {
                polyline.Remove();
            }

            mPolylines.Clear();
            mPaths.Clear();
            mLatLngBounds = null;
        }

        private class OnNetworkListenerClass : NetworkRequestManager.IOnNetworkListener
        {
            private Action<string> RequestSuccessAction;

            private Action<string> RequestFailAction;

            public OnNetworkListenerClass(Action<string> RequestSuccessAction, Action<string> RequestFailAction)
            {
                this.RequestSuccessAction = RequestSuccessAction;
                this.RequestFailAction = RequestFailAction;
            }

            public void RequestFail(string errorMsg)
            {
                this.RequestFailAction(errorMsg);
            }

            public void RequestSuccess(string result)
            {
                this.RequestSuccessAction(result);
            }
        }

        private class HandlerCallback : Handler
        {
            private Action<Message> HandleMessageAction;

            public HandlerCallback(Action<Message> HandleMessageAction)
            {
                this.HandleMessageAction = HandleMessageAction;
            }

            public override void HandleMessage(Message msg)
            {
                this.HandleMessageAction(msg);
            }
        }
    }
}