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
using Huawei.Hms.Maps.Model;
using Java.IO;
using Java.Net;
using Java.Util.Concurrent;
using Org.Json;
using Square.OkHttp3;

namespace XHms_Map_Kit_Demo_Project.Utils
{
    public class NetClient
    {
        private const string TAG = "NetClient";

        private static NetClient netClient;

        private static OkHttpClient client;

        // Please place your API KEY here. If the API KEY contains special characters, you need to encode it using
        // EncodeURI.
        private string mDefaultKey = Constants.API_KEY;

        private string mWalkingRoutePlanningURL = "https://mapapi.cloud.huawei.com/mapApi/v1/routeService/walking";

        private string mBicyclingRoutePlanningURL = "https://mapapi.cloud.huawei.com/mapApi/v1/routeService/bicycling";

        private string mDrivingRoutePlanningURL = "https://mapapi.cloud.huawei.com/mapApi/v1/routeService/driving";

        private static MediaType JSON = MediaType.Parse("application/json; charset=utf-8");

        private NetClient()
        {
            client = InitOkHttpClient();
        }

        public OkHttpClient InitOkHttpClient()
        {
            if (client == null)
            {
                client = new OkHttpClient.Builder().ReadTimeout(10000, TimeUnit.Milliseconds)// Set the read timeout.
                    .ConnectTimeout(10000, TimeUnit.Milliseconds)// Set the connect timeout.
                    .Build();
            }
            return client;
        }

        public static NetClient GetNetClient()
        {
            if (netClient == null)
            {
                netClient = new NetClient();
            }
            return netClient;
        }

        public Response GetWalkingRoutePlanningResult(LatLng latLng1, LatLng latLng2, bool needEncode)
        {
            string key = mDefaultKey;
            if (needEncode)
            {
                try
                {
                    key = Java.Net.URLEncoder.Encode(mDefaultKey, "UTF-8");
                }
                catch (Java.IO.UnsupportedEncodingException e)
                {
                    e.PrintStackTrace();
                }
            }
            string url = mWalkingRoutePlanningURL + "?key=" + key;

            Response response = null;
            JSONObject origin = new JSONObject();
            JSONObject destination = new JSONObject();
            JSONObject json = new JSONObject();
            try
            {
                origin.Put("lat", latLng1.Latitude);
                origin.Put("lng", latLng1.Longitude);

                destination.Put("lat", latLng2.Latitude);
                destination.Put("lng", latLng2.Longitude);

                json.Put("origin", origin);
                json.Put("destination", destination);

                RequestBody requestBody = RequestBody.Create(JSON, json.ToString());
                Request request = new Request.Builder().Url(url).Post(requestBody).Build();
                response = GetNetClient().InitOkHttpClient().NewCall(request).Execute();
            }
            catch (JSONException e)
            {
                e.PrintStackTrace();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
            return response;
        }

        public Response GetBicyclingRoutePlanningResult(LatLng latLng1, LatLng latLng2, bool needEncode)
        {
            string key = mDefaultKey;
            if (needEncode)
            {
                try
                {
                    key = URLEncoder.Encode(mDefaultKey, "UTF-8");
                }
                catch (UnsupportedEncodingException e)
                {
                    e.PrintStackTrace();
                }
            }
            string url = mBicyclingRoutePlanningURL + "?key=" + key;

            Response response = null;
            JSONObject origin = new JSONObject();
            JSONObject destination = new JSONObject();
            JSONObject json = new JSONObject();
            try
            {
                origin.Put("lat", latLng1.Latitude);
                origin.Put("lng", latLng1.Longitude);

                destination.Put("lat", latLng2.Latitude);
                destination.Put("lng", latLng2.Longitude);

                json.Put("origin", origin);
                json.Put("destination", destination);

                RequestBody requestBody = RequestBody.Create(JSON, json.ToString());
                Request request = new Request.Builder().Url(url).Post(requestBody).Build();
                response = GetNetClient().InitOkHttpClient().NewCall(request).Execute();
            }
            catch (JSONException e)
            {
                e.PrintStackTrace();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
            return response;
        }

        public Response GetDrivingRoutePlanningResult(LatLng latLng1, LatLng latLng2, bool needEncode)
        {
            String key = mDefaultKey;
            if (needEncode)
            {
                try
                {
                    key = URLEncoder.Encode(mDefaultKey, "UTF-8");
                }
                catch (UnsupportedEncodingException e)
                {
                    e.PrintStackTrace();
                }
            }
            String url = mDrivingRoutePlanningURL + "?key=" + key;

            Response response = null;
            JSONObject origin = new JSONObject();
            JSONObject destination = new JSONObject();
            JSONObject json = new JSONObject();
            try
            {
                origin.Put("lat", latLng1.Latitude);
                origin.Put("lng", latLng1.Longitude);

                destination.Put("lat", latLng2.Latitude);
                destination.Put("lng", latLng2.Longitude);

                json.Put("origin", origin);
                json.Put("destination", destination);

                RequestBody requestBody = RequestBody.Create(JSON, json.ToString());
                Request request = new Request.Builder().Url(url).Post(requestBody).Build();
                response = GetNetClient().InitOkHttpClient().NewCall(request).Execute();
            }
            catch (JSONException e)
            {
                e.PrintStackTrace();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
            return response;
        }
    }
}